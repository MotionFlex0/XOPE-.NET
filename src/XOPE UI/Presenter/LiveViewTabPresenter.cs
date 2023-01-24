using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOPE_UI.Model;
using XOPE_UI.View;

namespace XOPE_UI.Presenter
{
    internal class LiveViewTabPresenter
    {
        ILiveViewTab _view;

        public LiveViewTabPresenter(ILiveViewTab view)
        {
            _view = view;
        }

        public void UpdateHexEditorWithPacket(Packet packet)
        {

        }

        public void ForwardButtonClicked()
        {

        }

        public void DropPacketButtonClicked()
        {

        }
    }
}
