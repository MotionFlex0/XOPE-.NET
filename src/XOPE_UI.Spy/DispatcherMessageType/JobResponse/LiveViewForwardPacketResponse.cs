using System;

namespace XOPE_UI.Spy.DispatcherMessageType.JobResponse
{
    // The ctor is used here to set the relevant fields passed instead of props.
    // Just testing if this looks better compared to using props
    public class LiveViewForwardPacketResponse : MessageResponseImpl
    {
        public byte[] Data { get; }

        public LiveViewForwardPacketResponse(Guid jobId, byte[] data) :
            base(Model.SpyMessageType.JOB_RESPONSE_SUCCESS,
                Model.SpyJobResponseType.LIVE_VIEW_FORWARD_PACKET, jobId)
        {
            Data = data;
        }
    }
}
