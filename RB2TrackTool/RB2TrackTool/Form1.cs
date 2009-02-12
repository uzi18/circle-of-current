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

        private int ReadVariableLength(BinaryReader r)
        {
            int sum = 0;
            int temp;

            for (int i = 0; i < 4; i++)
            {
                temp = (int)r.ReadByte();

                if (temp >= 0x80)
                {
                    sum += temp - 0x80;
                    sum *= 128;
                }
                else
                {
                    sum += temp;
                    break;
                }
            }

            return sum;
        }

        private void LogMidi(OpenFileDialog ofd)
        {
            Stream MidiFile = ofd.OpenFile();
            BinaryReader MidiReader = new BinaryReader(MidiFile);
            StreamWriter LogWriter = new StreamWriter(ofd.FileName + ".midilog.csv");
            StreamWriter GuitarWriter = new StreamWriter(ofd.FileName + ".guitartrack");
            StreamWriter BassWriter = new StreamWriter(ofd.FileName + ".basstrack");
            StreamWriter VocalWriter = new StreamWriter(ofd.FileName + ".vocaltrack");
            StreamWriter DrumWriter = new StreamWriter(ofd.FileName + ".drumtrack");

            LogWriter.Write(string.Format("file: {0}\r\n", MidiOpenFileDialog.FileName));
            LogWriter.Write("reading header chunk\r\n");
            LogWriter.Write(string.Format("midi file header: {0}{1}{2}{3}\r\n", MidiReader.ReadChar(), MidiReader.ReadChar(), MidiReader.ReadChar(), MidiReader.ReadChar()));

            int chunk_size = (int)MidiReader.ReadByte() * 256 * 256 * 256 + (int)MidiReader.ReadByte() * 256 * 256 + (int)MidiReader.ReadByte() * 256 + (int)MidiReader.ReadByte();

            LogWriter.Write(string.Format("chunk size: {0}\r\n", chunk_size));

            int format_type = (int)MidiReader.ReadByte() * 256 + (int)MidiReader.ReadByte();
            int num_tracks = (int)MidiReader.ReadByte() * 256 + (int)MidiReader.ReadByte();
            int time_division = (int)MidiReader.ReadByte() * 256 + (int)MidiReader.ReadByte();

            LogWriter.Write(string.Format("format type: {0}\r\n", format_type));
            LogWriter.Write(string.Format("# of tracks: {0}\r\n", num_tracks));
            LogWriter.Write(string.Format("time division: {0}\r\n", time_division));

            string time_mode;
            int ticks_per_frame;
            int frame_rate = 0;
            int ticks_per_beat = 0;
            double tempo = 100;

            if (time_division < 0x8000)
            {
                ticks_per_beat = time_division;
                time_mode = "a";
                LogWriter.Write(string.Format("ticks per beat: {0}\r\n", ticks_per_beat));
            }
            else
            {
                time_division -= 0x8000;
                ticks_per_frame = time_division % 256;
                frame_rate = (time_division - ticks_per_frame) / 256;
                time_mode = "b";
                LogWriter.Write(string.Format("frames per second: {0}\r\n", frame_rate));
                LogWriter.Write(string.Format("ticks per frame: {0}\r\n", ticks_per_frame));
            }

            long[] track_start_addr = new long[16];

            for (int track_index = 0; track_index < num_tracks; track_index++)
            {
                track_start_addr[track_index] = MidiReader.BaseStream.Position;

                try
                {
                    LogWriter.Write(string.Format("track #: {0}\r\n", track_index + 1));
                    LogWriter.Write(string.Format("track header: {0}{1}{2}{3}\r\n", MidiReader.ReadChar(), MidiReader.ReadChar(), MidiReader.ReadChar(), MidiReader.ReadChar()));

                    chunk_size = (int)MidiReader.ReadByte() * 256 * 256 * 256 + (int)MidiReader.ReadByte() * 256 * 256 + (int)MidiReader.ReadByte() * 256 + (int)MidiReader.ReadByte();

                    LogWriter.Write(string.Format("track chunk size: {0}\r\n", chunk_size));

                    int event_index = 0;

                    LogWriter.Write(string.Format("track #, event #, delta time, event type, parameters\r\n"));

                    for (long byte_index = MidiReader.BaseStream.Position; MidiReader.BaseStream.Position < byte_index + chunk_size;)
                    {
                        try
                        {
                            int delta_time = ReadVariableLength(MidiReader);
                            LogWriter.Write(string.Format("{0}, ", track_index + 1));
                            LogWriter.Write(string.Format("{0}, ", event_index + 1));
                            LogWriter.Write(string.Format("{0}, ", delta_time));
                            double time_in_sec = 0;

                            /*
                            if (time_mode == "b")
                            {
                                double frame_length = 1 / Convert.ToDouble(frame_rate);
                                time_in_sec = Convert.ToDouble(delta_time) / frame_length;
                            }
                            else if (time_mode == "a")
                            {
                                time_in_sec = Convert.ToDouble(delta_time) / (Convert.ToDouble(ticks_per_beat) * (tempo / 60));
                            }

                            LogWriter.Write(string.Format("real time: {0}\r\n", time_in_sec));
                            // */

                            int temp = (int)MidiReader.ReadByte();

                            if (temp < 0xF0)
                            {
                                int channel = temp % 16;
                                int event_type = (temp - channel) / 16;
                                string event_type_s = "";

                                switch (event_type)
                                {
                                    case 0x09:
                                        event_type_s = "note on";
                                        break;
                                    case 0x08:
                                        event_type_s = "note off";
                                        break;
                                    case 0x0A:
                                        event_type_s = "note aftertouch";
                                        break;
                                    case 0x0B:
                                        event_type_s = "controller";
                                        break;
                                    case 0x0C:
                                        event_type_s = "program change";
                                        break;
                                    case 0x0D:
                                        event_type_s = "channel aftertouch";
                                        break;
                                    case 0x0E:
                                        event_type_s = "pitch blend";
                                        break;
                                    default:
                                        event_type_s = string.Format("unknown: 0x{0} at byte index: {1}", Convert.ToString(event_type, 16), MidiReader.BaseStream.Position);
                                        break;
                                }

                                LogWriter.Write(string.Format("\"{0}\", ", event_type_s));
                                LogWriter.Write(string.Format("ch: {0}, ", channel));

                                int para1 = (int)MidiReader.ReadByte();
                                int para2;

                                if (event_type != 0x0C && event_type != 0x0D && event_type >= 0x08)
                                {
                                    para2 = (int)MidiReader.ReadByte();
                                    LogWriter.Write(string.Format("p1: {0}, p2: {1}\r\n", para1, para2));
                                    while (true)
                                    {
                                        long old_position = MidiReader.BaseStream.Position;
                                        delta_time = ReadVariableLength(MidiReader);
                                        int new_para1 = (int)MidiReader.ReadByte();
                                        int new_para2;

                                        if (new_para1 < 0x80)
                                        {
                                            new_para2 = (int)MidiReader.ReadByte();
                                            LogWriter.Write(string.Format("{0}, {1}, {2}, \"{3}\", ch: {4}, p1: {5}, p2: {6}\r\n", track_index + 1, event_index + 1, delta_time, event_type_s, channel, new_para1, new_para2));
                                        }
                                        else
                                        {
                                            MidiReader.BaseStream.Position = old_position;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    LogWriter.Write(string.Format("p1: {0}\r\n", para1));
                                }
                            }
                            else if (temp == 0xFF)
                            {
                                string meta_event = "";

                                int type = (int)MidiReader.ReadByte();

                                switch (type)
                                {
                                    case 0x00:
                                        meta_event = "sequence number";
                                        break;
                                    case 0x01:
                                        meta_event = "text event";
                                        break;
                                    case 0x02:
                                        meta_event = "copyright notice";
                                        break;
                                    case 0x03:
                                        meta_event = "sequence/track name";
                                        break;
                                    case 0x04:
                                        meta_event = "instrument name";
                                        break;
                                    case 0x05:
                                        meta_event = "lyrics";
                                        break;
                                    case 0x06:
                                        meta_event = "marker";
                                        break;
                                    case 0x07:
                                        meta_event = "cue point";
                                        break;
                                    case 0x20:
                                        meta_event = "midi channel prefix";
                                        break;
                                    case 0x2F:
                                        meta_event = "end of track";
                                        break;
                                    case 0x51:
                                        meta_event = "set tempo";
                                        break;
                                    case 0x54:
                                        meta_event = "SMPTE offset";
                                        break;
                                    case 0x58:
                                        meta_event = "time signature";
                                        break;
                                    case 0x59:
                                        meta_event = "key signature";
                                        break;
                                    case 0x7F:
                                        meta_event = "sequencer specific";
                                        break;
                                    default:
                                        meta_event = "unknown";
                                        break;
                                }

                                LogWriter.Write(string.Format("meta event: \"{0}\", ", meta_event));

                                if (type == 0x00)
                                {
                                    MidiReader.ReadByte();
                                    int sequence = (int)MidiReader.ReadByte() * 256 + (int)MidiReader.ReadByte();
                                    LogWriter.Write(string.Format("seq: {0}\r\n", sequence));
                                }
                                else if (type == 0x20)
                                {
                                    MidiReader.ReadByte();
                                    int chan = (int)MidiReader.ReadByte();
                                    LogWriter.Write(string.Format("ch: {0}\r\n", chan));
                                }
                                else if (type == 0x2F)
                                {
                                    MidiReader.ReadByte();
                                }
                                else if (type == 0x51)
                                {
                                    MidiReader.ReadByte();
                                    tempo = Convert.ToDouble((int)MidiReader.ReadByte() * 256 * 256 + (int)MidiReader.ReadByte() * 256 + (int)MidiReader.ReadByte());
                                    LogWriter.Write(string.Format("tempo: {0}\r\n", tempo));
                                }
                                else if (type == 0x54)
                                {
                                    MidiReader.ReadByte();
                                    int hour = (int)MidiReader.ReadByte();
                                    int min = (int)MidiReader.ReadByte();
                                    int sec = (int)MidiReader.ReadByte();
                                    int frame = (int)MidiReader.ReadByte();
                                    int subframe = (int)MidiReader.ReadByte();
                                    LogWriter.Write(string.Format("h: {0}, m: {1}, s: {2}, fr: {3}, subfr: {4}\r\n", hour, min, sec, frame, subframe));
                                }
                                else if (type == 0x58)
                                {
                                    MidiReader.ReadByte();
                                    int number = (int)MidiReader.ReadByte();
                                    int denominator = (int)MidiReader.ReadByte();
                                    int metronome = (int)MidiReader.ReadByte();
                                    int _32nds = (int)MidiReader.ReadByte();
                                    LogWriter.Write(string.Format("#: {0}, denom: {1}, metro: {2}, 32nds: {3}\r\n", number, denominator, metronome, _32nds));
                                }
                                else if (type == 0x59)
                                {
                                    MidiReader.ReadByte();
                                    MidiReader.ReadByte();
                                    MidiReader.ReadByte();
                                }
                                else if (type == 0x7F)
                                {
                                    int len = ReadVariableLength(MidiReader);
                                    for (int i = 0; i < len; i++)
                                    {
                                        MidiReader.ReadByte();
                                    }
                                }
                                else
                                {
                                    string target_string = "";
                                    int string_length = MidiReader.ReadByte();
                                    for (int i = 0; i < string_length; i++)
                                    {
                                        target_string += Convert.ToChar(MidiReader.ReadByte());
                                    }
                                    LogWriter.Write(string.Format("text: \"{0}\"\r\n", target_string));
                                }
                            }
                            else if (temp >= 0xF0)
                            {
                                int len = ReadVariableLength(MidiReader);
                                for (int i = 0; i < len; i++)
                                {
                                    MidiReader.ReadByte();
                                }
                            }

                        }
                        catch (EndOfStreamException)
                        {
                            break;
                        }

                        event_index++;
                    }


                }
                catch (EndOfStreamException)
                {
                    LogWriter.Write("Corrupted File\r\n");
                    break;
                }
            }

            MidiReader.BaseStream.Position = track_start_addr[0];




            MidiReader.Close();
            LogWriter.Close();
            MidiFile.Close();
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
