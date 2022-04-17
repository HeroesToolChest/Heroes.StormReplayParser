namespace Heroes.StormReplayParser.MpqHeroesTool;

internal struct MpqHash
{
    public const uint Size = 16;

    public MpqHash(uint name1, uint name2, uint locale, uint blockIndex)
    {
        Name1 = name1;
        Name2 = name2;
        Locale = locale;
        BlockIndex = blockIndex;
    }

    public MpqHash(ref BitReader bitReaderStruct)
    {
        Name1 = bitReaderStruct.ReadUInt32Aligned();
        Name2 = bitReaderStruct.ReadUInt32Aligned();
        Locale = bitReaderStruct.ReadUInt32Aligned(); // Normally 0 or UInt32.MaxValue (0xffffffff)
        BlockIndex = bitReaderStruct.ReadUInt32Aligned();
    }

    public uint Name1 { get; }
    public uint Name2 { get; }
    public uint Locale { get; }
    public uint BlockIndex { get; }
}
