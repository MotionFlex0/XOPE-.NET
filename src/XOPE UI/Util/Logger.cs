using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XOPE_UI.Util
{
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

        public override void Write(bool value)
        {
            base.Write(value);
            OnFlush?.Invoke(this, value.ToString());
        }

        public override void Write(char value)
        {
            base.Write(value);
            OnFlush?.Invoke(this, value.ToString());
        }

        public override void Write(char[] buffer)
        {
            base.Write(buffer);
            OnFlush?.Invoke(this, new string(buffer));
        }

        public override void Write(char[] buffer, int index, int count)
        {
            base.Write(buffer, index, count);
            OnFlush?.Invoke(this, new string(buffer, index, count));
        }

        public override void Write(decimal value)
        {
            base.Write(value);
            OnFlush?.Invoke(this, value.ToString());
        }

        public override void Write(double value)
        {
            base.Write(value);
            OnFlush?.Invoke(this, value.ToString());
        }

        public override void Write(float value)
        {
            base.Write(value);
            OnFlush?.Invoke(this, value.ToString());
        }

        public override void Write(int value)
        {
            base.Write(value);
            OnFlush?.Invoke(this, value.ToString());
        }

        public override void Write(long value)
        {
            base.Write(value);
            OnFlush?.Invoke(this, value.ToString());
        }

        public override void Write(object value)
        {
            base.Write(value);
            OnFlush?.Invoke(this, value.ToString() + "\r\n");
        }

        public override void Write(string value)
        {
            base.Write(value);
            OnFlush?.Invoke(this, value);
        }

        public override void Write(uint value)
        {
            base.Write(value);
            OnFlush?.Invoke(this, value.ToString());
        }

        public override void Write(ulong value)
        {
            base.Write(value);
            OnFlush?.Invoke(this, value.ToString());
        }

        public override void WriteLine(bool value)
        {
            base.WriteLine(value);
            OnFlush?.Invoke(this, value + "\r\n");
        }

        public override void WriteLine(char value)
        {
            base.WriteLine(value);
            OnFlush?.Invoke(this, value + "\r\n");
        }

        public override void WriteLine(char[] buffer)
        {
            base.WriteLine(buffer);
            OnFlush?.Invoke(this, new string(buffer) + "\r\n");
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            base.WriteLine(buffer, index, count);
            OnFlush?.Invoke(this, new string(buffer, index, count) + "\r\n");
        }

        public override void WriteLine(decimal value)
        {
            base.WriteLine(value);
            OnFlush?.Invoke(this, value + "\r\n");
        }

        public override void WriteLine(double value)
        {
            base.WriteLine(value);
            OnFlush?.Invoke(this, value + "\r\n");
        }

        public override void WriteLine(float value)
        {
            base.WriteLine(value);
            OnFlush?.Invoke(this, value + "\r\n");
        }

        public override void WriteLine(int value)
        {
            base.WriteLine(value);
            OnFlush?.Invoke(this, value + "\r\n");
        }

        public override void WriteLine(long value)
        {
            base.WriteLine(value);
            OnFlush?.Invoke(this, value + "\r\n");
        }

        public override void WriteLine(object value)
        {
            base.WriteLine(value);
            OnFlush?.Invoke(this, value + "\r\n");
        }

        public override void WriteLine(string format, object arg0)
        {
            base.WriteLine(format, arg0);
            OnFlush?.Invoke(this, String.Format(format, arg0) + "\r\n");
        }

        public override void WriteLine(string value)
        {
            base.WriteLine(value);
            OnFlush?.Invoke(this, value + "\r\n");
        }

        public override void WriteLine(uint value)
        {
            base.WriteLine(value);
            OnFlush?.Invoke(this, value + "\r\n");
        }

        public override void WriteLine(ulong value)
        {
            base.WriteLine(value);
            OnFlush?.Invoke(this, value + "\r\n");
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


        public override void WriteLine()
        {
            base.WriteLine();
            OnFlush?.Invoke(this, "\r\n");
        }

        public override Encoding Encoding
        {
            get { return Encoding.Unicode; }
        }
    }
}
