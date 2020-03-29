using Heroes.StormReplayParser.MpqHeroesTool;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heroes.StormReplayParser.Decoders
{
    /// <summary>
    /// Contains the information for the version decoder.
    /// </summary>
    public class VersionedDecoder
    {
        private readonly byte _dataType;
        private readonly byte[]? _value = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionedDecoder"/> class.
        /// </summary>
        /// <param name="source">The read-only span of bytes to read.</param>
        public VersionedDecoder(ReadOnlySpan<byte> source)
        {
            _dataType = source.ReadAlignedByte();

            switch (_dataType)
            {
                case 0x00: // array
                    ArrayData = new VersionedDecoder[source.ReadVInt()];
                    for (var i = 0; i < ArrayData.Length; i++)
                        ArrayData[i] = new VersionedDecoder(source);
                    break;
                case 0x01: // bitblob
                    throw new NotImplementedException();
                case 0x02: // blob
                    _value = source.ReadAlignedBytes((int)source.ReadVInt()).ToArray();

                    break;
                case 0x03: // choice
                    _value = source.ReadBytesForVInt().ToArray();
                    ChoiceData = new VersionedDecoder(source);
                    break;
                case 0x04: // optional
                    if (source.ReadAlignedByte() != 0)
                        OptionalData = new VersionedDecoder(source);
                    break;
                case 0x05: // struct
                    StructureByIndex = new Dictionary<int, VersionedDecoder>();
                    int size = (int)source.ReadVInt();

                    for (int i = 0; i < size; i++)
                    {
                        StructureByIndex[(int)source.ReadVInt()] = new VersionedDecoder(source);
                    }

                    break;
                case 0x06: // u8
                    _value = new byte[] { source.ReadAlignedByte() };
                    break;
                case 0x07: // u32
                    _value = source.ReadAlignedBytes(4).ToArray();
                    break;
                case 0x08: // u64
                    _value = source.ReadAlignedBytes(8).ToArray();
                    break;
                case 0x09: // vint
                    _value = source.ReadBytesForVInt().ToArray();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the dictionary containing another version decoder.
        /// </summary>
        public Dictionary<int, VersionedDecoder>? StructureByIndex { get; private set; } = null;

        /// <summary>
        /// Gets the optional data.
        /// </summary>
        public VersionedDecoder? OptionalData { get; private set; } = null;

        /// <summary>
        /// Gets the choice data.
        /// </summary>
        public VersionedDecoder? ChoiceData { get; private set; } = null;

        /// <summary>
        /// Gets the array data.
        /// </summary>
        public VersionedDecoder[]? ArrayData { get; private set; } = null;

        /// <summary>
        /// Gets the value in the current structure as a signed 32-bit unsigned integer.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArithmeticException"></exception>
        /// <returns></returns>
        public uint GetValueAsUInt32()
        {
            return _dataType switch
            {
                0x00 => throw new InvalidOperationException("Invalid call, use ArrayData"),
                0x01 => throw new NotImplementedException(),
                0x02 => throw new InvalidOperationException("Invalid call, use GetValueAsString()"),
                0x03 => Get32UIntFromVInt(),
                0x04 => throw new InvalidOperationException("Invalid call, use OptionalData"),
                0x05 => throw new InvalidOperationException("Invalid call, use StructureByIndex"),
                0x06 => _value != null ? _value[0] : throw new InvalidOperationException("No value available"),
                0x07 => BinaryPrimitives.ReadUInt32LittleEndian(_value),
                0x08 => throw new ArithmeticException("Incorrect conversion. Use Int64 method instead."),
                0x09 => Get32UIntFromVInt(),

                _ => throw new NotImplementedException(),
            };
        }

        /// <summary>
        /// Gets the value in the current structure as a signed 64-bit integer.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArithmeticException"></exception>
        /// <returns></returns>
        public long GetValueAsInt64()
        {
            return _dataType switch
            {
                0x00 => throw new InvalidOperationException("Invalid call, use ArrayData"),
                0x01 => throw new NotImplementedException(),
                0x02 => throw new InvalidOperationException("Invalid call, use GetValueAsString()"),
                0x03 => Get64IntFromVInt(),
                0x04 => throw new InvalidOperationException("Invalid call, use OptionalData"),
                0x05 => throw new InvalidOperationException("Invalid call, use StructureByIndex"),
                0x06 => throw new ArithmeticException("Incorrect conversion. Use Int32 method instead."),
                0x07 => throw new ArithmeticException("Incorrect conversion. Use Int32 method instead."),
                0x08 => (long)BinaryPrimitives.ReadUInt64LittleEndian(_value),
                0x09 => Get64IntFromVInt(),

                _ => throw new NotImplementedException(),
            };
        }

        /// <summary>
        /// Gets the value in the current structure as a string.
        /// </summary>
        /// <returns></returns>
        public string GetValueAsString() => _value != null ? Encoding.UTF8.GetString(_value) : string.Empty;

        /// <inheritdoc/>
        public override string? ToString()
        {
            return _dataType switch
            {
                0x00 => ArrayData != null ? $"[{string.Join(", ", ArrayData.Select(i => i?.ToString()))}]" : null,
                0x02 => _value != null ? @$"""{Encoding.UTF8.GetString(_value)}""" : null,
                0x03 => $"Choice: Flag: {BinaryPrimitivesExtensions.ReadVIntLittleEndian(_value)} , Data: {ChoiceData}",
                0x04 => OptionalData?.ToString(),
                0x05 => StructureByIndex != null ? $"{{{string.Join(", ", StructureByIndex.Values.Select(i => i?.ToString()))}}}" : null,
                0x06 => _value?[0].ToString(),
                0x07 => BinaryPrimitives.ReadUInt32LittleEndian(_value).ToString(),
                0x08 => BinaryPrimitives.ReadUInt64LittleEndian(_value).ToString(),
                0x09 => BinaryPrimitivesExtensions.ReadVIntLittleEndian(_value).ToString(),

                _ => string.Empty,
            };
        }

        private uint Get32UIntFromVInt()
        {
            uint value = (uint)BinaryPrimitivesExtensions.ReadVIntLittleEndian(_value, out int size);
            if (size > 4)
                throw new ArithmeticException($"Incorrect conversion for VInt (has byte size of {size}. Use Int64 method instead.");

            return value;
        }

        private long Get64IntFromVInt()
        {
            long value = BinaryPrimitivesExtensions.ReadVIntLittleEndian(_value, out int _);

            return value;
        }
    }
}
