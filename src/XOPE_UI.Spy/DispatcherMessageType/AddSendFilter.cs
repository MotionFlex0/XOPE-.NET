using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOPE_UI.Spy.DispatcherMessageType
{
    public class AddSendFilter : IMessageWithResponse
    {
        
        public int SocketId { get; set; }
        public byte[] OldValue { get; set; }
        public byte[] NewValue { get; set; }
        public bool ReplaceEntirePacket { get; set; } = false; // true = replaces entire packet with NewValue, if OldValue is found. (default is false)

        public AddSendFilter()
        {
            Type = Definitions.SpyMessageType.ADD_SEND_FITLER;
        }

        public AddSendFilter(EventHandler<JObject> callback) : this()
        {
            OnResponse += callback;
        }

        public override JObject ToJson()
        {
            JObject json = base.ToJson();
            json["OldValue"] = Convert.ToBase64String(OldValue);
            json["OldValueLength"] = OldValue.Length;
            json["NewValue"] = Convert.ToBase64String(NewValue);
            json["NewValueLength"] = NewValue.Length;
            return json;
        }
    }
}
