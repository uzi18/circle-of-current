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
		public bool[] note;
		public NoteEvent(double time_, double delta_time_)
		{
            note = new bool[8];
			time = time_;
			delta_time = delta_time_;
			for (int i = 0; i < 8; i++)
			{
				note[i] = false;
			}
		}
	}
	
	public struct DevEvent
	{
		public int sp_code, delay, bitmask;
		public DevEvent(int id, int delay_, int bitmask_)
		{
			sp_code = id;
			delay = delay_;
			bitmask = bitmask_;
		}
	}

    partial class Form1
    {
        int green_bit = 3;
        int blue_bit = 2;
        int yellow_bit = 1;
        int red_bit = 0;
        int bass_bit = 4;

        private int NoteToBit(int n)
        {
            switch (n)
            {
                case 100:
                    return green_bit;
                case 99:
                    return blue_bit;
                case 98:
                    return yellow_bit;
                case 97:
                    return red_bit;
                case 96:
                    return bass_bit;
                default:
                    return -1;
            }
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

        private double MidiToDrumChart(string fpath, double clk_freq)
        {
            StreamReader MidiStreamReader = new StreamReader(fpath);
            BinaryReader MidiReader = new BinaryReader(MidiStreamReader.BaseStream);
            StreamWriter MidiLogWriter = new StreamWriter(fpath + ".midilog.csv");
            StreamWriter DrumMidiLogWriter = new StreamWriter(fpath + ".drumtrack.csv");
            StreamWriter DrumStream = new StreamWriter(fpath + ".drumtrack.bin");
            BinaryWriter DrumWriter = new BinaryWriter(DrumStream.BaseStream);

            int ticks_per_frame;
            int frame_rate = 0;
            int ticks_per_beat = 0;
            double tempo = 120;
            int delta_time;

            #region

            MidiLogWriter.Write(string.Format("file: {0}\r\n", fpath));
            MidiLogWriter.Write("reading header chunk\r\n");
            MidiLogWriter.Write(string.Format("midi file header: {0}{1}{2}{3}\r\n", MidiReader.ReadChar(), MidiReader.ReadChar(), MidiReader.ReadChar(), MidiReader.ReadChar()));

            int chunk_size = (int)MidiReader.ReadByte() * 256 * 256 * 256 + (int)MidiReader.ReadByte() * 256 * 256 + (int)MidiReader.ReadByte() * 256 + (int)MidiReader.ReadByte();

            MidiLogWriter.Write(string.Format("chunk size: {0}\r\n", chunk_size));

            int format_type = (int)MidiReader.ReadByte() * 256 + (int)MidiReader.ReadByte();
            int num_tracks = (int)MidiReader.ReadByte() * 256 + (int)MidiReader.ReadByte();
            int time_division = (int)MidiReader.ReadByte() * 256 + (int)MidiReader.ReadByte();

            MidiLogWriter.Write(string.Format("format type: {0}\r\n", format_type));
            MidiLogWriter.Write(string.Format("# of tracks: {0}\r\n", num_tracks));
            MidiLogWriter.Write(string.Format("time division: {0}\r\n", time_division));

            if (time_division < 0x8000)
            {
                ticks_per_beat = time_division;
                MidiLogWriter.Write(string.Format("ticks per beat: {0}\r\n", ticks_per_beat));
            }
            else
            {
                time_division -= 0x8000;
                ticks_per_frame = time_division % 256;
                frame_rate = (time_division - ticks_per_frame) / 256;
                MidiLogWriter.Write(string.Format("frames per second: {0}\r\n", frame_rate));
                MidiLogWriter.Write(string.Format("ticks per frame: {0}\r\n", ticks_per_frame));
            }

            #endregion

            long[] track_start_addr = new long[16];

            #region

            for (int track_index = 0; track_index < num_tracks; track_index++)
            {
                try
                {
                    MidiLogWriter.Write(string.Format("track #: {0}\r\n", track_index + 1));
                    MidiLogWriter.Write(string.Format("track header: {0}{1}{2}{3}\r\n", MidiReader.ReadChar(), MidiReader.ReadChar(), MidiReader.ReadChar(), MidiReader.ReadChar()));

                    chunk_size = (int)MidiReader.ReadByte() * 256 * 256 * 256 + (int)MidiReader.ReadByte() * 256 * 256 + (int)MidiReader.ReadByte() * 256 + (int)MidiReader.ReadByte();

                    MidiLogWriter.Write(string.Format("track chunk size: {0}\r\n", chunk_size));

                    int event_index = 0;

                    MidiLogWriter.Write(string.Format("track #, event #, delta time, event type, parameters\r\n"));

                    track_start_addr[track_index] = MidiReader.BaseStream.Position;

                    for (long byte_index = MidiReader.BaseStream.Position; MidiReader.BaseStream.Position < byte_index + chunk_size; )
                    {
                        try
                        {
                            delta_time = ReadVariableLength(MidiReader);
                            MidiLogWriter.Write(string.Format("{0}, ", track_index + 1));
                            MidiLogWriter.Write(string.Format("{0}, ", event_index + 1));
                            MidiLogWriter.Write(string.Format("{0}, ", delta_time));

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

                                MidiLogWriter.Write(string.Format("\"{0}\", ", event_type_s));
                                MidiLogWriter.Write(string.Format("ch: {0}, ", channel));

                                int para1 = (int)MidiReader.ReadByte();
                                int para2;

                                if (event_type != 0x0C && event_type != 0x0D && event_type >= 0x08)
                                {
                                    para2 = (int)MidiReader.ReadByte();
                                    MidiLogWriter.Write(string.Format("p1: {0}, p2: {1}\r\n", para1, para2));
                                    while (true)
                                    {
                                        long old_position = MidiReader.BaseStream.Position;
                                        delta_time = ReadVariableLength(MidiReader);
                                        int new_para1 = (int)MidiReader.ReadByte();
                                        int new_para2;

                                        if (new_para1 < 0x80)
                                        {
                                            new_para2 = (int)MidiReader.ReadByte();
                                            MidiLogWriter.Write(string.Format("{0}, {1}, {2}, \"{3}\", ch: {4}, p1: {5}, p2: {6}\r\n", track_index + 1, event_index + 1, delta_time, event_type_s, channel, new_para1, new_para2));
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
                                    MidiLogWriter.Write(string.Format("p1: {0}\r\n", para1));
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

                                MidiLogWriter.Write(string.Format("meta event: \"{0}\", ", meta_event));

                                if (type == 0x00)
                                {
                                    MidiReader.ReadByte();
                                    int sequence = (int)MidiReader.ReadByte() * 256 + (int)MidiReader.ReadByte();
                                    MidiLogWriter.Write(string.Format("seq: {0}", sequence));
                                }
                                else if (type == 0x20)
                                {
                                    MidiReader.ReadByte();
                                    int chan = (int)MidiReader.ReadByte();
                                    MidiLogWriter.Write(string.Format("ch: {0}", chan));
                                }
                                else if (type == 0x2F)
                                {
                                    MidiReader.ReadByte();
                                }
                                else if (type == 0x51)
                                {
                                    MidiReader.ReadByte();
                                    tempo = Convert.ToDouble((int)MidiReader.ReadByte() * 256 * 256 + (int)MidiReader.ReadByte() * 256 + (int)MidiReader.ReadByte());
                                    MidiLogWriter.Write(string.Format("tempo: {0}", tempo));
                                }
                                else if (type == 0x54)
                                {
                                    MidiReader.ReadByte();
                                    int hour = (int)MidiReader.ReadByte();
                                    int min = (int)MidiReader.ReadByte();
                                    int sec = (int)MidiReader.ReadByte();
                                    int frame = (int)MidiReader.ReadByte();
                                    int subframe = (int)MidiReader.ReadByte();
                                    MidiLogWriter.Write(string.Format("h: {0}, m: {1}, s: {2}, fr: {3}, subfr: {4}", hour, min, sec, frame, subframe));
                                }
                                else if (type == 0x58)
                                {
                                    MidiReader.ReadByte();
                                    int number = (int)MidiReader.ReadByte();
                                    int denominator = (int)MidiReader.ReadByte();
                                    int metronome = (int)MidiReader.ReadByte();
                                    int _32nds = (int)MidiReader.ReadByte();
                                    MidiLogWriter.Write(string.Format("#: {0}, denom: {1}, metro: {2}, 32nds: {3}", number, denominator, metronome, _32nds));
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
                                    MidiLogWriter.Write(string.Format("text: \"{0}\"", target_string));
                                }
                                MidiLogWriter.Write(string.Format("\r\n"));
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
                    MidiLogWriter.Write("Corrupted File\r\n");
                    break;
                }
            }

            #endregion

            MidiReader.BaseStream.Position = track_start_addr[0];

            TempoPeriod[] tempo_period = new TempoPeriod[3000];
			
			#region

            int tempo_index = 0;
            double current_time = 0;
            tempo = 120;

            while (true)
            {
                delta_time = ReadVariableLength(MidiReader);

                current_time = current_time + ((Convert.ToDouble(delta_time) / Convert.ToDouble(ticks_per_beat)) / (tempo / 60));

                int foo = (int)MidiReader.ReadByte();

                #region

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

                #endregion

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
                        MidiLogWriter.Write(string.Format("New Tempo # {0} is {1} BPM at {2}\r\n", tempo_index + 1, tempo, current_time));
                        tempo_index++;
                    }

                    #region

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

            #endregion

            MidiReader.BaseStream.Position = track_start_addr[1];
			
			NoteEvent[] drum_note_event = new NoteEvent[3000];
			
			#region

            current_time = 0;
			double last_time = 0;
            tempo = 120;
			int note_event_index = 0;
            bool first_note_flag = true;
            bool[] note = new bool[8];
            for (int i = 0; i < 8; i++)
            {
                note[i] = false;
            }

            DrumMidiLogWriter.Write(string.Format("_,_,"));
            for (int i = 0; i < 8; i++)
            {
                DrumMidiLogWriter.Write(string.Format("{0},", i % 10));
            }
            DrumMidiLogWriter.Write(string.Format("\r\n"));

            drum_note_event[note_event_index] = new NoteEvent(0, 0);

            while (true)
            {
                delta_time = ReadVariableLength(MidiReader);

                if (delta_time != 0)
                {
                    for (int j = 0; j < delta_time; j++)
                    {
                        for (int i = 0; i < tempo_index; i++)
                        {
                            if (tempo_period[i].begin <= current_time)
                            {
                                tempo = tempo_period[i].tempo;
                            }
                            else
                            {
                                break;
                            }
                        }

                        current_time += (Convert.ToDouble(60) / (Convert.ToDouble(ticks_per_beat) * tempo));
                    }
                }

                int foo = (int)MidiReader.ReadByte();

                if (foo < 0xF0)
                {
                    int channel = foo % 16;
                    int event_type = (foo - channel) / 16;

                    int para1 = (int)MidiReader.ReadByte();
                    int para2;
					
                    if (event_type == 0x09)
					{
                        int bit = NoteToBit(para1);
                        if (bit >= 0)
                        {
                            if (first_note_flag)
                            {
                                first_note_flag = false;
                                current_time = 0;
                                note[bit] = true;
                            }

                            if (last_time != current_time)
                            {
                                DrumMidiLogWriter.Write(string.Format("{0,8:F2},,", current_time));

                                for (int i = 0; i < 8; i++)
                                {
                                    drum_note_event[note_event_index].note[i] = note[i];
                                    if (drum_note_event[note_event_index].note[i] == true)
                                    {
                                        DrumMidiLogWriter.Write(string.Format("#,"));
                                    }
                                    else
                                    {
                                        DrumMidiLogWriter.Write(string.Format("_,"));
                                    }
                                }

                                DrumMidiLogWriter.Write(string.Format("\r\n"));

                                for (int i = 0; i < 8; i++)
                                {
                                    note[i] = false;
                                }

                                note_event_index++;
                                drum_note_event[note_event_index] = new NoteEvent(current_time, current_time - last_time);
                                last_time = current_time;
                            }

                            note[bit] = true;
                        }
					}

                    if (event_type != 0x0C && event_type != 0x0D && event_type >= 0x08)
                    {
                        para2 = (int)MidiReader.ReadByte();
                        while (true)
                        {						
                            long old_position = MidiReader.BaseStream.Position;
                            delta_time = ReadVariableLength(MidiReader);

                            int new_para1 = (int)MidiReader.ReadByte();
                            int new_para2;

                            if (new_para1 < 0x80)
                            {
                                if (delta_time != 0)
                                {
                                    for (int j = 0; j < delta_time; j++)
                                    {
                                        for (int i = 0; i < tempo_index; i++)
                                        {
                                            if (tempo_period[i].begin <= current_time)
                                            {
                                                tempo = tempo_period[i].tempo;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }

                                        current_time += (Convert.ToDouble(60) / (Convert.ToDouble(ticks_per_beat) * tempo));
                                    }
                                }

                                new_para2 = (int)MidiReader.ReadByte();

                                if (event_type == 0x09)
                                {
                                    int bit = NoteToBit(new_para1);
                                    if (bit >= 0)
                                    {
                                        if (first_note_flag)
                                        {
                                            first_note_flag = false;
                                            current_time = 0;
                                            note[bit] = true;
                                        }

                                        if (last_time != current_time)
                                        {
                                            DrumMidiLogWriter.Write(string.Format("{0,8:F2},,", current_time));

                                            for (int i = 0; i < 8; i++)
                                            {
                                                drum_note_event[note_event_index].note[i] = note[i];
                                                if (drum_note_event[note_event_index].note[i] == true)
                                                {
                                                    DrumMidiLogWriter.Write(string.Format("#,"));
                                                }
                                                else
                                                {
                                                    DrumMidiLogWriter.Write(string.Format("_,"));
                                                }
                                            }

                                            DrumMidiLogWriter.Write(string.Format("\r\n"));

                                            for (int i = 0; i < 8; i++)
                                            {
                                                note[i] = false;
                                            }

                                            note_event_index++;
                                            drum_note_event[note_event_index] = new NoteEvent(current_time, current_time - last_time);
                                            last_time = current_time;
                                        }

                                        note[bit] = true;
                                    }
                                }
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

                #region

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

                #endregion

            }

            DrumMidiLogWriter.Write(string.Format("{0,8:F2},,", current_time));

            for (int i = 0; i < 8; i++)
            {
                drum_note_event[note_event_index].note[i] = note[i];
                if (drum_note_event[note_event_index].note[i] == true)
                {
                    DrumMidiLogWriter.Write(string.Format("#,"));
                }
                else
                {
                    DrumMidiLogWriter.Write(string.Format("_,"));
                }
            }

            note_event_index++;

            DrumMidiLogWriter.Write(string.Format("\r\n"));
			
			#endregion
			
			DevEvent[] drum_dev_event = new DevEvent[3000];

            #region

            int bit_mask = 0xFF;
			double last_time_error = 0;
            int dev_event_index = 0;
            bool first_over_5_flag = true;
            bool dont_mask_flag = false;
			
            for (int i = 0; i < note_event_index; i++)
			{
				bit_mask = 0xFF;

                for (int j = 0; j < 8; j++)
                {
                    if (drum_note_event[i].note[j])
                    {
                        bit_mask -= Convert.ToInt32(Math.Pow(2, j));
                    }
                }

				int ticks;
				
				double dt = drum_note_event[i + 1].delta_time;

                double ticks_d;

                while (dt >= (double)5)
				{
                    dt -= (double)5;
                    ticks = Convert.ToInt32(Math.Floor((double)5 * ((double)clk_freq / (double)1024)));
                    ticks_d = ((double)5 * ((double)clk_freq / (double)1024));
                    last_time_error += ticks_d - Convert.ToDouble(ticks);
                    if (first_over_5_flag)
                    {
                        drum_dev_event[dev_event_index] = new DevEvent(1, ticks, bit_mask);
                        first_over_5_flag = false;
                        dont_mask_flag = true;
                    }
                    else
                    {
                        drum_dev_event[dev_event_index] = new DevEvent(1, ticks, 0xFF);
                    }
					dev_event_index++;
				}
				
				ticks = Convert.ToInt32(Math.Floor(dt * (clk_freq / 1024d)));
				ticks_d = (dt * (clk_freq / 1024d));
				last_time_error += ticks_d - Convert.ToDouble(ticks);

                while (last_time_error >= (double)1)
				{
					ticks += 1;
                    last_time_error -= (double)1;
				}

                while (last_time_error <= (double)-1)
                {
                    ticks -= 1;
                    last_time_error += (double)1;
                }

                if (dont_mask_flag)
                {
                    drum_dev_event[dev_event_index] = new DevEvent(1, ticks, 0xFF);
                    dont_mask_flag = false;
                }
                else
                {
                    drum_dev_event[dev_event_index] = new DevEvent(1, ticks, bit_mask);
                }

                first_over_5_flag = false;
				
				dev_event_index++;
			}

            drum_dev_event[dev_event_index - 1] = new DevEvent(3, 1000, bit_mask);

            #endregion

            #region

            bool first_instruct = true;

            for (int i = 0; i < dev_event_index; i++)
            {
                if (first_instruct == false)
                {
                    DrumMidiLogWriter.Write(string.Format("{0,7},", drum_dev_event[i].delay));
                    DrumMidiLogWriter.Write(string.Format("{0:8},{1}", Convert.ToString(drum_dev_event[i].bitmask, 2), drum_dev_event[i].sp_code));
                    DrumMidiLogWriter.Write(string.Format("\r\n"));

                    DrumWriter.Write(Convert.ToByte(drum_dev_event[i].sp_code));
                    DrumWriter.Write(Convert.ToUInt16(drum_dev_event[i].delay));
                    DrumWriter.Write(Convert.ToByte(drum_dev_event[i].bitmask));
                }
                else
                {
                    first_instruct = false;

                    DrumMidiLogWriter.Write(string.Format("{0,7},", drum_dev_event[i].delay));
                    DrumMidiLogWriter.Write(string.Format("{0:8},{1}", Convert.ToString(drum_dev_event[i].bitmask, 2), 2));
                    DrumMidiLogWriter.Write(string.Format("\r\n"));

                    DrumWriter.Write(Convert.ToByte(2));
                    DrumWriter.Write(Convert.ToUInt16(drum_dev_event[i].delay));
                    DrumWriter.Write(Convert.ToByte(drum_dev_event[i].bitmask));
                }
            }

            #endregion

            MidiReader.Close();
            MidiLogWriter.Close();
            DrumMidiLogWriter.Close();
            DrumStream.Close();
            DrumWriter.Close();
            MidiStreamReader.Close();

            return current_time;
        }
    }
}
