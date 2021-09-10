using System;
using System.IO;
using NUnit.Framework;

namespace NukedOpl.Test
{
    [TestFixture]
    public class Opl3Test
    {
        private void RenderImf(string resourceName, int ticksPerSecond)
        {
            var opl = new Opl3(TestContext.Out);
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

            using var output = File.OpenWrite(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "out.wav"));
            Riff.WriteWav16(output, buffer.AsSpan(0, size), Opl3.OPL_RATE, 2);
            output.Flush();
        }
        
        [Test]
        [Explicit]
        public void ImfTest()
        {
            RenderImf("fanfare.imf", 280);
        }
    }
}