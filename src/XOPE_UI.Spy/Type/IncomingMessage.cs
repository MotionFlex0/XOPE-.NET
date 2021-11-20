using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOPE_UI.Definitions;

namespace XOPE_UI.Spy.Type
{
    public record IncomingMessage
    {
        public ServerMessageType Type { get; }
        public JObject Json { get; }

        public IncomingMessage(ServerMessageType type, JObject json)
        {
            Type = type;
            Json = json;
        }
    }
}
