using System;
using XOPE_UI.Model;

namespace XOPE_UI.View
{
    internal interface IInterceptorViewTab
    {
        byte[] BytesInEditor { get; }
        public string JobId { get; }
        public int QueueCount { get; }
        public bool PacketInEditor { get; }

        //void AddPacketToLiveViewQueue(Guid jobId, Packet packet);
        void UpdateEditor(Guid jobId, Packet packet);
        void MoveToNextPacket();
    }
}
