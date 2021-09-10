namespace NukedOpl
{
    public class Opl3Channel
    {
        public Opl3Channel(Opl3Chip chip) => this.chip = chip;

        public Opl3Slot[] slots { get; } = {null, null};
        public Opl3Channel pair { get; set; }
        public Opl3Chip chip { get; }

        public short[][] out_ { get; } = {new short[1], new short[1], new short[1], new short[1]};

        public int leftpan { get; set; }
        public int rightpan { get; set; }
        public byte chtype { get; set; }
        public ushort f_num { get; set; }
        public byte block { get; set; }
        public byte fb { get; set; }
        public byte con { get; set; }
        public byte alg { get; set; }
        public byte ksv { get; set; }
        public ushort cha { get; set; }
        public ushort chb { get; set; }
        public byte ch_num { get; set; }

        public void Reset()
        {
            slots[0] = null;
            slots[1] = null;
            pair = null;
            out_[0] = new short[1];
            out_[1] = new short[1];
            out_[2] = new short[1];
            out_[3] = new short[1];
            leftpan = default;
            rightpan = default;
            chtype = default;
        }
    }
}