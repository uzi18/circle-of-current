using System;

namespace AVRProjectIDE
{
    public enum TextBoxChangeMode
    {
        Set,
        SetNewLine,
        Prepend,
        PrependNewLine,
        Append,
        AppendNewLine
    }

    public enum ListViewChangeMode
    {
        Add,
        Clear
    }

    public enum FileChangeEventType
    {
        Renamed,
        Deleted,
        Changed
    }

    public struct AVRGCCErrorWarning
    {
        public string FileName;
        public string FilePath;
        public string Type;
        public int LineNumber;
        public string Location;
        public string Message;

        public AVRGCCErrorWarning(string fn, string fp, string t, string l, string m, string loc)
        {
            FileName = fn.Trim();
            FilePath = fp.Trim();
            Type = t.Trim();
            if (int.TryParse(l, out LineNumber) == false)
                LineNumber = 0;
            Message = m.Trim();
            Location = loc.Trim().TrimEnd(':').Trim();
        }

        public AVRGCCErrorWarning(string fn, string fp, string t, int l, string m, string loc)
        {
            FileName = fn.Trim();
            FilePath = fp.Trim();
            Type = t.Trim();
            LineNumber = l;
            Message = m.Trim();
            Location = loc.Trim().TrimEnd(':').Trim();
        }

        public AVRGCCErrorWarning(string fn, string t, string l, string m, string loc)
        {
            FileName = fn.Trim();
            FilePath = "";
            Type = t.Trim();
            if (int.TryParse(l, out LineNumber) == false)
                LineNumber = 0;
            Message = m.Trim();
            Location = loc.Trim().TrimEnd(':').Trim();
        }

        public AVRGCCErrorWarning(string fn, string t, int l, string m, string loc)
        {
            FileName = fn.Trim();
            FilePath = "";
            Type = t.Trim();
            LineNumber = l;
            Message = m.Trim();
            Location = loc.Trim().TrimEnd(':').Trim();
        }

        public override string ToString()
        {
            return String.Format("File Name: {0}, {4} Line {1}: {2}: {3}", FileName, LineNumber, Type, Message, Location);
        }
    }

    public struct MemorySegment
    {
        private string type;
        public string Type
        {
            get { return type.ToLower().Replace(' ', '_'); }
            set { type = value.ToLower().Replace(' ', '_'); }
        }

        private string name;
        public string Name
        {
            get { return name.ToLower().Replace(' ', '_'); }
            set { name = value.ToLower().Replace(' ', '_'); }
        }

        private uint addr;
        public uint Addr
        {
            get { return addr; }
            set { addr = value; }
        }

        public MemorySegment(string t, string n, uint addr)
        {
            this.type = t.ToLower().Replace(' ', '_'); this.name = n.ToLower().Replace(' ', '_'); this.addr = addr;
        }
    }
}