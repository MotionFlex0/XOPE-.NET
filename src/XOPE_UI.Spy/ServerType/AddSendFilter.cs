using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOPE_UI.Spy.ServerType
{
    class AddSendFilter : IMessage
    {
        
        public int SocketId { get; set; }
        public byte[] Before { get; set; }
        public byte[] After { get; set; }
        public bool ReplaceWithAfter { get; set; } // true = replace a packet with matching Before, with After 

        public AddSendFilter()
        {
            Type = Definitions.SpyMessageType.ADD_SEND_FITLER;
        }

        public override JObject ToJson()
        {
            JObject json = base.ToJson();
            json["SocketId"] = SocketId;
            json["OldValue"] = Convert.ToBase64String(Before);
            json["OldValueLength"] = Before.Length;
            json["NewValue"] = Convert.ToBase64String(After);
            json["NewValueLength"] = After.Length;
            json["ReplaceWithAfter"] = ReplaceWithAfter;
            return json;
        }
    }
}
