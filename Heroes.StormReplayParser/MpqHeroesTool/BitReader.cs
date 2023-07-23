namespace Heroes.StormReplayParser.MpqHeroesTool;

/// <summary>
/// A reader that provides methods to read bits or bytes from the buffer.
/// </summary>
public ref struct BitReader
{
    private readonly ReadOnlySpan<byte> _buffer;
    private int _bitIndex;
    private byte _currentByte;

    /// <summary>
    /// Initializes a new instance of the <see cref="BitReader"/> struct.
    /// </summary>
    /// <param name="buffer">A read-only span of bytes.</param>
    /// <param name="endianType">Sets the endian type.</param>
    public BitReader(ReadOnlySpan<byte> buffer, EndianType endianType)
    {
        Index = 0;
        _bitIndex = 0;
        _currentByte = 0;
        _buffer = buffer;
        EndianType = endianType;
    }

    /// <summary>
    /// Gets the number of items in the buffer.
    /// </summary>
    public readonly int Length => _buffer.Length;

    /// <summary>
    /// Gets or sets the current index in the buffer.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Gets or sets the endian type.
    /// </summary>
    public EndianType EndianType { get; set; }

    /// <summary>
    /// Reads up to 32 bits from the buffer as an uint.
    /// </summary>
    /// <param name="numberOfBits">The number of bits to read.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="numberOfBits"/> is less than 0 or greater than 32.</exception>
    /// <returns>An unsigned integer.</returns>
    public uint ReadBits(int numberOfBits)
    {
        if (numberOfBits > 32)
            throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be less than 33");
        if (numberOfBits < 0)
            throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be greater than -1");

        return EndianType == EndianType.BigEndian ? GetValueFromBits(numberOfBits) : BinaryPrimitives.ReverseEndianness(GetValueFromBits(numberOfBits));
    }

    /// <summary>
    /// Reads up to 64 bits from the buffer as an ulong.
    /// </summary>
    /// <param name="numberOfBits">The number of bits to read.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="numberOfBits"/> is less than 1 or greater than 64.</exception>
    /// <returns>An unsigned long.</returns>
    public ulong ReadULongBits(int numberOfBits)
    {
        if (numberOfBits > 64)
            throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be less than 65");
        if (numberOfBits < 1)
            throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be greater than 0");

        return EndianType == EndianType.BigEndian ? GetULongValueFromBits(numberOfBits) : BinaryPrimitives.ReverseEndianness(GetULongValueFromBits(numberOfBits));
    }

    /// <summary>
    /// Reads up to 64 bits from the buffer as an long.
    /// </summary>
    /// <param name="numberOfBits">The number of bits to read.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <returns>A long.</returns>
    public long ReadLongBits(int numberOfBits)
    {
        if (numberOfBits > 64)
            throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be less than 65");
        if (numberOfBits < 1)
            throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be greater than 0");

        return EndianType == EndianType.BigEndian ? GetLongValueFromBits(numberOfBits) : BinaryPrimitives.ReverseEndianness(GetLongValueFromBits(numberOfBits));
    }

    /// <summary>
    /// Read a number of bits from the buffer as an array of booleans.
    /// </summary>
    /// <param name="numberOfBits">The number of bits to read.</param>
    /// <returns>An array of booleans.</returns>
    public bool[] ReadBitArray(uint numberOfBits)
    {
        bool[] bitArray = new bool[numberOfBits];

        SetBitArray(bitArray);

        return bitArray;
    }

    /// <summary>
    /// Read a number of bits from the buffer as an array of booleans.
    /// </summary>
    /// <param name="numberOfBits">The number of bits to read.</param>
    /// <returns>An array of booleans.</returns>
    public bool[] ReadBitArray(int numberOfBits)
    {
        bool[] bitArray = new bool[numberOfBits];

        SetBitArray(bitArray);

        return bitArray;
    }

    /// <summary>
    /// Read a single bit from the buffer as a boolean.
    /// </summary>
    /// <returns>A boolean at the current bit index.</returns>
    public bool ReadBoolean()
    {
        int bytePosition = _bitIndex & 7;

        if (bytePosition == 0)
        {
            _currentByte = ReadAlignedByte();
        }

        bool bit = ((_currentByte >> bytePosition) & 1) == 1;

        _bitIndex++;

        return bit;
    }

    /// <summary>
    /// Reads 2 aligned bytes from the buffer as an ushort.
    /// </summary>
    /// <returns>An unsigned short.</returns>
    public ushort ReadUInt16Aligned()
    {
        if (EndianType == EndianType.LittleEndian)
            return BinaryPrimitives.ReadUInt16LittleEndian(_buffer[Index..(Index += 2)]);
        else
            return BinaryPrimitives.ReadUInt16BigEndian(_buffer[Index..(Index += 2)]);
    }

    /// <summary>
    /// Reads 2 aligned bytes from the buffer as a short.
    /// </summary>
    /// <returns>A short.</returns>
    public short ReadInt16Aligned()
    {
        if (EndianType == EndianType.LittleEndian)
            return BinaryPrimitives.ReadInt16LittleEndian(_buffer[Index..(Index += 2)]);
        else
            return BinaryPrimitives.ReadInt16BigEndian(_buffer[Index..(Index += 2)]);
    }

    /// <summary>
    /// Reads 2 unaligned bytes from the buffer as a short.
    /// </summary>
    /// <returns>A short.</returns>
    public short ReadInt16Unaligned() => (short)ReadBits(16);

    /// <summary>
    /// Reads 4 aligned bytes from the buffer as an uint.
    /// </summary>
    /// <returns>An unsigned interger.</returns>
    public uint ReadUInt32Aligned()
    {
        if (EndianType == EndianType.LittleEndian)
            return BinaryPrimitives.ReadUInt32LittleEndian(_buffer[Index..(Index += 4)]);
        else
            return BinaryPrimitives.ReadUInt32BigEndian(_buffer[Index..(Index += 4)]);
    }

    /// <summary>
    /// Reads 4 aligned bytes from the buffer as a int.
    /// </summary>
    /// <returns>An integer.</returns>
    public int ReadInt32Aligned()
    {
        if (EndianType == EndianType.LittleEndian)
            return BinaryPrimitives.ReadInt32LittleEndian(_buffer[Index..(Index += 4)]);
        else
            return BinaryPrimitives.ReadInt32BigEndian(_buffer[Index..(Index += 4)]);
    }

    /// <summary>
    /// Reads 4 unaligned bytes from the buffer as an uint.
    /// </summary>
    /// <returns>An unsigned integer.</returns>
    public uint ReadUInt32Unaligned() => ReadBits(32);

    /// <summary>
    /// Reads 4 unaligned bytes from the buffer as an int.
    /// </summary>
    /// <returns>An integer.</returns>
    public int ReadInt32Unaligned() => (int)ReadBits(32);

    /// <summary>
    /// Reads 8 aligned bytes from the buffer as a ulong.
    /// </summary>
    /// <returns>An unsigned long.</returns>
    public ulong ReadUInt64Aligned()
    {
        if (EndianType == EndianType.LittleEndian)
            return BinaryPrimitives.ReadUInt64LittleEndian(_buffer[Index..(Index += 8)]);
        else
            return BinaryPrimitives.ReadUInt64BigEndian(_buffer[Index..(Index += 8)]);
    }

    /// <summary>
    /// Reads 8 aligned bytes from the buffer as a long.
    /// </summary>
    /// <returns>A long.</returns>
    public long ReadInt64Aligned()
    {
        if (EndianType == EndianType.LittleEndian)
            return BinaryPrimitives.ReadInt64LittleEndian(_buffer[Index..(Index += 8)]);
        else
            return BinaryPrimitives.ReadInt64BigEndian(_buffer[Index..(Index += 8)]);
    }

    /// <summary>
    /// Reads 8 unaligned bytes from the buffer as an ulong.
    /// </summary>
    /// <returns>An unsigned long.</returns>
    public ulong ReadUInt64Unaligned() => ReadULongBits(64);

    /// <summary>
    /// Reads 8 unaligned bytes from the buffer as a long.
    /// </summary>
    /// <returns>A long.</returns>
    public long ReadInt64Unaligned() => ReadLongBits(64);

    /// <summary>
    /// Reads a signed integer of variable length.
    /// </summary>
    /// <returns>A long.</returns>
    public long ReadVInt()
    {
        byte dataByte = ReadAlignedByte();
        int negative = dataByte & 1;
        long result = (dataByte >> 1) & 0x3f;
        int bits = 6;

        while ((dataByte & 0x80) != 0)
        {
            dataByte = ReadAlignedByte();
            result |= ((long)dataByte & 0x7f) << bits;
            bits += 7;
        }

        return negative < 0 ? -negative : result;
    }

    /// <summary>
    /// Returns the number of bytes read for a vInt.
    /// </summary>
    /// <returns>A read-only span of bytes.</returns>
    public ReadOnlySpan<byte> ReadBytesForVInt()
    {
        int count = 1;

        byte dataByte = ReadAlignedByte();
        long result = (dataByte >> 1) & 0x3f;
        int bits = 6;

        while ((dataByte & 0x80) != 0)
        {
            count++;
            dataByte = ReadAlignedByte();
            result |= ((long)dataByte & 0x7f) << bits;
            bits += 7;
        }

        Index -= count;

        return ReadAlignedBytes(count);
    }

    /// <summary>
    /// Reads one byte.
    /// </summary>
    /// <returns>The byte at the current index.</returns>
    public byte ReadAlignedByte() => _buffer[Index++];

    /// <summary>
    /// Reads one byte.
    /// </summary>
    /// <returns>A byte.</returns>
    public byte ReadUnalignedByte() => (byte)ReadBits(8);

    /// <summary>
    /// Reads a number of bytes.
    /// </summary>
    /// <param name="count">The number of bytes to read.</param>
    /// <returns>A read-only span of bytes.</returns>
    public ReadOnlySpan<byte> ReadAlignedBytes(int count)
    {
        AlignToByte();

        return _buffer[Index..(Index += count)];
    }

    /// <summary>
    /// Reads a number of bytes.
    /// </summary>
    /// <param name="count">The number of bytes to read.</param>
    /// <returns>A read-only span of bytes.</returns>
    public ReadOnlySpan<byte> ReadUnalignedBytes(int count)
    {
        Span<byte> bytes = new byte[count];
        for (int i = 0; i < count; i++)
        {
            bytes[i] = ReadUnalignedByte();
        }

        return bytes;
    }

    /// <summary>
    /// Reads a number of bits to obtain the number of aligned bytes to read from the read-only span as a UTF-8 string.
    /// </summary>
    /// <param name="numberOfBits">The number of bits to read to obtain the number of bytes to additional read.</param>
    /// <param name="additionalBytesToRead">Adds an additional number of bytes to read.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="numberOfBits"/> is less than 1.</exception>
    /// <returns>A string.</returns>
    public string ReadBlobAsString(int numberOfBits, int? additionalBytesToRead = null)
    {
        if (numberOfBits < 1)
            throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be greater than 0");

        return Encoding.UTF8.GetString(ReadBlob(numberOfBits, additionalBytesToRead));
    }

    /// <summary>
    /// Reads a number of bits from the read-only span as a UTF-8 string.
    /// </summary>
    /// <param name="numberOfBits">The number of bits to read.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="numberOfBits"/> is less than 1 or less than 33.</exception>
    /// <returns>A string.</returns>
    public string ReadStringFromBits(int numberOfBits)
    {
        if (numberOfBits < 1)
            throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be greater than 0");

        if (numberOfBits < 33)
            return GetUTF8StringBaseOnMachineEndiness(BitConverter.GetBytes(ReadBits(numberOfBits)));
        else
            return GetUTF8StringBaseOnMachineEndiness(BitConverter.GetBytes(ReadLongBits(numberOfBits)));
    }

    /// <summary>
    /// Reads a number of aligned bytes from the read-only span as a UTF-8 string.
    /// </summary>
    /// <param name="numberOfBytes">The number of bytes to read.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="numberOfBytes"/> is less than 1.</exception>
    /// <returns>A string.</returns>
    public string ReadStringFromAlignedBytes(int numberOfBytes)
    {
        if (numberOfBytes < 1)
            throw new ArgumentOutOfRangeException(nameof(numberOfBytes), "Number of bytes must be greater than 0");

        ReadOnlySpan<byte> bytes = ReadAlignedBytes(numberOfBytes);
        bytes = bytes.Trim((byte)0);

        if (bytes.Length == 0)
            return string.Empty;

        return GetUTF8String(bytes);
    }

    /// <summary>
    /// Reads a number of unaligned bytes from the read-only span as a UTF-8 string.
    /// </summary>
    /// <param name="numberOfBytes">The number of bytes to read.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="numberOfBytes"/> is less than 1.</exception>
    /// <returns>A string.</returns>
    public string ReadStringFromUnalignedBytes(int numberOfBytes)
    {
        if (numberOfBytes < 1)
            throw new ArgumentOutOfRangeException(nameof(numberOfBytes), "Number of bytes must be greater than 0");

        return ReadStringFromBits(numberOfBytes * 8);
    }

    /// <summary>
    /// Reads a number of bytes.
    /// </summary>
    /// <param name="count">The number of bytes to read.</param>
    /// <returns>A byte.</returns>
    public ReadOnlySpan<byte> ReadBytes(int count) => _buffer[Index..(Index += count)];

    /// <summary>
    /// If in the middle of a byte, moves to the start of the next byte.
    /// </summary>
    public void AlignToByte()
    {
        if ((_bitIndex & 7) > 0)
        {
            _bitIndex = (_bitIndex & 0x7ffffff8) + 8;
        }
    }

    /// <summary>
    /// Sets the index and bit index back to a number of bits.
    /// </summary>
    /// <param name="numberOfBits">Tje number of bits to read back.</param>
    public void BitReversement(int numberOfBits)
    {
        if (numberOfBits < 1)
            return;

        // check if at end of byte already, need to realign the current byte
        int bytePosition = _bitIndex & 7;
        if (bytePosition == 0)
        {
            _currentByte = ReadAlignedByte();
        }

        while (numberOfBits > 0)
        {
            bytePosition = _bitIndex & 7;

            if (bytePosition == 0)
            {
                Index--;
                _currentByte = ((Index - 1) < 0) ? (byte)0 : _buffer[Index - 1];
                bytePosition = 8; // we want to be at the end of the byte
            }

            int bitsToRemove = (bytePosition < numberOfBits) ? bytePosition : numberOfBits;

            _bitIndex -= bitsToRemove;
            numberOfBits -= bitsToRemove;
        }

        bytePosition = _bitIndex & 7;

        if (bytePosition == 0 && Index > 0)
        {
            Index--;
            _currentByte = _buffer[Index];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string GetUTF8StringBaseOnMachineEndiness(byte[] bytes)
    {
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        return Encoding.UTF8.GetString(bytes);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly string GetUTF8String(ReadOnlySpan<byte> bytes)
    {
        Span<byte> buffer = stackalloc byte[bytes.Length];
        bytes.CopyTo(buffer);

        if (EndianType == EndianType.LittleEndian)
            buffer.Reverse();

        return Encoding.UTF8.GetString(buffer);
    }

    private uint GetValueFromBits(int numberOfBits)
    {
        uint value = 0;

        while (numberOfBits > 0)
        {
            int bytePosition = _bitIndex & 7;

            if (bytePosition == 0)
            {
                _currentByte = ReadAlignedByte();
            }

            int bitsLeftInByte = 8 - bytePosition;
            int bitsToRead = (bitsLeftInByte > numberOfBits) ? numberOfBits : bitsLeftInByte;

            value = (value << bitsToRead) | (((uint)_currentByte >> bytePosition) & ((1u << bitsToRead) - 1u));

            _bitIndex += bitsToRead;
            numberOfBits -= bitsToRead;
        }

        return value;
    }

    private ReadOnlySpan<byte> ReadBlob(int numberOfBits, int? additionalBytes = null)
    {
        if (numberOfBits < 1)
            throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be greater than 0");

        int length;

        if (numberOfBits < 33)
            length = (int)ReadBits(numberOfBits);
        else
            length = (int)ReadULongBits(numberOfBits);

        if (additionalBytes.HasValue)
            length += additionalBytes.Value;

        return ReadAlignedBytes(length);
    }

    private ulong GetULongValueFromBits(int numberOfBits)
    {
        ulong value = 0;

        while (numberOfBits > 0)
        {
            int bytePosition = _bitIndex & 7;
            int bitsLeftInByte = 8 - bytePosition;

            if (bytePosition == 0)
            {
                _currentByte = ReadAlignedByte();
            }

            int bitsToRead = (bitsLeftInByte > numberOfBits) ? numberOfBits : bitsLeftInByte;

            value = (value << bitsToRead) | (((uint)_currentByte >> bytePosition) & ((1u << bitsToRead) - 1u));
            _bitIndex += bitsToRead;
            numberOfBits -= bitsToRead;
        }

        return value;
    }

    private long GetLongValueFromBits(int numberOfBits)
    {
        long value = 0;

        while (numberOfBits > 0)
        {
            int bytePosition = _bitIndex & 7;
            int bitsLeftInByte = 8 - bytePosition;

            if (bytePosition == 0)
            {
                _currentByte = ReadAlignedByte();
            }

            int bitsToRead = (bitsLeftInByte > numberOfBits) ? numberOfBits : bitsLeftInByte;

            value = (value << bitsToRead) | (((uint)_currentByte >> bytePosition) & ((1u << bitsToRead) - 1u));
            _bitIndex += bitsToRead;
            numberOfBits -= bitsToRead;
        }

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetBitArray(bool[] bitArray)
    {
        for (int i = 0; i < bitArray.Length; i++)
            bitArray[i] = ReadBoolean();
    }
}
