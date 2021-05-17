using System;
using System.IO;
using System.Windows.Forms;
using XOPE_UI.Native;
using XOPE_UI.Spy;

namespace XOPE_UI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!File.Exists("XOPESpy32.dll"))
                System.Windows.Forms.MessageBox.Show("Canont find XOPESpy32.dll\nMake sure it is in the current directory\nWithout it, you cannot attach to 32-bit processes", "Missing DLL",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

            if (!File.Exists("XOPESpy64.dll") && Environment.Is64BitProcess)
                System.Windows.Forms.MessageBox.Show("Canont find XOPESpy64.dll\nMake sure it is in the current directory\nWithout it, you cannot attach to 64-bit processes", "Missing DLL",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

            if (!File.Exists("helper32.exe") && Environment.Is64BitProcess)
                System.Windows.Forms.MessageBox.Show("Canont find helper32.exe\nMake sure it is in the current directory\nWithout it, you cannot attach to 32-bit processes", "Missing helper executable",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

            IServer server = new NamedPipeServer();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow(server));
            //Application.Run(new ProcessDialog());
        }
    }
}
