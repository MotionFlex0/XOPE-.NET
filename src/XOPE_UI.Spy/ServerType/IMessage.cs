using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using PeterO.Cbor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOPE_UI.Definitions;

namespace XOPE_UI.Spy.ServerType
{
    // TODO: Change this to an interface when C# version > 8.0
    public abstract class IMessage
    {
        [JsonProperty]
        protected SpyMessageType Type { get; set; }

        public virtual JObject ToJson()
        {
            //CBORObject.FromObject(this).EncodeToBytes
            
            return JObject.FromObject(this);
        }
    }
}
