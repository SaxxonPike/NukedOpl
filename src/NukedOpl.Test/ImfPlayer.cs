using System;
using System.IO;

namespace NukedOpl.Test
{
    public sealed class ImfPlayer
    {
        public static int GetTicks(Stream stream)
        {
            var input = new byte[4];
            var result = 0;
            while (true)
            {
                if (stream.Read(input, 0, 4) < 4)
                    return result;

                result += input[2] | (input[3] << 8);
            }
        }
        
        public static int Generate(Stream stream, ImfState state, Span<short> buffer)
        {
            var idx = 0;
            var input = new byte[4];
            var count = buffer.Length;

            while (count > 0)
            {
                if (state.Timer < state.TicksPerSecond)
                {
                    state.Timer += Opl3.OPL_RATE;
                    if (state.Delay > 0)
                    {
                        state.Delay--;
                        state.Tick++;
                    }

                    if (state.Delay < 1 && !state.Stopped)
                    {
                        while (state.Delay < 1 && !state.Stopped)
                        {
                            if (stream.Read(input, 0, 4) < 4)
                            {
                                state.Stopped = true;
                                break;
                            }

                            state.Opl3.WriteReg(state.Chip, input[0], input[1]);
                            state.Delay = input[2] | (input[3] << 8);
                        }
                    }
                }

                state.Timer -= state.TicksPerSecond;
                var rendered = state.Opl3.Generate(state.Chip, buffer[idx..]);
                idx += rendered;
                count -= rendered;
            }

            return idx;
        }
    }
}