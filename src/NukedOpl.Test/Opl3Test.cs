using System;
using System.IO;
using NUnit.Framework;

namespace NukedOpl.Test;

[TestFixture]
public class Opl3Test
{
    private void RenderImf(string resourceName, int ticksPerSecond)
    {
        var opl = new Opl3();
        var chip = new Opl3Chip();
        opl.Reset(chip, 44100);

        using var source = new MemoryStream();
        using var resource = Resources.Open(resourceName);
        resource.CopyTo(source);
        source.Position = 0;

        var ticks = ImfPlayer.GetTicks(source);
        source.Position = 0;
            
        var buffer = new short[(Opl3.OPL_RATE * ticks / ticksPerSecond) * 2];
            
        var state = new ImfState
        {
            Opl3 = opl,
            Chip = chip,
            TicksPerSecond = ticksPerSecond
        };

        var size = ImfPlayer.Generate(source, state, buffer);
        Dsp.Normalize(buffer.AsSpan(0, size));

        using var output = File.Open(
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{resourceName}-{DateTime.Now.Ticks}.wav"),
            FileMode.Create, FileAccess.Write);
        Riff.WriteWav16(output, buffer.AsSpan(0, size), Opl3.OPL_RATE, 2);
        output.Flush();
    }
        
    [Test]
    [TestCase("fanfare.imf", 280)]
    [TestCase("darkhall.wlf", 700)]
    [TestCase("drshock.imf", 560)]
    [Explicit]
    public void ImfTest(string name, int rate)
    {
        RenderImf(name, rate);
    }
}