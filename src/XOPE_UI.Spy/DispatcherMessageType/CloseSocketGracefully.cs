using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOPE_UI.Spy.DispatcherMessageType
{
    public class CloseSocketGracefully : IMessage
    {
        public int SocketId { get; set; }

        public CloseSocketGracefully()
        {
            Type = Model.SpyMessageType.CLOSE_SOCKET_GRACEFULLY;
        }
    }
}
