using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOPE_UI.Definitions
{
    public class Packet
    {
        public Guid Id { get; set; }
        public int Socket { get; set; }
        public byte[] Data { get; set; }
        public int Length { get; set; }
        public HookedFuncType Type { get; set; }
        public bool Modified { get; set; } = false;
    }
}
