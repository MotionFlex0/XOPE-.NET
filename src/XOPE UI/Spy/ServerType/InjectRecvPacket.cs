﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOPE_UI.Spy.ServerType
{
    //NOT IMPLEMENTED IN SPY
    class InjectRecvPacket : IMessage
    {
        public byte[] Data { get; set; }
        public int SocketId { get; set; }

        public InjectRecvPacket()
        {
            Type = Definitions.SpyMessageType.INJECT_RECV;
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