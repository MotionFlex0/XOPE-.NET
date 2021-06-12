using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOPE_UI.Spy;

namespace SDK
{
    class Environment
    {
        public event EventHandler<IntPtr> OnProcessAttached;
        public event EventHandler<IntPtr> OnProcessDetached;

        public event EventHandler<byte[]> OnNewSendPacket;
        public event EventHandler<byte[]> OnNewRecvPacket;
        public event EventHandler<byte[]> OnNewSendPacketModify;
        public event EventHandler<byte[]> OnNewRecvPacketModify;


        public IntPtr Process { get; set; }
        public IServer Server { get; set; }

        private static Environment environment;

        private Environment() { }

        public static Environment GetEnvironment() => 
            environment ?? (environment = new Environment());



    }
}
