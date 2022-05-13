using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using XOPE_UI.Extensions;

namespace XOPE_UI.View.Component
{
    // Read-only ; TODO: Add write functionality
    // TODO: Override DataGridView and implement drawing directly inside
    public partial class HexEditor : UserControl
    {
        const string OFFSET_FORMAT = "X4";
        const string BYTE_FORMAT = "X2";

        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        
        public Color CellBackColor { get; set; } = Color.White;
        public Color SelectionBackColor { get; set; } = Color.FromArgb(70, Color.Blue);
        public Color SelectionForeColor { get; set; } = Color.Black;
        public Color CellHoverBackColor { get; set; } = Color.Cyan;

        Stream _data;
        DataGridViewCell _prevSelectedCell = null;

        bool _shiftPressed = false;
        bool _currentlyMouseDragging = false;
        DataGridViewCell _firstCellDrag = null;
        DataGridViewCell _currentCellDrag = null;

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

            this.textGridView.CellMouseDown += byteGridView_CellMouseDown;
            this.textGridView.MouseUp += byteGridView_MouseUp;
            this.textGridView.CellMouseMove += byteGridView_CellMouseMove;
            this.textGridView.CellMouseEnter += byteGridView_CellMouseEnter;
            this.textGridView.CellMouseLeave += byteGridView_CellMouseLeave;

            // Fix columns' width
            for (int i = 0; i < this.byteGridView.Columns.Count; i++)
            {
                DataGridViewColumn bc = this.byteGridView.Columns[i];
                DataGridViewColumn tc = this.textGridView.Columns[i];
                bc.DataPropertyName = $"0x{i:X}";
                tc.DataPropertyName = $"0x{i:X}";
                //tc.Width = 18;
                tc.Width = 12;

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
            defaultCellStyle.Padding = new Padding(0, 0, 0, 0);
            //defaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //defaultCellStyle.Font = new Font(defaultCellStyle.Font.FontFamily, defaultCellStyle.Font.Size);
            this.byteGridView.DefaultCellStyle = defaultCellStyle;
            this.textGridView.DefaultCellStyle = defaultCellStyle;
        }

        public void ClearBytes()
        {
            _data = null;

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

            _data = new MemoryStream(bytes);

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
            for (int i = 0; i < _data.Length; i += 16)
            {
                DataRow byteRow = byteDataTable.NewRow();
                DataRow textRow = textDataTable.NewRow();

                int bytesRead = _data.Read(bytesInRow, 0, 16);

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

            _data.Position = 0;

            stopwatch.Stop();
            Debug.WriteLine($"HexEditor.SetBytes(..) Total Refresh time: {stopwatch.ElapsedMilliseconds}ms");
            Debug.WriteLine("------------------------------------");
        }

        public byte[] GetBytes()
        {
            throw new NotImplementedException("GetBytes has not been implemented.");
        }

        public void SelectPosition(int column, int row)
        {
            this.byteGridView.CurrentCell = this.byteGridView.Rows[row].Cells[column];
        }

        private void UpdateSelectionFromMouseEvent(DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
                return;

            int selectedCellsCount = this.byteGridView.SelectedCells.Count;
            if (selectedCellsCount < 1)
                return;

            var prevEndCell = _currentCellDrag;
            var selectionStartCell = _firstCellDrag;
            var selectionEndCell = this.byteGridView[e.ColumnIndex, e.RowIndex];

            if (selectionEndCell.RowIndex == _currentCellDrag.RowIndex &&
                selectionEndCell.ColumnIndex == _currentCellDrag.ColumnIndex)
                return;

            _currentCellDrag = selectionEndCell;

            bool topToBottom = true;

            // Swap the cells if the first selected cell is after the
            //  last selected cell.
            if (selectionStartCell.RowIndex > selectionEndCell.RowIndex ||
                (selectionStartCell.RowIndex == selectionEndCell.RowIndex && 
                selectionStartCell.ColumnIndex > selectionEndCell.ColumnIndex))
            {
                var copy = selectionStartCell;
                selectionStartCell = selectionEndCell;
                selectionEndCell = copy;
                topToBottom = false;
            }

            this.BeginInvoke(() =>
            {
                this.byteGridView.SuspendLayout();
                this.textGridView.SuspendLayout();

                // Clear previous cells which is no longer be selected
                var selectedCells = this.byteGridView.SelectedCells;
                foreach (DataGridViewCell c in selectedCells)
                    c.Selected = IsCellBetweenCells(c, selectionStartCell, selectionEndCell);

                for (int i = selectionStartCell.RowIndex; i <= selectionEndCell.RowIndex; i++)
                {
                    DataGridViewRow row = this.byteGridView.Rows[i];

                    int firstCellToSelect;
                    if (row.Index == selectionStartCell.RowIndex)
                        firstCellToSelect = selectionStartCell.ColumnIndex;
                    else
                        firstCellToSelect = 0;

                    int lastCellToSelect;
                    if (row.Index == selectionEndCell.RowIndex)
                        lastCellToSelect = selectionEndCell.ColumnIndex;
                    else
                        lastCellToSelect = row.Cells.Count - 1;

                    for (int j = firstCellToSelect; j <= lastCellToSelect; j++)
                    {
                        if (!row.Cells[j].Selected)
                            row.Cells[j].Selected = true;

                        //if (topToBottom && i >= selectionEndCell.RowIndex - 1 && j >= selectionStartCell.ColumnIndex)
                        //{
                        //    this.byteGridView.InvalidateCell(j, i);
                        //    this.textGridView.InvalidateCell(j, i);
                        //}
                        //else if (!topToBottom && i == selectionEndCell.RowIndex - 1 && j <= selectionStartCell.ColumnIndex)
                        //{
                        //    this.byteGridView.InvalidateCell(j, i);
                        //    this.textGridView.InvalidateCell(j, i);
                        //}
                    }
                }
                this.byteGridView.ResumeLayout();
                this.textGridView.ResumeLayout();
            });
        }

        // startCell and endCell are inclusive
        private bool IsCellBetweenCells(DataGridViewCell cell, DataGridViewCell startCell, DataGridViewCell endCell)
        {
            return cell.RowIndex >= startCell.RowIndex && cell.RowIndex <= endCell.RowIndex &&
                                    cell.ColumnIndex >= startCell.ColumnIndex && cell.ColumnIndex <= endCell.ColumnIndex;
        }

        private void byteGridView_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            this.byteGridView.Rows[e.RowIndex].HeaderCell.Value = $"0x{(e.RowIndex * 16).ToString(OFFSET_FORMAT)}";
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
                    this.byteGridView.InvalidateCell(e.ColumnIndex, e.RowIndex);
                    this.textGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag = null;
                    this.textGridView.InvalidateCell(e.ColumnIndex, e.RowIndex);
                }

            }
        }

        private void byteGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (!e.State.HasFlag(DataGridViewElementStates.Displayed))
                return;

            // Check if value of current cell matching the value of the selected cell
            DataGridViewSelectedCellCollection cellCollection = this.byteGridView.SelectedCells;
            if (cellCollection.Count > 0)
            {
                int paintingCellValue, selectedCellValue;
                DataGridViewCell selectedCell = cellCollection[0];

                _data.Position = e.ColumnIndex + (e.RowIndex * 16);
                paintingCellValue = _data.ReadByte();
                _data.Position = selectedCell.ColumnIndex + (selectedCell.RowIndex * 16);
                selectedCellValue = _data.ReadByte();
                _data.Position = 0;

                if (paintingCellValue == selectedCellValue)
                {
                    e.CellStyle.BackColor = Color.Yellow;
                }
            }

            // Custom painting for each cell
            DataGridViewCell paintingCell = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (paintingCell.Tag != null && paintingCell.Tag is Color)
                e.CellStyle.BackColor = (Color)paintingCell.Tag;

            int dividerWidth = paintingCell.OwningColumn.DividerWidth;
            Rectangle cellBounds = e.CellBounds with { Width = e.CellBounds.Width - dividerWidth };

            Rectangle reducedCellRect = cellBounds;
            reducedCellRect.Y += 4;
            reducedCellRect.Height -= 8;

            bool isSelected = e.State.HasFlag(DataGridViewElementStates.Selected);
            Color textColor = isSelected ? e.CellStyle.SelectionForeColor : e.CellStyle.ForeColor;

            using (Brush clearBrush = new SolidBrush(Color.White))
            using (Pen linePen = new Pen(Color.Red))
            using (Brush backBrush = isSelected ?
                new SolidBrush(e.CellStyle.SelectionBackColor) :
                new SolidBrush(e.CellStyle.BackColor))
            {
                // Clear the cell (incl. its divider) by setting it to a solid color.
                e.Graphics.FillRectangle(clearBrush, cellBounds with { Width = cellBounds.Width + dividerWidth });

                e.Graphics.FillRectangle(backBrush, reducedCellRect);

                // Not used right now. It should draw a polygon around the selected bytes/text
                if (false && isSelected)
                {
                    bool drawTopLine = paintingCell.RowIndex == 0 ||
                        !paintingCell.DataGridView[paintingCell.ColumnIndex, paintingCell.RowIndex - 1].Selected;

                    bool drawBottomLine = paintingCell.RowIndex == paintingCell.DataGridView.RowCount - 1 ||
                        !paintingCell.DataGridView[paintingCell.ColumnIndex, paintingCell.RowIndex + 1].Selected;

                    bool drawLeftLine = paintingCell.ColumnIndex == 0 ||
                        !paintingCell.DataGridView[paintingCell.ColumnIndex - 1, paintingCell.RowIndex].Selected;

                    bool drawRightLine = paintingCell.ColumnIndex == paintingCell.DataGridView.ColumnCount - 1 ||
                        !paintingCell.DataGridView[paintingCell.ColumnIndex + 1, paintingCell.RowIndex].Selected;

                    if (drawTopLine)
                        e.Graphics.DrawLine(linePen, new Point(e.CellBounds.Left, e.CellBounds.Top),
                            new Point(e.CellBounds.Right, e.CellBounds.Top));

                    if (drawBottomLine)
                        e.Graphics.DrawLine(linePen, new Point(e.CellBounds.Left, e.CellBounds.Bottom - 1),
                            new Point(e.CellBounds.Right, e.CellBounds.Bottom - 1));

                    if (drawLeftLine)
                        e.Graphics.DrawLine(linePen, new Point(e.CellBounds.Left, e.CellBounds.Top),
                            new Point(e.CellBounds.Left, e.CellBounds.Bottom - 1));

                    if (drawRightLine)
                        e.Graphics.DrawLine(linePen, new Point(e.CellBounds.Right - 1, e.CellBounds.Top),
                            new Point(e.CellBounds.Right - 1, e.CellBounds.Bottom - 1));
                }

                if (e.Value != DBNull.Value)
                {
                    Size textSize = TextRenderer.MeasureText((string)e.Value, e.CellStyle.Font);
                    Point textPoint = cellBounds.Location;
                    textPoint.X += (cellBounds.Width / 2) - (textSize.Width / 2);
                    textPoint.Y += (cellBounds.Height / 2) - (textSize.Height / 2);

                    TextRenderer.DrawText(e.Graphics, (string)e.Value,
                                        e.CellStyle.Font, textPoint, textColor);
                }
                else
                {
                    Brush nullValueBrush = new SolidBrush(Color.White);
                    e.Graphics.FillRectangle(nullValueBrush, reducedCellRect);
                    nullValueBrush.Dispose();
                }
            }
            e.Handled = true;
        }

        private void byteGridView_CellStateChanged(object sender, DataGridViewCellStateChangedEventArgs e)
        {
            if (e.StateChanged == DataGridViewElementStates.Selected)
            {
                if (e.Cell.Value == DBNull.Value && e.Cell.Selected && _prevSelectedCell != null)
                {
                    e.Cell.Selected = false;
                    _prevSelectedCell.Selected = true;
                    _prevSelectedCell.OwningRow.Selected = true;

                    //((DataGridView)sender).Invalidate();
                    
                }
                else if (e.Cell.ColumnIndex >= 0 && e.Cell.RowIndex >= 0)
                {
                    // To reduce time taken to SetBytes, the cell row header is dynamically set to the correct value when first drawn
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
                        _prevSelectedCell = e.Cell;

                        //((DataGridView)sender).Invalidate();
                        //otherGridView.Invalidate();
                    }
                }
            }
        }

        private void byteGridView_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            DataGridViewHeaderCell headerCell = this.byteGridView.Rows[e.RowIndex].HeaderCell;
            if (e.RowIndex > 0 && headerCell.Value.ToString() == $"0x{(0).ToString(OFFSET_FORMAT)}")
                headerCell.Value = $"0x{(e.RowIndex * 16).ToString(OFFSET_FORMAT)}";
        }


        private void byteGridView_Scroll(object sender, ScrollEventArgs e)
        {
            DataGridView otherGridView = sender == this.byteGridView ? this.textGridView : this.byteGridView;

            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll &&
                otherGridView.HorizontalScrollingOffset != e.NewValue)
                otherGridView.HorizontalScrollingOffset = e.NewValue;
            else if (e.ScrollOrientation == ScrollOrientation.VerticalScroll &&
                otherGridView.FirstDisplayedScrollingRowIndex != e.NewValue)
                otherGridView.FirstDisplayedScrollingRowIndex = e.NewValue;
        }

        private void byteGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
                return;

            if (_shiftPressed)
                UpdateSelectionFromMouseEvent(e);
            else
            {
                _currentlyMouseDragging = true;
                _firstCellDrag = this.byteGridView[e.ColumnIndex, e.RowIndex];
                _currentCellDrag = _firstCellDrag;
            }
        }

        private void byteGridView_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (!_currentlyMouseDragging)
                return;

            if (e.ColumnIndex < 0 || e.RowIndex < 0)
                return;

            UpdateSelectionFromMouseEvent(e);
        }

        protected override bool ProcessKeyPreview(ref Message m)
        {
            if ((Keys)m.WParam == Keys.ShiftKey)
            {
                if (m.Msg == WM_KEYDOWN || m.Msg == WM_KEYUP)
                {
                    _shiftPressed = m.Msg == WM_KEYDOWN;
                    return true;
                }
            }
            return base.ProcessKeyPreview(ref m);
        }

        private void byteGridView_MouseUp(object sender, MouseEventArgs e)
        {
            _currentlyMouseDragging = false;
        }

        private void byteGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.C)
            {
                DataGridView dataGridView = sender as DataGridView;
                DataObject dataObject = dataGridView.GetClipboardContent();
                string content = Regex.Replace(dataObject.GetText().Trim(), @"\s+", " "); // Changes default delimiter (\t) to a single space
                Clipboard.SetText(content);
                e.Handled = true;
            }
        }

        private void textGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.C)
            {
                DataGridView dataGridView = sender as DataGridView;
                DataObject dataObject = dataGridView.GetClipboardContent();
                string content = Regex.Replace(dataObject.GetText().Trim(), @"\s+", "");
                Clipboard.SetText(content);
                e.Handled = true;
            }
        }
    }
}
