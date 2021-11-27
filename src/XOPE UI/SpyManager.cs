using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
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
        public event EventHandler<Packet> OnNewPacket;
        public event EventHandler<Connection> OnNewConnection;
        public event EventHandler<Connection> OnCloseConnection;
        public event EventHandler<Connection> OnConnectionStatusChange;

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

        public void RunAsync(Process spyProcess)
        {
            if (spyThread != null)
                return;

            cancellationTokenSource = new CancellationTokenSource();

            MessageReceiver.RunAsync();

            spyThread = Task.Run(() =>
            {
                while ((MessageDispatcher == null || MessageDispatcher.IsConnected) && MessageReceiver.IsConnectingOrConnected && !cancellationTokenSource.IsCancellationRequested)
                {
                    IncomingMessage incomingMessage = MessageReceiver.GetIncomingMessage();
                    if (incomingMessage != null)
                    {
                        if (incomingMessage.Type == ServerMessageType.CONNECTED_SUCCESS)
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
                            Console.Write($"Received message before CONNECTED_SUCCESS. " +
                                $"Dropping message {incomingMessage.Type}");
                    }
                    Thread.Sleep(30);
                }

                if (MessageDispatcher != null && MessageDispatcher.IsConnected)
                {
                    MessageDispatcher.Send(new ShutdownSpy());
                    MessageDispatcher.ShutdownAndWait();
                }

                if (MessageReceiver.IsConnected)
                    MessageReceiver.ShutdownServerAndWait();

            }, cancellationTokenSource.Token);

            //Waits until MessageReceiver has started and is waiting for a connection
            while (!MessageReceiver.IsConnectingOrConnected) Thread.Sleep(50);
        }

        private void ProcessIncomingMessage(IncomingMessage incomingMessage)
        {
            JObject json = incomingMessage.Json;
            ServerMessageType messageType = incomingMessage.Type;

            if (messageType == ServerMessageType.HOOKED_FUNCTION_CALL)
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
                        new IPAddress(json.Value<Int32>("addr")),
                        json.Value<Int32>("port"),
                        Connection.Status.ESTABLISHED
                    );

                    int connectRet = json.Value<int>("ret");
                    int connectLastError = json.Value<int>("lastError");
                    if (connectRet == 0)
                    {
                        SpyData.Connections.TryAdd(socket, connection);
                        OnNewConnection?.Invoke(this, connection);
                        Console.WriteLine($"Socket {socket} has been opened");
                    }
                    else if (connectRet == -1 && connectLastError == 10035) // ret == SOCKET_ERROR and error == WSAEWOULDBLOCK
                    {
                        connection.SocketStatus = Connection.Status.CONNECTING;
                        SpyData.Connections.TryAdd(json.Value<Int32>("socket"), connection);

                        System.Timers.Timer timer = new System.Timers.Timer();
                        int counter = 0;
                        timer.Elapsed += (object sender, ElapsedEventArgs e) =>
                        {
                            EventHandler<JObject> callback = (object o, JObject resp) =>
                            {
                                if (++counter >= 5)
                                {
                                    Console.WriteLine($"Socket never became writable. Socket: {connection.SocketId}");
                                    SpyData.Connections.TryRemove(connection.SocketId, out _);
                                }
                                else if (resp.Value<bool>("writable") == false)
                                {
                                    timer.Start();
                                }
                            };

                            if (MessageDispatcher != null)
                                MessageDispatcher.Send(new IsSocketWritable(callback) { SocketId = connection.SocketId });
                        };
                        timer.Interval = 1000;
                        timer.AutoReset = false;
                        timer.Start();
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
                    Connection matchingConnection = SpyData.Connections[socketId]; //Data.Connections.FirstOrDefault((Connection c) => c.SocketId == socketId);

                    if (matchingConnection != null && matchingConnection.SocketStatus != Connection.Status.CLOSED)
                    {
                        matchingConnection.SocketStatus = Connection.Status.CLOSED;
                        matchingConnection.LastStatusChangeTime = DateTime.Now;

                        System.Timers.Timer t = new System.Timers.Timer();
                        t.Elapsed += (object sender, ElapsedEventArgs e) =>
                        {
                            SpyData.Connections.TryRemove(matchingConnection.SocketId, out Connection v);
                        };

                        t.Interval = 7000;
                        t.AutoReset = false;
                        t.Start();
                        OnCloseConnection?.Invoke(this, matchingConnection);
                    }
                }
                else
                {
                    int socket = json.Value<int>("socket");

                    if (!SpyData.Connections.ContainsKey(socket))
                    {
                        Console.WriteLine($"Missed Connect/WSAConnect for socket {socket}. Reqesting info...");

                        Connection connection = new Connection(
                            socket,
                            json.Value<Int32>("protocol"),
                            json.Value<Int32>("addrFamily"),
                            new IPAddress(json.Value<Int32>("addr")),
                            json.Value<Int32>("port"),
                            Connection.Status.REQUESTING_INFO
                        );
                        SpyData.Connections.TryAdd(connection.SocketId, connection);

                        SocketInfo socketInfo = new SocketInfo()
                        {
                            SocketId = socket
                        };
                        socketInfo.OnResponse += (object o, JObject resp) =>
                        {
                            SpyData.Connections.TryAdd(socket, connection);
                            OnNewConnection?.Invoke(this, connection); // 
                            Console.WriteLine($"Added previously opened socket {socket}");
                        };

                        MessageDispatcher.Send(socketInfo);
                    }

                    byte[] data = Convert.FromBase64String(json.Value<String>("packetDataB64"));
                    if (json.Value<int>("ret") == 0)
                    {
                        if (SpyData.Connections.ContainsKey(socket))
                            SpyData.Connections.Remove(socket, out _);
                        Console.WriteLine($"Socket {socket} closed gracefully");
                    }
                    else
                    {
                        Packet packet = new Packet
                        {
                            Id = Guid.NewGuid(),
                            Type = hookedFuncType,
                            Data = data,
                            Length = data.Length,
                            Socket = socket,
                        };
                        OnNewPacket?.Invoke(this, packet);
                    }
                }

            }
            else if (messageType == ServerMessageType.JOB_RESPONSE_SUCCESS
                    || messageType == ServerMessageType.JOB_RESPONSE_ERROR)
            {
                Guid guid = Guid.Parse(json.Value<String>("jobId"));
                if (jobs.ContainsKey(guid))
                {
                    jobs[guid].NotifyResponse(json); // TODO: Maybe make this async?
                    jobs.Remove(guid);
                }
                else
                    Console.WriteLine($"Received JOB_RESPONSE for id '{guid}' but it does not appear to be a valid id.");
            }
            else if (messageType == ServerMessageType.ERROR_MESSAGE)
            {
                Console.WriteLine($"[Spy-Error] {json.Value<String>("errorMessage")}");
            }
            else if (messageType == ServerMessageType.INVALID_MESSAGE)
            {
                Console.WriteLine($"[Server-Error] Received a INVALID_MESSAGE. Check if the messageType was sent by Spy.");
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

        public void Shutdown()
        {
            if (spyThread != null && !cancellationTokenSource.IsCancellationRequested)
            {
                cancellationTokenSource.Cancel();
                bool completed = spyThread.Wait(7000);
                if (!completed)
                    Console.WriteLine("App.Shutdown waited for SpyManager Thread to exit but it timed-out");
                ResetState();
            }
        }

    }
}
