using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOPE_UI.Definitions
{
    public class FilterEntry
    {
        public string Name { get; set; }
        public byte[] Before { get; set; }
        public byte[] After { get; set; }
        public int SocketId { get; set; }
    }
}
