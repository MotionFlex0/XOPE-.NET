using System;
using XOPE_UI.Model;
using XOPE_UI.Spy.Type;

namespace XOPE_UI.Spy.DispatcherMessageType
{
    public class ToggleActivatePacketFilter : MessageWithResponseImpl
    {
        public string FilterId { get; set; }
        public bool Activated { get; set; }

        public ToggleActivatePacketFilter()
        {
            Type = SpyMessageType.TOGGLE_ACTIVATE_FILTER;
        }

        public ToggleActivatePacketFilter(EventHandler<IncomingMessage> callback) : this()
        {
            OnResponse += callback;
        }
    }
}
