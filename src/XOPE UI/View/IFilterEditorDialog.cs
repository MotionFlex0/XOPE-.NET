using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOPE_UI.Model;

namespace XOPE_UI.View
{
    public interface IFilterEditorDialog
    {
        string FilterName { get; set; }
        byte[] OldValue { get; set; }
        byte[] NewValue { get; set; }
        int SocketId { get; set; }
        ReplayableFunction PacketType { get; set; }
        bool ShouldRecursiveReplace { get; set; }
        bool AllSockets { get; set; }
        bool DropPacket { get; set; }
        FilterEntry Filter { get; set; }
    }
}
