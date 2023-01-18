using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOPE_UI.Spy.DispatcherMessageType
{
    // This now calls closesocket directly
    public class CloseSocket : IMessage
    {
        public int SocketId { get; set; }

        public CloseSocket()
        {
            Type = Model.SpyMessageType.CLOSE_SOCKET;
        }
    }
}
