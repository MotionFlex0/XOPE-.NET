using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOPE_UI.Model;

namespace XOPE_UI.Spy.DispatcherMessageType.JobResponse
{
    public abstract class MessageResponseImpl : MessageImpl
    {
        [JsonProperty]
        SpyJobResponseType JobResponseType { get; }
        [JsonProperty]
        Guid JobId;

        public MessageResponseImpl(SpyMessageType type, SpyJobResponseType jobResponseType, Guid jobId)
        {
            Type = type;
            JobResponseType = jobResponseType;
            JobId = jobId;
        }
    }
}
