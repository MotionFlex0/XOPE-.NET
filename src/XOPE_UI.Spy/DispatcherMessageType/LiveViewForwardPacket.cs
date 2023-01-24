namespace XOPE_UI.Spy.DispatcherMessageType
{
    public class LiveViewForwardPacket : IMessage
    {
        public byte[] Data { get; set; }

        public LiveViewForwardPacket()
        {
            Type = Model.SpyMessageType.LIVE_VIEW_FORWARD_PACKET;
        }
    }
}
