using System;
using System.IO;

namespace NukedOpl.Test
{
    public sealed class ImfPlayer
    {
        private static int GetLength(Stream stream)
        {
            var input = new byte[2];
            if (stream.Read(input, 0, 2) < 2)
                return (int) (stream.Length - 2);

            var length = input[0] | (input[1] << 8);
            if (length != 0)
                return length;

            // this will skip the first command, in most cases it's all zeroes anyway
            stream.Read(input, 0, 2);
            return (int) (stream.Length - 4);
        }

        public static int GetTicks(Stream stream)
        {
            var input = new byte[4];
            var length = GetLength(stream);
            var offset = 0;
            var result = 0;

            // read the rest..
            while (offset < length)
            {
                if (stream.Read(input, 0, 4) < 4)
                    return result;

                offset += 4;
                result += input[2] | (input[3] << 8);
            }

            return result;
        }

        public static int Generate(Stream stream, ImfState state, Span<short> buffer)
        {
            var offset = 0;
            var length = GetLength(stream);
            var idx = 0;
            var input = new byte[4];
            var count = buffer.Length;

            while (count > 0 && offset < length)
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
                        while (state.Delay < 1 && !state.Stopped && offset < length)
                        {
                            offset += 4;
                            if (stream.Read(input, 0, 4) < 4)
                            {
                                state.Stopped = true;
                                break;
                            }
                            
                            state.Opl3.WriteRegBuffered(state.Chip, input[0], input[1]);
                            state.Delay = input[2] | (input[3] << 8);
                        }
                    }
                }

                state.Timer -= state.TicksPerSecond;
                var rendered = state.Opl3.Generate(state.Chip, buffer.Slice(idx));
                idx += rendered;
                count -= rendered;
            }

            return idx;
        }
    }
}