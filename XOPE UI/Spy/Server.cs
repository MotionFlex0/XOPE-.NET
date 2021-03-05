﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

namespace XOPE_UI.Spy
{
    //TODO: this is kinda weird so maybe just remove it and put it back into the mainWindow class;
    //thoough, all some sort of translation class for incomming data
    class Server
    {
        PacketList livePacketList = null;
        NamedPipeServerStream serverStream = null;
        TextBox outputBox;

        SpyData spyData;


        public Server(PacketList pl, TextBox logOutputBox, SpyData sd)
        {
            livePacketList = pl;
            outputBox = logOutputBox;
            spyData = sd;
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
                        switch ((ClientPacketType)o.Value<Int64>("messageType"))
                        {
                            case ClientPacketType.HOOKED_FUNCTION_CALL:
                                HookedFuncType type = (HookedFuncType)o.Value<Int64>("functionName");
                                if (type == HookedFuncType.CONNECT || type == HookedFuncType.WSACONNECT)
                                {
                                    spyData.Connections.Add(new Connection(o.Value<Int32>("socket"), 
                                        o.Value<Int32>("protocol"),
                                        o.Value<Int32>("addrFamily"),
                                        new IPAddress(o.Value<Int32>("addr")),
                                        o.Value<Int32>("port"),
                                        Connection.Status.ESTABLISHED));
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
                                        t.Tick += new EventHandler((object sender, EventArgs e) =>
                                        {
                                            bool exists = spyData.Connections.Contains(matchingConnection);
                                            if (exists)
                                                spyData.Connections.Remove(matchingConnection);
                                        });

                                        t.Interval = 7000;
                                        t.Start();
                                    } 
                                }
                                else
                                {
                                    int socketId = o.Value<Int32>("socket");
                                    byte[] data = Convert.FromBase64String(o.Value<String>("packetData"));
                                    livePacketList.add(type, socketId, data);
                                }

                                //spyData.Packets

                                outputBox.AppendText($"\r\n{(ClientPacketType)o.Value<Int32>("messageType")} | {(HookedFuncType)o.Value<Int32>("functionName")}\r\n");
                                outputBox.AppendText(o.ToString() + "\r\n");

                            

                                break;
                        }
                    }
                    
                }
            }, System.Threading.CancellationToken.None, TaskCreationOptions.DenyChildAttach | TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
    }
}
