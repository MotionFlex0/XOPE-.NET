using CSScriptLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using XOPE_UI.Forms;

namespace XOPE_UI.Script
{
    public enum ScriptStatus
    {
        RUNNING,
        STOPPED
    }

    public class ScriptData
    {
        public DateTime StartedAt { get; set; }
        public DateTime StoppedAt { get; set; }
        public ScriptStatus Status { get; set; }
        public string FileLocation { get; set; }
        public string Name { get; set; }
        public bool SingleInstance { get; set; }
        public SDK.IScript LoadedScript { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
    }

    public class ScriptManager
    {
        private Dictionary<Guid, ScriptData> scripts = new Dictionary<Guid, ScriptData>();

        public ScriptManager()
        {

        }

        public Guid AddCSScript(string csFileName)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            Guid guid = Guid.NewGuid();
            using (ScriptLoadingDialog scriptLoadingDialog = new ScriptLoadingDialog(cancellationTokenSource))
            {
                Task task = Task.Run(() =>
                {
                    try
                    {
                        SDK.IScript script = CSScript.Evaluator.LoadFile<SDK.IScript>(csFileName);
                        token.ThrowIfCancellationRequested();
                        scriptLoadingDialog.ScriptLoaded();

                        scripts.Add(guid, new ScriptData
                        {
                            StartedAt = DateTime.Now,
                            StoppedAt = DateTime.MinValue,
                            Status = ScriptStatus.RUNNING,
                            Name = Path.GetFileName(csFileName),
                            FileLocation = csFileName,
                            SingleInstance = false,
                            LoadedScript = script,
                            CancellationTokenSource = cancellationTokenSource
                        });

                        
                        script.OnInit();
                        
                        while (!token.IsCancellationRequested)
                        {
                            script.OnTick();
                            Thread.Sleep(100);
                        }

                        script.OnExit();

                        scripts[guid].Status = ScriptStatus.STOPPED;

                    }
                    catch (CSScriptLib.CompilerException ex)
                    {
                        MessageBox.Show($"Compiler error:\n\n -----------------\n{ex.Message}\n-----------------");
                    }
                    catch (OperationCanceledException ex)
                    {
                        Console.WriteLine($"The loading of script '{csFileName}' has been cancelled");
                    }
                }, token);
                scriptLoadingDialog.ShowDialog();
            }
            return guid;
        }

        public Guid[] GetGuids()
        {
            return scripts.Keys.ToArray();
        }

        public ScriptData GetScript(Guid guid)
        {
            bool res = scripts.TryGetValue(guid, out ScriptData script);
            return res ? script : null;
        }

        public void StopScript(Guid guid)
        {
            ScriptData scriptData = GetScript(guid);
            if (scriptData == null)
                return;

            scriptData.CancellationTokenSource.Cancel();
            scriptData.Status = ScriptStatus.STOPPED;
            scriptData.StoppedAt = DateTime.Now;
        }
    }
}
