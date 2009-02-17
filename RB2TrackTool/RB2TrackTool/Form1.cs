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

namespace RB2TrackTool
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
		double height_scale = 1;

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
                SongStatusLabel.Text = "Song Status: Midi Exists";
                GenerateBin();
				LoadAdjFile();
                GroupBox.Enabled = true;
            }
            else
            {
                SongStatusLabel.Text = "Song Status: Midi Missing";
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
            PlayProgBar.Maximum = event_length + 1;
            PlayProgBar.Value = event_cnt;

            SongLengthLabel.Text = Convert.ToString(length_of_song);
            TimeTakenLabel.Text = Convert.ToString(time_taken);
            TimeTakenToPlayLabel.Text = Convert.ToString(time_taken_tp);
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
            byte[] b = new byte[5];
            b[0] = 0;
            b[1] = 3;
            b[2] = 0;
            b[3] = 0;
            b[4] = 0xFF;
            SerPort.Write(b, 0, 5);
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
		
		private void HeightChange(object sender, DataGridViewRowEventArgs e)
		{
			int delay = Convert.ToInt32(Math.Round((double)(e.Height - minimum_row_height) * height_scale));
			int i = Convert.ToString(e.Cells[0].Value);
			dev_event[i].delay_manualadj = delay - dev_event[i].delay - dev_event[i].delay_autoadj;
			ApplyToList();
		}
		
		private void ChangeManualAdj(object sender, DataGridViewCellValidatingEventArgs e)
		{
			if (e.ColumnIndex == 3)
			{
				int new_manadj;
				if (Int32.TryParse(ListOfNotes.Rows[i].Cells[0].Value, out new_manadj))
				{
					dev_event[e.RowIndex].delay_manualadj = new_manadj;				
				}
				ApplyToList();
			}
		}
		
		private bool BitIsClear(int b, int i)
		{
			if (Convert.ToInt32(Math.Floor(b / Math.Pow(2, i))) % 2 == 1)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		
		private void ApplyToList()
		{
			string bitmap_fpath = folder_path + "\\";
			
			Bitmap BMbl = new Bitmap(bitmap_fpath + "black.bmp");
			Bitmap BMr = new Bitmap(bitmap_fpath + "red.bmp");
			Bitmap BMy = new Bitmap(bitmap_fpath + "yellow.bmp");
			Bitmap BMb = new Bitmap(bitmap_fpath + "blue.bmp");
			Bitmap BMg = new Bitmap(bitmap_fpath + "green.bmp");
			Bitmap BMo = new Bitmap(bitmap_fpath + "orange.bmp");
			Bitmap BMrb = new Bitmap(bitmap_fpath + "redb.bmp");
			Bitmap BMyb = new Bitmap(bitmap_fpath + "yellowb.bmp");
			Bitmap BMbb = new Bitmap(bitmap_fpath + "blueb.bmp");
			Bitmap BMgb = new Bitmap(bitmap_fpath + "greenb.bmp");
			
			int i;
			for (i = 0; i < event_length; i++)
			{
				try
				{
					ListOfNotes.Rows[i].MinimumHeight = minimum_row_height;
					ListOfNotes.Rows[i].Height = minimum_row_height + Convert.ToInt32(Math.Round(((double)(dev_event[i].delay + dev_event[i].delay_autoadj + dev_event[i].delay_manualadj) * scale)));
					ListOfNotes.Rows[i].Cells[0].Value = Convert.ToString(i);
					ListOfNotes.Rows[i].Cells[1].Value = Convert.ToString(dev_event[i].delay);
					ListOfNotes.Rows[i].Cells[2].Value = Convert.ToString(dev_event[i].delay_autoadj);
					ListOfNotes.Rows[i].Cells[3].Value = Convert.ToString(dev_event[i].delay_manualadj);
					
					if (BitIsClear(dev_event[i].bitmask, bass_bit))
					{
						ListOfNotes.Rows[i].Cells[4].Value = BMo;
						ListOfNotes.Rows[i].Cells[5].Value = BMo;
						ListOfNotes.Rows[i].Cells[6].Value = BMo;
						ListOfNotes.Rows[i].Cells[7].Value = BMo;
						
						if (BitIsClear(dev_event[i].bitmask, red_bit))
						{
							ListOfNotes.Rows[i].Cells[4].Value = BMrb;
						}
						
						if (BitIsClear(dev_event[i].bitmask, yellow_bit))
						{
							ListOfNotes.Rows[i].Cells[5].Value = BMyb;
						}
						
						if (BitIsClear(dev_event[i].bitmask, blue_bit))
						{
							ListOfNotes.Rows[i].Cells[6].Value = BMbb;
						}
						
						if (BitIsClear(dev_event[i].bitmask, green_bit))
						{
							ListOfNotes.Rows[i].Cells[7].Value = BMgb;
						}
						
						ListOfNotes.Rows[i].Cells[8].Value = BMo;
					}
					else
					{
						ListOfNotes.Rows[i].Cells[4].Value = BMbl;
						ListOfNotes.Rows[i].Cells[5].Value = BMbl;
						ListOfNotes.Rows[i].Cells[6].Value = BMbl;
						ListOfNotes.Rows[i].Cells[7].Value = BMbl;
						
						if (BitIsClear(dev_event[i].bitmask, red_bit))
						{
							ListOfNotes.Rows[i].Cells[4].Value = BMr;
						}
						
						if (BitIsClear(dev_event[i].bitmask, yellow_bit))
						{
							ListOfNotes.Rows[i].Cells[5].Value = BMy;
						}
						
						if (BitIsClear(dev_event[i].bitmask, blue_bit))
						{
							ListOfNotes.Rows[i].Cells[6].Value = BMb;
						}
						
						if (BitIsClear(dev_event[i].bitmask, green_bit))
						{
							ListOfNotes.Rows[i].Cells[7].Value = BMg;
						}
						
						ListOfNotes.Rows[i].Cells[8].Value = BMbl;
					}					
				}
				catch
				{
					ListOfNotes.Rows.Add();
					i--;
				}
			}
			for (int j = i; event_length <= ListOfNotes.Rows.Count - 1; j++)
			{
				ListOfNotes.Rows.RemoveAt(i);
			}
		}
    }
}
