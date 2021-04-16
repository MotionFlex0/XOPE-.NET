using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOPE_UI.Spy.ServerType
{
    class InjectSendPacket : IMessage
    {
        public byte[] Data { get; set; }
        public int SocketId { get; set; }

        public InjectSendPacket()
        {
            Type = Definitions.SpyPacketType.INJECT_SEND;
        }
    }
}
