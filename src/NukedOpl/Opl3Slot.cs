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
    public sealed class Opl3Slot
    {
        public Opl3Slot(Opl3Chip chip) => this.chip = chip;

        public Opl3Channel channel { get; set; }
        public Opl3Chip chip { get; }
        public short[] out_ { get; set; }
        public short[] fbmod { get; set; }
        public short[] mod { get; set; }
        public short prout { get; set; }
        public ushort eg_rout { get; set; }
        public ushort eg_out { get; set; }
        public byte eg_inc { get; set; }
        public byte eg_gen { get; set; }
        public byte eg_rate { get; set; }
        public byte eg_ksl { get; set; }
        public short[] trem { get; set; }
        public bool reg_vib { get; set; }
        public bool reg_type { get; set; }
        public bool reg_ksr { get; set; }
        public byte reg_mult { get; set; } // 0-F
        public byte reg_ksl { get; set; } // 0-3
        public byte reg_tl { get; set; } // 00-3F
        public byte reg_ar { get; set; } // 0-F
        public byte reg_dr { get; set; } // 0-F
        public byte reg_sl { get; set; } // 0-F
        public byte reg_rr { get; set; } // 0-F
        public byte reg_wf { get; set; } // 0-7
        public byte key { get; set; } // 0-3
        public bool pg_reset { get; set; }
        public uint pg_phase { get; set; }
        public ushort pg_phase_out { get; set; }
        public byte slot_num { get; set; }

        public void Reset()
        {
            out_ = new short[1];
            fbmod = new short[1];
            mod = new short[1];
            prout = default;
            eg_rout = default;
            eg_out = default;
            eg_inc = default;
            eg_gen = default;
            eg_rate = default;
            eg_ksl = default;
            trem = new short[1];
            reg_vib = default;
            reg_type = default;
            reg_ksl = default;
            reg_tl = default;
            reg_ar = default;
            reg_dr = default;
            reg_sl = default;
            reg_rr = default;
            reg_wf = default;
            key = default;
            pg_reset = default;
            pg_phase = default;
            pg_phase_out = default;
            slot_num = default;
        }
    }
}