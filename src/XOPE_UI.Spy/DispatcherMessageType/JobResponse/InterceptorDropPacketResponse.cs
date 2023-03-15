using Newtonsoft.Json;
using System;

namespace XOPE_UI.Spy.DispatcherMessageType.JobResponse
{
    public class InterceptorDropPacketResponse : MessageResponseImpl
    {
        public InterceptorDropPacketResponse(Guid jobId) : base(Model.SpyMessageType.JOB_RESPONSE_SUCCESS,
                Model.SpyJobResponseType.LIVE_VIEW_DROP_PACKET, jobId)
        {
        }
    }
}
