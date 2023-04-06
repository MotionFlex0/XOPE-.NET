using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOPE_UI.Spy.DispatcherMessageType
{
    public class RequestSocketInfo : MessageWithResponseImpl
    {
        public int SocketId { get; set; }

        public RequestSocketInfo()
        {
            Type = Model.SpyMessageType.REQUEST_SOCKET_INFO;
        }
    }
}
