﻿using System.Diagnostics;
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

            this.textGridView.Scroll += byteGridView_Scroll;

            this.textGridView.CellMouseEnter += byteGridView_CellMouseEnter;
            this.textGridView.CellMouseLeave += byteGridView_CellMouseLeave;
            this.textGridView.CellPainting += byteGridView_CellPainting;
            this.textGridView.CellStateChanged += byteGridView_CellStateChanged;

            //foreach (DataGridViewColumn c in this.byteGridView.Columns)
            //{
            //    if (c.DividerWidth > 0)
            //        c.Width = 43;
            //    else
            //        c.Width = 23;
            //}

            foreach (DataGridViewColumn c in this.textGridView.Columns)
            {
                c.Width = 18;
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

                int textRowIndex = this.textGridView.Rows.Add(textGridViewRow);
                DataGridViewCellStyle textStyle = new DataGridViewCellStyle();
                textStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                textStyle.SelectionBackColor = SelectionBackColor;
                textStyle.SelectionForeColor = SelectionForeColor;
                textStyle.BackColor = CellBackColor;
                this.textGridView.Rows[textRowIndex].DefaultCellStyle = textStyle;

            }
            data.Position = 0;
            Debug.WriteLine($"cell DefaultCellStyle: {this.byteGridView.Rows[0].DefaultCellStyle}");
        }

        public byte[] GetBytes()
        {
            return new byte[2];// data.GetBuffer();
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
                Debug.WriteLine($"byteGridView_CellStateChanged - (CxR) {e.Cell.ColumnIndex}x{e.Cell.RowIndex}");
                if (e.Cell.Value == null && e.Cell.Selected)
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
            {
                otherGridView.FirstDisplayedScrollingRowIndex = e.NewValue;
            }
        }
    }
}
