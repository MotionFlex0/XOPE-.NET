using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using XOPE_UI.Spy.DispatcherMessageType;
using XOPE_UI.View;

namespace XOPE_UI.Presenter
{
    internal class HttpTunnelingDialogPresenter
    {
        IHttpTunnelingDialog _view;
        SpyManager _spyManager;

        public HttpTunnelingDialogPresenter(IHttpTunnelingDialog view, SpyManager spyManager)
        {
            _view = view;
            _spyManager = spyManager;
        }

        public void StartHttpTunneling()
        {
            if (_spyManager.IsTunneling)
                return;

            // Validate IP:Port

            if (_view.IPAddress == "" || _view.Port == "")
                _view.ShowErrorMessage("One of the field(s) is blank");

            bool shouldContinue = _view.ShowUnstableWarningYesNo("Please note - This tunneling feature may be very unstable.\n" +
                "It may result in crashes or improper send/receives.\n" +
                "Once started, it may not be possible to properly stop tunneling.\n\n" +
                "Are you sure you want to continue?");

            if (!shouldContinue)
                return;

            IPAddress ip;
            try
            {
                ip = IPAddress.Parse(_view.IPAddress);
            }
            catch
            {
                _view.ShowErrorMessage("Please enter a valid IP Address.");
                return;
            }

            int port;
            try
            {
                port = int.Parse(_view.Port);
            }
            catch
            {
                _view.ShowErrorMessage("Please enter a valid port.");
                return;
            }

            try
            {
                TcpClient tcpClient = new TcpClient();

                tcpClient.Connect(ip, port);
                if (!tcpClient.Connected)
                    throw new SocketException();
                tcpClient.Close();
                _spyManager.EnableHttpTunneling(ip, port);
                _view.ShowUiConnectedToProxy();
            }
            catch (ArgumentOutOfRangeException)
            {
                _view.ShowErrorMessage("Please enter a port no greater than 65535 and no less than 0");
            }
            catch (SocketException ex)
            {
                _view.ShowErrorMessage("Error when trying to connect to that IP/Port combination\n" +
                    "Please make sure they're correct and try again.\n" +
                    $"Message:\n{ex.Message}");
            }
            catch (Exception ex)
            {
                _view.ShowErrorMessage("Unknown error when connecting to IP/Port combination.\n" +
                    $"Message: {ex.Message}");
            }
        }

        public void StopHttpTunneling()
        {
            if (!_spyManager.IsTunneling)
                return;

            _spyManager.DisableHttpTunneling();
            _view.ShowUiDisconnectedFromProxy();
        }
    }
}
