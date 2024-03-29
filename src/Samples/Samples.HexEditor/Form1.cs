﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Samples.HexEditor
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            //this.hexEditor.SetBytes(new byte[] { 0x31, (byte)'G', 0x33, 
            //    0x41, 0x42, 0x43, 0x01, 0x02, 0xEE, 0x91, 0x22, 0xFF, 0x76, 0x52, 0x2F,
            //    0xE5, 0xE6, 0xEE, 0x90, 0x92, 0x7F, 0x1C, 0xB1, 0xF5, 0xB8, 0x6F, 0x0E, 
            //    0xA5, 0x0A, 0xE1, 0x61, 0x62, 0x63, 0xED, 0x00, 0x00, 0x00, 0x00, 0x00,
            //    0xD4, 0xF3, 0x0C, 0x5E, 0x90, 0xA7, 0x12, 0x4C, 0x30, 0xF5, 0xF7, 0x00, 0x90});

            byte[] testData = new byte[256];
            for (int i = 0; i < testData.Length; i++)
                testData[i] = (byte)i;

            Random rand = new Random();
            testData = testData.OrderBy(x => rand.Next()).ToArray();
            this.hexEditor.SetBytes(testData);
        }

        private void randomiseData_Click(object sender, EventArgs e)
        {
            byte[] testData = new byte[16536];

            Random rand = new Random();
            rand.NextBytes(testData);

            this.hexEditor.SetBytes(testData);
        }
    }
}
