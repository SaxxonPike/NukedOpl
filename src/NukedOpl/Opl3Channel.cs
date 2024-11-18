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

namespace NukedOpl;

public sealed class Opl3Channel
{
    public Opl3Channel(Opl3Chip chip) => this.chip = chip;

    public Opl3Slot[] slots { get; } = [null, null];
    public Opl3Channel pair { get; set; }
    public Opl3Chip chip { get; }

    public int[][] out_ { get; } = [new int[1], new int[1], new int[1], new int[1]];

    public int leftpan { get; set; }
    public int rightpan { get; set; }
    public int chtype { get; set; }
    public int f_num { get; set; }
    public int block { get; set; }
    public int fb { get; set; }
    public int con { get; set; }
    public int alg { get; set; }
    public int ksv { get; set; }
    public int cha { get; set; }
    public int chb { get; set; }
    public int ch_num { get; set; }

    public void Reset()
    {
        slots[0] = null;
        slots[1] = null;
        pair = null;
        out_[0] = new int[1];
        out_[1] = new int[1];
        out_[2] = new int[1];
        out_[3] = new int[1];
        leftpan = default;
        rightpan = default;
        chtype = default;
        f_num = default;
        block = default;
        fb = default;
        con = default;
        alg = default;
        ksv = default;
        cha = default;
        chb = default;
        ch_num = default;
    }
}