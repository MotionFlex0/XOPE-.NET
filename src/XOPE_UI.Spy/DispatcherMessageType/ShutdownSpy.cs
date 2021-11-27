using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOPE_UI.Spy.DispatcherMessageType
{
    public class ShutdownSpy : IMessage
    {
        public ShutdownSpy()
        {
            Type = Definitions.SpyMessageType.SHUTDOWN_RECV_THREAD;
        }
    }
}
