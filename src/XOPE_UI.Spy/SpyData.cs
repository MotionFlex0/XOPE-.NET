using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOPE_UI.Definitions;

namespace XOPE_UI.Spy
{
    public class SpyData
    {
        public SynchronizedCollection<Connection> Connections { get; set; } = new SynchronizedCollection<Connection>();
        public SynchronizedCollection<Packet> Packets { get; set; } = new SynchronizedCollection<Packet>();
    }
}
