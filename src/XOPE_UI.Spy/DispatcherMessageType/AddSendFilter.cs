using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOPE_UI.Spy.DispatcherMessageType
{
    class AddSendFilter : IMessageWithResponse
    {
        
        public int SocketId { get; set; }
        public byte[] OldValue { get; set; }
        public byte[] NewValue { get; set; }
        public bool ReplaceEntirePacket { get; set; } // true = replaces entire packet with NewValue, if OldValue is found

        public AddSendFilter()
        {
            Type = Definitions.SpyMessageType.ADD_SEND_FITLER;
        }

        public override JObject ToJson()
        {
            JObject json = base.ToJson();
            json["SocketId"] = SocketId;
            json["OldValue"] = Convert.ToBase64String(OldValue);
            json["OldValueLength"] = OldValue.Length;
            json["NewValue"] = Convert.ToBase64String(NewValue);
            json["NewValueLength"] = NewValue.Length;
            json["ReplaceEntirePacket"] = ReplaceEntirePacket;
            return json;
        }
    }
}
