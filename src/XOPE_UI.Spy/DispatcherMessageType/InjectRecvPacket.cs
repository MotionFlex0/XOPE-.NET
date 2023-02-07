using Newtonsoft.Json.Linq;
using System;

namespace XOPE_UI.Spy.DispatcherMessageType
{
    // Works with both recv and WSARecv
    public class InjectRecvPacket : MessageImpl
    {
        public byte[] Data { get; set; }
        public int SocketId { get; set; }

        public InjectRecvPacket()
        {
            Type = Model.SpyMessageType.INJECT_RECV;
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
