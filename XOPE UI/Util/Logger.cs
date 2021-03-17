using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XOPE_UI.Util
{
    public class Logger : TextWriter
    {
        public event EventHandler<char> OnFlush;

        public Logger()
        {
            Console.SetOut(this);
        }

        protected override void Dispose(bool disposing)
        {
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));
            //Console.F
            base.Dispose(disposing);
        }

        public override void Flush()
        {
            MessageBox.Show(this.ToString());
            //OnFlush?.Invoke(this, this.ToString());
        }

        public override Task FlushAsync()
        {
            return Task.Run(() => Flush());
        }

        public override void Write(char value)
        {
            //base.Write(value);
            OnFlush?.Invoke(this, value);
        }


        public override Encoding Encoding
        {
            get { return Encoding.Unicode; }
        }
    }
}
