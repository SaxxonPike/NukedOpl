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

// ReSharper disable InconsistentNaming

using System;
using System.Runtime.CompilerServices;

namespace NukedOpl;

public sealed class Opl3 : IOpl3
{
    public const int OPL_RATE = 49716;

    private const int OPL_WRITEBUF_DELAY = 2;
    private const int RSM_FRAC = 10;

    /* Channel types */

    private const int ch_2op = 0;
    private const int ch_4op = 1;
    private const int ch_4op2 = 2;
    private const int ch_drum = 3;

    /* Envelope key types */

    private const int egk_norm = 0x01;
    private const int egk_drum = 0x02;

    /* logsin table */

    private static int OPL_SIN(int x) => (int)(Math.Sin(x * Math.PI / 512d) * 65536d);

    private static readonly int[] logsinrom =
    [
        0x859, 0x6c3, 0x607, 0x58b, 0x52e, 0x4e4, 0x4a6, 0x471,
        0x443, 0x41a, 0x3f5, 0x3d3, 0x3b5, 0x398, 0x37e, 0x365,
        0x34e, 0x339, 0x324, 0x311, 0x2ff, 0x2ed, 0x2dc, 0x2cd,
        0x2bd, 0x2af, 0x2a0, 0x293, 0x286, 0x279, 0x26d, 0x261,
        0x256, 0x24b, 0x240, 0x236, 0x22c, 0x222, 0x218, 0x20f,
        0x206, 0x1fd, 0x1f5, 0x1ec, 0x1e4, 0x1dc, 0x1d4, 0x1cd,
        0x1c5, 0x1be, 0x1b7, 0x1b0, 0x1a9, 0x1a2, 0x19b, 0x195,
        0x18f, 0x188, 0x182, 0x17c, 0x177, 0x171, 0x16b, 0x166,
        0x160, 0x15b, 0x155, 0x150, 0x14b, 0x146, 0x141, 0x13c,
        0x137, 0x133, 0x12e, 0x129, 0x125, 0x121, 0x11c, 0x118,
        0x114, 0x10f, 0x10b, 0x107, 0x103, 0x0ff, 0x0fb, 0x0f8,
        0x0f4, 0x0f0, 0x0ec, 0x0e9, 0x0e5, 0x0e2, 0x0de, 0x0db,
        0x0d7, 0x0d4, 0x0d1, 0x0cd, 0x0ca, 0x0c7, 0x0c4, 0x0c1,
        0x0be, 0x0bb, 0x0b8, 0x0b5, 0x0b2, 0x0af, 0x0ac, 0x0a9,
        0x0a7, 0x0a4, 0x0a1, 0x09f, 0x09c, 0x099, 0x097, 0x094,
        0x092, 0x08f, 0x08d, 0x08a, 0x088, 0x086, 0x083, 0x081,
        0x07f, 0x07d, 0x07a, 0x078, 0x076, 0x074, 0x072, 0x070,
        0x06e, 0x06c, 0x06a, 0x068, 0x066, 0x064, 0x062, 0x060,
        0x05e, 0x05c, 0x05b, 0x059, 0x057, 0x055, 0x053, 0x052,
        0x050, 0x04e, 0x04d, 0x04b, 0x04a, 0x048, 0x046, 0x045,
        0x043, 0x042, 0x040, 0x03f, 0x03e, 0x03c, 0x03b, 0x039,
        0x038, 0x037, 0x035, 0x034, 0x033, 0x031, 0x030, 0x02f,
        0x02e, 0x02d, 0x02b, 0x02a, 0x029, 0x028, 0x027, 0x026,
        0x025, 0x024, 0x023, 0x022, 0x021, 0x020, 0x01f, 0x01e,
        0x01d, 0x01c, 0x01b, 0x01a, 0x019, 0x018, 0x017, 0x017,
        0x016, 0x015, 0x014, 0x014, 0x013, 0x012, 0x011, 0x011,
        0x010, 0x00f, 0x00f, 0x00e, 0x00d, 0x00d, 0x00c, 0x00c,
        0x00b, 0x00a, 0x00a, 0x009, 0x009, 0x008, 0x008, 0x007,
        0x007, 0x007, 0x006, 0x006, 0x005, 0x005, 0x005, 0x004,
        0x004, 0x004, 0x003, 0x003, 0x003, 0x002, 0x002, 0x002,
        0x002, 0x001, 0x001, 0x001, 0x001, 0x001, 0x001, 0x001,
        0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000
    ];

    /* exp table */

    private static readonly int[] exprom =
    [
        0x7fa, 0x7f5, 0x7ef, 0x7ea, 0x7e4, 0x7df, 0x7da, 0x7d4,
        0x7cf, 0x7c9, 0x7c4, 0x7bf, 0x7b9, 0x7b4, 0x7ae, 0x7a9,
        0x7a4, 0x79f, 0x799, 0x794, 0x78f, 0x78a, 0x784, 0x77f,
        0x77a, 0x775, 0x770, 0x76a, 0x765, 0x760, 0x75b, 0x756,
        0x751, 0x74c, 0x747, 0x742, 0x73d, 0x738, 0x733, 0x72e,
        0x729, 0x724, 0x71f, 0x71a, 0x715, 0x710, 0x70b, 0x706,
        0x702, 0x6fd, 0x6f8, 0x6f3, 0x6ee, 0x6e9, 0x6e5, 0x6e0,
        0x6db, 0x6d6, 0x6d2, 0x6cd, 0x6c8, 0x6c4, 0x6bf, 0x6ba,
        0x6b5, 0x6b1, 0x6ac, 0x6a8, 0x6a3, 0x69e, 0x69a, 0x695,
        0x691, 0x68c, 0x688, 0x683, 0x67f, 0x67a, 0x676, 0x671,
        0x66d, 0x668, 0x664, 0x65f, 0x65b, 0x657, 0x652, 0x64e,
        0x649, 0x645, 0x641, 0x63c, 0x638, 0x634, 0x630, 0x62b,
        0x627, 0x623, 0x61e, 0x61a, 0x616, 0x612, 0x60e, 0x609,
        0x605, 0x601, 0x5fd, 0x5f9, 0x5f5, 0x5f0, 0x5ec, 0x5e8,
        0x5e4, 0x5e0, 0x5dc, 0x5d8, 0x5d4, 0x5d0, 0x5cc, 0x5c8,
        0x5c4, 0x5c0, 0x5bc, 0x5b8, 0x5b4, 0x5b0, 0x5ac, 0x5a8,
        0x5a4, 0x5a0, 0x59c, 0x599, 0x595, 0x591, 0x58d, 0x589,
        0x585, 0x581, 0x57e, 0x57a, 0x576, 0x572, 0x56f, 0x56b,
        0x567, 0x563, 0x560, 0x55c, 0x558, 0x554, 0x551, 0x54d,
        0x549, 0x546, 0x542, 0x53e, 0x53b, 0x537, 0x534, 0x530,
        0x52c, 0x529, 0x525, 0x522, 0x51e, 0x51b, 0x517, 0x514,
        0x510, 0x50c, 0x509, 0x506, 0x502, 0x4ff, 0x4fb, 0x4f8,
        0x4f4, 0x4f1, 0x4ed, 0x4ea, 0x4e7, 0x4e3, 0x4e0, 0x4dc,
        0x4d9, 0x4d6, 0x4d2, 0x4cf, 0x4cc, 0x4c8, 0x4c5, 0x4c2,
        0x4be, 0x4bb, 0x4b8, 0x4b5, 0x4b1, 0x4ae, 0x4ab, 0x4a8,
        0x4a4, 0x4a1, 0x49e, 0x49b, 0x498, 0x494, 0x491, 0x48e,
        0x48b, 0x488, 0x485, 0x482, 0x47e, 0x47b, 0x478, 0x475,
        0x472, 0x46f, 0x46c, 0x469, 0x466, 0x463, 0x460, 0x45d,
        0x45a, 0x457, 0x454, 0x451, 0x44e, 0x44b, 0x448, 0x445,
        0x442, 0x43f, 0x43c, 0x439, 0x436, 0x433, 0x430, 0x42d,
        0x42a, 0x428, 0x425, 0x422, 0x41f, 0x41c, 0x419, 0x416,
        0x414, 0x411, 0x40e, 0x40b, 0x408, 0x406, 0x403, 0x400
    ];

    /* freq mult table multiplied by 2
       1/2, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 12, 12, 15, 15 */

    private static readonly int[] mt =
    [
        1, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 20, 24, 24, 30, 30
    ];

    /* ksl table */

    private static readonly int[] kslrom =
    [
        0, 32, 40, 45, 48, 51, 53, 55, 56, 58, 59, 60, 61, 62, 63, 64
    ];

    private static readonly int[] kslshift =
    [
        8, 1, 2, 0
    ];

    /* envelope generator constants */

    private static readonly int[][] eg_incstep =
    [
        [0, 0, 0, 0],
        [1, 0, 0, 0],
        [1, 0, 1, 0],
        [1, 1, 1, 0]
    ];

    /* address decoding */

    private static readonly int[] ad_slot =
    [
        0, 1, 2, 3, 4, 5, -1, -1, 6, 7, 8, 9, 10, 11, -1, -1,
        12, 13, 14, 15, 16, 17, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
    ];

    private static readonly int[] ch_slot =
    [
        0, 1, 2, 6, 7, 8, 12, 13, 14, 18, 19, 20, 24, 25, 26, 30, 31, 32
    ];

    /* stereo extension panning table */

    private static readonly int[] panpot_lut = new int[256];

    private static bool panpot_lut_build;

    /* envelope generator */

    private delegate int EnvelopeSinFunc(int phase, int envelope);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int OPL3_EnvelopeCalcExp(int level)
    {
        if (level is > 0x1FFF or < 0)
            level = 0x1FFF;

        return (((exprom[level & 0xFF] << 1) >> (level >> 8)) << 16) >> 16;
    }

    private static int OPL3_EnvelopeCalcSin0(int phase, int envelope)
    {
        phase &= 0x3ff;
        var neg = (phase & 0x200) != 0 ? -1 : 0;

        var out_ = (phase & 0x100) != 0
            ? logsinrom[(phase & 0xff) ^ 0xff]
            : logsinrom[phase & 0xff];

        return OPL3_EnvelopeCalcExp(out_ + (envelope << 3)) ^ neg;
    }

    private static int OPL3_EnvelopeCalcSin1(int phase, int envelope)
    {
        int out_;
        phase &= 0x3ff;
        if ((phase & 0x200) != 0)
        {
            out_ = 0x1000;
        }
        else if ((phase & 0x100) != 0)
        {
            out_ = logsinrom[(phase & 0xff) ^ 0xff];
        }
        else
        {
            out_ = logsinrom[phase & 0xff];
        }

        return OPL3_EnvelopeCalcExp(out_ + (envelope << 3));
    }

    private static int OPL3_EnvelopeCalcSin2(int phase, int envelope)
    {
        phase &= 0x3ff;

        var out_ = (phase & 0x100) != 0
            ? logsinrom[(phase & 0xff) ^ 0xff]
            : logsinrom[phase & 0xff];

        return OPL3_EnvelopeCalcExp(out_ + (envelope << 3));
    }

    private static int OPL3_EnvelopeCalcSin3(int phase, int envelope)
    {
        phase &= 0x3ff;

        var out_ = (phase & 0x100) != 0
            ? 0x1000
            : logsinrom[phase & 0xff];

        return OPL3_EnvelopeCalcExp(out_ + (envelope << 3));
    }

    private static int OPL3_EnvelopeCalcSin4(int phase, int envelope)
    {
        int out_;
        var neg = 0;
        phase &= 0x3ff;
        if ((phase & 0x300) == 0x100)
        {
            neg = -1;
        }

        if ((phase & 0x200) != 0)
        {
            out_ = 0x1000;
        }
        else if ((phase & 0x80) != 0)
        {
            out_ = logsinrom[((phase ^ 0xff) << 1) & 0xff];
        }
        else
        {
            out_ = logsinrom[(phase << 1) & 0xff];
        }

        return OPL3_EnvelopeCalcExp(out_ + (envelope << 3)) ^ neg;
    }

    private static int OPL3_EnvelopeCalcSin5(int phase, int envelope)
    {
        int out_;
        phase &= 0x3ff;
        if ((phase & 0x200) != 0)
        {
            out_ = 0x1000;
        }
        else if ((phase & 0x80) != 0)
        {
            out_ = logsinrom[((phase ^ 0xff) << 1) & 0xff];
        }
        else
        {
            out_ = logsinrom[(phase << 1) & 0xff];
        }

        return OPL3_EnvelopeCalcExp(out_ + (envelope << 3));
    }

    private static int OPL3_EnvelopeCalcSin6(int phase, int envelope)
    {
        phase &= 0x3ff;
        var neg = (phase & 0x200) != 0 ? -1 : 0;

        return OPL3_EnvelopeCalcExp(envelope << 3) ^ neg;
    }

    private static int OPL3_EnvelopeCalcSin7(int phase, int envelope)
    {
        var neg = 0;
        phase &= 0x3ff;
        if ((phase & 0x200) != 0)
        {
            neg = -1;
            phase = (phase & 0x1ff) ^ 0x1ff;
        }

        var out_ = (phase << 3) & 0xFFFF;
        return OPL3_EnvelopeCalcExp(out_ + (envelope << 3)) ^ neg;
    }

    private static readonly EnvelopeSinFunc[] envelope_sin =
    [
        OPL3_EnvelopeCalcSin0,
        OPL3_EnvelopeCalcSin1,
        OPL3_EnvelopeCalcSin2,
        OPL3_EnvelopeCalcSin3,
        OPL3_EnvelopeCalcSin4,
        OPL3_EnvelopeCalcSin5,
        OPL3_EnvelopeCalcSin6,
        OPL3_EnvelopeCalcSin7
    ];

    private const int envelope_gen_num_attack = 0;
    private const int envelope_gen_num_decay = 1;
    private const int envelope_gen_num_sustain = 2;
    private const int envelope_gen_num_release = 3;

    private static void OPL3_EnvelopeUpdateKSL(Opl3Slot slot)
    {
        var ksl = (kslrom[slot.channel.f_num >> 6] << 2) - ((0x08 - slot.channel.block) << 5);
        if (ksl < 0)
            ksl = 0;

        slot.eg_ksl = ksl & 0xFF;
    }


    private static void OPL3_EnvelopeCalc(Opl3Slot slot)
    {
        var reg_rate = 0;
        var reset = false;
        slot.eg_out = (slot.eg_rout + (slot.reg_tl << 2)
                                    + (slot.eg_ksl >> kslshift[slot.reg_ksl]) +
                                    ((slot.trem?.Invoke() ?? 0) & 0xFF)) & 0xFFFF;
        if (slot.key != 0 && slot.eg_gen == envelope_gen_num_release)
        {
            reset = true;
            reg_rate = slot.reg_ar;
        }
        else
        {
            switch (slot.eg_gen)
            {
                case envelope_gen_num_attack:
                    reg_rate = slot.reg_ar;
                    break;
                case envelope_gen_num_decay:
                    reg_rate = slot.reg_dr;
                    break;
                case envelope_gen_num_sustain:
                    if (!slot.reg_type)
                    {
                        reg_rate = slot.reg_rr;
                    }

                    break;
                case envelope_gen_num_release:
                    reg_rate = slot.reg_rr;
                    break;
            }
        }

        slot.pg_reset = reset;
        var ks = slot.channel.ksv >> (slot.reg_ksr ? 0 : 2);
        var nonzero = reg_rate != 0;
        var rate = ks + (reg_rate << 2);
        var rate_hi = (rate >> 2) & 0xFF;
        var rate_lo = rate & 0x03;
        if ((rate_hi & 0x10) != 0)
        {
            rate_hi = 0x0f;
        }

        var eg_shift = rate_hi + slot.chip.eg_add;
        var shift = 0;
        if (nonzero)
        {
            if (rate_hi < 12)
            {
                if (slot.chip.eg_state)
                {
                    shift = eg_shift switch
                    {
                        12 => 1,
                        13 => (rate_lo >> 1) & 0x01,
                        14 => rate_lo & 0x01,
                        _ => shift
                    };
                }
            }
            else
            {
                shift = (rate_hi & 0x03) + eg_incstep[rate_lo][slot.chip.eg_timer_lo];

                if ((shift & 0x04) != 0)
                    shift = 0x03;

                if (shift == 0)
                    shift = slot.chip.eg_state ? 1 : 0;
            }
        }

        var eg_rout = slot.eg_rout;
        var eg_inc = 0;
        var eg_off = false;
        /* Instant attack */
        if (reset && rate_hi == 0x0f)
            eg_rout = 0x00;

        /* Envelope off */
        if ((slot.eg_rout & 0x1f8) == 0x1f8)
            eg_off = true;

        if (slot.eg_gen != envelope_gen_num_attack && !reset && eg_off)
            eg_rout = 0x1ff;

        switch (slot.eg_gen)
        {
            case envelope_gen_num_attack:
                if (slot.eg_rout == 0)
                    slot.eg_gen = envelope_gen_num_decay;
                else if (slot.key != 0 && shift > 0 && rate_hi != 0x0f)
                    eg_inc = ~slot.eg_rout >> (4 - shift);

                break;
            case envelope_gen_num_decay:
                if (slot.eg_rout >> 4 == slot.reg_sl)
                    slot.eg_gen = envelope_gen_num_sustain;
                else if (!eg_off && !reset && shift > 0)
                    eg_inc = 1 << (shift - 1);

                break;
            case envelope_gen_num_sustain:
            case envelope_gen_num_release:
                if (!eg_off && !reset && shift > 0)
                    eg_inc = 1 << (shift - 1);

                break;
        }

        slot.eg_rout = (eg_rout + eg_inc) & 0x1ff;
        /* Key off */
        if (reset)
            slot.eg_gen = envelope_gen_num_attack;

        if (slot.key == 0)
            slot.eg_gen = envelope_gen_num_release;
    }

    private static void OPL3_EnvelopeKeyOn(Opl3Slot slot, int type) =>
        slot.key |= type & 0xFF;

    private static void OPL3_EnvelopeKeyOff(Opl3Slot slot, int type) =>
        slot.key &= ~type & 0xFF;

    /* Phase Generator */

    private static void OPL3_PhaseGenerate(Opl3Slot slot)
    {
        var chip = slot.chip;
        var f_num = slot.channel.f_num;
        if (slot.reg_vib)
        {
            var range = (f_num >> 7) & 7;
            var vibpos = slot.chip.vibpos;

            if ((vibpos & 3) == 0)
                range = 0;
            else if ((vibpos & 1) != 0)
                range >>= 1;

            range >>= slot.chip.vibshift;

            if ((vibpos & 4) != 0)
                range = -range;

            f_num = (f_num + range) & 0xFFFF;
        }

        var basefreq = (f_num << slot.channel.block) >> 1;
        var phase = (slot.pg_phase >> 9) & 0xFFFF;

        if (slot.pg_reset)
            slot.pg_phase = 0;

        slot.pg_phase += (basefreq * mt[slot.reg_mult]) >> 1;

        /* Rhythm mode */
        var noise = chip.noise;
        slot.pg_phase_out = phase;
        if (slot.slot_num == 13) /* hh */
        {
            chip.rm_hh_bit2 = ((phase >> 2) & 1) != 0;
            chip.rm_hh_bit3 = ((phase >> 3) & 1) != 0;
            chip.rm_hh_bit7 = ((phase >> 7) & 1) != 0;
            chip.rm_hh_bit8 = ((phase >> 8) & 1) != 0;
        }

        else if (slot.slot_num == 17 && (chip.rhy & 0x20) != 0) /* tc */
        {
            chip.rm_tc_bit3 = ((phase >> 3) & 1) != 0;
            chip.rm_tc_bit5 = ((phase >> 5) & 1) != 0;
        }

        if ((chip.rhy & 0x20) != 0)
        {
            var noise1 = (noise & 1) != 0;

            var rm_xor = (chip.rm_hh_bit2 ^ chip.rm_hh_bit7)
                         | (chip.rm_hh_bit3 ^ chip.rm_tc_bit5)
                         | (chip.rm_tc_bit3 ^ chip.rm_tc_bit5);
            switch (slot.slot_num)
            {
                case 13: /* hh */
                    slot.pg_phase_out = rm_xor ? 0x200 : 0x000;
                    if (rm_xor ^ noise1)
                    {
                        slot.pg_phase_out |= 0xd0;
                    }
                    else
                    {
                        slot.pg_phase_out |= 0x34;
                    }

                    break;
                case 16: /* sd */
                    slot.pg_phase_out = (chip.rm_hh_bit8 ? 0x200 : 0x000) |
                                        (chip.rm_hh_bit8 ^ noise1 ? 0x100 : 0x000);
                    break;
                case 17: /* tc */
                    slot.pg_phase_out = (rm_xor ? 0x200 : 0x000) | 0x80;
                    break;
            }
        }

        var n_bit = ((noise >> 14) ^ noise) & 0x01;
        chip.noise = (noise >> 1) | (n_bit << 22);
    }

    /* Slot */

    private static void OPL3_SlotWrite20(Opl3Slot slot, byte data)
    {
        slot.trem = ((data >> 7) & 0x01) != 0 ? slot.chip.GetTremolo : default;
        slot.reg_vib = ((data >> 6) & 0x01) != 0;
        slot.reg_type = ((data >> 5) & 0x01) != 0;
        slot.reg_ksr = ((data >> 4) & 0x01) != 0;
        slot.reg_mult = data & 0x0f;
    }

    private static void OPL3_SlotWrite40(Opl3Slot slot, byte data)
    {
        slot.reg_ksl = (data >> 6) & 0x03;
        slot.reg_tl = data & 0x3f;
        OPL3_EnvelopeUpdateKSL(slot);
    }

    private static void OPL3_SlotWrite60(Opl3Slot slot, byte data)
    {
        slot.reg_ar = (data >> 4) & 0x0f;
        slot.reg_dr = data & 0x0f;
    }

    private static void OPL3_SlotWrite80(Opl3Slot slot, byte data)
    {
        slot.reg_sl = (data >> 4) & 0x0f;

        if (slot.reg_sl == 0x0f)
            slot.reg_sl = 0x1f;

        slot.reg_rr = data & 0x0f;
    }

    private static void OPL3_SlotWriteE0(Opl3Slot slot, byte data)
    {
        slot.reg_wf = data & 0x07;

        if (!slot.chip.newm)
            slot.reg_wf &= 0x03;
    }

    private static void OPL3_SlotGenerate(Opl3Slot slot)
    {
        slot.out_ = envelope_sin[slot.reg_wf](slot.pg_phase_out + (slot.mod?.Invoke() ?? 0), slot.eg_out);
    }

    private static void OPL3_SlotCalcFB(Opl3Slot slot)
    {
        if (slot.channel.fb != 0x00)
            slot.fbmod = (slot.prout + slot.out_) >> (0x09 - slot.channel.fb);
        else
            slot.fbmod = 0;

        slot.prout = slot.out_;
    }

    /* Channel */

    private static void OPL3_ChannelUpdateRhythm(Opl3Chip chip, byte data)
    {
        chip.rhy = data & 0x3f;
        if ((chip.rhy & 0x20) != 0)
        {
            var channel6 = chip.channel[6];
            var channel7 = chip.channel[7];
            var channel8 = chip.channel[8];
            channel6.out_[0] = channel6.out_[1] = () => channel6.slotz[1].out_;
            channel6.out_[2] = channel6.out_[3] = default;
            channel7.out_[0] = channel7.out_[1] = () => channel7.slotz[0].out_;
            channel7.out_[2] = channel7.out_[3] = () => channel7.slotz[1].out_;
            channel8.out_[0] = channel8.out_[1] = () => channel8.slotz[0].out_;
            channel8.out_[2] = channel8.out_[3] = () => channel8.slotz[1].out_;

            for (var chnum = 6; chnum < 9; chnum++)
                chip.channel[chnum].chtype = ch_drum;

            OPL3_ChannelSetupAlg(channel6);
            OPL3_ChannelSetupAlg(channel7);
            OPL3_ChannelSetupAlg(channel8);

            /* hh */
            if ((chip.rhy & 0x01) != 0)
                OPL3_EnvelopeKeyOn(channel7.slotz[0], egk_drum);
            else
                OPL3_EnvelopeKeyOff(channel7.slotz[0], egk_drum);

            /* tc */
            if ((chip.rhy & 0x02) != 0)
                OPL3_EnvelopeKeyOn(channel8.slotz[1], egk_drum);
            else
                OPL3_EnvelopeKeyOff(channel8.slotz[1], egk_drum);

            /* tom */
            if ((chip.rhy & 0x04) != 0)
                OPL3_EnvelopeKeyOn(channel8.slotz[0], egk_drum);
            else
                OPL3_EnvelopeKeyOff(channel8.slotz[0], egk_drum);

            /* sd */
            if ((chip.rhy & 0x08) != 0)
                OPL3_EnvelopeKeyOn(channel7.slotz[1], egk_drum);
            else
                OPL3_EnvelopeKeyOff(channel7.slotz[1], egk_drum);

            /* bd */
            if ((chip.rhy & 0x10) != 0)
            {
                OPL3_EnvelopeKeyOn(channel6.slotz[0], egk_drum);
                OPL3_EnvelopeKeyOn(channel6.slotz[1], egk_drum);
            }
            else
            {
                OPL3_EnvelopeKeyOff(channel6.slotz[0], egk_drum);
                OPL3_EnvelopeKeyOff(channel6.slotz[1], egk_drum);
            }
        }
        else
        {
            for (var chnum = 6; chnum < 9; chnum++)
            {
                chip.channel[chnum].chtype = ch_2op;
                OPL3_ChannelSetupAlg(chip.channel[chnum]);
                OPL3_EnvelopeKeyOff(chip.channel[chnum].slotz[0], egk_drum);
                OPL3_EnvelopeKeyOff(chip.channel[chnum].slotz[1], egk_drum);
            }
        }
    }

    private static void OPL3_ChannelWriteA0(Opl3Channel channel, byte data)
    {
        if (channel.chip.newm && channel.chtype == ch_4op2)
            return;

        channel.f_num = (channel.f_num & 0x300) | data;
        channel.ksv = (channel.block << 1)
                      | ((channel.f_num >> (0x09 - channel.chip.nts)) & 0x01);
        OPL3_EnvelopeUpdateKSL(channel.slotz[0]);
        OPL3_EnvelopeUpdateKSL(channel.slotz[1]);

        if (channel.chip.newm && channel.chtype == ch_4op)
        {
            channel.pair.f_num = channel.f_num;
            channel.pair.ksv = channel.ksv;
            OPL3_EnvelopeUpdateKSL(channel.pair.slotz[0]);
            OPL3_EnvelopeUpdateKSL(channel.pair.slotz[1]);
        }
    }

    private static void OPL3_ChannelWriteB0(Opl3Channel channel, byte data)
    {
        if (channel.chip.newm && channel.chtype == ch_4op2)
            return;

        channel.f_num = (channel.f_num & 0xff) | ((data & 0x03) << 8);
        channel.block = (data >> 2) & 0x07;
        channel.ksv = (channel.block << 1)
                      | ((channel.f_num >> (0x09 - channel.chip.nts)) & 0x01);
        OPL3_EnvelopeUpdateKSL(channel.slotz[0]);
        OPL3_EnvelopeUpdateKSL(channel.slotz[1]);

        if (channel.chip.newm && channel.chtype == ch_4op)
        {
            channel.pair.f_num = channel.f_num;
            channel.pair.block = channel.block;
            channel.pair.ksv = channel.ksv;
            OPL3_EnvelopeUpdateKSL(channel.pair.slotz[0]);
            OPL3_EnvelopeUpdateKSL(channel.pair.slotz[1]);
        }
    }

    private static void OPL3_ChannelSetupAlg(Opl3Channel channel)
    {
        if (channel.chtype == ch_drum)
        {
            if (channel.ch_num is 7 or 8)
            {
                channel.slotz[0].mod = default;
                channel.slotz[1].mod = default;
                return;
            }

            switch (channel.alg & 0x01)
            {
                case 0x00:
                    channel.slotz[0].mod = () => channel.slotz[0].fbmod;
                    channel.slotz[1].mod = () => channel.slotz[0].out_;
                    break;
                case 0x01:
                    channel.slotz[0].mod = () => channel.slotz[0].fbmod;
                    channel.slotz[1].mod = default;
                    break;
            }

            return;
        }

        if ((channel.alg & 0x08) != 0)
            return;

        if ((channel.alg & 0x04) != 0)
        {
            channel.pair.out_.AsSpan().Clear();
            switch (channel.alg & 0x03)
            {
                case 0x00:
                    channel.pair.slotz[0].mod = () => channel.pair.slotz[0].fbmod;
                    channel.pair.slotz[1].mod = () => channel.pair.slotz[0].out_;
                    channel.slotz[0].mod = () => channel.pair.slotz[1].out_;
                    channel.slotz[1].mod = () => channel.slotz[0].out_;
                    channel.out_[0] = () => channel.slotz[1].out_;
                    channel.out_[1] = default;
                    channel.out_[2] = default;
                    channel.out_[3] = default;
                    break;
                case 0x01:
                    channel.pair.slotz[0].mod = () => channel.pair.slotz[0].fbmod;
                    channel.pair.slotz[1].mod = () => channel.pair.slotz[0].out_;
                    channel.slotz[0].mod = default;
                    channel.slotz[1].mod = () => channel.slotz[0].out_;
                    channel.out_[0] = () => channel.pair.slotz[1].out_;
                    channel.out_[1] = () => channel.slotz[1].out_;
                    channel.out_[2] = default;
                    channel.out_[3] = default;
                    break;
                case 0x02:
                    channel.pair.slotz[0].mod = () => channel.pair.slotz[0].fbmod;
                    channel.pair.slotz[1].mod = default;
                    channel.slotz[0].mod = () => channel.pair.slotz[1].out_;
                    channel.slotz[1].mod = () => channel.slotz[0].out_;
                    channel.out_[0] = () => channel.pair.slotz[0].out_;
                    channel.out_[1] = () => channel.slotz[1].out_;
                    channel.out_[2] = default;
                    channel.out_[3] = default;
                    break;
                case 0x03:
                    channel.pair.slotz[0].mod = () => channel.pair.slotz[0].fbmod;
                    channel.pair.slotz[1].mod = default;
                    channel.slotz[0].mod = () => channel.pair.slotz[1].out_;
                    channel.slotz[1].mod = default;
                    channel.out_[0] = () => channel.pair.slotz[0].out_;
                    channel.out_[1] = () => channel.slotz[0].out_;
                    channel.out_[2] = () => channel.slotz[1].out_;
                    channel.out_[3] = default;
                    break;
            }
        }
        else
        {
            switch (channel.alg & 0x01)
            {
                case 0x00:
                    channel.slotz[0].mod = () => channel.slotz[0].fbmod;
                    channel.slotz[1].mod = () => channel.slotz[0].out_;
                    channel.out_[0] = () => channel.slotz[1].out_;
                    channel.out_[1] = default;
                    channel.out_[2] = default;
                    channel.out_[3] = default;
                    break;
                case 0x01:
                    channel.slotz[0].mod = () => channel.slotz[0].fbmod;
                    channel.slotz[1].mod = default;
                    channel.out_[0] = () => channel.slotz[0].out_;
                    channel.out_[1] = () => channel.slotz[1].out_;
                    channel.out_[2] = default;
                    channel.out_[3] = default;
                    break;
            }
        }
    }

    private static void OPL3_ChannelUpdateAlg(Opl3Channel channel)
    {
        channel.alg = channel.con;
        if (channel.chip.newm)
        {
            if (channel.chtype == ch_4op)
            {
                channel.pair.alg = 0x04 | (channel.con << 1) | channel.pair.con;
                channel.alg = 0x08;
                OPL3_ChannelSetupAlg(channel.pair);
            }
            else if (channel.chtype == ch_4op2)
            {
                channel.alg = 0x04 | (channel.pair.con << 1) | channel.con;
                channel.pair.alg = 0x08;
                OPL3_ChannelSetupAlg(channel);
            }
            else
            {
                OPL3_ChannelSetupAlg(channel);
            }
        }
        else
        {
            OPL3_ChannelSetupAlg(channel);
        }
    }

    private static void OPL3_ChannelWriteC0(Opl3Channel channel, byte data)
    {
        channel.fb = (data & 0x0e) >> 1;
        channel.con = data & 0x01;
        OPL3_ChannelUpdateAlg(channel);

        if (channel.chip.newm)
        {
            channel.cha = (((data >> 4) & 0x01) != 0 ? ~0 : 0) & 0xFFFF;
            channel.chb = (((data >> 5) & 0x01) != 0 ? ~0 : 0) & 0xFFFF;
            channel.chc = (((data >> 6) & 0x01) != 0 ? ~0 : 0) & 0xFFFF;
            channel.chd = (((data >> 7) & 0x01) != 0 ? ~0 : 0) & 0xFFFF;
        }
        else
        {
            channel.cha = channel.chb = 0xFFFF;
            // TODO: Verify on real chip if DAC2 output is disabled in compat mode
            channel.chc = channel.chd = 0;
        }

        if (!channel.chip.stereoext)
        {
            channel.leftpan = channel.cha << 16;
            channel.rightpan = channel.chb << 16;
        }
    }

    private static void OPL3_ChannelWriteD0(Opl3Channel channel, byte data)
    {
        if (channel.chip.stereoext)
        {
            channel.leftpan = panpot_lut[data ^ 0xff];
            channel.rightpan = panpot_lut[data];
        }
    }

    private static void OPL3_ChannelKeyOn(Opl3Channel channel)
    {
        if (channel.chip.newm)
        {
            switch (channel.chtype)
            {
                case ch_4op:
                    OPL3_EnvelopeKeyOn(channel.slotz[0], egk_norm);
                    OPL3_EnvelopeKeyOn(channel.slotz[1], egk_norm);
                    OPL3_EnvelopeKeyOn(channel.pair.slotz[0], egk_norm);
                    OPL3_EnvelopeKeyOn(channel.pair.slotz[1], egk_norm);
                    break;
                case ch_2op or ch_drum:
                    OPL3_EnvelopeKeyOn(channel.slotz[0], egk_norm);
                    OPL3_EnvelopeKeyOn(channel.slotz[1], egk_norm);
                    break;
            }
        }
        else
        {
            OPL3_EnvelopeKeyOn(channel.slotz[0], egk_norm);
            OPL3_EnvelopeKeyOn(channel.slotz[1], egk_norm);
        }
    }

    private static void OPL3_ChannelKeyOff(Opl3Channel channel)
    {
        if (channel.chip.newm)
        {
            switch (channel.chtype)
            {
                case ch_4op:
                    OPL3_EnvelopeKeyOff(channel.slotz[0], egk_norm);
                    OPL3_EnvelopeKeyOff(channel.slotz[1], egk_norm);
                    OPL3_EnvelopeKeyOff(channel.pair.slotz[0], egk_norm);
                    OPL3_EnvelopeKeyOff(channel.pair.slotz[1], egk_norm);
                    break;
                case ch_2op or ch_drum:
                    OPL3_EnvelopeKeyOff(channel.slotz[0], egk_norm);
                    OPL3_EnvelopeKeyOff(channel.slotz[1], egk_norm);
                    break;
            }
        }
        else
        {
            OPL3_EnvelopeKeyOff(channel.slotz[0], egk_norm);
            OPL3_EnvelopeKeyOff(channel.slotz[1], egk_norm);
        }
    }

    private static void OPL3_ChannelSet4Op(Opl3Chip chip, byte data)
    {
        for (var bit = 0; bit < 6; bit++)
        {
            var chnum = bit;
            if (bit >= 3)
                chnum += 9 - 3;

            if (((data >> bit) & 0x01) != 0)
            {
                chip.channel[chnum].chtype = ch_4op;
                chip.channel[chnum + 3].chtype = ch_4op2;
                OPL3_ChannelUpdateAlg(chip.channel[chnum]);
            }
            else
            {
                chip.channel[chnum].chtype = ch_2op;
                chip.channel[chnum + 3].chtype = ch_2op;
                OPL3_ChannelUpdateAlg(chip.channel[chnum]);
                OPL3_ChannelUpdateAlg(chip.channel[chnum + 3]);
            }
        }
    }

    private static int OPL3_ClipSample(int sample)
    {
        return sample switch
        {
            > 32767 => 32767,
            < -32768 => -32768,
            _ => sample
        };
    }

    private static void OPL3_ProcessSlot(Opl3Slot slot)
    {
        OPL3_SlotCalcFB(slot);
        OPL3_EnvelopeCalc(slot);
        OPL3_PhaseGenerate(slot);
        OPL3_SlotGenerate(slot);
    }

    private static int OPL3_Generate4Ch(Opl3Chip chip, Span<short> buf4)
    {
        Opl3Channel channel;
        Func<int>?[] out_;
        int accm;
        var shift = 0;

        buf4[1] = unchecked((short)OPL3_ClipSample(chip.mixbuff[1]));
        buf4[3] = unchecked((short)OPL3_ClipSample(chip.mixbuff[3]));

        for (var ii = 0; ii < 36; ii++)
            OPL3_ProcessSlot(chip.slot[ii]);

        var mix0 = 0;
        var mix1 = 0;

        for (var ii = 0; ii < 18; ii++)
        {
            channel = chip.channel[ii];
            out_ = channel.out_;
            accm = (out_[0]?.Invoke() ?? 0) +
                   (out_[1]?.Invoke() ?? 0) +
                   (out_[2]?.Invoke() ?? 0) +
                   (out_[3]?.Invoke() ?? 0);
            mix0 += unchecked((short)(chip.stereoext
                ? (accm * channel.leftpan) >> 16
                : accm & channel.cha));
            mix1 += unchecked((short)(accm & channel.chc));
        }

        chip.mixbuff[0] = mix0;
        chip.mixbuff[2] = mix1;

        buf4[0] = unchecked((short)OPL3_ClipSample(chip.mixbuff[0]));
        buf4[2] = unchecked((short)OPL3_ClipSample(chip.mixbuff[0]));

        mix0 = mix1 = 0;

        for (var ii = 0; ii < 18; ii++)
        {
            channel = chip.channel[ii];
            out_ = channel.out_;
            accm = (out_[0]?.Invoke() ?? 0) +
                   (out_[1]?.Invoke() ?? 0) +
                   (out_[2]?.Invoke() ?? 0) +
                   (out_[3]?.Invoke() ?? 0);
            mix0 += unchecked((short)(chip.stereoext
                ? (accm * channel.rightpan) >> 16
                : accm & channel.chb));
            mix1 += unchecked((short)(accm & channel.chd));
        }

        chip.mixbuff[1] = mix0;
        chip.mixbuff[3] = mix1;

        if ((chip.timer & 0x3f) == 0x3f)
            chip.tremolopos = (chip.tremolopos + 1) % 210;

        if (chip.tremolopos < 105)
            chip.tremolo = chip.tremolopos >> chip.tremoloshift;
        else
            chip.tremolo = (210 - chip.tremolopos) >> chip.tremoloshift;

        if ((chip.timer & 0x3ff) == 0x3ff)
            chip.vibpos = (chip.vibpos + 1) & 7;

        chip.timer = (chip.timer + 1) & 0xFFFF;

        if (chip.eg_state)
        {
            while (shift < 13 && ((chip.eg_timer >> shift) & 1) == 0)
                shift++;

            if (shift > 12)
                chip.eg_add = 0;
            else
                chip.eg_add = shift + 1;

            chip.eg_timer_lo = unchecked((int)(chip.eg_timer & 3));
        }

        if (chip.eg_timerrem || chip.eg_state)
        {
            if (chip.eg_timer == 0xfffffffff)
            {
                chip.eg_timer = 0;
                chip.eg_timerrem = true;
            }
            else
            {
                chip.eg_timer++;
                chip.eg_timerrem = false;
            }
        }

        chip.eg_state ^= true;

        while (chip.writebuf.Count > 0)
        {
            var writebuf = chip.writebuf.Peek();
            if (!(writebuf.time <= chip.writebuf_samplecnt))
                break;

            writebuf = chip.writebuf.Dequeue();
            writebuf.reg &= 0x1ff;
            OPL3_WriteReg(chip, writebuf.reg, writebuf.data);
        }

        chip.writebuf_samplecnt++;

        return 4;
    }

    private static int OPL3_Generate(Opl3Chip chip, Span<short> buf)
    {
        Span<short> samples = stackalloc short[4];
        OPL3_Generate4Ch(chip, samples);
        buf[0] = samples[0];
        buf[1] = samples[1];

        return 2;
    }

    private static int OPL3_Generate4ChResampled(Opl3Chip chip, Span<short> buf)
    {
        while (chip.samplecnt >= chip.rateratio)
        {
            chip.samples.AsSpan().CopyTo(chip.oldsamples);
            OPL3_Generate4Ch(chip, chip.samples);
            chip.samplecnt -= chip.rateratio;
        }

        buf[0] = (short)((chip.oldsamples[0] * (chip.rateratio - chip.samplecnt)
                          + chip.samples[0] * chip.samplecnt) / chip.rateratio);
        buf[1] = (short)((chip.oldsamples[1] * (chip.rateratio - chip.samplecnt)
                          + chip.samples[1] * chip.samplecnt) / chip.rateratio);
        buf[2] = (short)((chip.oldsamples[2] * (chip.rateratio - chip.samplecnt)
                          + chip.samples[2] * chip.samplecnt) / chip.rateratio);
        buf[3] = (short)((chip.oldsamples[3] * (chip.rateratio - chip.samplecnt)
                          + chip.samples[3] * chip.samplecnt) / chip.rateratio);

        chip.samplecnt += 1 << RSM_FRAC;

        return 4;
    }

    private static int OPL3_GenerateResampled(Opl3Chip chip, Span<short> buf)
    {
        Span<short> samples = stackalloc short[4];
        OPL3_Generate4ChResampled(chip, samples);
        buf[0] = samples[0];
        buf[1] = samples[1];

        return 2;
    }

    private static void OPL3_Reset(Opl3Chip chip, int samplerate)
    {
        chip.Reset();
        for (var slotnum = 0; slotnum < 36; slotnum++)
        {
            var slot = chip.slot[slotnum];
            slot.mod = default;
            slot.eg_rout = 0x1ff;
            slot.eg_out = 0x1ff;
            slot.eg_gen = envelope_gen_num_release;
            slot.trem = default;
            slot.slot_num = slotnum;
        }

        for (var channum = 0; channum < 18; channum++)
        {
            var channel = chip.channel[channum];
            var local_ch_slot = ch_slot[channum];
            channel.slotz[0] = chip.slot[local_ch_slot];
            channel.slotz[1] = chip.slot[local_ch_slot + 3];
            chip.slot[local_ch_slot].channel = channel;
            chip.slot[local_ch_slot + 3].channel = channel;
            channel.pair = (channum % 9) switch
            {
                < 3 => chip.channel[channum + 3],
                < 6 => chip.channel[channum - 3],
                _ => channel.pair
            };

            channel.out_.AsSpan().Clear();
            channel.chtype = ch_2op;
            channel.cha = 0xffff;
            channel.chb = 0xffff;
            channel.leftpan = 0x10000;
            channel.rightpan = 0x10000;
            channel.ch_num = channum;
            OPL3_ChannelSetupAlg(channel);
        }

        chip.noise = 1;
        chip.rateratio = (samplerate << RSM_FRAC) / OPL_RATE;
        chip.tremoloshift = 4;
        chip.vibshift = 1;

        if (panpot_lut_build)
            return;

        for (var i = 0; i < 256; i++)
            panpot_lut[i] = OPL_SIN(i);

        panpot_lut_build = true;
    }

    private static void OPL3_WriteReg(Opl3Chip chip, int reg, byte v)
    {
        var high = (reg >> 8) & 0x01;
        var regm = reg & 0xff;

        switch (regm & 0xf0)
        {
            case 0x00:
                if (high != 0)
                {
                    switch (regm & 0x0f)
                    {
                        case 0x04:
                            OPL3_ChannelSet4Op(chip, v);
                            break;
                        case 0x05:
                            chip.newm = (v & 0x01) != 0;
                            chip.stereoext = ((v >> 1) & 0x01) != 0;
                            break;
                    }
                }
                else
                {
                    chip.nts = (regm & 0x0f) switch
                    {
                        0x08 => (v >> 6) & 0x01,
                        _ => chip.nts
                    };
                }

                break;
            case 0x20:
            case 0x30:
                if (ad_slot[regm & 0x1f] >= 0)
                {
                    OPL3_SlotWrite20(chip.slot[18 * high + ad_slot[regm & 0x1f]], v);
                }

                break;
            case 0x40:
            case 0x50:
                if (ad_slot[regm & 0x1f] >= 0)
                {
                    OPL3_SlotWrite40(chip.slot[18 * high + ad_slot[regm & 0x1f]], v);
                }

                break;
            case 0x60:
            case 0x70:
                if (ad_slot[regm & 0x1f] >= 0)
                {
                    OPL3_SlotWrite60(chip.slot[18 * high + ad_slot[regm & 0x1f]], v);
                }

                break;
            case 0x80:
            case 0x90:
                if (ad_slot[regm & 0x1f] >= 0)
                {
                    OPL3_SlotWrite80(chip.slot[18 * high + ad_slot[regm & 0x1f]], v);
                }

                break;
            case 0xe0:
            case 0xf0:
                if (ad_slot[regm & 0x1f] >= 0)
                {
                    OPL3_SlotWriteE0(chip.slot[18 * high + ad_slot[regm & 0x1f]], v);
                }

                break;
            case 0xa0:
                if ((regm & 0x0f) < 9)
                {
                    OPL3_ChannelWriteA0(chip.channel[9 * high + (regm & 0x0f)], v);
                }

                break;
            case 0xb0:
                if (regm == 0xbd && high == 0)
                {
                    chip.tremoloshift = (((v >> 7) ^ 1) << 1) + 2;
                    chip.vibshift = ((v >> 6) & 0x01) ^ 1;
                    OPL3_ChannelUpdateRhythm(chip, v);
                }
                else if ((regm & 0x0f) < 9)
                {
                    OPL3_ChannelWriteB0(chip.channel[9 * high + (regm & 0x0f)], v);
                    if ((v & 0x20) != 0)
                    {
                        OPL3_ChannelKeyOn(chip.channel[9 * high + (regm & 0x0f)]);
                    }
                    else
                    {
                        OPL3_ChannelKeyOff(chip.channel[9 * high + (regm & 0x0f)]);
                    }
                }

                break;
            case 0xc0:
                if ((regm & 0x0f) < 9)
                {
                    OPL3_ChannelWriteC0(chip.channel[9 * high + (regm & 0x0f)], v);
                }

                break;
            case 0xd0:
                if ((regm & 0x0f) < 9)
                {
                    OPL3_ChannelWriteD0(chip.channel[9 * high + (regm & 0x0f)], v);
                }

                break;
        }
    }

    private static void OPL3_WriteRegBuffered(Opl3Chip chip, int reg, byte v)
    {
        var write = new Opl3Writebuf
        {
            reg = reg | 0x200,
            data = v
        };

        var time1 = chip.writebuf_lasttime + OPL_WRITEBUF_DELAY;
        var time2 = chip.writebuf_samplecnt;

        if (time1 < time2)
            time1 = time2;

        write.time = time1;
        chip.writebuf_lasttime = time1;
        chip.writebuf.Enqueue(write);
    }

    private static int OPL3_Generate4ChStream(Opl3Chip chip, Span<short> sndptr, int numsamples)
    {
        var sndptr_idx = 0;

        for (var i = 0; i < numsamples; i++)
            sndptr_idx += OPL3_Generate4ChResampled(chip, sndptr.Slice(sndptr_idx));

        return sndptr_idx;
    }

    private static int OPL3_GenerateStream(Opl3Chip chip, Span<short> sndptr, int numsamples)
    {
        var sndptr_idx = 0;

        for (var i = 0; i < numsamples; i++)
            sndptr_idx += OPL3_GenerateResampled(chip, sndptr.Slice(sndptr_idx));

        return sndptr_idx;
    }

    public int Generate4Ch(Opl3Chip chip, Span<short> buf) =>
        OPL3_Generate4Ch(chip, buf);

    public int Generate(Opl3Chip chip, Span<short> buf) =>
        OPL3_Generate(chip, buf);

    public int Generate4ChResampled(Opl3Chip chip, Span<short> buf) =>
        OPL3_Generate4ChResampled(chip, buf);

    public int GenerateResampled(Opl3Chip chip, Span<short> buf) =>
        OPL3_GenerateResampled(chip, buf);

    public void Reset(Opl3Chip chip, int samplerate) =>
        OPL3_Reset(chip, samplerate);

    public void WriteReg(Opl3Chip chip, int reg, byte v) =>
        OPL3_WriteReg(chip, reg, v);

    public void WriteRegBuffered(Opl3Chip chip, int reg, byte v) =>
        OPL3_WriteRegBuffered(chip, reg, v);

    public int Generate4ChStream(Opl3Chip chip, Span<short> sndptr, int numsamples) =>
        OPL3_Generate4ChStream(chip, sndptr, numsamples);

    public int GenerateStream(Opl3Chip chip, Span<short> sndptr, int numsamples) =>
        OPL3_GenerateStream(chip, sndptr, numsamples);
}