using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Newtonsoft.Json.Linq;
using PeterO.Cbor;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using XOPE_UI.Model;
using XOPE_UI.Spy.Type;

namespace XOPE_UI.Spy
{
    public class NamedPipeReceiver : IMessageReceiver
    {
        Object _incomingMessageQueueLock;
        ConcurrentQueue<IncomingMessage> _incomingMessageQueue;
        CancellationTokenSource _cancellationTokenSource;

        Task _currentReceiverTask;

        public bool IsConnecting { get; private set; }
        public bool IsConnected { get; private set; }
        public bool IsConnectingOrConnected { get => IsConnecting || IsConnected; }

        public NamedPipeReceiver()
        {
            _incomingMessageQueueLock = new Object();
            _incomingMessageQueue = new ConcurrentQueue<IncomingMessage>();
        }


        public IncomingMessage GetIncomingMessage()
        {
            // GetIncomingMessage does not need the _incomingMessageQueueLock
            //  as the normal locking ability of ConcurrentQueue will suffice
            bool success = _incomingMessageQueue.TryDequeue(out var message);
            return success ? message : null;
        }

        public IncomingMessage[] GetIncomingMessages()
        {
            if (_incomingMessageQueue.Count < 1) return null;

            lock (_incomingMessageQueueLock)
            {
                IncomingMessage[] ims = _incomingMessageQueue.ToArray();
                _incomingMessageQueue.Clear();
                return ims;
            }
        }

        public async Task StartReceiver(string receiverName)
        {
            if (IsConnectingOrConnected)
            {
                Console.WriteLine("Cannot start new receiver thread, as one already exists");
                return;
            }

            setIsConnectingState();

            NamedPipeServerStream receiver = new NamedPipeServerStream(receiverName);
            _cancellationTokenSource = new CancellationTokenSource();

            await receiver.WaitForConnectionAsync(_cancellationTokenSource.Token);
            setIsConnectedState();
            Console.WriteLine("Spy connected to Receiver. Waiting for CONNECTION_SUCCESS message...");
            _currentReceiverTask = ProcessAsync(receiver);
        }

        public void ShutdownAndWait()
        {
            _cancellationTokenSource.Cancel();
            setNoConnectionState();
            _currentReceiverTask.Wait(2500);
            _currentReceiverTask = null;
        }

        private async Task ProcessAsync(NamedPipeServerStream receiverStream) 
        {
            using (receiverStream)
            {
                _cancellationTokenSource.Token.Register(() => receiverStream.Close());

                try
                {
                    byte[] inBuffer = new byte[65536];
                    while (receiverStream.IsConnected && !_cancellationTokenSource.IsCancellationRequested)
                    {
                        int bytesReceived = await receiverStream.ReadAsync(inBuffer, 0, inBuffer.Length, _cancellationTokenSource.Token);
                        if (bytesReceived == 0)
                        {
                            if (receiverStream.IsConnected)
                                _cancellationTokenSource.Cancel();
                            break;
                        }
                        
                        try
                        {
                            MemoryStream outputStream = new MemoryStream();
                            using (MemoryStream memoryStream = new MemoryStream(new ArraySegment<byte>(inBuffer, 0, bytesReceived).ToArray()))
                            using (var inflater = new InflaterInputStream(memoryStream))
                            {
                                inflater.CopyTo(outputStream);
                            }
                            
                            CBORObject cbor = CBORObject.DecodeFromBytes(outputStream.ToArray());
                            JObject json = JObject.Parse(cbor.ToJSONString());

                            UiMessageType messageType = (UiMessageType)json.Value<Int32>("messageType");

                            //if (json.ContainsKey("packetDataB64") && json.Value<string>("packetDataB64").Length > 0)
                            //{
                            //    Console.WriteLine($"JSON Size / CBOR Size / bytesReceived / Real Packet Len: {cbor.ToJSONString().Length} / {cbor.EncodeToBytes().Length} / {bytesReceived} / {json.Value<string>("packetLen")}");
                            //}

                            lock (_incomingMessageQueueLock) _incomingMessageQueue.Enqueue(new IncomingMessage(messageType, json));
                        }
                        catch (CBORException ex)
                        {
                            Console.WriteLine($"[ui-receiver] Error occurred when decoding message from spy. " +
                                $"Message: {ex.Message}. " +
                                $"Dropping message...");
                        }
                    }
                    Console.WriteLine("Closing receiver...");
                }
                catch (ObjectDisposedException) { }
                catch (Exception ex)
                {
                    Console.WriteLine($"Receiver error. Aborting receiver! Message: {ex.Message}");
                    Debug.WriteLine($"Receiver error. Message: {ex.Message}");
                    Debug.WriteLine($"Stacktrack:\n{ex.StackTrace}");
                    _cancellationTokenSource.Cancel();
                }
                finally
                {
                    Console.WriteLine("Receiver closed!");
                }

                setNoConnectionState();
            }
        }

        private void setIsConnectingState()
        {
            IsConnecting = true;
            IsConnected = false;
        }

        private void setIsConnectedState()
        {
            IsConnecting = false;
            IsConnected = true;
        }

        private void setNoConnectionState()
        {
            IsConnecting = false;
            IsConnected = false;
        }
    }
}
