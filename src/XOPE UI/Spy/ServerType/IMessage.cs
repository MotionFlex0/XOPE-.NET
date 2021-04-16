using Newtonsoft.Json;
using PeterO.Cbor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOPE_UI.Definitions;

namespace XOPE_UI.Spy
{
    public class IMessage
    {
        protected SpyPacketType Type { get; set; }
        public string ToJson()
        {
            //CBORObject.FromObject(this).EncodeToBytes();
            return JsonConvert.SerializeObject(this);
        }
    }
}
