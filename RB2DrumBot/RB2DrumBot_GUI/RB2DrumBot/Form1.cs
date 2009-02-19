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
        double time_taken_tp;
		DevEvent[] dev_event = new DevEvent[3000];
        int event_cnt;
        int event_length;		
        int bot_status;
        Stopwatch stop_watch = new Stopwatch();
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
        }

        private void FileListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
			is_playing = false;
			is_cali = false;
			stop_watch.Stop();
			
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
                is_playing = false;
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

            if (is_playing)
            {
                PlayButton.Enabled = false;
                CaliButton.Enabled = false;
                AbortButton.Enabled = true;
                FileListBox.Enabled = false;
            }
            else
            {
                CaliButton.Enabled = true;
                PlayButton.Enabled = true;
                AbortButton.Enabled = false;
                FileListBox.Enabled = true;
                stop_watch.Stop();
            }

            PlayProgBar.Minimum = 0;
            PlayProgBar.Maximum = event_length;
            PlayProgBar.Value = event_cnt;

            SongLengthLabel.Text = Convert.ToString(length_of_song);
            TimeTakenLabel.Text = Convert.ToString(time_taken);
            TimeTakenToPlayLabel.Text = Convert.ToString(time_taken_tp);

            PercentAdjLabel.Text = Convert.ToString(percent_adj * 100);
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
            is_playing = true;
            is_cali = false;
        }

        private void AbortButton_Click(object sender, EventArgs e)
        {
            is_playing = false;
			if (SerPort.IsOpen)
			{
	            byte[] b = new byte[5];
	            b[0] = 0;
	            b[1] = 3;
	            b[2] = 0;
	            b[3] = 0;
	            b[4] = 0xFF;
	            SerPort.Write(b, 0, 5);
			}
        }

        private void Player_Tick(object sender, EventArgs e)
        {
            if (SerPort.IsOpen)
            {
                if (SerPort.BytesToRead > 0)
                {
                    bot_status = (int)SerPort.ReadByte();

                    if (bot_status == 1 && is_playing)
                    {
                        event_cnt = 0;
                        SendNextInstruct();
                        stop_watch.Reset();
                    }
                    else if (bot_status == 0 && is_playing)
                    {
                        SendNextInstruct();
                    }
                    else if (bot_status == 2)
                    {
                        is_playing = false;
                        event_cnt = 0;
                    }
                    else if (bot_status == 3)
                    {
                        stop_watch.Reset();
                        stop_watch.Start();
                    }
					else if (bot_status == 4)
                    {
                        stop_watch.Stop();
                        if (is_cali)
                        {
                            CalcAutoAdj();
                        }
                    }
                }
            }
            if (is_cali)
            {
                time_taken = Convert.ToDouble(stop_watch.ElapsedMilliseconds) / (double)1000;
            }
            else
            {
                time_taken_tp = Convert.ToDouble(stop_watch.ElapsedMilliseconds) / (double)1000;
            }
        }

        private void CalBut_Click(object sender, EventArgs e)
        {
            is_playing = true;
            is_cali = true;
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

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveAdjFile();
        }

        private void LoadButton_Click(object sender, EventArgs e)
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
        }

        private void ClearCaliBut_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < event_length; i++)
            {
                dev_event[i].delay_autoadj = 0;
				ListOfNotes.Rows[i].Cells[2].Value = Convert.ToString(dev_event[i].delay_autoadj);
				manual_lock = true;
                ListOfNotes.Rows[i].Height = minimum_row_height + Convert.ToInt32(Math.Round(((double)(dev_event[i].delay + dev_event[i].delay_autoadj + dev_event[i].delay_manualadj) * height_scale)));
                manual_lock = false;
            }
        }

        private void PercentAdjBar_Scroll(object sender, EventArgs e)
        {
            percent_adj = (double)PercentAdjBar.Value / (double)10000;
        }
    }
}
