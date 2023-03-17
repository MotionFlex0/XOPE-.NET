using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOPE_UI.Spy.DispatcherMessageType
{
    public class ToggleInterceptor : MessageImpl
    {
        public bool IsInterceptorEnabled { get; set; }

        public ToggleInterceptor()
        {
            Type = Model.SpyMessageType.TOGGLE_INTERCEPTOR;
        }
    }
}
