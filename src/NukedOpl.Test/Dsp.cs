using System;

namespace NukedOpl.Test
{
    public static class Dsp
    {
        public static void Normalize(Span<short> samples)
        {
            var max = 0;
            foreach (var sample in samples)
            {
                if (sample > max)
                    max = sample;
                if (sample < -max)
                    max = -sample;
            }

            // nothing to do if the input is silent
            if (max == 0)
                return;

            // nothing to do if already normalized
            if (max >= 32767)
                return;

            var factor = 32767f / (double) max;
            for (var i = 0; i < samples.Length; i++)
            {
                var s = samples[i] * factor;
                if (s > 32767)
                    s = 32767;
                if (s < -32768)
                    s = -32768;
                samples[i] = (short) s;
            }
        }
    }
}