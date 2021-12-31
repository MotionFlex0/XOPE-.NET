using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using XOPE_UI.Definitions;
using XOPE_UI.Spy;
using XOPE_UI.Spy.DispatcherMessageType;
using XOPE_UI.Spy.Type;

namespace XOPE_UI
{
    public class SpyManager
    {
        public event EventHandler<Packet> NewPacket;
        public event EventHandler<Connection> ConnectionConnecting;
        public event EventHandler<Connection> ConnectionEstablished;
        public event EventHandler<Connection> ConnectionClosed;
        public event EventHandler<Connection> ConnectionStatusChanged;
        public event EventHandler<Connection> ConnectionPropUpdated;

        public SpyData SpyData { get; private set; } = null;

        public NamedPipeDispatcher MessageDispatcher { get; private set; } = null;
        NamedPipeReceiver MessageReceiver { get; set; } = null;

        CancellationTokenSource cancellationTokenSource = null;
        Task spyThread = null;

        Dictionary<Guid, IMessageWithResponse> jobs; // This contains Messages that are expecting a response


        public SpyManager()
        {
            SpyData = new SpyData();
            MessageReceiver = new NamedPipeReceiver();
            jobs = new Dictionary<Guid, IMessageWithResponse>();
        }

        ~SpyManager()
        {
            Shutdown();
        }

        // TODO: Refactor this method
        public void RunAsync(Process spyProcess)
        {
            if (spyThread != null)
                return;

            cancellationTokenSource = new CancellationTokenSource();

            MessageReceiver.RunAsync();

            spyThread = Task.Factory.StartNew(() =>
            {
                while ((MessageDispatcher == null || MessageDispatcher.IsConnected) 
                && MessageReceiver.IsConnectingOrConnected && !cancellationTokenSource.IsCancellationRequested)
                {
                    IncomingMessage incomingMessage = MessageReceiver.GetIncomingMessage();
                    if (incomingMessage != null)
                    {
                        if (incomingMessage.Type == UiMessageType.CONNECTED_SUCCESS)
                        {
                            JObject json = incomingMessage.Json;
                            Console.WriteLine($"Connection success: {json}");
                            string spyPipeServerName = json.Value<string>("spyPipeServerName");
                            MessageDispatcher = new NamedPipeDispatcher(spyPipeServerName, jobs);
                            if (!MessageDispatcher.IsConnected)
                            {
                                Console.WriteLine("Unable to connect to Spy's Server. Aborting...");
                                break;
                            }
                        }
                        else if (MessageDispatcher != null)
                            ProcessIncomingMessage(incomingMessage);
                        else
                            Console.WriteLine($"Received message before CONNECTED_SUCCESS. " +
                                $"Dropping message {incomingMessage.Type}");
                    }
                    Thread.Sleep(30);
                }

                Console.WriteLine("SpyManager loop ended");

                if (MessageDispatcher != null && MessageDispatcher.IsConnected)
                {
                    MessageDispatcher.Send(new ShutdownSpy());
                    MessageDispatcher.ShutdownAndWait();
                }

                if (MessageReceiver.IsConnected)
                    MessageReceiver.ShutdownAndWait();

            }, cancellationTokenSource.Token, TaskCreationOptions.DenyChildAttach | TaskCreationOptions.LongRunning, TaskScheduler.Default);

            //Waits until MessageReceiver has started and is waiting for a connection
            while (!MessageReceiver.IsConnectingOrConnected) Thread.Sleep(50);
        }

        private void ProcessIncomingMessage(IncomingMessage incomingMessage)
        {
            JObject json = incomingMessage.Json;
            UiMessageType messageType = incomingMessage.Type;

            if (messageType == UiMessageType.HOOKED_FUNCTION_CALL)
            {
                HookedFuncType hookedFuncType = (HookedFuncType)json.Value<Int32>("functionName");

                //Console.WriteLine($"Message hookedType: {hookedFuncType}");

                if (hookedFuncType == HookedFuncType.CONNECT || hookedFuncType == HookedFuncType.WSACONNECT)
                {
                    int socket = json.Value<Int32>("socket");
                    Connection connection = new Connection(
                        socket,
                        json.Value<Int32>("protocol"),
                        json.Value<Int32>("addrFamily"),
                        json.Value<string>("addr"),
                        json.Value<Int32>("port"),
                        Connection.Status.ESTABLISHED
                    );

                    int connectRet = json.Value<int>("ret");
                    int connectLastError = json.Value<int>("lastError");
                    if (connectRet == 0)
                    {
                        SpyData.Connections.TryAdd(socket, connection);
                        ConnectionEstablished?.Invoke(this, connection);
                        Console.WriteLine($"Socket {socket} has been opened");
                    }
                    else if (connectRet == -1 && connectLastError == 10035) // ret == SOCKET_ERROR and error == WSAEWOULDBLOCK
                    {
                        connection.SocketStatus = Connection.Status.CONNECTING;
                        SpyData.Connections.TryAdd(json.Value<Int32>("socket"), connection);

                        int counter = 0;
                        System.Timers.Timer timer = new System.Timers.Timer();
                        ElapsedEventHandler timerCallback = (object sender, ElapsedEventArgs e) =>
                        {
                            EventHandler<IncomingMessage> callback = (object o, IncomingMessage resp) =>
                            {
                                if (++counter >= 5)
                                {
                                    Console.WriteLine($"Socket never became writable. Socket: {connection.SocketId}");
                                    RemoveExistingConnection(connection.SocketId);
                                }
                                else if (resp.Type == UiMessageType.JOB_RESPONSE_SUCCESS)
                                {
                                    if (resp.Json.Value<bool>("writable") == false)
                                        timer.Start();
                                    else // writable == true
                                        connection.SocketStatus = Connection.Status.ESTABLISHED;
                                }
                            };

                            if (MessageDispatcher != null)
                                MessageDispatcher.Send(new IsSocketWritable(callback) { SocketId = connection.SocketId });
                        };

                        timer.Elapsed += timerCallback;
                        timer.Interval = 1000;
                        timer.AutoReset = false;
                        timerCallback(null, null); // Call it immediately the first time, then try 4 more times with 1s intervals
                    }
                    else
                    {
                        Console.WriteLine($"Socket connected failed. Socket: {connection.SocketId} | " +
                            $"Connect Ret: {connectRet} | " +
                            $"WSALastError: {connectLastError}");
                    }
                }
                else if (hookedFuncType == HookedFuncType.CLOSE)
                {
                    int socketId = json.Value<Int32>("socket");
                    bool found = SpyData.Connections.TryGetValue(socketId, out Connection matchingConnection);
                    if (found)
                    {
                        if (matchingConnection != null && matchingConnection.SocketStatus != Connection.Status.CLOSED)
                        {
                            matchingConnection.SocketStatus = Connection.Status.CLOSED;
                            RemoveExistingConnection(matchingConnection.SocketId);
                        }
                    }
                }    
                else
                {
                    int socket = json.Value<int>("socket");

                    if (!SpyData.Connections.ContainsKey(socket))
                    {
                        Console.WriteLine($"Missed Connect/WSAConnect for socket {socket}. Reqesting info...");

                        Connection connection = new Connection(socket);
                        SpyData.Connections.TryAdd(connection.SocketId, connection);
                        ConnectionConnecting?.Invoke(this, connection);

                        SocketInfo socketInfo = new SocketInfo()
                        {
                            SocketId = socket
                        };
                        socketInfo.OnResponse += (object o, IncomingMessage resp) =>
                        {
                            if (resp.Type == UiMessageType.JOB_RESPONSE_SUCCESS)
                            {
                                connection.IP = resp.Json.Value<string>("addr");
                                connection.Port = resp.Json.Value<int>("port");
                                connection.IPFamily = (AddressFamily)resp.Json.Value<Int32>("addrFamily");
                                connection.Protocol = resp.Json.Value<int>("protocol");
                                connection.SocketStatus = Connection.Status.ESTABLISHED;

                                ConnectionEstablished?.Invoke(this, connection); // 
                                Console.WriteLine($"Added previously opened socket {socket}");
                            }
                            else if (resp.Type == UiMessageType.JOB_RESPONSE_ERROR)
                            {
                                Console.WriteLine($"Failed to find info on socket {socket}");
                                RemoveExistingConnection(connection.SocketId);
                            }

                        };

                        MessageDispatcher.Send(socketInfo);
                    }

                    if (hookedFuncType == HookedFuncType.SEND || 
                        hookedFuncType == HookedFuncType.RECV)
                    {
                        if (json.Value<int>("ret") == 0)
                        {
                            RemoveExistingConnection(socket);
                            Console.WriteLine($"Socket {socket} closed gracefully");
                        }
                        else if (json.Value<int>("ret") == -1)
                        {
                            Console.WriteLine($"Socket {socket} returned WSAError {json.Value<int>("lastError")}");
                        }
                        else
                        {
                            byte[] data = Convert.FromBase64String(json.Value<String>("packetDataB64"));
                            Packet packet = new Packet
                            {
                                Id = Guid.NewGuid(),
                                Type = hookedFuncType,
                                Data = data,
                                Length = data.Length,
                                Socket = socket,
                            };
                            NewPacket?.Invoke(this, packet);
                        }
                    }
                    else if (hookedFuncType == HookedFuncType.WSASEND ||
                        hookedFuncType == HookedFuncType.WSARECV)
                    {
                        int bufferCount = json.Value<int>("bufferCount");
                        int lastError = json.Value<int>("lastError");
                        bool isSocketClosed = lastError == 10101 || lastError == 10054 || bufferCount == 0;

                        if (hookedFuncType == HookedFuncType.WSARECV && isSocketClosed)
                        {
                            RemoveExistingConnection(socket);
                            Console.WriteLine($"Socket {socket} closed gracefully");
                        }
                        else
                        {
                            JArray buffers = json.Value<JArray>("buffers");
                            for (int i = 0; i < bufferCount; i++)
                            {
                                byte[] data = Convert.FromBase64String(buffers[i].Value<String>("dataB64"));
                                Packet packet = new Packet
                                {
                                    Id = Guid.NewGuid(),
                                    Type = hookedFuncType,
                                    Data = data,
                                    Length = data.Length,
                                    Socket = socket,
                                };
                                NewPacket?.Invoke(this, packet);
                            }
                        }
                    }
                }

            }
            else if (messageType == UiMessageType.JOB_RESPONSE_SUCCESS || 
                messageType == UiMessageType.JOB_RESPONSE_ERROR)
            {
                Guid guid = Guid.Parse(json.Value<String>("jobId"));
                if (jobs.ContainsKey(guid))
                {
                    // TODO: Maybe make this async?
                    jobs[guid].NotifyResponse(incomingMessage); 
                    jobs.Remove(guid);
                }
                else
                    Console.WriteLine($"Received JOB_RESPONSE for id '{guid}' but it does not appear to be a valid id.");
            }
            else if (messageType == UiMessageType.ERROR_MESSAGE)
            {
                Console.WriteLine($"[Spy-Error] {json.Value<String>("errorMessage")}");
            }
            else if (messageType == UiMessageType.INVALID_MESSAGE)
            {
                Console.WriteLine($"[Server-Error] Received a INVALID_MESSAGE. Check if the messageType was sent by Spy.");
            }
        }

        public void Shutdown()
        {
            if (spyThread != null && !spyThread.IsCompleted && !cancellationTokenSource.IsCancellationRequested)
            {
                cancellationTokenSource.Cancel();
                bool completed = spyThread.Wait(7000);
                if (!completed)
                    Console.WriteLine("App.Shutdown waited for SpyManager Thread to exit but it timed-out");
                ResetState();
            }
        }

        private void ResetState()
        {
            jobs.Clear();
            SpyData.Connections.Clear();
            SpyData.Packets.Clear();

            spyThread = null;
            MessageDispatcher = null;
        }

        private void RemoveExistingConnection(int socket)
        {
            if (SpyData.Connections.ContainsKey(socket))
            {
                ConnectionClosed?.Invoke(this, SpyData.Connections[socket]);
                SpyData.Connections.Remove(socket, out _);
            }
        }
    }
}
