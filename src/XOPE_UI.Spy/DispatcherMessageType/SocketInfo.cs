using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOPE_UI.Spy.DispatcherMessageType
{
    public class SocketInfo : IMessageWithResponse
    {
        public int SocketId { get; set; }

        public SocketInfo()
        {
            Type = Definitions.SpyMessageType.REQUEST_SOCKET_INFO;
        }
    }
}
