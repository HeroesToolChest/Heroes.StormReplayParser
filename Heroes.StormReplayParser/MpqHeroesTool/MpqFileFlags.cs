using System;
using System.Diagnostics.CodeAnalysis;

namespace Heroes.StormReplayParser.MpqHeroesTool
{
    [Flags]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "not needed")]
    internal enum MpqFileFlags : uint
    {
        CompressedPK = 0x100, // AKA Imploded
        CompressedMulti = 0x200,
        Compressed = 0xff00,
        Encrypted = 0x10000,
        BlockOffsetAdjustedKey = 0x020000, // AKA FixSeed
        SingleUnit = 0x1000000,
        FileHasMetadata = 0x04000000,
        Exists = 0x80000000,
    }
}
