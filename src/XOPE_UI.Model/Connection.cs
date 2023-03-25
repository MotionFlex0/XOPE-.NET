using System;
using System.Net.Sockets;

namespace XOPE_UI.Model
{
    // TODO: Use INotifyChanged
    public class Connection : EventArgs
    {
        private Status status;

        public int SocketId { get; private set; }
        public int Protocol { get; set; } //TODO: once implemented, change type to ProtocolType
        public AddressFamily IPFamily { get; set; }
        public string SourceAddress { get; set; } // original type: IPAddress
        public int SourcePort { get; set; }
        public string DestAddress { get; set; } // original type: IPAddress
        public int DestPort { get; set; }
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
            DestAddress = "0.0.0.0";
            DestPort = 0;
            SocketStatus = Status.REQUESTING_INFO;
            LastStatusChangeTime = DateTime.Now;
            Version = WinsockVersion.Version_1;
        }

        public Connection(int id, int protocol, int addrFamily, string sourceAddr, int sourcePort, string destAddr, int destPort, Status status, WinsockVersion version)
        {
            SocketId = id;
            Protocol = protocol;
            IPFamily = (AddressFamily)addrFamily;
            SourceAddress = sourceAddr;
            SourcePort = sourcePort;
            DestAddress = destAddr;
            DestPort = destPort;
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

        public string ConvertSourceToString()
        {
            return $"{SourceAddress}:{SourcePort}";
        }

        public string ConvertDestToString()
        {
            return $"{DestAddress}:{DestPort}";
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
