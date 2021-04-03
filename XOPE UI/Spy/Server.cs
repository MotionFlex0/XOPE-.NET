using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XOPE_UI.Core;
using XOPE_UI.Definitions;
using XOPE_UI.Spy.Type;

namespace XOPE_UI.Spy
{
    //TODO: this is kinda weird so maybe just remove it and put it back into the mainWindow class;
    //thoough, all some sort of translation class for incomming data
    class Server
    {
        public event EventHandler<Packet> OnNewPacket;
        public event EventHandler<Connection> OnNewConnection;
        public event EventHandler<Connection> OnCloseConnection;
        
        NamedPipeServerStream serverStream = null;
        TextBox outputBox;

        SpyData spyData;


        public Server(TextBox logOutputBox, SpyData sd)
        {
            outputBox = logOutputBox;
            spyData = sd;
        }


        public void sendPacket<T1, T2, T3>(string packetType, T1 arg1, T2 arg2, T3 arg3)
        {

        }

        public void fakeRecv()
        {

        }

        public void runASync()
        {
            serverStream = new NamedPipeServerStream("xopespy");
            
            Task.Factory.StartNew(() => {
                serverStream.WaitForConnection();
                byte[] buffer = new byte[65536];
                while (serverStream.IsConnected)
                {
                    int len = serverStream.Read(buffer, 0, 65536);
                    if (len > 0)
                    {
                        JObject o = JObject.Parse(System.Text.Encoding.UTF8.GetString(buffer, 0, len));
                        switch ((SpyPacketType)o.Value<Int64>("messageType"))
                        {
                            case SpyPacketType.HOOKED_FUNCTION_CALL:
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

                                        Timer t = new Timer();
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
                                    byte[] data = Convert.FromBase64String(o.Value<String>("packetData"));
                                    Packet packet = new Packet
                                    {
                                        Type = type,
                                        Data = data,
                                        Length = data.Length,
                                        Socket = o.Value<int>("socket"),
                                    };
                                    OnNewPacket?.Invoke(this, packet);
                                }

                                //spyData.Packets

                                //outputBox.AppendText($"\r\n{(SpyPacketType)o.Value<Int32>("messageType")} | {(HookedFuncType)o.Value<Int32>("functionName")}\r\n");
                                //outputBox.AppendText(o.ToString() + "\r\n");

                            

                                break;
                        }
                    }
                    
                }
            }, System.Threading.CancellationToken.None, TaskCreationOptions.DenyChildAttach | TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
    }
}
