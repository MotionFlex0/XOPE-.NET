namespace XOPE_UI.Spy.DispatcherMessageType
{
    public class LiveViewDropPacket : IMessage
    {
        public byte[] Data { get; set; }

        public LiveViewDropPacket()
        {
            Type = Model.SpyMessageType.LIVE_VIEW_DROP_PACKET;
        }
    }
}
