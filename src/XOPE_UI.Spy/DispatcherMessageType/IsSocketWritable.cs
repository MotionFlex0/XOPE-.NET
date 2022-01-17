using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOPE_UI.Spy.Type;

namespace XOPE_UI.Spy.DispatcherMessageType
{
    public class IsSocketWritable : IMessageWithResponse
    {
        public int SocketId { get; set; }

        public IsSocketWritable()
        {
            Type = Model.SpyMessageType.IS_SOCKET_WRITABLE;
        }

        public IsSocketWritable(EventHandler<IncomingMessage> onResponseCallback) : this()
        {
            OnResponse += onResponseCallback;
        }
    }
}
