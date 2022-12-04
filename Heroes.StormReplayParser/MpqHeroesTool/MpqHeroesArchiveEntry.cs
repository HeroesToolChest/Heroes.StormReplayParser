namespace Heroes.StormReplayParser.MpqHeroesTool;

internal struct MpqHeroesArchiveEntry
{
    public const uint Size = 16;

    private readonly uint _fileOffset; // Relative to the header offset

    private string? _fileName;

    internal MpqHeroesArchiveEntry(ref BitReader bitReaderStruct, uint headerOffset)
    {
        _fileOffset = bitReaderStruct.ReadUInt32Aligned();
        FilePosition = headerOffset + _fileOffset;
        CompressedSize = bitReaderStruct.ReadUInt32Aligned();
        FileSize = bitReaderStruct.ReadUInt32Aligned();
        Flags = (MpqFileFlags)bitReaderStruct.ReadUInt32Aligned();
        EncryptionSeed = 0;
        _fileName = null;
    }

    public uint CompressedSize { get; }
    public uint FileSize { get; }
    public MpqFileFlags Flags { get; }
    public uint EncryptionSeed { get; internal set; }
    public uint FilePosition { get; } // Absolute position in the file

    public string? FileName
    {
        get
        {
            return _fileName;
        }
        set
        {
            _fileName = value;
            EncryptionSeed = CalculateEncryptionSeed();
        }
    }

    public bool IsEncrypted => (Flags & MpqFileFlags.Encrypted) != 0;

    public bool IsCompressed => (Flags & MpqFileFlags.Compressed) != 0;

    public bool Exists => Flags != 0;

    public bool IsSingleUnit => (Flags & MpqFileFlags.SingleUnit) != 0;

    public int FlagsAsInt => (int)Flags;

    public override string ToString()
    {
        if (FileName is null)
        {
            if (!Exists)
                return "(Deleted file)";
            return string.Format("Unknown file @ {0}", FilePosition);
        }

        return FileName;
    }

    private uint CalculateEncryptionSeed()
    {
        if (FileName is null) return 0;

        uint seed = MpqHeroesArchive.HashString(Path.GetFileName(FileName), 0x300);
        if ((Flags & MpqFileFlags.BlockOffsetAdjustedKey) == MpqFileFlags.BlockOffsetAdjustedKey)
            seed = (seed + _fileOffset) ^ FileSize;
        return seed;
    }
}
