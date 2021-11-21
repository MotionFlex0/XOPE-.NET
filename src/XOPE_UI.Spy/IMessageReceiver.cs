using System;
using XOPE_UI.Definitions;
using XOPE_UI.Spy.ServerType;

namespace XOPE_UI.Spy
{
    public interface IMessageReceiver
    {
        bool IsConnected { get; }

        void RunAsync();
        void ShutdownServerAndWait();
    }
}
