using System;
using System.Collections.Generic;
using System.Text;
using XOPE_UI.Definitions;

namespace XOPE_UI.Spy
{
    public class PacketEventArgs : EventArgs
    {
        public Packet Packet { get; set; }
    }
}
