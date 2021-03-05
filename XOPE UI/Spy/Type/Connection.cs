using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace XOPE_UI.Spy
{
    public class Connection
    {
        public int SocketId { get; private set; }
        public int Protocol { get; private set; } //TODO: once implemented, change type to ProtocolType
        public AddressFamily IPFamily { get; private set; }
        public IPAddress IP { get; private set; }
        public int Port { get; private set; }
        public Status SocketStatus { get; set; }
        public DateTime LastStatusChangeTime { get; set; }

        public Connection(int id, int protocol, int addrFamily, IPAddress ip, int port, Status status)
        {
            
            SocketId = id;
            Protocol = protocol;
            IPFamily = (AddressFamily)addrFamily;
            IP = ip;
            Port = port;
            SocketStatus = status;
            LastStatusChangeTime = DateTime.Now;
        }

        public override bool Equals(object obj)
        {
            if (obj != null || this.GetType().Equals(obj.GetType()))
                return false;
            return ((Connection)obj).SocketId == this.SocketId;
        }

        public override int GetHashCode()
        {
            return this.SocketId;
        }

        public enum Status
        {
            CONNECTING,
            ESTABLISHED,
            CLOSED
        }
    }
}
