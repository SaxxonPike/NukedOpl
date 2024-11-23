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

using System;
using System.Collections.Generic;

namespace NukedOpl;

public sealed class Opl3Chip
{
    public const int OPL_WRITEBUF_SIZE = 1024;

    public Opl3Chip()
    {
        for (var i = 0; i < channel.Length; i++)
            channel[i] = new Opl3Channel(this);
        for (var i = 0; i < slot.Length; i++)
            slot[i] = new Opl3Slot(this);
        writebuf.Clear();
    }

    public Opl3Channel[] channel { get; } = new Opl3Channel[18];
    public Opl3Slot[] slot { get; } = new Opl3Slot[36];
    public int timer { get; set; }
    public ulong eg_timer { get; set; }
    public bool eg_timerrem { get; set; }
    public bool eg_state { get; set; }
    public int eg_add { get; set; }
    public int eg_timer_lo { get; set; }
    public bool newm { get; set; }
    public int nts { get; set; }
    public int rhy { get; set; }
    public int vibpos { get; set; }
    public int vibshift { get; set; }
    public int tremolo { get; set; }
    public int tremolopos { get; set; }
    public int tremoloshift { get; set; }
    public int noise { get; set; }
    public int mixbuff0 { get; set; }
    public int mixbuff1 { get; set; }
    public int mixbuff2 { get; set; }
    public int mixbuff3 { get; set; }
    public bool rm_hh_bit2 { get; set; }
    public bool rm_hh_bit3 { get; set; }
    public bool rm_hh_bit7 { get; set; }
    public bool rm_hh_bit8 { get; set; }
    public bool rm_tc_bit3 { get; set; }
    public bool rm_tc_bit5 { get; set; }
    public bool stereoext { get; set; }

    /* OPL3L */
    public int rateratio { get; set; }
    public int samplecnt { get; set; }
    public short[] oldsamples { get; } = new short[4];
    public short[] samples { get; } = new short[4];

    public ulong writebuf_samplecnt { get; set; }
    public ulong writebuf_lasttime { get; set; }
    public Queue<Opl3Writebuf> writebuf { get; } = new();

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
        tremolo = default;
        tremolopos = default;
        tremoloshift = default;
        noise = default;
        mixbuff0 = default;
        mixbuff1 = default;
        mixbuff2 = default;
        mixbuff3 = default;
        rm_hh_bit2 = default;
        rm_hh_bit3 = default;
        rm_hh_bit7 = default;
        rm_hh_bit8 = default;
        rm_tc_bit3 = default;
        rm_tc_bit5 = default;
        stereoext = default;
        rateratio = default;
        samplecnt = default;
        oldsamples.AsSpan().Clear();
        samples.AsSpan().Clear();
        writebuf_samplecnt = default;
        writebuf_lasttime = default;
        foreach (var x in writebuf)
            x.Reset();
    }

    internal int GetTremolo() => tremolo;
}