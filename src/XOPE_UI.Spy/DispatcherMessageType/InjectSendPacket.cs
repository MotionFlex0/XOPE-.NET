﻿using Newtonsoft.Json.Linq;
using System;

namespace XOPE_UI.Spy.DispatcherMessageType
{
    // Works with both send and WSASend
    public class InjectSendPacket : MessageImpl
    {
        public byte[] Data { get; set; }
        public int SocketId { get; set; }
        // Length : Set by ToJson

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
