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
        public byte[] OldValue { get; set; }
        public byte[] NewValue { get; set; }
        public int SocketId { get; set; }
        public ReplayableFunction PacketType { get; set; }
        public string FilterId { get; set; }
        public bool RecursiveReplace { get; set; }
    }
}
