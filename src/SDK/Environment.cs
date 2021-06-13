using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOPE_UI.Spy;

namespace SDK
{
    public class Environment
    {
        public event EventHandler<Process> OnProcessAttached;
        public event EventHandler<Process> OnProcessDetached;

        public event EventHandler<byte[]> OnNewPacket;
        public event EventHandler<byte[]> OnNewPacketModify;


        public Process AttachedProcess { get; private set; }
        public IServer Server { get; private set; }

        private static Environment environment;

        private Environment() { }

        public static Environment GetEnvironment() => 
            environment ?? (environment = new Environment());

        public void EmitNewPacket(byte[] bytes) =>
            OnNewPacket?.Invoke(this, bytes);

        public void EmitProcessAttached(Process process)
        {
            Process = process;
            OnProcessAttached?.Invoke(this, process);
        }
        
        public void EmitProcessDetached(Process process)
        {
            AttachedProcess = null;
            OnProcessDetached?.Invoke(this, process);
        }
    }
}
