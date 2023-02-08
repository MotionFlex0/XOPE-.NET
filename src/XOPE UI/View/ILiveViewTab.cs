using System;

namespace XOPE_UI.View
{
    internal interface ILiveViewTab
    {
        byte[] BytesInEditor { get; }
        public string JobId { get; }

        void UpdateEditor(Guid jobId, byte[] bytes);
        void ClearEditor();
    }
}
