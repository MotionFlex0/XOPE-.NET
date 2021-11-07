﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOPE_UI.Spy.ServerType
{
    public class IsSocketWritable : IMessageWithResponse
    {
        public int SocketId { get; set; }

        public IsSocketWritable()
        {
            Type = Definitions.SpyMessageType.IS_SOCKET_WRITABLE;
        }

        public IsSocketWritable(EventHandler<JObject> onResponseCallback) : this()
        {
            OnResponse += onResponseCallback;
        }
    }
}
