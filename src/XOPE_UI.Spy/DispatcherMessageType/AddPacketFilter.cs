using Newtonsoft.Json.Linq;
using System;
using XOPE_UI.Model;
using XOPE_UI.Spy.Type;

namespace XOPE_UI.Spy.DispatcherMessageType
{
    public class AddPacketFilter : IMessageWithResponse
    {
        public int SocketId { get; set; }
        public ReplayableFunction PacketType { get; set; }
        public byte[] OldValue { get; set; }
        public byte[] NewValue { get; set; }
        public bool RecursiveReplace { get; set; } 
        public bool ReplaceEntirePacket { get; set; } = false; // true = replaces entire packet with NewValue, if OldValue is found. (default is false)
        public bool Activated { get; set; } = true;

        public AddPacketFilter()
        {
            Type = SpyMessageType.ADD_PACKET_FITLER;
        }

        public AddPacketFilter(EventHandler<IncomingMessage> callback) : this()
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
