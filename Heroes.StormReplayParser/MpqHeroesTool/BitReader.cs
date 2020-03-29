using System;
using System.Buffers.Binary;
using System.Text;

namespace Heroes.StormReplayParser.MpqHeroesTool
{
    /// <summary>
    /// Contains the extension methods for Span and ReadOnlySpan.
    /// </summary>
    internal static class BitReader
    {
        private static int _bitIndex;
        private static byte _currentByte;

        /// <summary>
        /// Gets or sets the current byte index.
        /// </summary>
        public static int Index { get; set; } = 0;

        /// <summary>
        /// Gets or sets the <see cref="EndianType"/>.
        /// </summary>
        public static EndianType EndianType { get; set; } = EndianType.BigEndian;

        /// <summary>
        /// Resets the index, bitIndex, and currentByte to 0.
        /// </summary>
        public static void ResetIndex()
        {
            Index = 0;
            _bitIndex = 0;
            _currentByte = 0;
        }

        /// <summary>
        /// Reads up to 32 bits from the read-only span as an uint.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <param name="numberOfBits">The number of bits to read.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static uint ReadBits(this ReadOnlySpan<byte> source, int numberOfBits)
        {
            if (numberOfBits > 32)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be less than 33");
            if (numberOfBits < 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be greater than -1");

            return EndianType == EndianType.BigEndian ? GetValueFromBits(source, numberOfBits) : BinaryPrimitives.ReverseEndianness(GetValueFromBits(source, numberOfBits));
        }

        /// <summary>
        /// Reads up to 64 bits from the read-only span as an ulong.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <param name="numberOfBits">The number of bits to read.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static ulong ReadULongBits(this ReadOnlySpan<byte> source, int numberOfBits)
        {
            if (numberOfBits > 64)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be less than 65");
            if (numberOfBits < 1)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be greater than 0");

            return EndianType == EndianType.BigEndian ? GetULongValueFromBits(source, numberOfBits) : BinaryPrimitives.ReverseEndianness(GetULongValueFromBits(source, numberOfBits));
        }

        /// <summary>
        /// Reads up to 64 bits from the read-only span as an long.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <param name="numberOfBits">The number of bits to read.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static long ReadLongBits(this ReadOnlySpan<byte> source, int numberOfBits)
        {
            if (numberOfBits > 64)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be less than 65");
            if (numberOfBits < 1)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be greater than 0");

            return EndianType == EndianType.BigEndian ? GetLongValueFromBits(source, numberOfBits) : BinaryPrimitives.ReverseEndianness(GetLongValueFromBits(source, numberOfBits));
        }

        /// <summary>
        /// Read a number of bits from the read-only span as an array of booleans.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <param name="numberOfBits">The number of bits to read.</param>
        /// <returns></returns>
        public static bool[] ReadBitArray(this ReadOnlySpan<byte> source, int numberOfBits)
        {
            bool[] bitArray = new bool[numberOfBits];

            for (int i = 0; i < bitArray.Length; i++)
                bitArray[i] = source.ReadBoolean();

            return bitArray;
        }

        /// <summary>
        /// Read a single bit from the read-only span as a boolean.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <returns></returns>
        public static bool ReadBoolean(this ReadOnlySpan<byte> source)
        {
            int bytePosition = _bitIndex & 7;

            if (bytePosition == 0)
            {
                _currentByte = source.ReadAlignedByte();
            }

            bool bit = ((_currentByte >> bytePosition) & 1) == 1;

            _bitIndex++;

            return bit;
        }

        /// <summary>
        /// Reads 2 aligned bytes from the read-only span as an ushort.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <returns></returns>
        public static ushort ReadUInt16Aligned(this ReadOnlySpan<byte> source)
        {
            ushort value;

            if (EndianType == EndianType.LittleEndian)
                value = BinaryPrimitives.ReadUInt16LittleEndian(source.Slice(Index, 2));
            else
                value = BinaryPrimitives.ReadUInt16BigEndian(source.Slice(Index, 2));

            Index += 2;

            return value;
        }

        /// <summary>
        /// Reads 2 aligned bytes from the read-only span as a short.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <returns></returns>
        public static short ReadInt16Aligned(this ReadOnlySpan<byte> source)
        {
            short value;

            if (EndianType == EndianType.LittleEndian)
                value = BinaryPrimitives.ReadInt16LittleEndian(source.Slice(Index, 2));
            else
                value = BinaryPrimitives.ReadInt16BigEndian(source.Slice(Index, 2));

            Index += 2;

            return value;
        }

        /// <summary>
        /// Reads 2 unaligned bytes from the read-only span as a short.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <returns></returns>
        public static short ReadInt16Unaligned(this ReadOnlySpan<byte> source)
        {
            return (short)source.ReadBits(16);
        }

        /// <summary>
        /// Reads 4 aligned bytes from the read-only span as an uint.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <returns></returns>
        public static uint ReadUInt32Aligned(this ReadOnlySpan<byte> source)
        {
            uint value;

            if (EndianType == EndianType.LittleEndian)
                value = BinaryPrimitives.ReadUInt32LittleEndian(source.Slice(Index, 4));
            else
                value = BinaryPrimitives.ReadUInt32BigEndian(source.Slice(Index, 4));

            Index += 4;

            return value;
        }

        /// <summary>
        /// Reads 4 aligned bytes from the read-only span as a int.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <returns></returns>
        public static int ReadInt32Aligned(this ReadOnlySpan<byte> source)
        {
            int value;

            if (EndianType == EndianType.LittleEndian)
                value = BinaryPrimitives.ReadInt32LittleEndian(source.Slice(Index, 4));
            else
                value = BinaryPrimitives.ReadInt32BigEndian(source.Slice(Index, 4));

            Index += 4;

            return value;
        }

        /// <summary>
        /// Reads 4 unaligned bytes from the read-only span as an uint.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <returns></returns>
        public static uint ReadUInt32Unaligned(this ReadOnlySpan<byte> source)
        {
            return source.ReadBits(32);
        }

        /// <summary>
        /// Reads 4 unaligned bytes from the read-only span as an int.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <returns></returns>
        public static int ReadInt32Unaligned(this ReadOnlySpan<byte> source)
        {
            return (int)source.ReadBits(32);
        }

        /// <summary>
        /// Reads 8 aligned bytes from the read-only span as a ulong.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <returns></returns>
        public static ulong ReadUInt64Aligned(this ReadOnlySpan<byte> source)
        {
            ulong value;

            if (EndianType == EndianType.LittleEndian)
                value = BinaryPrimitives.ReadUInt64LittleEndian(source.Slice(Index, 8));
            else
                value = BinaryPrimitives.ReadUInt64BigEndian(source.Slice(Index, 8));

            Index += 8;

            return value;
        }

        /// <summary>
        /// Reads 8 aligned bytes from the read-only span as a long.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <returns></returns>
        public static long ReadInt64Aligned(this ReadOnlySpan<byte> source)
        {
            long value;

            if (EndianType == EndianType.LittleEndian)
                value = BinaryPrimitives.ReadInt64LittleEndian(source.Slice(Index, 8));
            else
                value = BinaryPrimitives.ReadInt64BigEndian(source.Slice(Index, 8));

            Index += 8;

            return value;
        }

        /// <summary>
        /// Reads 8 unaligned bytes from the read-only span as an ulong.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <returns></returns>
        public static ulong ReadUInt64Unaligned(this ReadOnlySpan<byte> source)
        {
            return source.ReadULongBits(64);
        }

        /// <summary>
        /// Reads 8 unaligned bytes from the read-only span as a long.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <returns></returns>
        public static long ReadInt64Unaligned(this ReadOnlySpan<byte> source)
        {
            return source.ReadLongBits(64);
        }

        /// <summary>
        /// Reads a signed integer of variable length.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <returns></returns>
        public static long ReadVInt(this ReadOnlySpan<byte> source)
        {
            byte dataByte = source.ReadAlignedByte();
            int negative = dataByte & 1;
            long result = (dataByte >> 1) & 0x3f;
            int bits = 6;

            while ((dataByte & 0x80) != 0)
            {
                dataByte = source.ReadAlignedByte();
                result |= ((long)dataByte & 0x7f) << bits;
                bits += 7;
            }

            return negative < 0 ? -negative : result;
        }

        /// <summary>
        /// Returns the number of bytes read for a vInt.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <returns></returns>
        public static ReadOnlySpan<byte> ReadBytesForVInt(this ReadOnlySpan<byte> source)
        {
            int count = 1;

            byte dataByte = source.ReadAlignedByte();
            long result = (dataByte >> 1) & 0x3f;
            int bits = 6;

            while ((dataByte & 0x80) != 0)
            {
                count++;
                dataByte = source.ReadAlignedByte();
                result |= ((long)dataByte & 0x7f) << bits;
                bits += 7;
            }

            Index -= count;

            return source.ReadAlignedBytes(count);
        }

        /// <summary>
        /// Reads one byte.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <returns></returns>
        public static byte ReadAlignedByte(this ReadOnlySpan<byte> source)
        {
            byte value = source[Index];
            Index++;

            return value;
        }

        /// <summary>
        /// Reads one byte.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <returns></returns>
        public static byte ReadUnalignedByte(this ReadOnlySpan<byte> source)
        {
            return (byte)source.ReadBits(8);
        }

        /// <summary>
        /// Reads a number of bytes.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns></returns>
        public static ReadOnlySpan<byte> ReadAlignedBytes(this ReadOnlySpan<byte> source, int count)
        {
            AlignToByte();

            ReadOnlySpan<byte> value = source.Slice(Index, count);
            Index += count;

            return value;
        }

        /// <summary>
        /// Reads a number of bytes.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns></returns>
        public static ReadOnlySpan<byte> ReadUnalignedBytes(this ReadOnlySpan<byte> source, int count)
        {
            Span<byte> bytes = new byte[count];
            for (int i = 0; i < count; i++)
            {
                bytes[i] = source.ReadUnalignedByte();
            }

            return bytes;
        }

        /// <summary>
        /// Reads a number of bits from the read-only span as a UTF-8 string.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <param name="numberOfBits">The number of bits to read.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static string ReadBlobAsString(this ReadOnlySpan<byte> source, int numberOfBits)
        {
            if (numberOfBits < 1)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be greater than 0");

            return Encoding.UTF8.GetString(ReadBlob(source, numberOfBits));
        }

        /// <summary>
        /// Reads a number of bits from the read-only span as a UTF-8 string.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <param name="numberOfBits">The number of bits to read.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static string ReadStringFromBits(this ReadOnlySpan<byte> source, int numberOfBits)
        {
            if (numberOfBits < 1)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be greater than 0");

            if (numberOfBits < 33)
            {
                if (EndianType == EndianType.LittleEndian)
                    return Encoding.UTF8.GetString(BitConverter.GetBytes(source.ReadBits(numberOfBits)));
                else
                    return Encoding.UTF8.GetString(BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(source.ReadBits(numberOfBits))));
            }
            else
            {
                if (EndianType == EndianType.LittleEndian)
                    return Encoding.UTF8.GetString(BitConverter.GetBytes(source.ReadLongBits(numberOfBits)));
                else
                    return Encoding.UTF8.GetString(BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(source.ReadLongBits(numberOfBits))));
            }
        }

        /// <summary>
        /// Reads a number of bytes from the read-only span as a UTF-8 string.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <param name="numberOfBytes">The number of bytes to read.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static string ReadStringFromBytes(this ReadOnlySpan<byte> source, int numberOfBytes)
        {
            if (numberOfBytes < 1)
                throw new ArgumentOutOfRangeException(nameof(numberOfBytes), "Number of bytes must be greater than 0");

            ReadOnlySpan<byte> bytes = source.ReadAlignedBytes(numberOfBytes);
            bytes = bytes.Trim((byte)0);

            if (bytes.Length == 0)
                return string.Empty;

            if (EndianType == EndianType.BigEndian)
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
        /// <param name="source">The span of bytes to read.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns></returns>
        public static Span<byte> ReadBytes(this Span<byte> source, int count)
        {
            Span<byte> value = source.Slice(Index, count);
            Index += count;

            return value;
        }

        /// <summary>
        /// If in the middle of a byte, moves to the start of the next byte.
        /// </summary>
        public static void AlignToByte()
        {
            if ((_bitIndex & 7) > 0)
            {
                _bitIndex = (_bitIndex & 0x7ffffff8) + 8;
            }
        }

        private static uint GetValueFromBits(ReadOnlySpan<byte> source, int numberOfBits)
        {
            uint value = 0;

            while (numberOfBits > 0)
            {
                int bytePosition = _bitIndex & 7;
                int bitsLeftInByte = 8 - bytePosition;

                if (bytePosition == 0)
                {
                    _currentByte = source.ReadAlignedByte();
                }

                int bitsToRead = (bitsLeftInByte > numberOfBits) ? numberOfBits : bitsLeftInByte;

                value = (value << bitsToRead) | (((uint)_currentByte >> bytePosition) & ((1u << bitsToRead) - 1u));

                _bitIndex += bitsToRead;
                numberOfBits -= bitsToRead;
            }

            return value;
        }

        private static ReadOnlySpan<byte> ReadBlob(ReadOnlySpan<byte> source, int numberOfBits)
        {
            if (numberOfBits < 1)
                throw new ArgumentOutOfRangeException(nameof(numberOfBits), "Number of bits must be greater than 0");

            if (numberOfBits < 33)
                return ReadAlignedBytes(source, (int)ReadBits(source, numberOfBits));
            else
                return ReadAlignedBytes(source, (int)ReadULongBits(source, numberOfBits));
        }

        private static ulong GetULongValueFromBits(ReadOnlySpan<byte> source, int numberOfBits)
        {
            ulong value = 0;

            while (numberOfBits > 0)
            {
                int bytePosition = _bitIndex & 7;
                int bitsLeftInByte = 8 - bytePosition;

                if (bytePosition == 0)
                {
                    _currentByte = source.ReadAlignedByte();
                }

                int bitsToRead = (bitsLeftInByte > numberOfBits) ? numberOfBits : bitsLeftInByte;

                value = (value << bitsToRead) | (((uint)_currentByte >> bytePosition) & ((1u << bitsToRead) - 1u));
                _bitIndex += bitsToRead;
                numberOfBits -= bitsToRead;
            }

            return value;
        }

        private static long GetLongValueFromBits(ReadOnlySpan<byte> source, int numberOfBits)
        {
            long value = 0;

            while (numberOfBits > 0)
            {
                int bytePosition = _bitIndex & 7;
                int bitsLeftInByte = 8 - bytePosition;

                if (bytePosition == 0)
                {
                    _currentByte = source.ReadAlignedByte();
                }

                int bitsToRead = (bitsLeftInByte > numberOfBits) ? numberOfBits : bitsLeftInByte;

                value = (value << bitsToRead) | (((uint)_currentByte >> bytePosition) & ((1u << bitsToRead) - 1u));
                _bitIndex += bitsToRead;
                numberOfBits -= bitsToRead;
            }

            return value;
        }
    }
}
