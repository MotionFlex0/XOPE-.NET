using Newtonsoft.Json.Linq;
using PeterO.Cbor;
using System;
using System.Collections.Concurrent;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XOPE_UI.Definitions;
using XOPE_UI.Native;
using XOPE_UI.Spy.ServerType;

namespace XOPE_UI.Spy
{
    //TODO: this is kinda weird so maybe just remove it and put it back into the mainWindow class;
    //though, all some sort of translation class for incomming data
    public class NamedPipeServer : IServer
    {
        public event EventHandler<Packet> OnNewPacket;
        public event EventHandler<Connection> OnNewConnection;
        public event EventHandler<Connection> OnCloseConnection;
        
        public SpyData Data { get; private set; }
        
        ConcurrentQueue<byte[]> outBuffer;
        CancellationTokenSource cancellationTokenSource;
        Task serverThread; // Change this to something easier to follow

        public NamedPipeServer()
        {
            Data = new SpyData();

            outBuffer = new ConcurrentQueue<byte[]>();
        }

        public void Send(IMessage message)
        {
            outBuffer.Enqueue(Encoding.ASCII.GetBytes(message.ToJson().ToString())); //TODO: Convert to bson
        }

        public void RunAsync() 
        {
            if (serverThread != null)
            {
                Console.WriteLine("Cannot start new server thread, as one already exists");
                return;
            }

            cancellationTokenSource = new CancellationTokenSource();

            serverThread = Task.Factory.StartNew(() => {
                using (NamedPipeServerStream serverStream = new NamedPipeServerStream("xopespy"))
                {
                    serverStream.WaitForConnection();
                    Console.WriteLine("Server connected to Server");

                    try
                    {
                        byte[] buffer = new byte[65536];
                        while (serverStream.IsConnected && !cancellationTokenSource.IsCancellationRequested)
                        {
                            byte[] _;
                            int __, ___;
                            int bytesAvailable;

                            Win32API.PeekNamedPipe((IntPtr)serverStream.SafePipeHandle.DangerousGetHandle(), out _, 0, out __, out bytesAvailable, out ___);
                            if (bytesAvailable > 0)
                            {
                                int len = serverStream.Read(buffer, 0, 65536);
                                if (len > 0)
                                {
                                    CBORObject cbor = CBORObject.DecodeFromBytes((new ArraySegment<byte>(buffer, 0, len)).ToArray());
                                    //JObject json = JObject.Parse(Encoding.ASCII.GetString(buffer, 0, len));
                                    JObject json = JObject.Parse(cbor.ToString());
                                    Console.WriteLine($"Incoming message: {(ServerMessageType)json.Value<Int32>("messageType")}");

                                    ServerMessageType messageType = (ServerMessageType)json.Value<Int64>("messageType");

                                    if (messageType == ServerMessageType.HOOKED_FUNCTION_CALL)
                                    {
                                        HookedFuncType hookedFuncType = (HookedFuncType)json.Value<Int64>("functionName");
                                        if (hookedFuncType == HookedFuncType.CONNECT || hookedFuncType == HookedFuncType.WSACONNECT)
                                        {
                                            Connection connection = new Connection(json.Value<Int32>("socket"),
                                                json.Value<Int32>("protocol"),
                                                json.Value<Int32>("addrFamily"),
                                                new IPAddress(json.Value<Int32>("addr")),
                                                json.Value<Int32>("port"),
                                                Connection.Status.ESTABLISHED);
                                            Data.Connections.TryAdd(json.Value<Int32>("socket"), connection);
                                            OnNewConnection?.Invoke(this, connection);
                                            Console.WriteLine("new connection");
                                        }
                                        else if (hookedFuncType == HookedFuncType.CLOSE)
                                        {
                                            int socketId = json.Value<Int32>("socket");
                                            Connection matchingConnection = Data.Connections[socketId]; //Data.Connections.FirstOrDefault((Connection c) => c.SocketId == socketId);

                                            if (matchingConnection != null && matchingConnection.SocketStatus != Connection.Status.CLOSED)
                                            {
                                                matchingConnection.SocketStatus = Connection.Status.CLOSED;
                                                matchingConnection.LastStatusChangeTime = DateTime.Now;

                                                System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
                                                t.Tick += (object sender, EventArgs e) =>
                                                {
                                                    Data.Connections.TryRemove(matchingConnection.SocketId, out Connection v);
                                                    t.Stop();
                                                };

                                                t.Interval = 7000;
                                                t.Start();
                                                OnCloseConnection?.Invoke(this, matchingConnection);
                                            }
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
                                                Socket = json.Value<int>("socket"),
                                            };
                                            OnNewPacket?.Invoke(this, packet);
                                        }

                                    }
                                    else if (messageType == ServerMessageType.ERROR_MESSAGE)
                                    {
                                        Console.WriteLine($"[Error] {json.Value<String>("errorMessage")}");
                                    }
                                }
                            }

                            FlushOutBuffer(serverStream);
                            Thread.Sleep(20);
                        }
                        Console.WriteLine("Closing server...");

                        if (serverStream.IsConnected)
                        {
                            Send(new ShutdownSpy());
                            FlushOutBuffer(serverStream);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Server error. Message: {ex.Message}");
                    }
                }
            }, cancellationTokenSource.Token, TaskCreationOptions.DenyChildAttach | TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void ShutdownServerAndWait()
        {
            cancellationTokenSource.Cancel();
            serverThread.Wait(5000);
            serverThread = null;
        }

        private void FlushOutBuffer(NamedPipeServerStream serverStream)
        {
            while (outBuffer.TryDequeue(out byte[] data))
            {
                Console.WriteLine("Sending data in FlushOutBuffer...");
                serverStream.Write(data, 0, data.Length);
            }
        }
    }
}
