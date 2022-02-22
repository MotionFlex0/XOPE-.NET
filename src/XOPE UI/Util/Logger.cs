using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XOPE_UI.Util
{
    // Reference: https://referencesource.microsoft.com/#mscorlib/system/io/stringwriter.cs
    public class Logger : StringWriter
    {
        public event EventHandler<string> TextWritten;

        public Logger()
        {
            Console.SetOut(this);
        }

        protected override void Dispose(bool disposing)
        {
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));
            base.Dispose(disposing);
        }

        public override void Write(char value)
        {
            base.Write(value);
            TextWritten?.Invoke(this, value.ToString());
        }

        public override void Write(char[] buffer, int index, int count)
        {
            base.Write(buffer, index, count);
            TextWritten?.Invoke(this, new string(buffer, index, count));
        }

        public override void Write(string value)
        {
            base.Write(value);
            TextWritten?.Invoke(this, value);
        }

        public override Encoding Encoding
        {
            get { return Encoding.Unicode; }
        }
    }
}
