using System;
using System.IO;

namespace NukedOpl.Test;

public static class Riff
{
    public static void WriteWav16(Stream output, Span<short> data, int sampleRate, int channels)
    {
        if (data.Length % channels != 0)
            throw new Exception($"Not enough data for {channels} channels.");

        var writer = new BinaryWriter(output);
        writer.Write(0x46464952); // RIFF
        writer.Write(data.Length * 2 + 32);
        writer.Write(0x45564157); // WAVE

        writer.Write(0x20746D66); // fmt
        writer.Write(0x10);
        writer.Write((short) 0x01);
        writer.Write((short) channels);
        writer.Write(sampleRate);
        writer.Write(sampleRate * channels * 2);
        writer.Write((short) (channels * 2));
        writer.Write((short) 16);
            
        writer.Write(0x61746164); // data
        writer.Write(data.Length * 2);
            
        foreach (var d in data)
            writer.Write(d);
    }
}