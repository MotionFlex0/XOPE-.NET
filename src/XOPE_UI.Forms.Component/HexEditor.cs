using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace XOPE_UI.Forms.Component
{
    public partial class HexEditor : UserControl
    {
        Stream data;
        public HexEditor()
        {
            InitializeComponent();
        }

        public void SetBytes(byte[] bytes)
        {
            data = new MemoryStream(bytes);

            byte[] bytesInRow = new byte[16];
            for (int i = 0; i < data.Length; i+=16)
            {
                int bytesRead = data.Read(bytesInRow, i, 16);
                DataGridViewRow dataGridViewRow = new DataGridViewRow();// ();
                dataGridViewRow.HeaderCell.Value = $"0x{i.ToString("X2")}";

                for (int j = 0; j < bytesRead; j++)
                {
                    DataGridViewTextBoxCell byteCell = new DataGridViewTextBoxCell();
                    byteCell.Value = bytesInRow[j].ToString("X2");
                    dataGridViewRow.Cells.Add(byteCell);
                }
                this.dataGridView.Rows.Add(dataGridViewRow);
                this.dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            }
        }

        public byte[] GetBytes()
        {
            return new byte[2];// data.GetBuffer();
        }

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Debug.WriteLine($"Celled clicked: {e.RowIndex}x{e.ColumnIndex}");
            this.dataGridView.Rows[e.RowIndex].HeaderCell.Value = $"0x{e.ColumnIndex.ToString("X2")}";
        }
    }
}
