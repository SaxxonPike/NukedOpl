/* Nuked OPL3
 * Copyright (C) 2013-2020 Nuke.YKT
 *
 * This file is part of Nuked OPL3.
 *
 * Nuked OPL3 is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation, either version 2.1
 * of the License, or (at your option) any later version.
 *
 * Nuked OPL3 is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with Nuked OPL3. If not, see <https://www.gnu.org/licenses/>.

 *  Nuked OPL3 emulator.
 *  Thanks:
 *      MAME Development Team(Jarek Burczynski, Tatsuyuki Satoh):
 *          Feedback and Rhythm part calculation information.
 *      forums.submarine.org.uk(carbon14, opl3):
 *          Tremolo and phase generator calculation information.
 *      OPLx decapsulated(Matthew Gambrell, Olli Niemitalo):
 *          OPL2 ROMs.
 *      siliconpr0n.org(John McMaster, digshadow):
 *          YMF262 and VRC VII decaps and die shots.
 *
 * version: 1.8
 */

/*
 * .NET conversion written by Anthony Konzel.
 * 
 * Github: saxxonpike
 * Email: saxxonpike@gmail.com
 */

namespace NukedOpl
{
    public class Opl3Chip
    {
        public const int OPL_WRITEBUF_SIZE = 1024;

        public Opl3Chip()
        {
            for (var i = 0; i < channel.Length; i++)
                channel[i] = new Opl3Channel(this);
            for (var i = 0; i < slot.Length; i++)
                slot[i] = new Opl3Slot(this);
            for (var i = 0; i < writebuf.Length; i++)
                writebuf[i] = new Opl3Writebuf();
        }

        public Opl3Channel[] channel { get; } = new Opl3Channel[18];
        public Opl3Slot[] slot { get; } = new Opl3Slot[36];
        public ushort timer { get; set; }
        public ulong eg_timer { get; set; }
        public byte eg_timerrem { get; set; }
        public byte eg_state { get; set; }
        public byte eg_add { get; set; }
        public byte newm { get; set; }
        public byte nts { get; set; }
        public byte rhy { get; set; }
        public byte vibpos { get; set; }
        public byte vibshift { get; set; }
        public short[] tremolo { get; set; }
        public byte tremolopos { get; set; }
        public byte tremoloshift { get; set; }
        public uint noise { get; set; }
        public short[] zeromod { get; set; }
        public int[] mixbuff { get; } = new int[2];
        public bool rm_hh_bit2 { get; set; }
        public bool rm_hh_bit3 { get; set; }
        public bool rm_hh_bit7 { get; set; }
        public bool rm_hh_bit8 { get; set; }
        public bool rm_tc_bit3 { get; set; }
        public bool rm_tc_bit5 { get; set; }
        public byte stereoext { get; set; }

        /* OPL3L */
        public int rateratio { get; set; }
        public int samplecnt { get; set; }
        public short[] oldsamples { get; } = new short[2];
        public short[] samples { get; } = new short[2];

        public ulong writebuf_samplecnt { get; set; }
        public uint writebuf_cur { get; set; }
        public uint writebuf_last { get; set; }
        public ulong writebuf_lasttime { get; set; }
        public Opl3Writebuf[] writebuf { get; } = new Opl3Writebuf[OPL_WRITEBUF_SIZE];

        public void Reset()
        {
            foreach (var x in channel)
                x.Reset();
            foreach (var x in slot)
                x.Reset();
            timer = default;
            eg_timer = default;
            eg_timerrem = default;
            eg_state = default;
            eg_add = default;
            newm = default;
            nts = default;
            rhy = default;
            vibpos = default;
            vibshift = default;
            tremolo = new short[1];
            tremolopos = default;
            tremoloshift = default;
            noise = default;
            zeromod = new short[1];
            for (var i = 0; i < mixbuff.Length; i++)
                mixbuff[i] = default;
            rm_hh_bit2 = default;
            rm_hh_bit3 = default;
            rm_hh_bit7 = default;
            rm_hh_bit8 = default;
            rm_tc_bit3 = default;
            rm_tc_bit5 = default;
            stereoext = default;
            rateratio = default;
            samplecnt = default;
            for (var i = 0; i < oldsamples.Length; i++)
                oldsamples[i] = default;
            for (var i = 0; i < samples.Length; i++)
                samples[i] = default;
            writebuf_samplecnt = default;
            writebuf_cur = default;
            writebuf_last = default;
            writebuf_lasttime = default;
            foreach (var x in writebuf)
                x.Reset();
        }
    }
}