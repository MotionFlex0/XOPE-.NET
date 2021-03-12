using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOPE_UI.Spy.Type
{
    public class Packet
    {
        public int Socket { get; set; }
        public byte[] Data { get; set; }
        public int Length { get; set; }
    }
}
