using System;
using System.Security.Cryptography;
using System.Windows.Media.Animation;
using XOPE_UI.Model;
using XOPE_UI.Spy.DispatcherMessageType.JobResponse;
using XOPE_UI.View;

namespace XOPE_UI.Presenter
{
    internal class InterceptorViewTabPresenter
    {
        IInterceptorViewTab _view;


        SpyManager _spyManager; // Never use this directly. Use SpyManager instead

        public SpyManager SpyManager
        {
            private get => _spyManager ??
                throw new InvalidOperationException("SpyManager needs to be initialised prior to its use");
            set => _spyManager = value;
        }

        public InterceptorViewTabPresenter(IInterceptorViewTab view)
        {
            _view = view;
        }

        public void UpdateEditor(Guid jobId, Packet packet)
        {
            _view.UpdateEditor(jobId, packet);
        }

        public void ForwardButtonClicked()
        {
            _spyManager.MessageDispatcher.Send(new InterceptorForwardPacketResponse(Guid.Parse(_view.JobId), _view.BytesInEditor));

            _view.MoveToNextPacket();
        }

        public void DropPacketButtonClicked()
        {
            _spyManager.MessageDispatcher.Send(new InterceptorDropPacketResponse(Guid.Parse(_view.JobId)));

            _view.MoveToNextPacket();
        }

        public void ForwardAllPackets()
        {
            while (_view.BytesInEditor != null)
            {
                _spyManager.MessageDispatcher.Send(new InterceptorForwardPacketResponse(Guid.Parse(_view.JobId), _view.BytesInEditor));
                _view.MoveToNextPacket();
            }
        }
    }
}
