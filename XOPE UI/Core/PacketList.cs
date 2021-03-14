using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XOPE_UI.Spy.Type;
using XOPE_UI.Definitions;

namespace XOPE_UI.Core
{
    class PacketList
    {
        private int packetCounter = 0;
        private ListView packetList;

        public PacketList(ListView lv)
        {
            packetList = lv;
        }

        public int Add(HookedFuncType type, int socketId, byte[] packet)
        {
            if (type == HookedFuncType.SEND || type == HookedFuncType.RECV || type == HookedFuncType.WSASEND || type == HookedFuncType.WSARECV)
            {
                ListViewItem listViewItem = new ListViewItem(packetCounter.ToString());
                listViewItem.Tag = packet;
                listViewItem.SubItems.Add(type.ToString());
                listViewItem.SubItems.Add(packet.Length.ToString());
                listViewItem.SubItems.Add(BitConverter.ToString(packet).Replace("-", " "));
                listViewItem.SubItems.Add(socketId.ToString());
                packetList.Invoke((MethodInvoker)(() => { packetList.Items.Add(listViewItem); }));
            }

            return packetCounter++;
        }

        public int Add(Packet packet)
        {
            return Add(packet.Type, packet.Socket, packet.Data);
        }
    }
}
