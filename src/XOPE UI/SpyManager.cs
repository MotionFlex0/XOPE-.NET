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
using XOPE_UI.Model;
using XOPE_UI.Spy;
using XOPE_UI.Spy.DispatcherMessageType;
using XOPE_UI.Spy.Type;

namespace XOPE_UI
{
    // TODO: Refactor!
    public class SpyManager
    {
        public event EventHandler<Packet> NewPacket;
        public event EventHandler<Connection> ConnectionConnecting;
        public event EventHandler<Connection> ConnectionEstablished;
        public event EventHandler<Connection> ConnectionClosed;
        public event EventHandler<Connection> ConnectionStatusChanged;
        public event EventHandler<Connection> ConnectionPropUpdated;

        public SpyData SpyData { get; private set; }
        public bool IsAttached { get => _attachedProcess != null; }
        public bool IsTunneling { get; private set; }
        public HttpTunnelingHandler HttpTunnel { get; private set; }

        public NamedPipeDispatcher MessageDispatcher { get; private set; }
        NamedPipeReceiver MessageReceiver { get; set; }

        CancellationTokenSource _cancellationTokenSource = null;
        Task _spyThread = null;

       

        Dictionary<Guid, IMessageWithResponse> _jobs; // This contains Messages that are expecting a response

        Process _attachedProcess = null;

        public SpyManager()
        {
            SpyData = new SpyData();
            IsTunneling = false;
            MessageReceiver = new NamedPipeReceiver();
            _jobs = new Dictionary<Guid, IMessageWithResponse>();
        }

        ~SpyManager()
        {
            Shutdown();
        }

        public void AttachedToProcess(Process process)
        {
            _attachedProcess = process;
        }

        public void DetachedFromProcess()
        {
            _attachedProcess = null;
        }

        public void DisableHttpTunneling()
        {
            HttpTunnel.Dispose();
            HttpTunnel = null;
            IsTunneling = false;
            if (MessageDispatcher != null)
                MessageDispatcher.Send(new ToggleHttpTunneling() { IsTunnelingEnabled = false });
        }

        public void EnableHttpTunneling(IPAddress ip, int port80, int port443)
        {
            if (MessageDispatcher == null)
            {
                Console.WriteLine("Cannot connect to spy as MessageDispatcher is null.");
                return;
            }

            HttpTunnel = new HttpTunnelingHandler(this, ip, port80, port443);
            if (MessageDispatcher != null && HttpTunnel.SinkConnected)
            {
                MessageDispatcher.Send(new ToggleHttpTunneling() { IsTunnelingEnabled = true });
                IsTunneling = true;
            }
        }

        // TODO: Refactor this method
        public void RunAsync()
        {
            if (_spyThread != null)
                return;

            _cancellationTokenSource = new CancellationTokenSource();

            MessageReceiver.RunAsync();

            _spyThread = Task.Factory.StartNew(() =>
            {
                while ((MessageDispatcher == null || MessageDispatcher.IsConnected) && 
                    MessageReceiver.IsConnectingOrConnected && !_cancellationTokenSource.IsCancellationRequested)
                {
                    IncomingMessage incomingMessage = MessageReceiver.GetIncomingMessage();
                    if (incomingMessage != null)
                    {
                        if (incomingMessage.Type == UiMessageType.CONNECTED_SUCCESS)
                        {
                            JObject json = incomingMessage.Json;
                            Console.WriteLine($"Connection success: {json}");
                            string spyPipeServerName = json.Value<string>("spyPipeServerName");
                            MessageDispatcher = new NamedPipeDispatcher(spyPipeServerName, _jobs);
                            if (!MessageDispatcher.IsConnected)
                            {
                                Console.WriteLine("Unable to connect to Spy's Server. Aborting...");
                                break;
                            }
                        }
                        else if (MessageDispatcher != null)
                            ProcessIncomingMessage(incomingMessage);
                        else
                            Console.WriteLine($"Received message before CONNECTED_SUCCESS." +
                                $"Dropping message {incomingMessage.Type}");
                    }
                    Thread.Sleep(1);
                }

                Console.WriteLine("SpyManager loop ended");

                if (MessageDispatcher != null && MessageDispatcher.IsConnected)
                {
                    MessageDispatcher.Send(new ShutdownSpy());
                    MessageDispatcher.ShutdownAndWait();
                }

                if (MessageReceiver.IsConnected)
                    MessageReceiver.ShutdownAndWait();

            }, _cancellationTokenSource.Token, TaskCreationOptions.DenyChildAttach | TaskCreationOptions.LongRunning, TaskScheduler.Default);

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
                if (hookedFuncType == HookedFuncType.CONNECT || hookedFuncType == HookedFuncType.WSACONNECT)
                {
                    int socket = json.Value<Int32>("socket");
                    Connection connection = new Connection(
                        socket,
                        json.Value<Int32>("protocol"),
                        json.Value<Int32>("addrFamily"),
                        json.Value<string>("addr"),
                        json.Value<Int32>("port"),
                        Connection.Status.ESTABLISHED,
                        hookedFuncType == HookedFuncType.CONNECT ?
                            Connection.WinsockVersion.Version_1 :
                            Connection.WinsockVersion.Version_2
                    );

                    int connectRet = json.Value<int>("ret");
                    int connectLastError = json.Value<int>("lastError");
                    if (connectRet == 0)
                    {
                        SpyData.Connections.TryAdd(socket, connection);
                        ConnectionEstablished?.Invoke(this, connection);
                        Console.WriteLine($"Socket {socket} has been opened.");

                        bool isTunneling = json.Value<bool>("tunneling");
                        if (isTunneling)
                        {
                            if (HttpTunnel == null)
                                throw new NullReferenceException($"_httpTunnelingHandler is null but socket is tunnelable.W");
                            HttpTunnel.TunnelNewConnection(connection);
                        }
                    }
                    else if (connectRet == -1 && connectLastError == 10035) // ret == SOCKET_ERROR and error == WSAEWOULDBLOCK
                    {
                        connection.SocketStatus = Connection.Status.CONNECTING;
                        SpyData.Connections.TryAdd(json.Value<Int32>("socket"), connection);
                        ConnectionConnecting?.Invoke(this, connection);

                        bool isTunneling = json.Value<bool>("tunneling");
                        if (isTunneling)
                        {
                            if (HttpTunnel == null)
                                throw new NullReferenceException($"_httpTunnelingHandler is null but socket is tunnelable.W");
                            HttpTunnel.TunnelNewConnection(connection);
                        }

                        int counter = 0;
                        System.Timers.Timer timer = new System.Timers.Timer();
                        ElapsedEventHandler timerCallback = (object sender, ElapsedEventArgs e) =>
                        {
                            EventHandler<IncomingMessage> callback = (object o, IncomingMessage resp) =>
                            {
                                if (connection.SocketStatus == Connection.Status.CLOSED)
                                    return;

                                if (++counter >= 5)
                                {
                                    Console.WriteLine($"Socket never became writable. Socket: {connection.SocketId}.");
                                    RemoveExistingConnection(connection.SocketId);
                                }
                                else if (resp.Type == UiMessageType.JOB_RESPONSE_SUCCESS)
                                {
                                    if (resp.Json.Value<bool>("writable") == false)
                                        timer.Start();
                                    else // writable == true
                                    {
                                        connection.SocketStatus = Connection.Status.ESTABLISHED;
                                        ConnectionEstablished?.Invoke(this, connection);
                                    }
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
                                Console.WriteLine($"Added previously opened socket {socket}.");
                            }
                            else if (resp.Type == UiMessageType.JOB_RESPONSE_ERROR)
                            {
                                Console.WriteLine($"Failed to find info on socket {socket}.");
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
                            Console.WriteLine($"Socket {socket} closed gracefully.");
                        }
                        else if (json.Value<int>("ret") == -1)
                        {
                            if (json.Value<int>("lastError") != 10035) // if WSAGetLastError() != WSAEWOULDBLOCK
                                Console.WriteLine($"Socket {socket} returned WSAError {json.Value<int>("lastError")}.");
                        }
                        else
                        {
                            byte[] data = Convert.FromBase64String(json.Value<String>("packetDataB64"));
                            bool modified = json.Value<bool>("modified");
                            Packet packet = new Packet
                            {
                                Id = Guid.NewGuid(),
                                Type = hookedFuncType,
                                Data = data,
                                Length = data.Length,
                                Socket = socket,
                                Modified = modified
                            };
                            if (HttpTunnel != null && 
                                HttpTunnel.IsTunnelingSocket(socket))
                            {
                                if (hookedFuncType == HookedFuncType.SEND)
                                    HttpTunnel.Send(packet);
                                packet.Tunneled = true;
                            }

                            NewPacket?.Invoke(this, packet);
                        }
                    }
                    else if (hookedFuncType == HookedFuncType.WSASEND ||
                        hookedFuncType == HookedFuncType.WSARECV)
                    {
                        int bufferCount = json.Value<int>("bufferCount");
                        int lastError = json.Value<int>("lastError");
                        bool isSocketClosed = (bufferCount == -1 && (lastError == 10101 || lastError == 10054));

                        if (hookedFuncType == HookedFuncType.WSARECV && isSocketClosed)
                        {
                            RemoveExistingConnection(socket);
                            Console.WriteLine($"Socket {socket} closed gracefully.");
                        }
                        else
                        {
                            JArray buffers = json.Value<JArray>("buffers");
                            for (int i = 0; i < bufferCount; i++)
                            {
                                if (hookedFuncType == HookedFuncType.WSARECV && buffers[i].Value<int>("length") == 0)
                                {
                                    RemoveExistingConnection(socket);
                                    Console.WriteLine($"Socket {socket} closed gracefully.");
                                    break;
                                }

                                byte[] data = Convert.FromBase64String(buffers[i].Value<String>("dataB64"));
                                bool modified = buffers[i].Value<bool>("modified");
                                Packet packet = new Packet
                                {
                                    Id = Guid.NewGuid(),
                                    Type = hookedFuncType,
                                    Data = data,
                                    Length = data.Length,
                                    Socket = socket,
                                    Modified = modified
                                };
                                if (HttpTunnel != null && 
                                    HttpTunnel.IsTunnelingSocket(socket))
                                {
                                    if (hookedFuncType == HookedFuncType.WSASEND)
                                        HttpTunnel.Send(packet);
                                    packet.Tunneled = true;
                                }
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
                if (_jobs.ContainsKey(guid))
                {
                    // TODO: Maybe make this async?
                    _jobs[guid].NotifyResponse(incomingMessage); 
                    _jobs.Remove(guid);
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
            if (_spyThread != null && 
                !_spyThread.IsCompleted && !_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
                bool completed = _spyThread.Wait(7000);
                if (!completed)
                    Console.WriteLine("App.Shutdown waited for SpyManager Thread to exit but it timed-out.");
                ResetState();
            }
        }

        private void ResetState()
        {
            _jobs.Clear();
            SpyData.Connections.Clear();
            SpyData.Packets.Clear();

            _spyThread = null;
            MessageDispatcher = null;

            if (IsTunneling)
            {
                HttpTunnel.Dispose();
                HttpTunnel = null;
                IsTunneling = false;
            }
        }

        private void RemoveExistingConnection(int socket)
        {
            if (SpyData.Connections.ContainsKey(socket))
            {
                if (HttpTunnel != null && HttpTunnel.IsTunnelingSocket(socket))
                    HttpTunnel.ConnectionClosedExternally(socket);

                ConnectionClosed?.Invoke(this, SpyData.Connections[socket]);
                SpyData.Connections.Remove(socket, out _);
            }
        }
    }
}
