using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
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
        DataGridViewCell prevSelectedCell = null;

        public HexEditor()
        {
            InitializeComponent();

            // Events
            this.byteGridView.ColumnHeadersHeightChanged += (object sender, System.EventArgs e) => 
                this.textGridView.ColumnHeadersHeight = this.byteGridView.ColumnHeadersHeight;

            this.textGridView.Scroll += byteGridView_Scroll;

            this.textGridView.CellMouseEnter += byteGridView_CellMouseEnter;
            this.textGridView.CellMouseLeave += byteGridView_CellMouseLeave;
            this.textGridView.CellPainting += byteGridView_CellPainting;
            this.textGridView.CellStateChanged += byteGridView_CellStateChanged;

            // Fix columns' width
            for (int i = 0; i < this.byteGridView.Columns.Count; i++)
            {
                DataGridViewColumn bc = this.byteGridView.Columns[i];
                DataGridViewColumn tc = this.textGridView.Columns[i];
                bc.DataPropertyName = $"0x{i:X}";
                tc.DataPropertyName = $"0x{i:X}";
                tc.Width = 18;
            }



            // Enable Double Buffering to prevent flickering on redraw
            this.byteGridView
                .GetType()
                .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .SetValue(byteGridView, true, null);

            this.textGridView
                .GetType()
                .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .SetValue(byteGridView, true, null);

            // Set default Row Styles
            DataGridViewCellStyle defaultCellStyle = new DataGridViewCellStyle();
            defaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            defaultCellStyle.SelectionBackColor = SelectionBackColor;
            defaultCellStyle.SelectionForeColor = SelectionForeColor;
            defaultCellStyle.BackColor = CellBackColor;
            this.byteGridView.DefaultCellStyle = defaultCellStyle;
            this.textGridView.DefaultCellStyle = defaultCellStyle;
        }

        public void ClearBytes()
        {
            data = null;

            DataTable byteDataTable = new DataTable();
            DataTable textDataTable = new DataTable();

            for (int i = 0; i < 16; i++)
            {
                byteDataTable.Columns.Add(new DataColumn($"0x{i:X}"));
                textDataTable.Columns.Add(new DataColumn($"0x{i:X}"));
            }

            this.byteGridView.DataSource = byteDataTable;
            this.textGridView.DataSource = textDataTable;
        }

        public void SetBytes(byte[] bytes)
        {
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            data = new MemoryStream(bytes);

            this.byteGridView.ClearSelection();
            this.textGridView.ClearSelection();
;

            DataTable byteDataTable = new DataTable();
            DataTable textDataTable = new DataTable();

            for (int i = 0; i < 16; i++)
            {
                byteDataTable.Columns.Add(new DataColumn($"0x{i:X}"));
                textDataTable.Columns.Add(new DataColumn($"0x{i:X}"));
            }


            byte[] bytesInRow = new byte[16];
            for (int i = 0; i < data.Length; i += 16)
            {
                DataRow byteRow = byteDataTable.NewRow();
                DataRow textRow = textDataTable.NewRow();
                
                int bytesRead = data.Read(bytesInRow, 0, 16);

                for (int j = 0; j < bytesRead; j++)
                {     
                    byteRow[$"0x{j:X}"] = bytesInRow[j].ToString(BYTE_FORMAT);

                    if (bytesInRow[j] >= 0x20 && bytesInRow[j] < 0x80)
                        textRow[$"0x{j:X}"] = (char)bytesInRow[j];
                    else
                        textRow[$"0x{j:X}"] = '.';
                }

                byteDataTable.Rows.Add(byteRow);
                textDataTable.Rows.Add(textRow);
            }
            this.byteGridView.DataSource = byteDataTable;
            this.textGridView.DataSource = textDataTable;

            data.Position = 0;

            stopwatch.Stop();
            Debug.WriteLine($"HexEditor.SetBytes(..) Total Refresh time: {stopwatch.ElapsedMilliseconds}ms");
            Debug.WriteLine("------------------------------------");
        }

        public byte[] GetBytes()
        {
            return new byte[2];
        }

        public void SelectPosition(int column, int row)
        {
            this.byteGridView.CurrentCell = this.byteGridView.Rows[row].Cells[column];
        }

        private void byteGridView_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            this.byteGridView.Rows[e.RowIndex].HeaderCell.Value = $"0x{(e.RowIndex * 16).ToString(OFFSET_FORMAT)}";
            Debug.WriteLine($"byteGridView_RowLeave - row: {e.RowIndex}");
        }

        private void byteGridView_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                object cellValue = this.byteGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (cellValue != null)
                {
                    this.byteGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag = CellHoverBackColor;
                    this.byteGridView.InvalidateCell(e.ColumnIndex, e.RowIndex);
                    this.textGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag = CellHoverBackColor;
                    this.textGridView.InvalidateCell(e.ColumnIndex, e.RowIndex);
                }
               
            }
        }

        private void byteGridView_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                object cellValue = this.byteGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (cellValue != null)
                {
                    this.byteGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag = null;
                    this.textGridView.InvalidateCell(e.ColumnIndex, e.RowIndex);
                    this.byteGridView.InvalidateCell(e.ColumnIndex, e.RowIndex);
                    this.textGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag = null;
                }

            }
        }

        private void byteGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1)
                return;

            DataGridViewSelectedCellCollection cellCollection = this.byteGridView.SelectedCells;
            if (cellCollection.Count > 0)
            {
                int paintingCellValue, selectedCellValue;
                DataGridViewCell selectedCell = cellCollection[0];

                data.Position = e.ColumnIndex + (e.RowIndex * 16);
                paintingCellValue = data.ReadByte();
                data.Position = selectedCell.ColumnIndex + (selectedCell.RowIndex * 16);
                selectedCellValue = data.ReadByte();
                data.Position = 0;

                if (paintingCellValue == selectedCellValue)
                {
                    e.CellStyle.BackColor = Color.Yellow; 
                }
            }

            DataGridViewCell paintingCell = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (paintingCell.Tag != null && paintingCell.Tag is Color)
                e.CellStyle.BackColor = (Color)paintingCell.Tag;
        }

        private void byteGridView_CellStateChanged(object sender, DataGridViewCellStateChangedEventArgs e)
        {
            if (e.StateChanged == DataGridViewElementStates.Selected)
            {
                if (e.Cell.Value == null && e.Cell.Selected && prevSelectedCell != null)
                {
                    e.Cell.Selected = false;
                    prevSelectedCell.Selected = true;
                    prevSelectedCell.OwningRow.Selected = true;

                    ((DataGridView)sender).Invalidate();
                }
                else if (e.Cell.ColumnIndex >= 0 && e.Cell.RowIndex >= 0)
                {
                    if (e.Cell.Selected)
                        this.byteGridView.Rows[e.Cell.RowIndex].HeaderCell.Value = $"0x{(e.Cell.ColumnIndex + (e.Cell.RowIndex * 16)).ToString(OFFSET_FORMAT)}";

                    DataGridView otherGridView = sender == this.byteGridView ? this.textGridView : this.byteGridView;

                    // otherGridView has not loaded the data in yet
                    if (otherGridView.Columns.Count <= e.Cell.ColumnIndex || otherGridView.Rows.Count <= e.Cell.RowIndex)
                        return;

                    DataGridViewCell otherCell = otherGridView.Rows[e.Cell.RowIndex].Cells[e.Cell.ColumnIndex];
                    if (otherCell.Selected != e.Cell.Selected)
                    {
                        otherCell.Selected = e.Cell.Selected;
                        prevSelectedCell = e.Cell;


                       ((DataGridView)sender).Invalidate();
                        otherGridView.Invalidate();
                    }
                }
            }
            
        }

        private void byteGridView_Scroll(object sender, ScrollEventArgs e)
        {
            DataGridView otherGridView = sender == this.byteGridView ? this.textGridView : this.byteGridView;

            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                otherGridView.HorizontalScrollingOffset = e.NewValue;
            else if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
                otherGridView.FirstDisplayedScrollingRowIndex = e.NewValue;
        }

        private void byteGridView_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            DataGridViewHeaderCell headerCell = this.byteGridView.Rows[e.RowIndex].HeaderCell;
            if (e.RowIndex > 0 && headerCell.Value.ToString() == $"0x{(0).ToString(OFFSET_FORMAT)}")
                headerCell.Value = $"0x{(e.RowIndex*16).ToString(OFFSET_FORMAT)}";
        }
    }
}
