using Newtonsoft.Json.Linq;
using PeterO.Cbor;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using XOPE_UI.Definitions;
using XOPE_UI.Native;
using XOPE_UI.Spy.Type;

namespace XOPE_UI.Spy
{
    public class NamedPipeReceiver : IMessageReceiver
    {
        ConcurrentQueue<IncomingMessage> incomingMessageQueue;
        CancellationTokenSource cancellationTokenSource;
        Task receiverThread;

        public bool IsConnecting { get; private set; }
        public bool IsConnected { get; private set; }
        public bool IsConnectingOrConnected { get => IsConnecting || IsConnected; }

        public NamedPipeReceiver()
        {
            incomingMessageQueue = new ConcurrentQueue<IncomingMessage>();
        }


        public IncomingMessage GetIncomingMessage()
        {
            bool success = incomingMessageQueue.TryDequeue(out var message);
            return success ? message : null;
        }

        public void RunAsync() 
        {
            if (receiverThread != null)
            {
                Console.WriteLine("Cannot start new receiver thread, as one already exists");
                return;
            }

            cancellationTokenSource = new CancellationTokenSource();

            setIsConnectingState();
            receiverThread = Task.Factory.StartNew(() => {
                using (NamedPipeServerStream receiverStream = new NamedPipeServerStream("xopeui"))
                {
                    receiverStream.WaitForConnection();
                    Console.WriteLine("Spy connected to Receiver");

                    try
                    {
                        setIsConnectedState();

                        byte[] buffer = new byte[65536];
                        while (receiverStream.IsConnected && !cancellationTokenSource.IsCancellationRequested)
                        {
                            int bytesAvailable;

                            Win32API.PeekNamedPipe((IntPtr)receiverStream.SafePipeHandle.DangerousGetHandle(), out _, 0, out _, out bytesAvailable, out _);
                            if (bytesAvailable > 0)
                            {
                                int len = receiverStream.Read(buffer, 0, 65536);
                                if (len > 0)
                                {
                                    try
                                    {
                                        CBORObject cbor = CBORObject.DecodeFromBytes((new ArraySegment<byte>(buffer, 0, len)).ToArray());
                                        JObject json = JObject.Parse(cbor.ToString());
                                        //Console.WriteLine($"Incoming message: {(UiMessageType)json.Value<Int32>("messageType")}");

                                        UiMessageType messageType = (UiMessageType)json.Value<Int32>("messageType");
                                        incomingMessageQueue.Enqueue(new IncomingMessage(messageType, json));
                                    }
                                    catch (CBORException ex)
                                    {
                                        Console.WriteLine($"[ui-receiver] Error occurred when decoding message from spy. " +
                                            $"Message: {ex.Message}. " +
                                            $"Dropping message...");
                                    }
                                }
                            }
                            Thread.Sleep(30); 
                        }
                        Console.WriteLine("Closing receiver...");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Receiver error. Aborting receiver! Message: {ex.Message}");
                        Debug.WriteLine($"Receiver error. Message: {ex.Message}");
                        Debug.WriteLine($"Stacktrack:\n{ex.StackTrace}");
                    }
                    finally
                    {
                        Console.WriteLine("Receiver closed!");
                    }

                    setNoConnectionState();
                }

            }, cancellationTokenSource.Token, TaskCreationOptions.DenyChildAttach | TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void ShutdownAndWait()
        {
            if (receiverThread == null)
                return;

            cancellationTokenSource.Cancel();
            receiverThread.Wait(5000);
            receiverThread = null;
            setNoConnectionState();
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
