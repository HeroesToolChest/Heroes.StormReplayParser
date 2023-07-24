namespace Heroes.StormReplayParser.Decoders;

/// <summary>
/// Contains the information for the version decoder.
/// </summary>
public class VersionedDecoder
{
    private EndianType _endianType;

    /// <summary>
    /// Initializes a new instance of the <see cref="VersionedDecoder"/> class.
    /// </summary>
    /// <param name="bitReader">The <see cref="BitReader"/> containg the bytes to read.</param>
    public VersionedDecoder(ref BitReader bitReader)
    {
        _endianType = bitReader.EndianType;

        DataType = bitReader.ReadAlignedByte();

        switch (DataType)
        {
            case 0x00: // array
                ArrayData = new VersionedDecoder[bitReader.ReadVInt()];
                for (var i = 0; i < ArrayData.Length; i++)
                    ArrayData[i] = new VersionedDecoder(ref bitReader);
                break;
            case 0x01: // bitblob
                throw new NotImplementedException();
            case 0x02: // blob
                Value = bitReader.ReadAlignedBytes((int)bitReader.ReadVInt()).ToArray();
                break;
            case 0x03: // choice
                Value = bitReader.ReadBytesForVInt().ToArray();
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
                Value = new byte[] { bitReader.ReadAlignedByte() };
                break;
            case 0x07: // u32
                Value = bitReader.ReadAlignedBytes(4).ToArray();
                break;
            case 0x08: // u64
                Value = bitReader.ReadAlignedBytes(8).ToArray();
                break;
            case 0x09: // vint
                Value = bitReader.ReadBytesForVInt().ToArray();
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
    /// Gets the current byte value.
    /// </summary>
    public byte[]? Value { get; } = null;

    /// <summary>
    /// Gets the current data type.
    /// </summary>
    public byte DataType { get; }

    /// <summary>
    /// Gets the value in the current structure as a 32-bit unsigned integer.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArithmeticException"></exception>
    /// <returns></returns>
    public uint GetValueAsUInt32()
    {
        return DataType switch
        {
            0x00 => throw new InvalidOperationException("Invalid call, use ArrayData"),
            0x01 => throw new NotImplementedException(),
            0x02 => throw new InvalidOperationException("Invalid call, use GetValueAsString()"),
            0x03 => Get32UIntFromVInt(),
            0x04 => throw new InvalidOperationException("Invalid call, use OptionalData"),
            0x05 => throw new InvalidOperationException("Invalid call, use StructureByIndex"),
            0x06 => Value is not null ? Value[0] : throw new InvalidOperationException("No value available"),
            0x07 => _endianType == EndianType.BigEndian ? BinaryPrimitives.ReadUInt32BigEndian(Value) : BinaryPrimitives.ReadUInt32LittleEndian(Value),
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
        return DataType switch
        {
            0x00 => throw new InvalidOperationException("Invalid call, use ArrayData"),
            0x01 => throw new NotImplementedException(),
            0x02 => throw new InvalidOperationException("Invalid call, use GetValueAsString()"),
            0x03 => Get64IntFromVInt(),
            0x04 => throw new InvalidOperationException("Invalid call, use OptionalData"),
            0x05 => throw new InvalidOperationException("Invalid call, use StructureByIndex"),
            0x06 => throw new ArithmeticException("Incorrect conversion. Use Int32 method instead."),
            0x07 => throw new ArithmeticException("Incorrect conversion. Use Int32 method instead."),
            0x08 => _endianType == EndianType.BigEndian ? (long)BinaryPrimitives.ReadUInt64BigEndian(Value) : (long)BinaryPrimitives.ReadUInt64LittleEndian(Value),
            0x09 => Get64IntFromVInt(),

            _ => throw new NotImplementedException(),
        };
    }

    /// <summary>
    /// Gets the value in the current structure as a string.
    /// </summary>
    /// <returns></returns>
    public string GetValueAsString() => Value is not null ? Encoding.UTF8.GetString(Value) : string.Empty;

    /// <inheritdoc/>
    public override string? ToString()
    {
        return DataType switch
        {
            0x00 => ArrayData is not null ? $"[{string.Join(", ", ArrayData.Select(i => i?.ToString()))}]" : null,
            0x02 => Value is not null ? @$"""{Encoding.UTF8.GetString(Value)}""" : null,
            0x03 => $"Choice: Flag: {BinaryPrimitivesExtensions.ReadVIntLittleEndian(Value)} , Data: {ChoiceData}",
            0x04 => OptionalData?.ToString(),
            0x05 => Structure is not null ? $"{{{string.Join(", ", Structure.Select(i => i?.ToString()))}}}" : null,
            0x06 => Value?[0].ToString(),
            0x07 => _endianType == EndianType.BigEndian ? BinaryPrimitives.ReadUInt32BigEndian(Value).ToString() : BinaryPrimitives.ReadUInt32LittleEndian(Value).ToString(),
            0x08 => _endianType == EndianType.BigEndian ? BinaryPrimitives.ReadUInt64BigEndian(Value).ToString() : BinaryPrimitives.ReadUInt64LittleEndian(Value).ToString(),
            0x09 => BinaryPrimitivesExtensions.ReadVIntLittleEndian(Value).ToString(),

            _ => string.Empty,
        };
    }

    private uint Get32UIntFromVInt()
    {
        uint value = (uint)BinaryPrimitivesExtensions.ReadVIntLittleEndian(Value, out int size);
        if (size > 4)
            throw new ArithmeticException($"Incorrect conversion for VInt (has byte size of {size}. Use Int64 method instead.");

        return value;
    }

    private long Get64IntFromVInt()
    {
        long value = BinaryPrimitivesExtensions.ReadVIntLittleEndian(Value, out int _);

        return value;
    }
}
