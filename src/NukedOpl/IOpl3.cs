using System;

namespace NukedOpl
{
    public interface IOpl3
    {
        int Generate(Opl3Chip chip, Span<short> buf);
        int GenerateResampled(Opl3Chip chip, Span<short> buf);
        void Reset(Opl3Chip chip, int samplerate);
        void WriteReg(Opl3Chip chip, int reg, byte v);
        void WriteRegBuffered(Opl3Chip chip, int reg, byte v);
        int GenerateStream(Opl3Chip chip, Span<short> sndptr, int numsamples);
    }
}