using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace XOPE_UI.Model
{
    // TODO: Use INotifyChanged
    public class Connection : EventArgs
    {
        private Status status;

        public int SocketId { get; private set; }
        public int Protocol { get; set; } //TODO: once implemented, change type to ProtocolType
        public AddressFamily IPFamily { get; set; }
        public string IP { get; set; } // original type: IPAddress
        public int Port { get; set; }
        public WinsockVersion Version { get; set; }
        public bool IsCurrentlyTunneling { get; set; } = false;
        public Status SocketStatus
        {
            get => status;
            set
            {
                status = value;
                LastStatusChangeTime = DateTime.Now;
            }
        }
        public DateTime LastStatusChangeTime { get; private set; }

        public Connection(int id)
        {
            SocketId = id;
            Protocol = (int)ProtocolType.Tcp;
            IPFamily = AddressFamily.InterNetwork;
            IP = "0.0.0.0";
            Port = 0;
            SocketStatus = Status.REQUESTING_INFO;
            LastStatusChangeTime = DateTime.Now;
            Version = WinsockVersion.Version_1;
        }

        public Connection(int id, int protocol, int addrFamily, string ip, int port, Status status, WinsockVersion version)
        {
            SocketId = id;
            Protocol = protocol;
            IPFamily = (AddressFamily)addrFamily;
            IP = ip;
            Port = port;
            SocketStatus = status;
            LastStatusChangeTime = DateTime.Now;
            Version = version;
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
            REQUESTING_INFO,
            CONNECTING,
            ESTABLISHED,
            CLOSED
        }

        public enum WinsockVersion
        { 
            Version_1 = 1,
            Version_2
        }

    }
}
