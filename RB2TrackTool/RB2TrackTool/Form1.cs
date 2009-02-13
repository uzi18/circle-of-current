using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace RB2TrackTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // set up file open dialog to only accept midi files
            MidiOpenFileDialog.Filter = "midi files (*.mid)|*.mid";
            MidiOpenFileDialog.FilterIndex = 1;
            MidiOpenFileDialog.RestoreDirectory = true;
        }

        private void OpenMidiButton_Click(object sender, EventArgs e)
        {
            MidiOpenFileDialog.ShowDialog();
        }

        private void MidiOpenFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            LogMidi(MidiOpenFileDialog);
        }
    }
}
