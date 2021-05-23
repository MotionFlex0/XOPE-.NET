using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace XOPE_UI.Forms.Component
{
    public partial class HexEditor : UserControl
    {
        const string OFFSET_FORMAT = "X4";

        public System.Drawing.Color CellBackColor { get; set; } = System.Drawing.Color.White;
        public System.Drawing.Color SelectionBackColor { get; set; } = System.Drawing.Color.Blue;
        public System.Drawing.Color SelectionForeColor { get; set; } = System.Drawing.Color.White;
        public System.Drawing.Color CellHoverBackColor { get; set; } = System.Drawing.Color.Cyan;

        Stream data;
        public HexEditor()
        {
            InitializeComponent();


            this.dataGridView.ColumnHeadersHeightChanged += (object sender, System.EventArgs e) => 
                this.textGridView.ColumnHeadersHeight = this.dataGridView.ColumnHeadersHeight;

            this.textGridView.CellClick += dataGridView_CellClick;
            this.textGridView.CellMouseEnter += dataGridView_CellMouseEnter;
            this.textGridView.CellMouseLeave += dataGridView_CellMouseLeave;

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            //this.dataGridView.
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

                    byteCell.Value = bytesInRow[j].ToString("X2");
                    dataGridViewRow.Cells.Add(byteCell);

                    if (bytesInRow[j] >= 0x20 && bytesInRow[j] < 0x80)
                        textCell.Value = (char)bytesInRow[j];
                    else
                        textCell.Value = '.';

                    textGridViewRow.Cells.Add(textCell);
                }

                //TODO: Make RowDefaultCellStyle the dataStyle so it only has to be set once in Ctor
                int dataRowIndex = this.dataGridView.Rows.Add(dataGridViewRow);
                DataGridViewCellStyle dataStyle = new DataGridViewCellStyle();
                dataStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataStyle.SelectionBackColor = SelectionBackColor;
                dataStyle.SelectionForeColor = SelectionForeColor;
                dataStyle.BackColor = CellBackColor;

                this.dataGridView.Rows[dataRowIndex].DefaultCellStyle = dataStyle;
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

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                this.dataGridView.Rows[e.RowIndex].HeaderCell.Value = $"0x{(e.ColumnIndex + (e.RowIndex * 16)).ToString(OFFSET_FORMAT)}";

                if (sender == this.dataGridView)
                    this.textGridView.CurrentCell = this.textGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                else if (sender == this.textGridView)
                    this.dataGridView.CurrentCell = this.dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            }
        }

        private void dataGridView_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            this.dataGridView.Rows[e.RowIndex].HeaderCell.Value = $"0x{(e.RowIndex * 16).ToString(OFFSET_FORMAT)}";
            Debug.WriteLine($"this.dataGridView.ColumnHeadersHeight: {this.dataGridView.ColumnHeadersHeight}");
        }

        private void dataGridView_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            Debug.WriteLine("dataGridView_CellMouseEnter");

            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                SuspendLayout();
                this.dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = CellHoverBackColor;
                this.textGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = CellHoverBackColor;
                ResumeLayout();
            }
        }

        private void dataGridView_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            Debug.WriteLine("dataGridView_CellMouseLeave");
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                SuspendLayout();
                this.dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = CellBackColor;
                this.textGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = CellBackColor;

                ResumeLayout();

            }
        }
    }
}
