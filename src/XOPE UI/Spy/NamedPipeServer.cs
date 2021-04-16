using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using PeterO.Cbor;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using XOPE_UI.Core;
using XOPE_UI.Definitions;
using XOPE_UI.Native;
using XOPE_UI.Spy.Type;

namespace XOPE_UI.Spy
{
    //TODO: this is kinda weird so maybe just remove it and put it back into the mainWindow class;
    //thoough, all some sort of translation class for incomming data
    class NamedPipeServer : IServer
    {
        public event EventHandler<Packet> OnNewPacket;
        public event EventHandler<Connection> OnNewConnection;
        public event EventHandler<Connection> OnCloseConnection;
        
        NamedPipeServerStream serverStream = null;
        TextBox outputBox;

        SpyData spyData;

        ConcurrentQueue<byte[]> outBuffer;

        public NamedPipeServer(TextBox logOutputBox, SpyData sd)
        {
            outputBox = logOutputBox;
            spyData = sd;
            outBuffer = new ConcurrentQueue<byte[]>();
        }


        public void Send(IMessage message)
        {
            outBuffer.Enqueue(Encoding.ASCII.GetBytes(message.ToJson())); //TODO: Convert to bson
        }

        public void RunAsync()
        {
            serverStream = new NamedPipeServerStream("xopespy");

            Task.Factory.StartNew(() => {
                serverStream.WaitForConnection();
                Console.WriteLine("Server connected to Spy");


                byte[] buffer = new byte[65536];
                while (serverStream.IsConnected)
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
                            //JObject o = JObject.Parse(Encoding.ASCII.GetString(buffer, 0, len));
                            JObject o = JObject.Parse(cbor.ToString());
                            Console.WriteLine($"Incoming message: {(ServerPacketType)o.Value<Int32>("messageType")}");
                            Console.WriteLine($"Entire message len: {len}");
                            Console.WriteLine(o.ToString());
                            switch ((ServerPacketType)o.Value<Int64>("messageType"))
                            {
                                case ServerPacketType.HOOKED_FUNCTION_CALL:
                                    HookedFuncType type = (HookedFuncType)o.Value<Int64>("functionName");
                                    if (type == HookedFuncType.CONNECT || type == HookedFuncType.WSACONNECT)
                                    {
                                        Connection connection = new Connection(o.Value<Int32>("socket"),
                                            o.Value<Int32>("protocol"),
                                            o.Value<Int32>("addrFamily"),
                                            new IPAddress(o.Value<Int32>("addr")),
                                            o.Value<Int32>("port"),
                                            Connection.Status.ESTABLISHED);
                                        spyData.Connections.Add(connection);
                                        OnNewConnection?.Invoke(this, connection);
                                        Console.WriteLine("new connection");
                                    }
                                    else if (type == HookedFuncType.CLOSE)
                                    {
                                        int socketId = o.Value<Int32>("socket");
                                        Connection matchingConnection = spyData.Connections.FirstOrDefault((Connection c) => c.SocketId == socketId);
                                        if (matchingConnection != null && matchingConnection.SocketStatus != Connection.Status.CLOSED)
                                        {
                                            matchingConnection.SocketStatus = Connection.Status.CLOSED;
                                            matchingConnection.LastStatusChangeTime = DateTime.Now;

                                            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
                                            t.Tick += (object sender, EventArgs e) =>
                                            {
                                                bool exists = spyData.Connections.Contains(matchingConnection);
                                                if (exists)
                                                    spyData.Connections.Remove(matchingConnection);
                                            };

                                            t.Interval = 7000;
                                            t.Start();
                                            OnCloseConnection?.Invoke(this, matchingConnection);
                                        } 
                                    }
                                    else
                                    {
                                        byte[] data = Convert.FromBase64String(o.Value<String>("packetDataB64"));
                                        Packet packet = new Packet
                                        {
                                            Type = type,
                                            Data = data,
                                            Length = data.Length,
                                            Socket = o.Value<int>("socket"),
                                        };
                                        OnNewPacket?.Invoke(this, packet);
                                    }

                                    //spyData.
                                    break;
                            }

                            //outputBox.Invoke((MethodInvoker)(() => outputBox.AppendText($"\r\n{(ServerPacketType)o.Value<Int32>("messageType")}\r\n")));
                            //outputBox.Invoke((MethodInvoker)(() => outputBox.AppendText(o.ToString() + "\r\n")));
                        }
                    }

                    FlushOutBuffer();
                    Thread.Sleep(100);
                }
                Console.WriteLine("Closing server...");

                
            }, System.Threading.CancellationToken.None, TaskCreationOptions.DenyChildAttach | TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
        private void FlushOutBuffer()
        {
            while (outBuffer.TryDequeue(out byte[] data))
            {
                Console.WriteLine("Sending data in FlushOutBuffer...");
                serverStream.Write(data, 0, data.Length);
            }
        }
    }
}
