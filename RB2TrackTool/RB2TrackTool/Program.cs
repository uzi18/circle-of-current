using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace RB2TrackTool
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    public struct TempoPeriod
    {
        public double begin, tempo;
        public TempoPeriod(int MPQN, double start_time)
        {
            tempo = (60 * 1000 * 1000) / (double)MPQN;
            begin = start_time;
        }
    }
	
	public struct NoteEvent
	{
		public double time, delta_time;
		public bool[] note = new bool[128];
		public NoteEvent(double time_, double delta_time_)
		{
			time = time_;
			delta_time = delta_time_;
			for (int i = 0; i < 128; i++)
			{
				note[i] = false;
			}
		}
	}
	
	public struct DevEvent
	{
		public int dev_id, delay, bitmask;
		public DevEvent(int id, int delay_, int bitmask_)
		{
			dev_id = id;
			delay = delay_;
			bitmask = bitmask_;
		}
	}

    partial class Form1
    {
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

            int ticks_per_frame;
            int frame_rate = 0;
            int ticks_per_beat = 0;
            double tempo = 120;

            #region

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

            #endregion

            long[] track_start_addr = new long[16];

            #region

            for (int track_index = 0; track_index < num_tracks; track_index++)
            {
                try
                {
                    LogWriter.Write(string.Format("track #: {0}\r\n", track_index + 1));
                    LogWriter.Write(string.Format("track header: {0}{1}{2}{3}\r\n", MidiReader.ReadChar(), MidiReader.ReadChar(), MidiReader.ReadChar(), MidiReader.ReadChar()));

                    chunk_size = (int)MidiReader.ReadByte() * 256 * 256 * 256 + (int)MidiReader.ReadByte() * 256 * 256 + (int)MidiReader.ReadByte() * 256 + (int)MidiReader.ReadByte();

                    LogWriter.Write(string.Format("track chunk size: {0}\r\n", chunk_size));

                    int event_index = 0;

                    LogWriter.Write(string.Format("track #, event #, delta time, event type, parameters\r\n"));

                    track_start_addr[track_index] = MidiReader.BaseStream.Position;

                    for (long byte_index = MidiReader.BaseStream.Position; MidiReader.BaseStream.Position < byte_index + chunk_size; )
                    {
                        try
                        {
                            int delta_time = ReadVariableLength(MidiReader);
                            LogWriter.Write(string.Format("{0}, ", track_index + 1));
                            LogWriter.Write(string.Format("{0}, ", event_index + 1));
                            LogWriter.Write(string.Format("{0}, ", delta_time));

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
                                    LogWriter.Write(string.Format("seq: {0}", sequence));
                                }
                                else if (type == 0x20)
                                {
                                    MidiReader.ReadByte();
                                    int chan = (int)MidiReader.ReadByte();
                                    LogWriter.Write(string.Format("ch: {0}", chan));
                                }
                                else if (type == 0x2F)
                                {
                                    MidiReader.ReadByte();
                                }
                                else if (type == 0x51)
                                {
                                    MidiReader.ReadByte();
                                    tempo = Convert.ToDouble((int)MidiReader.ReadByte() * 256 * 256 + (int)MidiReader.ReadByte() * 256 + (int)MidiReader.ReadByte());
                                    LogWriter.Write(string.Format("tempo: {0}", tempo));
                                }
                                else if (type == 0x54)
                                {
                                    MidiReader.ReadByte();
                                    int hour = (int)MidiReader.ReadByte();
                                    int min = (int)MidiReader.ReadByte();
                                    int sec = (int)MidiReader.ReadByte();
                                    int frame = (int)MidiReader.ReadByte();
                                    int subframe = (int)MidiReader.ReadByte();
                                    LogWriter.Write(string.Format("h: {0}, m: {1}, s: {2}, fr: {3}, subfr: {4}", hour, min, sec, frame, subframe));
                                }
                                else if (type == 0x58)
                                {
                                    MidiReader.ReadByte();
                                    int number = (int)MidiReader.ReadByte();
                                    int denominator = (int)MidiReader.ReadByte();
                                    int metronome = (int)MidiReader.ReadByte();
                                    int _32nds = (int)MidiReader.ReadByte();
                                    LogWriter.Write(string.Format("#: {0}, denom: {1}, metro: {2}, 32nds: {3}", number, denominator, metronome, _32nds));
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
                                    LogWriter.Write(string.Format("text: \"{0}\"", target_string));
                                }
                                LogWriter.Write(string.Format("\r\n"));
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

            #endregion

            MidiReader.BaseStream.Position = track_start_addr[0];

            TempoPeriod[] tempo_period = new TempoPeriod[200];
			
			#region

            int tempo_index = 0;
            double current_time = 0;
            tempo = 120;

            while (true)
            {
                int delta_time = ReadVariableLength(MidiReader);

                current_time = current_time + ((delta_time / ticks_per_beat) / (tempo / 60));

                int foo = (int)MidiReader.ReadByte();

                if (foo < 0xF0)
                {
                    int channel = foo % 16;
                    int event_type = (foo - channel) / 16;

                    int para1 = (int)MidiReader.ReadByte();
                    int para2;

                    if (event_type != 0x0C && event_type != 0x0D && event_type >= 0x08)
                    {
                        para2 = (int)MidiReader.ReadByte();
                        while (true)
                        {
                            long old_position = MidiReader.BaseStream.Position;
                            delta_time = ReadVariableLength(MidiReader);
                            current_time = current_time + ((delta_time / ticks_per_beat) / (tempo / 60));

                            int new_para1 = (int)MidiReader.ReadByte();
                            int new_para2;

                            if (new_para1 < 0x80)
                            {
                                new_para2 = (int)MidiReader.ReadByte();
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
                    }
                }
                else if (foo == 0xFF)
                {
                    int type = (int)MidiReader.ReadByte();

                    if (type == 0x00)
                    {
                        MidiReader.ReadByte();
                        int sequence = (int)MidiReader.ReadByte() * 256 + (int)MidiReader.ReadByte();
                    }
                    else if (type == 0x20)
                    {
                        MidiReader.ReadByte();
                        int chan = (int)MidiReader.ReadByte();
                    }
                    else if (type == 0x2F)
                    {
                        MidiReader.ReadByte();
                        break;
                    }
                    else if (type == 0x51)
                    {
                        MidiReader.ReadByte();
                        int tempo_raw = (int)MidiReader.ReadByte() * 256 * 256 + (int)MidiReader.ReadByte() * 256 + (int)MidiReader.ReadByte();
                        tempo_period[tempo_index] = new TempoPeriod(tempo_raw, current_time);
                        tempo = tempo_period[tempo_index].tempo;
                        LogWriter.Write(string.Format("New Tempo # {0} is {1} BPM at {2}\r\n", tempo_index + 1, tempo, current_time));
                        tempo_index++;
                    }
                    else if (type == 0x54)
                    {
                        MidiReader.ReadByte();
                        int hour = (int)MidiReader.ReadByte();
                        int min = (int)MidiReader.ReadByte();
                        int sec = (int)MidiReader.ReadByte();
                        int frame = (int)MidiReader.ReadByte();
                        int subframe = (int)MidiReader.ReadByte();
                    }
                    else if (type == 0x58)
                    {
                        MidiReader.ReadByte();
                        int number = (int)MidiReader.ReadByte();
                        int denominator = (int)MidiReader.ReadByte();
                        int metronome = (int)MidiReader.ReadByte();
                        int _32nds = (int)MidiReader.ReadByte();
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
                    }
                }
                else if (foo >= 0xF0)
                {
                    int len = ReadVariableLength(MidiReader);
                    for (int i = 0; i < len; i++)
                    {
                        MidiReader.ReadByte();
                    }
                }
            }
			
			#endregion
			
			MidiReader.BaseStream.Position = track_start_addr[1];
			
			NoteEvent[] drum_note_event = new NoteEvent[3000];
			
			#region

			int last_tempo_index = 0;
            current_time = 0;
			double last_time = 0;
            tempo = 120;
			int note_event_index = 0;
			int last_note_event_index = 0;
			drum_note_event[0] = new NoteEvent(current_time);

            while (true)
            {
                int delta_time = ReadVariableLength(MidiReader);

				for (int i = last_tempo_index; i < tempo_index; i++)
				{
					if (tempo_period[i].begin <= current_time)
					{
						tempo = tempo_period[i].tempo;
						last_tempo_index = i;
					}
					else
					{
						break;
					}
				}
				
                current_time = current_time + ((delta_time / ticks_per_beat) / (tempo / 60));
				
				if (last_time != current_time)
				{
					note_event_index++;
					drum_note_event[note_event_index] = new NoteEvent(current_time, current_time - last_time);
				}
				last_time = current_time;

                int foo = (int)MidiReader.ReadByte();

                if (foo < 0xF0)
                {
                    int channel = foo % 16;
                    int event_type = (foo - channel) / 16;

                    int para1 = (int)MidiReader.ReadByte();
                    int para2;
					
					if (event_type == 0x08)
					{
						drum_note_event[note_event_index].note[para1] = false;
					}
					else if (event_type == 0x09)
					{
						drum_note_event[note_event_index].note[para1] = true;
					}
					
					if (last_note_event_index != note_event_index)
					{
						DrumWriter.Write(string.Format("{0},", current_time));
						DrumWriter.Write(string.Format("{0},", delta_time));
						
						for (int i = 0; i < 128; i++)
						{
							if (drum_note_event[last_note_event_index].note[i] == true)
							{
								DrumWriter.Write(string.Format("#,"));
							}
							else
							{
								DrumWriter.Write(string.Format(","));
							}
						}
						
						DrumWriter.Write(string.Format("\r\n"));
					}
					
					last_note_event_index = note_event_index;

                    if (event_type != 0x0C && event_type != 0x0D && event_type >= 0x08)
                    {
                        para2 = (int)MidiReader.ReadByte();
                        while (true)
                        {
							for (int i = last_tempo_index; i < tempo_index; i++)
							{
								if (tempo_period[i].begin <= current_time)
								{
									tempo = tempo_period[i].tempo;
									last_tempo_index = i;
								}
								else
								{
									break;
								}
							}
						
                            long old_position = MidiReader.BaseStream.Position;
                            delta_time = ReadVariableLength(MidiReader);					
                            current_time = current_time + ((delta_time / ticks_per_beat) / (tempo / 60));
							
							if (last_time != current_time)
							{
								note_event_index++;
								drum_note_event[note_event_index] = new NoteEvent(current_time, current_time - last_time);
							}
							last_time = current_time;

                            int new_para1 = (int)MidiReader.ReadByte();
                            int new_para2;

                            if (new_para1 < 0x80)
                            {
                                new_para2 = (int)MidiReader.ReadByte();
								
								if (event_type == 0x08)
								{
									drum_note_event[note_event_index].note[para1] = false;
								}
								else if (event_type == 0x09)
								{
									drum_note_event[note_event_index].note[para1] = true;
								}
								
								if (last_note_event_index != note_event_index)
								{
									DrumWriter.Write(string.Format("{0},", current_time));
									DrumWriter.Write(string.Format("{0},", delta_time));
									
									for (int i = 0; i < 128; i++)
									{
										if (drum_note_event[last_note_event_index].note[i] == true)
										{
											DrumWriter.Write(string.Format("#,"));
										}
										else
										{
											DrumWriter.Write(string.Format(","));
										}
									}
									
									DrumWriter.Write(string.Format("\r\n"));
								}
								
								last_note_event_index = note_event_index;
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
                    }
                }
                else if (foo == 0xFF)
                {
                    int type = (int)MidiReader.ReadByte();

                    if (type == 0x00)
                    {
                        MidiReader.ReadByte();
                        int sequence = (int)MidiReader.ReadByte() * 256 + (int)MidiReader.ReadByte();
                    }
                    else if (type == 0x20)
                    {
                        MidiReader.ReadByte();
                        int chan = (int)MidiReader.ReadByte();
                    }
                    else if (type == 0x2F)
                    {
                        MidiReader.ReadByte();
                        break;
                    }
                    else if (type == 0x51)
                    {
                        MidiReader.ReadByte();
                        int tempo_raw = (int)MidiReader.ReadByte() * 256 * 256 + (int)MidiReader.ReadByte() * 256 + (int)MidiReader.ReadByte();
                    }
                    else if (type == 0x54)
                    {
                        MidiReader.ReadByte();
                        int hour = (int)MidiReader.ReadByte();
                        int min = (int)MidiReader.ReadByte();
                        int sec = (int)MidiReader.ReadByte();
                        int frame = (int)MidiReader.ReadByte();
                        int subframe = (int)MidiReader.ReadByte();
                    }
                    else if (type == 0x58)
                    {
                        MidiReader.ReadByte();
                        int number = (int)MidiReader.ReadByte();
                        int denominator = (int)MidiReader.ReadByte();
                        int metronome = (int)MidiReader.ReadByte();
                        int _32nds = (int)MidiReader.ReadByte();
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
                    }
                }
                else if (foo >= 0xF0)
                {
                    int len = ReadVariableLength(MidiReader);
                    for (int i = 0; i < len; i++)
                    {
                        MidiReader.ReadByte();
                    }
                }
            }
			
			DrumWriter.Write(string.Format("{0},", current_time));
			DrumWriter.Write(string.Format("{0},", delta_time));
			
			for (int i = 0; i < 128; i++)
			{
				if (drum_note_event[note_event_index].note[i] == true)
				{
					DrumWriter.Write(string.Format("#,"));
				}
				else
				{
					DrumWriter.Write(string.Format(","));
				}
			}
			
			DrumWriter.Write(string.Format("\r\n"));
			
			#endregion

			int dev_event_index = 0;
			
			DevEvent[] drum_dev_event = new DevEvent[3000];
			
			int bit_mask = 0xFF;
			int old_bit_mask = 0;
			double last_time_error = 0;
			
            for (int i = 0; i < note_event_index; i++)
			{
				bit_mask = 0xFF;
				
				if (drum_note_event[i].note[])
				{
					bit_mask -= Convert.ToInt32(Math.Pow(2, ));
				}
				
				if (drum_note_event[i].note[])
				{
					bit_mask -= Convert.ToInt32(Math.Pow(2, ));
				}
				
				if (drum_note_event[i].note[])
				{
					bit_mask -= Convert.ToInt32(Math.Pow(2, ));
				}
				
				if (drum_note_event[i].note[])
				{
					bit_mask -= Convert.ToInt32(Math.Pow(2, ));
				}
				
				if (drum_note_event[i].note[])
				{
					bit_mask -= Convert.ToInt32(Math.Pow(2, ));
				}				
				
				if (bit_mask != old_bit_mask)
				{
					int ticks;
					
					double dt = drum_note_event[i].delta_time;
					
					if (drum_note_event[i].delta_time >= 5)
					{
						for (int j = 0; j < Convert.ToInt32(Math.Floor(drum_note_event[i].delta_time / 5)); j++)
						{
							dt -= 5;
							ticks = (5 * (8000000 / 1024));
							drum_dev_event = new DevEvent(1, ticks - tick_offset, bit_mask);
							dev_event_index++;
						}
					}
					
					ticks = Convert.ToInt32(Math.Floor(dt * (8000000 / 1024)));
					double ticks_d = (dt * (8000000 / 1024));
					last_time_error += ticks_d - ticks;
					
					if (Convert.ToInt32(Math.Floor(last_time_error)) >= 1)
					{
						ticks += Convert.ToInt32(Math.Floor(last_time_error));
						last_time_error -= Math.Floor(last_time_error);
					}
					
					ticks -= ticks_offset;
					
					if (dev_event_index == 0)
					{
						ticks = 0;
					}
					
					drum_dev_event = new DevEvent(1, ticks, bit_mask);
					
					dev_event_index++;
				}
			}
			
			MidiReader.Close();
            LogWriter.Close();
            GuitarWriter.Close();
            BassWriter.Close();
            DrumWriter.Close();
            VocalWriter.Close();
            MidiFile.Close();
        }
    }
}
