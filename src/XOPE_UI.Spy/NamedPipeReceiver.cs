using Newtonsoft.Json.Linq;
using PeterO.Cbor;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using XOPE_UI.Definitions;
using XOPE_UI.Native;
using XOPE_UI.Spy.ServerType;
using XOPE_UI.Spy.Type;

namespace XOPE_UI.Spy
{
    public class NamedPipeReceiver : IMessageReceiver
    {
        ConcurrentQueue<IncomingMessage> incomingMessageQueue;
        CancellationTokenSource cancellationTokenSource;
        Task serverThread;

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
            if (serverThread != null)
            {
                Console.WriteLine("Cannot start new server thread, as one already exists");
                return;
            }

            cancellationTokenSource = new CancellationTokenSource();

            setIsConnectingState();
            serverThread = Task.Factory.StartNew(() => {
                using (NamedPipeServerStream serverStream = new NamedPipeServerStream("xopeui"))
                {
                    serverStream.WaitForConnection();
                    Console.WriteLine("Spy connected to Server");

                    try
                    {
                        setIsConnectedState();

                        byte[] buffer = new byte[65536];
                        while (serverStream.IsConnected && !cancellationTokenSource.IsCancellationRequested)
                        {
                            int bytesAvailable;

                            Win32API.PeekNamedPipe((IntPtr)serverStream.SafePipeHandle.DangerousGetHandle(), out _, 0, out _, out bytesAvailable, out _);
                            if (bytesAvailable > 0)
                            {
                                int len = serverStream.Read(buffer, 0, 65536);
                                if (len > 0)
                                {
                                    CBORObject cbor = CBORObject.DecodeFromBytes((new ArraySegment<byte>(buffer, 0, len)).ToArray());
                                    JObject json = JObject.Parse(cbor.ToString());
                                    //Console.WriteLine($"Incoming message: {(ServerMessageType)json.Value<Int32>("messageType")}");

                                    ServerMessageType messageType = (ServerMessageType)json.Value<Int32>("messageType");
                                    incomingMessageQueue.Enqueue(new IncomingMessage(messageType, json));   
                                }
                            }
                            Thread.Sleep(30); 
                        }
                        Console.WriteLine("Closing server...");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Server error. Aborting server! Message: {ex.Message}");
                        Debug.WriteLine($"Server error. Message: {ex.Message}");
                        Debug.WriteLine($"Stacktrack:\n{ex.StackTrace}");
                    }
                    finally
                    {
                        Console.WriteLine("Server closed!");
                    }

                    setNoConnectionState();
                }

            }, cancellationTokenSource.Token, TaskCreationOptions.DenyChildAttach | TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void ShutdownServerAndWait()
        {
            if (serverThread == null)
                return;

            cancellationTokenSource.Cancel();
            serverThread.Wait(5000);
            serverThread = null;
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
