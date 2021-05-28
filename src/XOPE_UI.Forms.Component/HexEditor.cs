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
        DataGridViewCell prevSelectedCell = null;

        public HexEditor()
        {
            InitializeComponent();


            this.byteGridView.ColumnHeadersHeightChanged += (object sender, System.EventArgs e) => 
                this.textGridView.ColumnHeadersHeight = this.byteGridView.ColumnHeadersHeight;

            this.textGridView.SelectionChanged += byteGridView_SelectionChanged;
            this.textGridView.CellMouseEnter += byteGridView_CellMouseEnter;
            this.textGridView.CellMouseLeave += byteGridView_CellMouseLeave;
            this.textGridView.CellPainting += byteGridView_CellPainting;

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

        public void SelectPosition(int column, int row)
        {
            //this.byteGridView.Rows[]
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

        private void byteGridView_SelectionChanged(object sender, System.EventArgs e)
        {
            //DataGridViewSelectedCellCollection cellCollection = ((DataGridView)sender).SelectedCells;
            //if (cellCollection.Count > 0)
            //{
            //    DataGridViewCell currentSelectedCell = cellCollection[0];
            //    //Debug.WriteLine($"byteGridView_SelectionChanged: {currentCell.ColumnIndex}x{currentCell.RowIndex}");

            //    if (currentSelectedCell.ColumnIndex >= 0 && currentSelectedCell.RowIndex >= 0)
            //    {


            //        //otherGridView.CurrentCell = otherGridView.Rows[currentSelectedCell.RowIndex].Cells[currentSelectedCell.ColumnIndex];


            //        //foreach (DataGridViewCell c in cellCollection)
            //        //{
            //        //    //if (!(c.ColumnIndex == currentSelectedCell.ColumnIndex && c.RowIndex == currentSelectedCell.RowIndex))
            //        //    //{
            //        //    DataGridViewCell cell = otherGridView.Rows[c.RowIndex].Cells[c.ColumnIndex];
            //        //    if (cell.Selected == false)
            //        //        cell.Selected = true;
            //        //    //}
            //        //}
            //        //Debug.WriteLine($"byteGridView_SelectionChanged: (Len: {cellCollection.Count})(Type: {(sender == this.byteGridView ? "BYTE_GRID_VIEW" : "TEXT_GRID_VIEW")}){currentSelectedCell.ColumnIndex}x{currentSelectedCell.RowIndex}");

            //        // Forces DataGridViews to repaint, showing matching bytes
            //        //this.byteGridView.Invalidate();
            //        //this.textGridView.Invalidate();
            //    }
            //}
        }

        private void byteGridView_CellStateChanged(object sender, DataGridViewCellStateChangedEventArgs e)
        {
            if (e.StateChanged == DataGridViewElementStates.Selected)
            {
                Debug.WriteLine($"byteGridView_CellStateChanged - (CxR) {e.Cell.ColumnIndex}x{e.Cell.RowIndex}");
                if (e.Cell.Value == null && e.Cell.Selected)
                {
                    e.Cell.Selected = false;
                    prevSelectedCell.Selected = true;
                    prevSelectedCell.OwningRow.Selected = true;

                    //((DataGridView)sender).CurrentCell = ((DataGridView)sender)
                    //.Rows[0]
                    //.Cells[0];
                    //e.Cell.OwningRow.Selected = true;

                    //this.byteGridView.CurrentCell = prevSelectedCell;

                    //DataGridViewSelectedCellCollection cellCollection = ((DataGridView)sender).SelectedCells;
                    //if (cellCollection.Count == 1)
                    //{
                    //    ((DataGridView)sender).CurrentCell = ((DataGridView)sender)
                    //    .Rows[this.prevSelectedCell.RowIndex]
                    //    .Cells[this.prevSelectedCell.ColumnIndex];
                    //}

                    ((DataGridView)sender).Invalidate();
                }
                else if (e.Cell.ColumnIndex >= 0 && e.Cell.RowIndex >= 0)
                {
                    this.byteGridView.Rows[e.Cell.RowIndex].HeaderCell.Value = $"0x{(e.Cell.ColumnIndex + (e.Cell.RowIndex * 16)).ToString(OFFSET_FORMAT)}";

                    DataGridView otherGridView = sender == this.byteGridView ? this.textGridView : this.byteGridView;
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
    }
}
