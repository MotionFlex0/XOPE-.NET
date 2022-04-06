using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOPE_UI.View
{
    internal interface IHttpTunnelingDialog
    {
        public string IPAddress { get; }
        public string Port80 { get; }
        public string Port443 { get; }

        void ShowUiConnectedToProxy();
        void ShowUiDisconnectedFromProxy();

        // Yes = true | No = false
        bool ShowUnstableWarningYesNo(string message);
        void ShowErrorMessage(string message);
    }
}
