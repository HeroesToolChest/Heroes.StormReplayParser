using System;
using System.Buffers.Binary;
using System.Text;

namespace Heroes.StormReplayParser.MpqHeroesTool
{
    public ref struct BitReader
    {
        private readonly ReadOnlySpan<byte> _buffer;
        private readonly EndianType _endianType;
        private int _bitIndex;
        private byte _currentByte;

        public BitReader(ReadOnlySpan<byte> buffer, EndianType endianType)
        {
            Index = 0;
            _bitIndex = 0;
            _currentByte = 0;
            _buffer = buffer;
            _endianType = endianType;
        }

        public readonly int Length => _buffer.Length;

        public int Index { get; set; }

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

            return _endianType == EndianType.BigEndian ? GetValueFromBits(numberOfBits) : BinaryPrimitives.ReverseEndianness(GetValueFromBits(numberOfBits));
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

            return _endianType == EndianType.BigEndian ? GetULongValueFromBits(numberOfBits) : BinaryPrimitives.ReverseEndianness(GetULongValueFromBits(numberOfBits));
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

            return _endianType == EndianType.BigEndian ? GetLongValueFromBits(numberOfBits) : BinaryPrimitives.ReverseEndianness(GetLongValueFromBits(numberOfBits));
        }

        /// <summary>
        /// Read a number of bits from the buffer as an array of booleans.
        /// </summary>
        /// <param name="numberOfBits">The number of bits to read.</param>
        /// <returns>An array of booleans.</returns>
        public bool[] ReadBitArray(uint numberOfBits)
        {
            bool[] bitArray = new bool[numberOfBits];

            return SetBitArray(bitArray);
        }

        /// <summary>
        /// Read a number of bits from the read-only span as an array of booleans.
        /// </summary>
        /// <param name="numberOfBits">The number of bits to read.</param>
        /// <returns>An array of booleans.</returns>
        public bool[] ReadBitArray(int numberOfBits)
        {
            bool[] bitArray = new bool[numberOfBits];

            return SetBitArray(bitArray);
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
            ushort value;

            if (_endianType == EndianType.LittleEndian)
                value = BinaryPrimitives.ReadUInt16LittleEndian(_buffer.Slice(Index, 2));
            else
                value = BinaryPrimitives.ReadUInt16BigEndian(_buffer.Slice(Index, 2));

            Index += 2;

            return value;
        }

        /// <summary>
        /// Reads 2 aligned bytes from the buffer as a short.
        /// </summary>
        /// <returns>A short.</returns>
        public short ReadInt16Aligned()
        {
            short value;

            if (_endianType == EndianType.LittleEndian)
                value = BinaryPrimitives.ReadInt16LittleEndian(_buffer.Slice(Index, 2));
            else
                value = BinaryPrimitives.ReadInt16BigEndian(_buffer.Slice(Index, 2));

            Index += 2;

            return value;
        }

        /// <summary>
        /// Reads 2 unaligned bytes from the buffer as a short.
        /// </summary>
        /// <returns>A short.</returns>
        public short ReadInt16Unaligned()
        {
            return (short)ReadBits(16);
        }

        /// <summary>
        /// Reads 4 aligned bytes from the buffer as an uint.
        /// </summary>
        /// <returns>An unsigned interger.</returns>
        public uint ReadUInt32Aligned()
        {
            uint value;

            if (_endianType == EndianType.LittleEndian)
                value = BinaryPrimitives.ReadUInt32LittleEndian(_buffer.Slice(Index, 4));
            else
                value = BinaryPrimitives.ReadUInt32BigEndian(_buffer.Slice(Index, 4));

            Index += 4;

            return value;
        }

        /// <summary>
        /// Reads 4 aligned bytes from the buffer as a int.
        /// </summary>
        /// <returns>An integer.</returns>
        public int ReadInt32Aligned()
        {
            int value;

            if (_endianType == EndianType.LittleEndian)
                value = BinaryPrimitives.ReadInt32LittleEndian(_buffer.Slice(Index, 4));
            else
                value = BinaryPrimitives.ReadInt32BigEndian(_buffer.Slice(Index, 4));

            Index += 4;

            return value;
        }

        /// <summary>
        /// Reads 4 unaligned bytes from the buffer as an uint.
        /// </summary>
        /// <returns>An unsigned integer.</returns>
        public uint ReadUInt32Unaligned()
        {
            return ReadBits(32);
        }

        /// <summary>
        /// Reads 4 unaligned bytes from the buffer as an int.
        /// </summary>
        /// <returns>An integer.</returns>
        public int ReadInt32Unaligned()
        {
            return (int)ReadBits(32);
        }

        /// <summary>
        /// Reads 8 aligned bytes from the buffer as a ulong.
        /// </summary>
        /// <returns>An unsigned long.</returns>
        public ulong ReadUInt64Aligned()
        {
            ulong value;

            if (_endianType == EndianType.LittleEndian)
                value = BinaryPrimitives.ReadUInt64LittleEndian(_buffer.Slice(Index, 8));
            else
                value = BinaryPrimitives.ReadUInt64BigEndian(_buffer.Slice(Index, 8));

            Index += 8;

            return value;
        }

        /// <summary>
        /// Reads 8 aligned bytes from the buffer as a long.
        /// </summary>
        /// <returns>A long.</returns>
        public long ReadInt64Aligned()
        {
            long value;

            if (_endianType == EndianType.LittleEndian)
                value = BinaryPrimitives.ReadInt64LittleEndian(_buffer.Slice(Index, 8));
            else
                value = BinaryPrimitives.ReadInt64BigEndian(_buffer.Slice(Index, 8));

            Index += 8;

            return value;
        }

        /// <summary>
        /// Reads 8 unaligned bytes from the buffer as an ulong.
        /// </summary>
        /// <returns>An unsigned long.</returns>
        public ulong ReadUInt64Unaligned()
        {
            return ReadULongBits(64);
        }

        /// <summary>
        /// Reads 8 unaligned bytes from the buffer as a long.
        /// </summary>
        /// <returns>A long.</returns>
        public long ReadInt64Unaligned()
        {
            return ReadLongBits(64);
        }

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
        public byte ReadAlignedByte()
        {
            byte currentByte = _buffer[Index];
            Index++;

            return currentByte;
        }

        /// <summary>
        /// Reads one byte.
        /// </summary>
        /// <returns>A byte.</returns>
        public byte ReadUnalignedByte()
        {
            return (byte)ReadBits(8);
        }

        /// <summary>
        /// Reads a number of bytes.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>A read-only span of bytes.</returns>
        public ReadOnlySpan<byte> ReadAlignedBytes(int count)
        {
            AlignToByte();

            ReadOnlySpan<byte> value = _buffer.Slice(Index, count);
            Index += count;

            return value;
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
        /// Reads a number of bits from the read-only span as a UTF-8 string.
        /// </summary>
        /// <param name="numberOfBits">The number of bits to read.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="numberOfBits"/> is less than 1.</exception>
        /// <returns>A string.</returns>
        public string ReadBlobAsString(int numberOfBits)
        {
            if (numberOfBits < 1)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be greater than 0");

            return Encoding.UTF8.GetString(ReadBlob(numberOfBits));
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
            {
                if (_endianType == EndianType.LittleEndian)
                    return Encoding.UTF8.GetString(BitConverter.GetBytes(ReadBits(numberOfBits)));
                else
                    return Encoding.UTF8.GetString(BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(ReadBits(numberOfBits))));
            }
            else
            {
                if (_endianType == EndianType.LittleEndian)
                    return Encoding.UTF8.GetString(BitConverter.GetBytes(ReadLongBits(numberOfBits)));
                else
                    return Encoding.UTF8.GetString(BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(ReadLongBits(numberOfBits))));
            }
        }

        /// <summary>
        /// Reads a number of bytes from the read-only span as a UTF-8 string.
        /// </summary>
        /// <param name="numberOfBytes">The number of bytes to read.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="numberOfBytes"/> is less than 1.</exception>
        /// <returns>A string.</returns>
        public string ReadStringFromBytes(int numberOfBytes)
        {
            if (numberOfBytes < 1)
                throw new ArgumentOutOfRangeException(nameof(numberOfBytes), "Number of bytes must be greater than 0");

            ReadOnlySpan<byte> bytes = ReadAlignedBytes(numberOfBytes);
            bytes = bytes.Trim((byte)0);

            if (bytes.Length == 0)
                return string.Empty;

            if (_endianType == EndianType.BigEndian)
            {
                return Encoding.UTF8.GetString(bytes);
            }
            else
            {
                Span<byte> buffer = stackalloc byte[bytes.Length];
                bytes.CopyTo(buffer);
                buffer.Reverse();

                return Encoding.UTF8.GetString(buffer);
            }
        }

        /// <summary>
        /// Reads a number of bytes.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>A byte.</returns>
        public ReadOnlySpan<byte> ReadBytes(int count)
        {
            ReadOnlySpan<byte> value = _buffer.Slice(Index, count);
            Index += count;

            return value;
        }

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

        private uint GetValueFromBits(int numberOfBits)
        {
            uint value = 0;

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

        private ReadOnlySpan<byte> ReadBlob(int numberOfBits)
        {
            if (numberOfBits < 1)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be greater than 0");

            if (numberOfBits < 33)
                return ReadAlignedBytes((int)ReadBits(numberOfBits));
            else
                return ReadAlignedBytes((int)ReadULongBits(numberOfBits));
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

        private bool[] SetBitArray(bool[] bitArray)
        {
            for (int i = 0; i < bitArray.Length; i++)
                bitArray[i] = ReadBoolean();

            return bitArray;
        }
    }
}
