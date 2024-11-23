using System;

namespace NukedOpl;

public partial class Opl3
{
    /* cached delegates for performance */

    private static readonly Opl3Channel.MixFunc MixRhythm6 = MixRhythm6Impl;
    private static readonly Opl3Channel.MixFunc MixRhythm7And8 = MixRhythm7And8Impl;
    private static readonly Opl3Channel.MixFunc MixAlg0And2 = MixAlg0And2Impl;
    private static readonly Opl3Channel.MixFunc MixAlg1And3 = MixAlg1And3Impl;
    private static readonly Opl3Channel.MixFunc MixAlg4 = MixAlg4Impl;
    private static readonly Opl3Channel.MixFunc MixAlg5 = MixAlg5Impl;
    private static readonly Opl3Channel.MixFunc MixAlg6 = MixAlg6Impl;
    private static readonly Opl3Channel.MixFunc MixAlg7 = MixAlg7Impl;
    private static readonly Opl3Slot.ModFunc ModSlot0FbMod = ModSlot0FbModImpl;
    private static readonly Opl3Slot.ModFunc ModPairSlot0FbMod = ModPairSlot0FbModImpl;
    private static readonly Opl3Slot.ModFunc ModSlot0Out = ModSlot0OutImpl;
    private static readonly Opl3Slot.ModFunc ModPairSlot0Out = ModPairSlot0OutImpl;
    private static readonly Opl3Slot.ModFunc ModPairSlot1Out = ModPairSlot1OutImpl;
    private static readonly Opl3Slot.TremFunc TremOn = TremOnImpl;

    private static int MixAlg0And2Impl(Opl3Channel c) =>
        c.slot1.out_;

    private static int MixAlg1And3Impl(Opl3Channel c) =>
        c.slot0.out_ + c.slot1.out_;

    private static int MixAlg4Impl(Opl3Channel c) =>
        c.slot1.out_;

    private static int MixAlg5Impl(Opl3Channel c) =>
        c.pair.slot1.out_ + c.slot1.out_;

    private static int MixAlg6Impl(Opl3Channel c) =>
        c.pair.slot0.out_ + c.slot1.out_;

    private static int MixAlg7Impl(Opl3Channel c) =>
        c.pair.slot0.out_ + c.slot0.out_ + c.slot1.out_;

    private static int ModSlot0FbModImpl(Opl3Channel c) =>
        c.slot0.fbmod;

    private static int ModPairSlot0FbModImpl(Opl3Channel c) =>
        c.pair.slot0.fbmod;

    private static int ModSlot0OutImpl(Opl3Channel c) =>
        c.slot0.out_;

    private static int ModPairSlot0OutImpl(Opl3Channel c) =>
        c.pair.slot0.out_;

    private static int ModPairSlot1OutImpl(Opl3Channel c) =>
        c.pair.slot1.out_;

    private static int TremOnImpl(Opl3Slot s) =>
        s.chip.tremolo;
    
    private static int MixRhythm6Impl(Opl3Channel c) =>
        c.slot1.out_ << 1;

    private static int MixRhythm7And8Impl(Opl3Channel c) =>
        (c.slot0.out_ + c.slot1.out_) << 1;
}