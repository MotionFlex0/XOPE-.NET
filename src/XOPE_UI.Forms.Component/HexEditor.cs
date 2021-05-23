using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace XOPE_UI.Forms.Component
{
    public partial class HexEditor : UserControl
    {
        const string OFFSET_FORMAT = "X4";
        const string BYTE_FORMAT = "X2";

        public Color CellBackColor { get; set; } = Color.White;
        public Color SelectionBackColor { get; set; } = Color.Blue;
        public Color SelectionForeColor { get; set; } = Color.White;
        public Color CellHoverBackColor { get; set; } = Color.Cyan;

        Stream data;
        public HexEditor()
        {
            InitializeComponent();


            this.byteGridView.ColumnHeadersHeightChanged += (object sender, System.EventArgs e) => 
                this.textGridView.ColumnHeadersHeight = this.byteGridView.ColumnHeadersHeight;

            this.textGridView.CellClick += byteGridView_CellClick;
            this.textGridView.CellMouseEnter += byteGridView_CellMouseEnter;
            this.textGridView.CellMouseLeave += byteGridView_CellMouseLeave;

            foreach (DataGridViewColumn c in this.textGridView.Columns)
            {
                c.Width = 16;
            }

            this.byteGridView
                .GetType()
                .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .SetValue(byteGridView, true, null);

            this.textGridView
                .GetType()
                .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .SetValue(byteGridView, true, null);

        }

        private void DataGridView_ColumnHeadersHeightChanged(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        public void SetBytes(byte[] bytes)
        {
            data = new MemoryStream(bytes);
            
            byte[] bytesInRow = new byte[16];
            for (int i = 0; i < data.Length; i += 16)
            {
                int bytesRead = data.Read(bytesInRow, 0, 16);
                DataGridViewRow dataGridViewRow = new DataGridViewRow();// 
                dataGridViewRow.HeaderCell.Value = $"0x{i.ToString(OFFSET_FORMAT)}";

                DataGridViewRow textGridViewRow = new DataGridViewRow();

                for (int j = 0; j < bytesRead; j++)
                {
                    DataGridViewTextBoxCell byteCell = new DataGridViewTextBoxCell();
                    DataGridViewTextBoxCell textCell = new DataGridViewTextBoxCell();

                    byteCell.Value = bytesInRow[j].ToString(BYTE_FORMAT);
                    dataGridViewRow.Cells.Add(byteCell);

                    if (bytesInRow[j] >= 0x20 && bytesInRow[j] < 0x80)
                        textCell.Value = (char)bytesInRow[j];
                    else
                        textCell.Value = '.';

                    
                    textGridViewRow.Cells.Add(textCell);
                }

                //TODO: Make RowDefaultCellStyle the dataStyle so it only has to be set once in Ctor
                int byteRowIndex = this.byteGridView.Rows.Add(dataGridViewRow);
                DataGridViewCellStyle byteStyle = new DataGridViewCellStyle();
                byteStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                byteStyle.SelectionBackColor = SelectionBackColor;
                byteStyle.SelectionForeColor = SelectionForeColor;
                byteStyle.BackColor = CellBackColor;

                this.byteGridView.Rows[byteRowIndex].DefaultCellStyle = byteStyle;
                //Debug.WriteLine($"default back: {}");
                //this.dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;

                int textRowIndex = this.textGridView.Rows.Add(textGridViewRow);
                DataGridViewCellStyle textStyle = new DataGridViewCellStyle();
                textStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                textStyle.SelectionBackColor = SelectionBackColor;
                textStyle.SelectionForeColor = SelectionForeColor;
                textStyle.BackColor = CellBackColor;
                this.textGridView.Rows[textRowIndex].DefaultCellStyle = textStyle;

            }
            data.Position = 0;
        }

        public byte[] GetBytes()
        {
            return new byte[2];// data.GetBuffer();
        }

        private void byteGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                this.byteGridView.Rows[e.RowIndex].HeaderCell.Value = $"0x{(e.ColumnIndex + (e.RowIndex * 16)).ToString(OFFSET_FORMAT)}";

                if (sender == this.byteGridView)
                    this.textGridView.CurrentCell = this.textGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                else if (sender == this.textGridView)
                    this.byteGridView.CurrentCell = this.byteGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            }
        }

        private void byteGridView_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            this.byteGridView.Rows[e.RowIndex].HeaderCell.Value = $"0x{(e.RowIndex * 16).ToString(OFFSET_FORMAT)}";
            Debug.WriteLine($"this.dataGridView.ColumnHeadersHeight: {this.byteGridView.ColumnHeadersHeight}");
        }

        private void byteGridView_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                Debug.WriteLine($"dataGridView_CellMouseEnter {e.ColumnIndex}x{e.RowIndex}");
                SuspendLayout();
                this.byteGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = CellHoverBackColor;
                this.textGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = CellHoverBackColor;
                ResumeLayout();
            }
        }

        private void byteGridView_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                SuspendLayout();
                this.byteGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = CellBackColor;
                this.textGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = CellBackColor;

                ResumeLayout();

            }
        }
    }
}
