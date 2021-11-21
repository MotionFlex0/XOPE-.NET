using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using XOPE_UI.Spy.ServerType;

namespace XOPE_UI.Spy
{
    public class NamedPipeDispatcher : IMessageDispatcher
    {
        private NamedPipeClientStream namedPipeClient;
        private bool pipeBroken = false;

        private Dictionary<Guid, IMessageWithResponse> jobs; // 

        private object pipeWriteLock = new object();

        public bool IsConnected => namedPipeClient != null && !pipeBroken;

        public NamedPipeDispatcher(String pipeName, Dictionary<Guid, IMessageWithResponse> jobs)
        {
            try
            {
                namedPipeClient = new NamedPipeClientStream(pipeName);
                namedPipeClient.Connect(2000);
                this.jobs = jobs;
            }
            catch (Exception)
            {
                namedPipeClient = null;
            }
        }

        public void Send(IMessage message)
        {
            if (!IsConnected)
                return;
            
            try
            {
                byte[] bytes = Encoding.ASCII.GetBytes(message.ToJson().ToString());

                lock (pipeWriteLock)
                {
                    namedPipeClient.Write(bytes, 0, bytes.Length);
                    namedPipeClient.WriteByte(0);
                }
            }
            catch (Exception ex) when (ex is IOException || ex is ObjectDisposedException)
            {
                pipeBroken = true;
            }
        }

        public void Send(IMessageWithResponse message)
        {
            if (!IsConnected)
                return;

            try
            {
                byte[] bytes = Encoding.ASCII.GetBytes(message.ToJson().ToString());

                lock(pipeWriteLock)
                {
                    namedPipeClient.Write(bytes, 0, bytes.Length);
                    namedPipeClient.WriteByte(0);
                }

                jobs.Add(message.JobId, message);
            }
            catch (Exception ex) when (ex is IOException || ex is ObjectDisposedException)
            {
                pipeBroken = true;
            }
        }

        public void ShutdownAndWait()
        {
            if (!IsConnected)
                return;

            namedPipeClient.Close();
            namedPipeClient.Dispose();
            pipeBroken = true;
        }
    }
}
