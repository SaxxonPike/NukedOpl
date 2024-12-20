﻿/* Nuked OPL3
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

using System;

namespace NukedOpl;

public sealed class Opl3Slot(Opl3Chip chip)
{
    public delegate int ModFunc(Opl3Channel c);
    public delegate int TremFunc(Opl3Slot s);
    
    public Opl3Channel channel { get; internal set; } = null!;
    public Opl3Chip chip { get; } = chip;
    public int out_ { get; set; }
    public int fbmod { get; set; }
    public ModFunc? mod { get; set; }
    public int prout { get; set; }
    public int eg_rout { get; set; }
    public int eg_out { get; set; }
    public int eg_inc { get; set; }
    public int eg_gen { get; set; }
    public int eg_rate { get; set; }
    public int eg_ksl { get; set; }
    public TremFunc? trem { get; set; }
    public bool reg_vib { get; set; }
    public bool reg_type { get; set; }
    public bool reg_ksr { get; set; }
    public int reg_mult { get; set; } // 0-F
    public int reg_ksl { get; set; } // 0-3
    public int reg_tl { get; set; } // 00-3F
    public int reg_ar { get; set; } // 0-F
    public int reg_dr { get; set; } // 0-F
    public int reg_sl { get; set; } // 0-F
    public int reg_rr { get; set; } // 0-F
    public int reg_wf { get; set; } // 0-7
    public int key { get; set; } // 0-3
    public bool pg_reset { get; set; }
    public int pg_phase { get; set; }
    public int pg_phase_out { get; set; }
    public int slot_num { get; set; }

    public void Reset()
    {
        out_ = default;
        fbmod = default;
        mod = default;
        prout = default;
        eg_rout = default;
        eg_out = default;
        eg_inc = default;
        eg_gen = default;
        eg_rate = default;
        eg_ksl = default;
        trem = default;
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