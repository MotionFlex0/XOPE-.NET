using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using XOPE_UI.Model;
using XOPE_UI.Spy.DispatcherMessageType;

namespace XOPE_UI
{
    public class HttpTunnelingHandler : IDisposable
    {
        internal class Data
        {
            public TcpClient Sink { get; set; } = null;
            public TcpClient Tunnel { get; set; } = null;
            public CancellationTokenSource TokenSource { get; } = new CancellationTokenSource();
        }

        public bool SinkConnected { get; private set; }
        public IPAddress TunnelIP { get; }
        public int TunnelPort80 { get; }
        public int TunnelPort443 { get; }
        public bool IsTunnelingAnySockets => _tunnelsData.Count > 0;

        ConcurrentDictionary<int, Data> _tunnelsData = new ConcurrentDictionary<int, Data>();

        CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        CancellationTokenSource _sinkCancelTokenSource = new CancellationTokenSource();

        SpyManager _spyManager;

        TcpListener _ipv4TcpListener;
        TcpListener _ipv6TcpListener;
        Task _ipv4SinkTask;
        Task _ipv6SinkTask;

        public HttpTunnelingHandler(SpyManager spyManager, IPAddress ip, int port80, int port443)
        {
            if (ip == null || port80 < 0 || port80 > ushort.MaxValue)
                throw new ArgumentException("ip must be non-null and port needs to be between 0 and 65535");
            
            _spyManager = spyManager;
            TunnelIP = ip;
            TunnelPort80 = port80;
            TunnelPort443 = port443;

            (_ipv4SinkTask, _ipv4TcpListener) = SetupSinkListener(IPAddress.Loopback, Config.Spy.SinkPortIPv4);
            (_ipv6SinkTask, _ipv6TcpListener) = SetupSinkListener(IPAddress.IPv6Loopback, Config.Spy.SinkPortIPv6);
            SinkConnected = _ipv4SinkTask != null && _ipv6SinkTask != null;
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _sinkCancelTokenSource.Cancel();

            if (_ipv4SinkTask != null)
            {
                _ipv4TcpListener.Stop();
                _ipv4SinkTask.Wait(2000);
            }

            if (_ipv6SinkTask != null)
            {
                _ipv6TcpListener.Stop();
                _ipv6SinkTask.Wait(2000);
            }

            foreach (var kvp in _tunnelsData)
                kvp.Value.TokenSource.Cancel();
            SinkConnected = false;
        }

        public bool IsTunnelingSocket(int socket) => _tunnelsData.ContainsKey(socket);

        public bool TunnelNewConnection(Connection connection)
        {
            TcpClient tunnelClient = new TcpClient();

            try
            {
                if (connection.Port == 80)
                    tunnelClient.Connect(TunnelIP, TunnelPort80);
                else if (connection.Port == 443)
                    tunnelClient.Connect(TunnelIP, TunnelPort443);
                else
                    throw new ArgumentException($"Connection to {connection.IP}:{connection.Port} was marked as tunnelable but " +
                        $"the connection is not on port 80 or 443.");
            }
            catch (Exception ex)
            {
                // Temporary solution. TODO: Separate code for UI and logic
                MessageBox.Show($"Failed to connect to {TunnelIP}:{TunnelPort80}\n" +
                    $"Message: {ex.Message}");
                return false;
            }

            Task.Factory.StartNew(() => 
            {
                Data socketData = _tunnelsData[connection.SocketId];
                byte[] outBuffer = new byte[65535];
                while (tunnelClient.Connected && !_cancellationTokenSource.IsCancellationRequested && 
                    !socketData.TokenSource.Token.IsCancellationRequested)
                {
                    if (!tunnelClient.GetStream().DataAvailable || socketData.Sink == null)
                    {
                        Thread.Sleep(10);
                        continue;
                    }

                    int bytesReceived = tunnelClient.GetStream().Read(outBuffer);

                    if (bytesReceived == 0)
                    {
                        socketData.Sink.Close();
                        break;
                    }

                    socketData.Sink.GetStream().Write(outBuffer, 0, bytesReceived);
                }

                if (tunnelClient.Connected)
                    tunnelClient.Close();

                if (socketData.Sink != null && socketData.Sink.Connected)
                    socketData.Sink.Close();

                RemoveTunneledConnection(connection);
            }, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            _tunnelsData.GetOrAdd(connection.SocketId, new Data()).Tunnel = tunnelClient;
            return true;
        }

        public void Send(Packet packet)
        {
            int socketId = packet.Socket;
            if (!_spyManager.SpyData.Connections.ContainsKey(socketId))
            {
                Console.WriteLine($"Cannot forward packet to socket {socketId} because " +
                    $"XOPE does not have information about that connection.");
            }

            if (!_tunnelsData.ContainsKey(socketId))
                return;

            if (_tunnelsData[socketId].Tunnel.Connected)
                _tunnelsData[socketId].Tunnel.GetStream().Write(packet.Data);
        }

        public void ConnectionClosedExternally(int socket)
        {
            if (_tunnelsData.ContainsKey(socket))
                _tunnelsData[socket].TokenSource.Cancel(); ;
        }

        // The sink is only used to provide a valid socket for tunneled connections
        private (Task, TcpListener) SetupSinkListener(IPAddress ip, int port)
        {
            Task listenerTask = null;
            TcpListener tcpListener = null;
            try
            {
                tcpListener = new TcpListener(new IPEndPoint(ip, port));
                tcpListener.Start();
            }
            catch (SocketException ex)
            {
                Console.Write($"HttpTunnelingHandler sink error. SocketException: {ex.Message}");
                if (tcpListener != null)
                    tcpListener.Stop();
                return (null, null);
            }

            listenerTask = Task.Factory.StartNew(() =>
            {
                try
                {
                    while (!_sinkCancelTokenSource.IsCancellationRequested)
                        SetupSinkServer(tcpListener.AcceptTcpClient());
                }
                catch (SocketException) { }
                tcpListener.Stop();
            }, _sinkCancelTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            return (listenerTask, tcpListener);
        }

        private void SetupSinkServer(TcpClient client)
        {
            Task.Factory.StartNew(() =>
            {
                byte[] data = new byte[1024];
                client.GetStream().Read(data, 0, data.Length);
                int socketId = BitConverter.ToInt32(data, 0);
                
                Data socketData = _tunnelsData.GetOrAdd(socketId, new Data());
                socketData.Sink = client;
            }, _sinkCancelTokenSource.Token, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }
        
        private void RemoveTunneledConnection(Connection connection)
        {
            int socketId = connection.SocketId;
            if (_tunnelsData.ContainsKey(socketId))
                _tunnelsData.Remove(socketId, out _);
        }
    }
}
