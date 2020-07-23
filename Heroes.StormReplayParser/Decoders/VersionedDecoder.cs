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
        /// <param name="bitReader">The <see cref="BitReader"/> containg the bytes to read.</param>
        public VersionedDecoder(ref BitReader bitReader)
        {
            _dataType = bitReader.ReadAlignedByte();

            switch (_dataType)
            {
                case 0x00: // array
                    ArrayData = new VersionedDecoder[bitReader.ReadVInt()];
                    for (var i = 0; i < ArrayData.Length; i++)
                        ArrayData[i] = new VersionedDecoder(ref bitReader);
                    break;
                case 0x01: // bitblob
                    throw new NotImplementedException();
                case 0x02: // blob
                    _value = bitReader.ReadAlignedBytes((int)bitReader.ReadVInt()).ToArray();
                    break;
                case 0x03: // choice
                    _value = bitReader.ReadBytesForVInt().ToArray();
                    ChoiceData = new VersionedDecoder(ref bitReader);
                    break;
                case 0x04: // optional
                    if (bitReader.ReadAlignedByte() != 0)
                        OptionalData = new VersionedDecoder(ref bitReader);
                    break;
                case 0x05: // struct
                    int size = (int)bitReader.ReadVInt();
                    Structure = new List<VersionedDecoder>(size);

                    for (int i = 0; i < size; i++)
                    {
                        int index = (int)bitReader.ReadVInt();

                        if (index != i)
                        {
                            for (int j = i; j < index; j++)
                            {
                                Structure.Add(null!);
                            }
                        }

                        Structure.Add(new VersionedDecoder(ref bitReader));
                    }

                    break;
                case 0x06: // u8
                    _value = new byte[] { bitReader.ReadAlignedByte() };
                    break;
                case 0x07: // u32
                    _value = bitReader.ReadAlignedBytes(4).ToArray();
                    break;
                case 0x08: // u64
                    _value = bitReader.ReadAlignedBytes(8).ToArray();
                    break;
                case 0x09: // vint
                    _value = bitReader.ReadBytesForVInt().ToArray();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the dictionary containing another version decoder.
        /// </summary>
        public List<VersionedDecoder>? Structure { get; private set; } = null;

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
        /// Gets the value in the current structure as a 32-bit unsigned integer.
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
        /// Gets the value in the current structure as a 64-bit signed integer.
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
                0x05 => Structure != null ? $"{{{string.Join(", ", Structure.Select(i => i?.ToString()))}}}" : null,
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
