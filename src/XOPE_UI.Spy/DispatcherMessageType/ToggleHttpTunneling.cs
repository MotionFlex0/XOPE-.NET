using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOPE_UI.Spy.DispatcherMessageType
{
    public class ToggleHttpTunneling : MessageImpl
    {
        public bool IsTunnelingEnabled { get; set; }

        public ToggleHttpTunneling()
        {
            Type = Model.SpyMessageType.TOGGLE_HTTP_TUNNELING;
        }
    }
}
