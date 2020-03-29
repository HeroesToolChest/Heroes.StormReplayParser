using System;

namespace Heroes.StormReplayParser.MpqHeroesTool
{
    internal class MpqHash
    {
        public const uint Size = 16;

        public MpqHash()
        {
        }

        public MpqHash(ReadOnlySpan<byte> source)
        {
            Name1 = source.ReadUInt32Aligned();
            Name2 = source.ReadUInt32Aligned();
            Locale = source.ReadUInt32Aligned(); // Normally 0 or UInt32.MaxValue (0xffffffff)
            BlockIndex = source.ReadUInt32Aligned();
        }

        public uint Name1 { get; private set; }
        public uint Name2 { get; private set; }
        public uint Locale { get; private set; }
        public uint BlockIndex { get; private set; }
    }
}
