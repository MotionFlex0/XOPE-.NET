using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using XOPE_UI.View;
using XOPE_UI.Spy;
using XOPE_UI.Settings;

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
                System.Windows.Forms.MessageBox.Show("Canont find XOPESpy32.dll\n" +
                    "Make sure it is in the current directory\nWithout it, you cannot attach to 32-bit processes", 
                    "Missing DLL",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

            if (!File.Exists("XOPESpy64.dll") && Environment.Is64BitProcess)
                System.Windows.Forms.MessageBox.Show("Canont find XOPESpy64.dll\n" +
                    "Make sure it is in the current directory\nWithout it, you cannot attach to 64-bit processes", 
                    "Missing DLL",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

            if (!File.Exists("helper32.exe") && Environment.Is64BitProcess)
                System.Windows.Forms.MessageBox.Show("Canont find helper32.exe\n" +
                    "Make sure it is in the current directory\n" +
                    "Without it, you cannot attach to 32-bit processes", 
                    "Missing helper executable",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

            SDK.Environment environment = SDK.Environment.GetEnvironment();

            PrecacheResources();

            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow(new UserSettings()));
        }

        static async Task PrecacheResources()
        {
            ProcessDialog.PrecacheResources();
            Console.WriteLine("Finished precaching resources.");
        }
    }
}
