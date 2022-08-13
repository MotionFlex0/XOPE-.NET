using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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
        public bool Disposed { get; private set; } = false;

        public bool SinkConnected { get; private set; }
        public IPAddress TunnelIP { get; }
        public int TunnelPort80 { get; }
        public int TunnelPort443 { get; }
        public bool IsTunnelingAnySockets => _tunneledConns.Count > 0;

        ConcurrentDictionary<int, SocketData> _tunneledConns = new ConcurrentDictionary<int, SocketData>();

        CancellationTokenSource _cancellationSource;


        public HttpTunnelingHandler(SpyManager spyManager, IPAddress ip, int port80, int port443)
        {
            if (ip == null || port80 < 0 || port80 > ushort.MaxValue)
                throw new ArgumentException("ip must be non-null and port needs to be between 0 and 65535");

            _cancellationSource = new CancellationTokenSource();
            _cancellationSource.Token.Register(Dispose);

            TunnelIP = ip;
            TunnelPort80 = port80;
            TunnelPort443 = port443;

            bool ipv4Success = StartSinkListener(IPAddress.Loopback, Config.Spy.SinkPortIPv4);
            bool ipv6Success = StartSinkListener(IPAddress.IPv6Loopback, Config.Spy.SinkPortIPv6);
            SinkConnected = ipv4Success && ipv6Success;
        }

        public void Dispose()
        {
            if (Disposed)
                return;

            if (!_cancellationSource.IsCancellationRequested)
                _cancellationSource.Cancel();

            SinkConnected = false;
            Disposed = true;
        }

        public bool IsTunnelingSocket(int socket) => _tunneledConns.ContainsKey(socket);

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

            SocketData socketData = _tunneledConns.GetOrAdd(connection.SocketId, new SocketData());
            socketData.Tunnel = tunnelClient;
            socketData.Connection = connection;

            _ = HandleTunnelClient(socketData);
            return true;
        }

        public void ConnectionClosedExternally(int socket)
        {
            if (_tunneledConns.ContainsKey(socket))
                _tunneledConns[socket].TokenSource.Cancel(); ;
        }

        //private void StartTunnelService()
        //{
        //    _tunnelServiceTask = Task.Factory.StartNew(() =>
        //    {
        //        byte[] inBuffer = new byte[65536];
        //        while (!_cancellationSource.IsCancellationRequested)
        //        {
        //            Thread.Sleep(10);
        //            foreach (var tc in _tunneledConns.Values)
        //            {


        //            }
        //        }
        //    }, _cancellationSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        //}

        //private void StartSinkService()
        //{
        //    _sinkServiceTask = Task.Factory.StartNew(() =>
        //    {
        //        byte[] inBuffer = new byte[65536];
        //        while (!_cancellationSource.IsCancellationRequested)
        //        {
        //            Thread.Sleep(10);
        //            foreach (var tc in _tunneledConns.Values)
        //            {
        //                try
        //                {
        //                    if (tc.Tunnel == null || tc.Sink == null || /*!tc.Sink.GetStream().DataAvailable*/)
        //                        continue;

        //                    // The TunnelService is responsible for closing and removing the socket if disconnect/canceled 
        //                    if (!tc.Tunnel.Connected || tc.TokenSource.Token.IsCancellationRequested)
        //                        continue; 

        //                    //int bytesReceived = tc.Sink.GetStream().Read(inBuffer, 0, inBuffer.Length).;
        //                    tc.Sink.GetStream().ReadAsync(inBuffer, 0, inBuffer.Length, _cancellationSource.Token).ContinueWith(bytesRead =>
        //                    {

        //                    });

        //                    if (bytesReceived == 0)
        //                    {
        //                        CloseAndRemoveTunneledConnection(tc);
        //                        continue;
        //                    }
        //                    Console.WriteLine($"bytesReceived: {bytesReceived}");
        //                    tc.Tunnel.GetStream().Write(inBuffer, 0, bytesReceived);
        //                }
        //                catch (ObjectDisposedException e)
        //                {
        //                    CloseAndRemoveTunneledConnection(tc);
        //                }
        //            }
        //        }
        //    }, _cancellationSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        //}

        // The sink is only used to provide a valid socket for tunneled connections
        private bool StartSinkListener(IPAddress ip, int port)
        {
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
                return false;
            }

            _ = HandleSinkListener(tcpListener);
            return true;
        }

        private async Task HandleSinkListener(TcpListener tcpListener)
        {
            _cancellationSource.Token.Register(() => tcpListener.Stop());
            try
            {
                while (!_cancellationSource.IsCancellationRequested)
                    _ = HandleSinkServer(await tcpListener.AcceptTcpClientAsync());
            }
            catch (SocketException) { }

            if (!_cancellationSource.IsCancellationRequested)
                _cancellationSource.Cancel();
        }

        private async Task HandleSinkServer(TcpClient client)
        {
            byte[] inBuffer = new byte[65536];
            client.GetStream().Read(inBuffer, 0, 4);
            int socketId = BitConverter.ToInt32(inBuffer, 0);
                
            SocketData socketData = _tunneledConns.GetOrAdd(socketId, new SocketData());
            socketData.Sink = client;

            _cancellationSource.Token.Register(() => socketData.Dispose(true));

            try
            {
                while (!_cancellationSource.IsCancellationRequested && 
                    !socketData.TokenSource.Token.IsCancellationRequested)
                {
                    if (socketData.Tunnel == null || socketData.Sink == null)
                    {
                        await Task.Delay(10);
                        continue;
                    }

                    // The TunnelService is responsible for closing and removing the socket if disconnect/canceled 
                    if (!socketData.Tunnel.Connected || socketData.TokenSource.Token.IsCancellationRequested)
                        break;

                    int bytesReceived = await socketData.Sink.GetStream().ReadAsync(inBuffer, 0, inBuffer.Length, _cancellationSource.Token);

                    if (bytesReceived == 0)
                    {
                        socketData.MarkSinkAsDone();
                        break;
                    }
                    await socketData.Tunnel.GetStream().WriteAsync(inBuffer, 0, bytesReceived, _cancellationSource.Token);
                }
            }
            catch (ObjectDisposedException) { }
            catch (IOException e) 
            {
                //Console.WriteLine($"IOException thrown by Sink on socket {socketId}. Message: {e.Message}");

                if (!socketData.TokenSource.IsCancellationRequested)
                    socketData.MarkSinkAsDone();
            }
        }

        private async Task HandleTunnelClient(SocketData socketData)
        {
            _cancellationSource.Token.Register(() => socketData.Dispose(true));

            byte[] inBuffer = new byte[65536];
            try
            {
                while (!_cancellationSource.IsCancellationRequested &&
                    !socketData.TokenSource.Token.IsCancellationRequested)
                {
                    // connection made to Sink before TunnelNewConnection was called
                    if (socketData.Tunnel == null || socketData.Sink == null)
                    {
                        await Task.Delay(50);
                        //Console.WriteLine("HandleTunnelClient .Delay() called");
                        continue;
                    }

                    if (!socketData.Tunnel.Connected)
                        break;

                    int bytesReceived = await socketData.Tunnel.GetStream().ReadAsync(inBuffer, 0, inBuffer.Length, _cancellationSource.Token);

                    if (bytesReceived == 0)
                        break;

                    await socketData.Sink.GetStream().WriteAsync(inBuffer, 0, bytesReceived, _cancellationSource.Token);
                }

            }
            catch (ObjectDisposedException) { }
            catch (IOException e) 
            { 
                Console.WriteLine($"IOException thrown by Tunnel on socket {socketData.Connection.SocketId}. Message: {e.Message}"); 
            }

            socketData.MarkTunnelAsDone();
        }

        private SocketData CreateOrGetSocketData(Connection connection)
        {
            bool found = _tunneledConns.TryGetValue(connection.SocketId, out SocketData socketData);
            if (!found)
            {
                socketData = new SocketData();
                socketData.OnDisposed += (s, e) => this.RemoveTunneledConnection(connection);
            }
            return socketData;
        }

        //private void CloseAndRemoveTunneledConnection(SocketData tc)
        //{
        //    if (!_tunneledConns.ContainsKey(tc.Connection.SocketId))
        //        return;

        //    tc.TokenSource.Cancel();

        //    RemoveTunneledConnection(tc.Connection);
        //}

        private void RemoveTunneledConnection(Connection connection)
        {
            int socketId = connection.SocketId;
            if (_tunneledConns.ContainsKey(socketId))
                _tunneledConns.TryRemove(socketId, out _);
        }

        internal class SocketData : IDisposable
        {
            public event EventHandler<Connection> OnDisposed;

            public Connection Connection { get; set; } = null;
            public TcpClient Sink { get; set; } = null;
            public TcpClient Tunnel { get; set; } = null;
            public CancellationTokenSource TokenSource { get; } = null;

            private bool _tunnelDone = false;
            private bool _sinkDone = false;

            public SocketData()
            {
                TokenSource = new CancellationTokenSource();
                TokenSource.Token.Register(() => this.Dispose(true));
            }

            public void Dispose() =>
                Dispose(false);

            public void Dispose(bool force)
            {
                if ((_tunnelDone && _sinkDone) || force)
                {
                    TokenSource.Cancel();
                    Sink?.Close();
                    Tunnel?.Close();
                    OnDisposed?.Invoke(this, Connection);
                }
            }

            public void MarkTunnelAsDone()
            {
                _tunnelDone = true;
                Dispose();
            }

            public void MarkSinkAsDone()
            {
                _sinkDone = true;
                Dispose();
            }
        }
    }
}
