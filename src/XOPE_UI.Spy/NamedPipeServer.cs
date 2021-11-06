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

namespace XOPE_UI.Spy
{
    //TODO: this is kinda weird so maybe just remove it and put it back into the mainWindow class;
    //though, add some sort of translation class for incomming data
    public class NamedPipeServer : IServer
    {
        public event EventHandler<Packet> OnNewPacket;
        public event EventHandler<Connection> OnNewConnection;
        public event EventHandler<Connection> OnCloseConnection;
        
        public SpyData Data { get; private set; }

        ConcurrentQueue<byte[]> outBuffer;
        CancellationTokenSource cancellationTokenSource;
        Task serverThread; // Change this to something easier to follow

        Dictionary<Guid, IMessageWithResponse> jobs; // This contains Messages that are expecting a response

        public NamedPipeServer()
        {
            Data = new SpyData();

            outBuffer = new ConcurrentQueue<byte[]>();
            jobs = new Dictionary<Guid, IMessageWithResponse>();
        }

        public void Send(IMessage message)
        {
            outBuffer.Enqueue(Encoding.ASCII.GetBytes(message.ToJson().ToString())); //TODO: Convert to cbor
        }


        public void Send(IMessageWithResponse message)
        {
            outBuffer.Enqueue(Encoding.ASCII.GetBytes(message.ToJson().ToString())); //TODO: Convert to cbor
            jobs.Add(message.JobId, message);
        }

        // TODO: refactor most of this function
        public void RunAsync() 
        {
            if (serverThread != null)
            {
                Console.WriteLine("Cannot start new server thread, as one already exists");
                return;
            }

            cancellationTokenSource = new CancellationTokenSource();

            serverThread = Task.Factory.StartNew(() => {
                using (NamedPipeServerStream serverStream = new NamedPipeServerStream("xopeui"))
                {
                    serverStream.WaitForConnection();
                    Console.WriteLine("Server connected to Server");

                    NamedPipeClientStream clientStream = null;
                    try
                    {
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

                                    if (messageType == ServerMessageType.CONNECTED_SUCCESS)
                                    {
                                        Console.WriteLine($"Connection success: {json}");
                                        string spyPipeServerName = json.Value<string>("spyPipeServerName");
                                        clientStream = new NamedPipeClientStream(spyPipeServerName);
                                        clientStream.Connect(2000);
                                        if (!clientStream.IsConnected)
                                        {
                                            Console.WriteLine("Unable to connect to Spy's Server. Aborting...");
                                            break;
                                        }
                                    }
                                    else if (messageType == ServerMessageType.HOOKED_FUNCTION_CALL)
                                    {
                                        HookedFuncType hookedFuncType = (HookedFuncType)json.Value<Int32>("functionName");

                                        //Console.WriteLine($"Message hookedType: {hookedFuncType}");

                                        if (hookedFuncType == HookedFuncType.CONNECT || hookedFuncType == HookedFuncType.WSACONNECT)
                                        {
                                            Connection connection = new Connection(
                                                json.Value<Int32>("socket"),
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
                                                Data.Connections.TryAdd(json.Value<Int32>("socket"), connection);
                                                OnNewConnection?.Invoke(this, connection);
                                                Console.WriteLine("new connection");
                                            }
                                            else if (connectRet == -1 && connectLastError == 10035) // ret == SOCKET_ERROR and error == WSAEWOULDBLOCK
                                            {
                                                connection.SocketStatus = Connection.Status.CONNECTING;
                                                Data.Connections.TryAdd(json.Value<Int32>("socket"), connection);

                                                System.Timers.Timer timer = new System.Timers.Timer();
                                                int counter = 0;
                                                timer.Elapsed += (object sender, ElapsedEventArgs e) =>
                                                {
                                                    EventHandler<JObject> callback = (object o, JObject resp) =>
                                                    {
                                                        if (++counter >= 5)
                                                        {
                                                            Console.WriteLine($"Socket never became writable. Socket: {connection.SocketId}");
                                                            Data.Connections.TryRemove(connection.SocketId, out _);
                                                        }
                                                        else if (resp.Value<bool>("writable") == false)
                                                        {
                                                            timer.Start();
                                                        }
                                                    };

                                                    Send(new IsSocketWritable(callback) { SocketId = connection.SocketId });
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
                                            Connection matchingConnection = Data.Connections[socketId]; //Data.Connections.FirstOrDefault((Connection c) => c.SocketId == socketId);

                                            if (matchingConnection != null && matchingConnection.SocketStatus != Connection.Status.CLOSED)
                                            {
                                                matchingConnection.SocketStatus = Connection.Status.CLOSED;
                                                matchingConnection.LastStatusChangeTime = DateTime.Now;

                                                System.Timers.Timer t = new System.Timers.Timer();
                                                t.Elapsed += (object sender, ElapsedEventArgs e) =>
                                                {
                                                    Data.Connections.TryRemove(matchingConnection.SocketId, out Connection v);
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
                                            byte[] data = Convert.FromBase64String(json.Value<String>("packetDataB64"));
                                            Packet packet = new Packet
                                            {
                                                Id = Guid.NewGuid(),
                                                Type = hookedFuncType,
                                                Data = data,
                                                Length = data.Length,
                                                Socket = socket,
                                            };
                                            OnNewPacket?.Invoke(this, packet);


                                            if (!Data.Connections.ContainsKey(socket))
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
                                                Data.Connections.TryAdd(connection.SocketId, connection);

                                                SocketInfo socketInfo = new SocketInfo()
                                                {
                                                    SocketId = socket
                                                };
                                                socketInfo.OnResponse += (object o, JObject resp) =>
                                                { 
                                                    Data.Connections.TryAdd(socket, connection);
                                                    OnNewConnection?.Invoke(this, connection); // 
                                                    Console.WriteLine($"Added previously opened socket {socket}");
                                                };

                                                Send(socketInfo);
                                            }
                                        }

                                    }
                                    else if (messageType == ServerMessageType.JOB_RESPONSE)
                                    {
                                        Guid guid = Guid.Parse(json.Value<String>("jobId"));
                                        jobs[guid].NotifyResponse(json);
                                        jobs.Remove(guid);
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
                            }

                            FlushOutBuffer(clientStream);
                            Thread.Sleep(20);
                        }
                        Console.WriteLine("Closing server...");

                        if (serverStream.IsConnected)
                        {
                            Send(new ShutdownSpy());
                            FlushOutBuffer(clientStream);
                        }
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

                    if (clientStream.IsConnected)
                        clientStream.Close();
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
        }

        private void FlushOutBuffer(NamedPipeClientStream clientStream)
        {
            if (clientStream == null)
            {
                Console.WriteLine("Client Stream is null. Cannot send message(s) to Spy.");
                return;
            }

            while (outBuffer.TryDequeue(out byte[] data))
            {
                clientStream.Write(data, 0, data.Length);
            }
        }
    }
}
