using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XOPE_UI.Model;

namespace XOPE_UI.Spy.DispatcherMessageType
{
    public class ModifyPacketFilter : MessageWithResponseImpl
    {
        public FilterEntry Filter { get; set; }

        public ModifyPacketFilter()
        {
            Type = SpyMessageType.MODIFY_PACKET_FILTER;
        }

        public override JObject ToJson()
        {
            JObject json = base.ToJson();
            JProperty property = json.Property("Filter");
            json.Add(property.Value.Children<JProperty>());
            property.Remove();
            return json;
        }
    }
}
