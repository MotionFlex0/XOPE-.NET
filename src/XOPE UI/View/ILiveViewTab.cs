using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOPE_UI.Model;

namespace XOPE_UI.View
{
    internal interface ILiveViewTab
    {
        Packet BytesInEditor { get; }

        void UpdateEditor(byte[] bytes);
        void ClearEditor();
    }
}
