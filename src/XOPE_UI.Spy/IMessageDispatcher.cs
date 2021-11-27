using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOPE_UI.Spy.DispatcherMessageType;

namespace XOPE_UI.Spy
{
    public interface IMessageDispatcher
    {
        bool IsConnected { get; }

        void Send(IMessage message);
        void Send(IMessageWithResponse message);
        void ShutdownAndWait();
    }
}
