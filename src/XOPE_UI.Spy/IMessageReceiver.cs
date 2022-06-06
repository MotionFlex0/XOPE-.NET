using System;
using XOPE_UI.Model;
using XOPE_UI.Spy.DispatcherMessageType;

namespace XOPE_UI.Spy
{
    public interface IMessageReceiver
    {
        bool IsConnected { get; }

        // Not sure where to put this paramater
        void RunAsync(string receiverName);
        void ShutdownAndWait();
    }
}
