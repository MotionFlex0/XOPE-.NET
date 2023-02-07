using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using XOPE_UI.Spy.DispatcherMessageType;

namespace XOPE_UI.Spy
{
    public class NamedPipeDispatcher : IMessageDispatcher
    {
        private NamedPipeClientStream _namedPipeClient;
        private bool _pipeBroken = false;

        private Dictionary<Guid, MessageWithResponseImpl> _jobs; // 

        private object _pipeWriteLock = new object();

        public bool IsConnected => _namedPipeClient != null && !_pipeBroken;

        public NamedPipeDispatcher(String pipeName, Dictionary<Guid, MessageWithResponseImpl> jobs)
        {
            try
            {
                _namedPipeClient = new NamedPipeClientStream(pipeName);
                _namedPipeClient.Connect(2000);
                this._jobs = jobs;
            }
            catch (Exception)
            {
                _namedPipeClient = null;
            }
        }

        public void Send(MessageImpl message)
        {
            if (!IsConnected)
                return;
            
            try
            {
                byte[] bytes = Encoding.ASCII.GetBytes(message.ToJson().ToString());

                lock (_pipeWriteLock)
                {
                    _namedPipeClient.Write(bytes, 0, bytes.Length);
                    _namedPipeClient.WriteByte(0);
                }
            }
            catch (Exception ex) when (ex is IOException || ex is ObjectDisposedException)
            {
                _pipeBroken = true;
            }
        }

        public void Send(MessageWithResponseImpl message)
        {
            if (!IsConnected)
                return;

            try
            {
                byte[] bytes = Encoding.ASCII.GetBytes(message.ToJson().ToString());

                lock(_pipeWriteLock)
                {
                    _namedPipeClient.Write(bytes, 0, bytes.Length);
                    _namedPipeClient.WriteByte(0);
                }

                _jobs.Add(message.JobId, message);
            }
            catch (Exception ex) when (ex is IOException || ex is ObjectDisposedException)
            {
                _pipeBroken = true;
            }
        }

        public void ShutdownAndWait()
        {
            if (!IsConnected)
                return;

            _namedPipeClient.Close();
            _pipeBroken = true;
        }
    }
}
