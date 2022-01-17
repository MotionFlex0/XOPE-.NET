using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOPE_UI.Model;
using XOPE_UI.Spy.Type;

namespace XOPE_UI.Spy.DispatcherMessageType
{
    public class DeletePacketFilter : IMessageWithResponse
    {
        public string FilterId { get; set; }

        public DeletePacketFilter()
        {
            Type = SpyMessageType.DELETE_PACKET_FILTER;
        }

        public DeletePacketFilter(EventHandler<IncomingMessage> callback) : this()
        {
            OnResponse += callback;
        }
    }
}
