using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace RB2DrumBot
{
    public partial class Form1 : Form
    {
        string folder_path = "C:\\MOO\\midi\\songs";
        bool is_playing = false;
		string fpath;
        double length_of_song;
		double time_taken;
		DevEvent[] dev_event = new DevEvent[3000];
        int event_length;		
        int bot_status;
		bool is_cali = false;
		int green_bit = 3;
        int blue_bit = 2;
        int yellow_bit = 1;
        int red_bit = 0;
        int bass_bit = 4;
		DevEvent[] drum_dev_event = new DevEvent[3000];
		int minimum_row_height = 22;
		double height_scale = 0.025;
        double percent_adj = 1;
		bool manual_lock;

        public Form1()
        {
            InitializeComponent();
            FolderPathText.Text = folder_path;
        }

        private void LoadFolderButton_Click(object sender, EventArgs e)
        {
            DirectoryInfo dir = new DirectoryInfo(folder_path);
            DirectoryInfo[] dirArray = dir.GetDirectories();
            for (; FileListBox.Items.Count > 0;)
            {
                FileListBox.Items.Remove(FileListBox.Items[0]);
            }
            for (int i = 0; i < dirArray.Length; i++)
            {
                FileListBox.Items.Add(dirArray[i].Name);
            }
			FileListBox.SelectedIndex = 0;
			EventArgs e = new EventArgs();
			FileListBox.OnSelectedIndexChanged(e);
        }

        private void FileListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DirectoryInfo dir = new DirectoryInfo(folder_path + "\\" + FileListBox.Items[FileListBox.SelectedIndex]);
            FileInfo[] fileArray = dir.GetFiles("*.mid");
            bool file_exists = false;
            for (int i = 0; i < fileArray.Length; i++)
            {
                if (fileArray[i].Name == FileListBox.Items[FileListBox.SelectedIndex] + ".mid")
                {
                    file_exists = true;
                    break;
                }
            }

            if (file_exists)
            {
                SongStatusLabel.Text = "Midi File Exists";
                GenerateBin();
				LoadAdjFile();
                GroupBox.Enabled = true;
            }
            else
            {
                SongStatusLabel.Text = "Midi Missing";
                GroupBox.Enabled = false;
            }
        }

        private void Checker_Tick(object sender, EventArgs e)
        {
            if (SerPort.IsOpen)
            {
                PortStatusLabel.Text = "Port Status: " + SerPort.PortName + " is Opened at " + Convert.ToString(SerPort.BaudRate) + " Baud";
            }
            else
            {
                PortStatusLabel.Text = "Port Status: Closed";
                try
                {
                    SerPort.PortName = "COM6";
                    SerPort.BaudRate = 57600;
                    SerPort.Open();
                }
                catch
                {
                    PortStatusLabel.Text = "Port Status: Error";
                }
            }
        }

        private void FolderBrowseButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            string temp = "";
            for (int i = Convert.ToInt32(Math.Max(fbd.SelectedPath.Length - 5, 0)); i < fbd.SelectedPath.Length; i++)
            {
                temp += fbd.SelectedPath[i];
            }
            if (temp == "\\songs")
            {
                folder_path = fbd.SelectedPath;
            }
            FolderPathText.Text = folder_path;
            DirectoryInfo dir = new DirectoryInfo(folder_path);
            DirectoryInfo[] dirArray = dir.GetDirectories();
            for (; FileListBox.Items.Count > 0; )
            {
                FileListBox.Items.Remove(FileListBox.Items[0]);
            }
            for (int i = 0; i < dirArray.Length; i++)
            {
                FileListBox.Items.Add(dirArray[i].Name);
            }
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
			if (SerPort.IsOpen)
			{
				bot_busy = true;
	            byte[] b = new byte[1];
	            b[0] = 2;
	            SerPort.Write(b, 0, 5);
			}
        }

        private void LoadFileButton_Click(object sender, EventArgs e)
        {
            if (SerPort.IsOpen)
			{
				bot_busy = true;
	            byte[] b = new byte[1];
	            b[0] = 1;
	            SerPort.Write(b, 0, 5);
			}
        }
		
		private void StopPlayButton_Click(object sender, EventArgs e)
		{
			if (SerPort.IsOpen)
			{
				bot_busy = true;
	            byte[] b = new byte[1];
	            b[0] = 3;
	            SerPort.Write(b, 0, 5);
			}
		}

        private void PortChecker_Tick(object sender, EventArgs e)
        {
            if (SerPort.IsOpen)
            {
                if (SerPort.BytesToRead > 0)
                {
                    bot_status = (int)SerPort.ReadByte();
				}
				BotCtrl();
            }
        }

        private void ListOfNotes_RowHeightChanged(object sender, DataGridViewRowEventArgs e)
		{
            if (manual_lock == false)
            {
                int delay = Convert.ToInt32(Math.Round((double)(e.Row.Height - minimum_row_height) / height_scale));
                int i = Convert.ToInt32(e.Row.Cells[0].Value);				
                dev_event[i].delay_manualadj = delay - dev_event[i].delay - dev_event[i].delay_autoadj;
				if (e.Row.Height == (minimum_row_height + Convert.ToInt32(Math.Round(((double)(dev_event[i].delay + dev_event[i].delay_autoadj) * height_scale)))))
				{
					dev_event[i].delay_manualadj = 0;
				}
				manual_lock = true;
                ListOfNotes.Rows[i].Height = minimum_row_height + Convert.ToInt32(Math.Round(((double)(dev_event[i].delay + dev_event[i].delay_autoadj + dev_event[i].delay_manualadj) * height_scale)));
				manual_lock = false;
				ListOfNotes.Rows[i].Cells[3].Value = Convert.ToString(dev_event[i].delay_manualadj);
            }
		}

        private void ListOfNotes_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                int new_manadj;
                if (Int32.TryParse(Convert.ToString(ListOfNotes.Rows[e.RowIndex].Cells[0].Value), out new_manadj))
                {
                    dev_event[e.RowIndex].delay_manualadj = new_manadj;
                }
                manual_lock = true;
                ListOfNotes.Rows[e.RowIndex].Height = minimum_row_height + Convert.ToInt32(Math.Round(((double)(dev_event[e.RowIndex].delay + dev_event[e.RowIndex].delay_autoadj + dev_event[e.RowIndex].delay_manualadj) * height_scale)));
				manual_lock = false;
                ListOfNotes.Rows[e.RowIndex].Cells[3].Value = Convert.ToString(dev_event[e.RowIndex].delay_manualadj);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
		
		private void SerPort_Event(object sender, EventArgs e)
        {
			EventArgs ee = new EventArgs();
			PortChecker.OnTick(ee);
        }

        private void ScaleBar_Scroll(object sender, EventArgs e)
        {
            height_scale = (double)ScaleBar.Value / 1000;
            for (int i = 0; i < event_length; i++)
            {
                manual_lock = true;
                ListOfNotes.Rows[i].Height = minimum_row_height + Convert.ToInt32(Math.Round(((double)(dev_event[i].delay + dev_event[i].delay_autoadj + dev_event[i].delay_manualadj) * height_scale)));
                manual_lock = false;
            }
        }

        private void SaveConfigButton_Click(object sender, EventArgs e)
        {
            SaveAdjFile();
        }

        private void LoadConfigButton_Click(object sender, EventArgs e)
        {
            LoadAdjFile();
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < event_length; i++)
            {
                dev_event[i].delay_manualadj = 0;
				ListOfNotes.Rows[i].Cells[3].Value = Convert.ToString(dev_event[i].delay_manualadj);
				manual_lock = true;
                ListOfNotes.Rows[i].Height = minimum_row_height + Convert.ToInt32(Math.Round(((double)(dev_event[i].delay + dev_event[i].delay_autoadj + dev_event[i].delay_manualadj) * height_scale)));
                manual_lock = false;
            }
			
			PercentAdjBar.Value = 10000;
			percent_adj = (double)PercentAdjBar.Value / (double)10000;
			PercentAdjLabel.Text = Convert.ToString(percent_adj * 100);
        }

        private void PercentAdjBar_Scroll(object sender, EventArgs e)
        {
            percent_adj = (double)PercentAdjBar.Value / (double)10000;
			PercentAdjLabel.Text = Convert.ToString(percent_adj * 100);
        }
    }
}
