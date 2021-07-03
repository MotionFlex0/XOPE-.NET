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
        public event EventHandler<string> OnFlush;

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
            base.Write(value);
            OnFlush?.Invoke(this, value.ToString());
        }

        public override void Write(char[] buffer, int index, int count)
        {
            base.Write(buffer, index, count);
            OnFlush?.Invoke(this, new string(buffer, index, count));
        }

        public override void Write(string value)
        {
            base.Write(value);
            OnFlush?.Invoke(this, value);
        }

        //public void Write<T>(T value)
        //{
        //    base.Write(value);
        //    MessageBox.Show($"Write<T>(T value) with value type: {value.GetType().Name}");
        //    OnFlush?.Invoke(this, value.ToString());
        //}

        //public void WriteLine<T>(T value)
        //{
        //    base.WriteLine(value);
        //    MessageBox.Show($"WriteLine<T>(T value) with value type: {value.GetType().Name}");
        //    OnFlush?.Invoke(this, value + "\r\n");
        //}


        //public override void WriteLine()
        //{
        //    base.WriteLine();
        //    OnFlush?.Invoke(this, "\r\n");
        //}

        public override Encoding Encoding
        {
            get { return Encoding.Unicode; }
        }
    }
}
