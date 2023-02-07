using Newtonsoft.Json;
using System;

namespace XOPE_UI.Spy.DispatcherMessageType.JobResponse
{
    // The ctor is used here to set the relevant fields passed instead of props.
    // Just testing if this looks better compared to using props
    public class LiveViewDropPacketResponse : MessageResponseImpl
    {
        [JsonProperty]
        byte[] Data { get; set; }

        public LiveViewDropPacketResponse(Guid jobId) : base(Model.SpyMessageType.JOB_RESPONSE_SUCCESS,
                Model.SpyJobResponseType.LIVE_VIEW_DROP_PACKET, jobId)
        {
        }
    }
}
