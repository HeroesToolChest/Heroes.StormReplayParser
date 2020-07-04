namespace Heroes.StormReplayParser.MpqHeroesTool
{
    internal class MpqHeader
    {
        public const uint MpqId = 0x1a51504d; // 441536589
        public const uint Size = 32;

        public MpqHeader(ref BitReader bitReaderStruct)
        {
            if (!LocateHeader(ref bitReaderStruct))
                throw new MpqHeroesToolException("Could not locate the header");

            DataOffset = bitReaderStruct.ReadUInt32Aligned();
            ArchiveSize = bitReaderStruct.ReadUInt32Aligned();
            MpqVersion = bitReaderStruct.ReadUInt16Aligned();
            BlockSize = bitReaderStruct.ReadUInt16Aligned();
            HashTablePos = bitReaderStruct.ReadUInt32Aligned();
            BlockTablePos = bitReaderStruct.ReadUInt32Aligned();
            HashTableSize = bitReaderStruct.ReadUInt32Aligned();
            BlockTableSize = bitReaderStruct.ReadUInt32Aligned();

            if (MpqVersion == 1)
            {
                ExtendedBlockTableOffset = bitReaderStruct.ReadInt64Aligned();
                HashTableOffsetHigh = bitReaderStruct.ReadInt16Aligned();
                BlockTableOffsetHigh = bitReaderStruct.ReadInt16Aligned();
            }

            HashTablePos += (uint)HeaderOffset;
            BlockTablePos += (uint)HeaderOffset;

            if (DataOffset == 0x6d9e4b86) // A protected archive.
                DataOffset = (uint)(Size + HeaderOffset);
        }

        public uint DataOffset { get; private set; } // Offset of the first file.  AKA Header size
        public uint ArchiveSize { get; private set; }
        public ushort MpqVersion { get; private set; }
        public ushort BlockSize { get; private set; } // Size of file block is 0x200 << BlockSize
        public uint HashTablePos { get; private set; }
        public uint BlockTablePos { get; private set; }
        public uint HashTableSize { get; private set; }
        public uint BlockTableSize { get; private set; }

        // Version 1 fields
        // The extended block table is an array of Int16 - higher bits of the offsets in the block table.
        public long ExtendedBlockTableOffset { get; private set; }
        public short HashTableOffsetHigh { get; private set; }
        public short BlockTableOffsetHigh { get; private set; }

        public long HeaderOffset { get; private set; }

        private bool LocateHeader(ref BitReader bitReaderStruct)
        {
            for (int i = 0x200; i < bitReaderStruct.Length - Size; i += 0x200)
            {
                bitReaderStruct.Index = i;
                uint id = bitReaderStruct.ReadUInt32Aligned();

                if (id == MpqId)
                {
                    HeaderOffset = i;

                    return true;
                }
            }

            return false;
        }
    }
}
