using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOPE_UI.Spy
{
    public interface IServer
    {
        void RunAsync();
        void Send(IMessage message);
    }
}
