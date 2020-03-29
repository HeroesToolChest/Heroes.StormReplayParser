using System;

namespace Heroes.StormReplayParser.Decoders
{
    /// <summary>
    /// Contains additional binary primitive methods.
    /// </summary>
    public static class BinaryPrimitivesExtensions
    {
        /// <summary>
        /// Reads a signed integer of variable length of a read-only span of bytes, as little endian.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <returns>The little endian value.</returns>
        public static long ReadVIntLittleEndian(ReadOnlySpan<byte> source)
        {
            int negative = source[0] & 1;
            long result = (source[0] >> 1) & 0x3f;
            int bits = 6;

            for (int i = 1; i < source.Length && (source[i - 1] & 0x80) != 0; i++)
            {
                result |= ((long)source[i] & 0x7f) << bits;
                bits += 7;
            }

            return negative < 0 ? -negative : result;
        }

        /// <summary>
        /// Reads a signed integer of variable length of a read-only span of bytes, as little endian.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        /// <param name="size">The number of bytes read.</param>
        /// <returns>The little endian value.</returns>
        public static long ReadVIntLittleEndian(ReadOnlySpan<byte> source, out int size)
        {
            int negative = source[0] & 1;
            long result = (source[0] >> 1) & 0x3f;
            int bits = 6;

            for (int i = 1; i < source.Length && (source[i - 1] & 0x80) != 0; i++)
            {
                result |= ((long)source[i] & 0x7f) << bits;
                bits += 7;
            }

            size = (bits / 7) + 1;

            return negative < 0 ? -negative : result;
        }
    }
}
