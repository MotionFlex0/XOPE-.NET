using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace XOPE_UI.View.Component
{
    public partial class SearchTextBox : TextBox
    {
        const int WM_PAINT = 0X000F;
        const int WM_ERASEBKGND = 0x0014;

        bool _redrawOnNextPaint;
        string _placeholderText;

        // <operator, hint>
        Dictionary<string, string> _queryOperators;

        public override string PlaceholderText
        {
            get => _placeholderText;
            set
            {
                _placeholderText = value is null ? "" : value;
                Invalidate();
            }
        }

        public string FormattedText 
            => Regex.Split(base.Text, @"(^|[\s])/[\w]+")[0];

        public SearchTextBox()
        {
            InitializeComponent();

            _redrawOnNextPaint = false;
            _queryOperators = new Dictionary<string, string>();

            this.GetType()
                .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .SetValue(this, true, null);

            this.LostFocus += SearchTextBox_LostFocus;
        }

        public SearchTextBox(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public void AddQueryOperator(string op, string description) =>
            _queryOperators.Add(op.Replace("/", ""), description);

        public void RemoveQueryOperatorValue(string op) =>
            _queryOperators.Remove(op.Replace("/", ""));

        public string GetQueryOperatorValue(string op)
        {
            string formattedOp = op.Replace("/", "");

            if (!_queryOperators.ContainsKey(formattedOp))
                throw new ArgumentOutOfRangeException($"{formattedOp} is not a valid query operator. Call AddQueryOperator first");

            Match operatorMatch = Regex.Match(this.Text, @$"(?:^|[\s])/(?i){formattedOp}[\s]*(?:""([^""]+)[""]|([\S]*)\s?)", 
                RegexOptions.IgnoreCase);
            if (operatorMatch.Success && operatorMatch.Groups.Count > 0)
                return operatorMatch.Groups[^1].Value;

            return null;
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            // WM_PAINT is sometimes not fired when text has been removed so we need to manually
            //  invalidate the control if it matches a query operator.
            foreach (string op in _queryOperators.Keys)
            {
                if (!Text.EndsWith($"/{op}", StringComparison.OrdinalIgnoreCase))
                    continue;

                this.Invalidate();
                break;
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_PAINT && _redrawOnNextPaint)
            {
                if (Text == "" && PlaceholderText != "")
                {
                    Rectangle bounds = this.ClientRectangle;
                    bounds.X -= 2;
                    bounds.Y += 1;
                    bounds.Width += 2;
                    bounds.Height -= 1;

                    Graphics graphics = this.CreateGraphics();
                    TextRenderer.DrawText(graphics, _placeholderText,
                        this.Font, bounds, Color.DarkViolet,
                            TextFormatFlags.Left);
                }
                else
                {
                    foreach (string op in _queryOperators.Keys)
                    {
                        if (!Text.EndsWith($"/{op}", StringComparison.OrdinalIgnoreCase))
                            continue;

                        Point lastCharPoint = this.GetPositionFromCharIndex(this.Text.Length - 1);
                        Size padding = TextRenderer.MeasureText(" ", this.Font);

                        Rectangle bounds = this.ClientRectangle;
                        bounds.X += lastCharPoint.X + padding.Width - 2;
                        bounds.Y += 1;
                        bounds.Width -= lastCharPoint.X + padding.Width - 2;
                        bounds.Height -= 1;

                        Graphics graphics = this.CreateGraphics();
                        TextRenderer.DrawText(graphics, $"- [{_queryOperators[op]}]",
                            this.Font, bounds, Color.DarkViolet,
                            TextFormatFlags.Left);
                    }
                }

                _redrawOnNextPaint = false;
            }
            else if (m.Msg == WM_ERASEBKGND)
                _redrawOnNextPaint = true;

        }

        private void SearchTextBox_LostFocus(object sender, EventArgs e)
        {
            _redrawOnNextPaint = true;
        }
    }
}
