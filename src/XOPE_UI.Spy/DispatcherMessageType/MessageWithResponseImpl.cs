using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOPE_UI.Spy.Type;

namespace XOPE_UI.Spy.DispatcherMessageType
{
    public abstract class MessageWithResponseImpl : MessageImpl
    {
        public event EventHandler<IncomingMessage> OnResponse;

        [JsonProperty]
        public Guid JobId { get; } = Guid.NewGuid();

        public void NotifyResponse(IncomingMessage resposne) =>
            OnResponse?.Invoke(this, resposne);

    }
}
