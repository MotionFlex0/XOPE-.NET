﻿using System;
using XOPE_UI.Definitions;
using XOPE_UI.Spy.DispatcherMessageType;

namespace XOPE_UI.Spy
{
    public interface IMessageReceiver
    {
        bool IsConnected { get; }

        void RunAsync();
        void ShutdownAndWait();
    }
}
