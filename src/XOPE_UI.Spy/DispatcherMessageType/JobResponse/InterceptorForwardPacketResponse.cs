using Newtonsoft.Json.Linq;
using System;

namespace XOPE_UI.Spy.DispatcherMessageType.JobResponse
{
    // The ctor is used here to set the relevant fields passed instead of props.
    // Just testing if this looks better compared to using props
    public class InterceptorForwardPacketResponse : MessageResponseImpl
    {
        public byte[] Data { get; }
        // Length : Set by ToJson

        public InterceptorForwardPacketResponse(Guid jobId, byte[] data) :
            base(Model.SpyMessageType.JOB_RESPONSE_SUCCESS,
                Model.SpyJobResponseType.LIVE_VIEW_FORWARD_PACKET, jobId)
        {
            Data = data;
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
