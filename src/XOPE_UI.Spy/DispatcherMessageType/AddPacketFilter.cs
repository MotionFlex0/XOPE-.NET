using Newtonsoft.Json.Linq;
using System;
using XOPE_UI.Model;
using XOPE_UI.Spy.Type;

namespace XOPE_UI.Spy.DispatcherMessageType
{
    // TODO: Remove all the props from this class and instead get them from the underlying 
    //       filter object, like ModifyPacketFilter does
    public class AddPacketFilter : IMessageWithResponse
    {
        public int SocketId { get; set; }
        public ReplayableFunction PacketType { get; set; }
        public byte[] OldValue { get; set; }
        public byte[] NewValue { get; set; }
        public bool RecursiveReplace { get; set; } 
        public bool ReplaceEntirePacket { get; set; } = false; // true = replaces entire packet with NewValue, if OldValue is found. (default is false)
        public bool Activated { get; set; } = true;
        public bool DropPacket { get; set; } = false;

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
