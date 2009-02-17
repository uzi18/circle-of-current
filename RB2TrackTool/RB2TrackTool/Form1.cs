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
        StreamReader instruct_stream;
        BinaryReader instruct_bin;

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
                GroupBox.Enabled = true;
            }
            else
            {
                SongStatusLabel.Text = "Song Status: Midi Missing";
                GroupBox.Enabled = false;
            }
        }

        bool is_playing = false;

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

        string fpath;
        double length_of_song;
        double time_taken;
        double time_taken_tp;
        double accum_adj;

        private void GenerateBin()
        {
            fpath = folder_path + "\\" + FileListBox.Items[FileListBox.SelectedIndex] + "\\" + FileListBox.Items[FileListBox.SelectedIndex] + ".mid";
            length_of_song = MidiToDrumChart(fpath, 12000000d);
        }

        DevEvent[] dev_event = new DevEvent[3000];
        int event_cnt;
        int event_length;

        private void PlayButton_Click(object sender, EventArgs e)
        {
            instruct_stream = new StreamReader(fpath + ".drumtrack.bin");
            instruct_bin = new BinaryReader(instruct_stream.BaseStream);
            instruct_bin.BaseStream.Position = 0;

            int i;

            for (i = 0; i < 3000; i++)
            {
                dev_event[i].sp_code = instruct_bin.ReadByte();
                dev_event[i].delay = instruct_bin.ReadUInt16();
                dev_event[i].bitmask = instruct_bin.ReadByte();
                if (dev_event[i].sp_code == 3)
                {
                    break;
                }
            }
            event_cnt = 0;
            event_length = i;

            accum_adj = 0;

            is_playing = true;
            is_cali = false;

            instruct_stream.Close();
            instruct_bin.Close();
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

        private void SendNextInstruct()
        {
            if (SerPort.BytesToWrite < 8)
            {
                byte[] b = new byte[5];
                b[0] = 0;
                b[1] = (byte)dev_event[event_cnt].sp_code;

                int delay = dev_event[event_cnt].delay;

                if (is_cali == false)
                {
                    double new_delay_d = ((double)delay * length_of_song) / time_taken;
                    int new_delay = Convert.ToInt32(Math.Floor(new_delay_d));

                    accum_adj += new_delay_d - (double)new_delay;

                    delay = new_delay;

                    while (accum_adj >= (double)1)
                    {
                        delay += 1;
                        accum_adj -= 1;
                    }
                    while (accum_adj <= (double)-1)
                    {
                        delay -= 1;
                        accum_adj += 1;
                    }
                }

                b[3] = Convert.ToByte(delay % 256);
                b[2] = Convert.ToByte((delay - Convert.ToInt32(b[3])) / 256);

                b[4] = (byte)dev_event[event_cnt].bitmask;

                SerPort.Write(b, 0, 5);

                event_cnt++;

                if (b[1] == 3)
                {
                    is_playing = false;
                }
            }
        }

        int bot_status;

        Stopwatch stop_watch = new Stopwatch();

        private void Player_Tick(object sender, EventArgs e)
        {
            if (SerPort.IsOpen && is_playing)
            {
                if (SerPort.BytesToRead > 0)
                {
                    bot_status = (int)SerPort.ReadByte();

                    if (bot_status == 1)
                    {
                        event_cnt = 0;
                        SendNextInstruct();
                    }
                    else if (bot_status == 0)
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
                }
            }
            else if (SerPort.IsOpen && is_playing == false)
            {
                if (SerPort.BytesToRead > 0)
                {
                    bot_status = (int)SerPort.ReadByte();

                    if (bot_status == 4)
                    {
                        stop_watch.Stop();
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

        bool is_cali = false;

        private void CalBut_Click(object sender, EventArgs e)
        {
            instruct_stream = new StreamReader(fpath + ".drumtrack.bin");
            instruct_bin = new BinaryReader(instruct_stream.BaseStream);
            instruct_bin.BaseStream.Position = 0;

            int i;

            for (i = 0; i < 3000; i++)
            {
                dev_event[i].sp_code = instruct_bin.ReadByte();
                dev_event[i].delay = instruct_bin.ReadUInt16();
                dev_event[i].bitmask = instruct_bin.ReadByte();
                if (dev_event[i].sp_code == 3)
                {
                    break;
                }
            }
            event_cnt = 0;
            event_length = i;

            is_playing = true;
            is_cali = true;

            accum_adj = 0;

            instruct_stream.Close();
            instruct_bin.Close();
        }
    }
}
