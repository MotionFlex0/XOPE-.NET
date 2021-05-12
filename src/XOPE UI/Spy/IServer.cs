﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOPE_UI.Definitions;
using XOPE_UI.Spy.ServerType;

namespace XOPE_UI.Spy
{
    public interface IServer
    {
        event EventHandler<Packet> OnNewPacket;
        event EventHandler<Connection> OnNewConnection;
        event EventHandler<Connection> OnCloseConnection;

        SpyData Data { get; }

        void RunAsync();
        void Send(IMessage message);
    }
}
