using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOPE_UI.Model;
using XOPE_UI.Spy.DispatcherMessageType.JobResponse;
using XOPE_UI.View;

namespace XOPE_UI.Presenter
{
    internal class LiveViewTabPresenter
    {
        ILiveViewTab _view;


        SpyManager _spyManager; // Never use this directly. Use SpyManager instead

        public SpyManager SpyManager
        {
            private get => _spyManager ??
                throw new InvalidOperationException("SpyManager needs to be initialised prior to its use");
            set => _spyManager = value;
        }

        public LiveViewTabPresenter(ILiveViewTab view)
        {
            _view = view;
        }

        public void ForwardButtonClicked()
        {
            _spyManager.MessageDispatcher.Send(new LiveViewForwardPacketResponse(Guid.Parse(_view.JobId), _view.BytesInEditor));
            _view.ClearEditor();
        }

        public void DropPacketButtonClicked()
        {
            _spyManager.MessageDispatcher.Send(new LiveViewDropPacketResponse(Guid.Parse(_view.JobId)));
            _view.ClearEditor();
        }
    }
}
