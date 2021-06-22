using System;
using System.Diagnostics;
using XOPE_UI.Spy;

namespace SDK
{
    public class Environment
    {
        public event EventHandler<Process> OnProcessAttached;
        public event EventHandler<Process> OnProcessDetached;

        public event EventHandler<byte[]> OnNewPacket;
        public event EventHandler<byte[]> OnNewPacketModify;

        // TODO: May cause a race condition. Watch out for this.
        public Process AttachedProcess { get; private set; }
        public IServer Server { get; set; }

        private static Environment environment;

        private Environment() { }

        public static Environment GetEnvironment() => 
            environment ?? (environment = new Environment());

        public void NotifyNewPacket(byte[] bytes) =>
            OnNewPacket?.Invoke(this, bytes);

        public void NotifyProcessAttached(Process process)
        {
            AttachedProcess = process;
            OnProcessAttached?.Invoke(this, process);
        }
        
        public void NotifyProcessDetached(Process process)
        {
            AttachedProcess = null;
            OnProcessDetached?.Invoke(this, process);
        }
    }
}
