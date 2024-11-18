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

namespace NukedOpl;

public interface IOpl3
{
    /// <summary>
    /// Generate a single stereo sample pair at the native 49716hz sample rate.
    /// Returns the number of samples actually generated (likely 2.)
    /// </summary>
    int Generate(Opl3Chip chip, Span<short> buf);
        
    /// <summary>
    /// Generate a single stereo sample pair at the sample rate specified by Reset().
    /// Returns the number of samples actually generated (likely 2.)
    /// </summary>
    int GenerateResampled(Opl3Chip chip, Span<short> buf);
        
    /// <summary>
    /// Resets the OPL3 engine and configures the resample rate.
    /// </summary>
    void Reset(Opl3Chip chip, int samplerate);
        
    /// <summary>
    /// Performs a write to an OPL3 register.
    /// </summary>
    void WriteReg(Opl3Chip chip, int reg, byte v);
        
    /// <summary>
    /// Performs a buffered write to an OPL3 register. This behaves a bit more like how real writes to the chip
    /// work by performing the writes at a specific time in the generator clock cycle.
    /// </summary>
    void WriteRegBuffered(Opl3Chip chip, int reg, byte v);
        
    /// <summary>
    /// Generate a number of stereo sample pairs at the sample rate specified by Reset().
    /// Returns the number of samples actually generated.
    /// </summary>
    int GenerateStream(Opl3Chip chip, Span<short> sndptr, int numsamples);
}