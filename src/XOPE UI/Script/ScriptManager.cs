using CSScriptLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using XOPE_UI.Forms;

namespace XOPE_UI.Script
{
    public class ScriptManager
    {
        public class Script
        {
            public DateTime StartedAt { get; set; }
            public string FileLocation { get; set; }
        }

        private Dictionary<Guid, Script> scripts = new Dictionary<Guid, Script>();

        public ScriptManager()
        {

        }

        public Guid AddCSScript(string csFileName)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;
            using (ScriptLoadingDialog scriptLoadingDialog = new ScriptLoadingDialog(cancellationTokenSource))
            {
                Task task = Task.Run(() =>
                {
                    try
                    {
                        SDK.IScript script = CSScript.Evaluator.LoadFile<SDK.IScript>(csFileName);
                        token.ThrowIfCancellationRequested();
                        scriptLoadingDialog.ScriptLoaded();

                        script.Init();

                    }
                    catch (CSScriptLib.CompilerException ex)
                    {
                        MessageBox.Show($"Compiler error: {ex.Message}");
                    }
                    catch (OperationCanceledException ex)
                    {
                        Console.WriteLine($"The loading of script '{csFileName}' has been cancelled");
                    }
                }, token);
                scriptLoadingDialog.ShowDialog();
            }
        }

        public Script GetScript(Guid guid)
        {
            bool res = scripts.TryGetValue(guid, out Script script);
            return res ? script : null;
        }
    }
}
