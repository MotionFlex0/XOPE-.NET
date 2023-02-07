using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOPE_UI.Spy.DispatcherMessageType
{
    public class ShutdownSpy : MessageImpl
    {
        public ShutdownSpy()
        {
            Type = Model.SpyMessageType.SHUTDOWN_RECV_THREAD;
        }
    }
}
