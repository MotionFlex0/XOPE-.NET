using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOPE_UI.Spy.DispatcherMessageType
{
    public class Ping : IMessageWithResponse
    {
        public Ping()
        {
            Type = Model.SpyMessageType.PING;
        }
    }
}
