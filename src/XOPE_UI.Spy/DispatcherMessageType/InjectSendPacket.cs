using Newtonsoft.Json.Linq;
using System;

namespace XOPE_UI.Spy.DispatcherMessageType
{
    public class InjectSendPacket : IMessage
    {
        public byte[] Data { get; set; }
        public int SocketId { get; set; }

        public InjectSendPacket()
        {
            Type = Model.SpyMessageType.INJECT_SEND;
        }

        public override JObject ToJson()
        {
            JObject json = base.ToJson();
            json["Data"] = Convert.ToBase64String(Data);
            json["Length"] = Data.Length;
            return json;
        }
    }
}
