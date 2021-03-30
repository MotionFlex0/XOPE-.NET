using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XOPE_UI.Native;

namespace XOPE_UI.Test.UI
{
    static class HexEditorFormTest
    {
        [STAThread]
        static void Main()
        {
            NativeMethods.CreateConsole();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new HexEditorForm());
            //Application.Run(new ProcessDialog());
        }
    }
}
