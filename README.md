# NukedOpl

*C# port of the Nuked OPL3 core.* The code has been ported from [nukeykt/Nuked-OPL3](https://github.com/nukeykt/Nuked-OPL3).

This library is licensed under the Lesser GNU Public License 2.1, identical to the original code.

### Prerequisites

One of the following:

- .NET 6.0 or greater
- .NET Standard 2.0
- .NET Framework 4.6.2+

### Using it in your .NET project

```csharp
// initialize the Nuked OPL3 engine
var opl = new Opl3();

// initialize the sound chip (which holds all internal registers)
var chip = new Opl3Chip();

// initialize registers and resampling frequency
opl.Reset(chip, 44100);
```

You can then write some registers like so:

```csharp
// enable percussion mode
opl.WriteReg(chip, 0x0BD, 0x20);
```

There's a couple functions to render the output:

```csharp
// render a single stereo sample pair at native 49716hz
var buffer = new short[2];
opl.Generate(chip, buffer);

// render at the specified resample rate in Reset() above
opl.GenerateResampled(chip, buffer);

// render multiple samples at the resample rate
var buffer2 = new short[10000];
opl.GenerateStream(chip, buffer2, 10000);
```

### Porting Notes

Because one goal was to use no `unsafe` code, pointers are off limits.
References are still okay, so all pointers from the original code have been
rewritten to arrays of length 1.

There is no concept of boolean types in C. Some of the variables have been analyzed
to work like booleans and were converted over from (u)int8 types.

Structs were broken out from the header file and are classes in individual files.
