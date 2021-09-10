namespace NukedOpl
{
    public class Opl3Writebuf
    {
        public ulong time { get; set; }
        public ushort reg { get; set; }
        public byte data { get; set; }

        public void Reset()
        {
            time = default;
            reg = default;
            data = default;
        }
    }
}