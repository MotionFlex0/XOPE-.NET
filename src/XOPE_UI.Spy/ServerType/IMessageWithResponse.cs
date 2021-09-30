using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOPE_UI.Spy.ServerType
{
    public abstract class IMessageWithResponse : IMessage
    {
        public event EventHandler<JObject> OnResponse;

        [JsonProperty]
        public Guid JobId { get; } = Guid.NewGuid();

        public void NotifyResponse(JObject resposne) =>
            OnResponse?.Invoke(this, resposne);
    }
}
