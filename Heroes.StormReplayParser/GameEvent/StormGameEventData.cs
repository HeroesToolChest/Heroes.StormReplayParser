namespace Heroes.StormReplayParser.GameEvent;

/// <summary>
/// Contains the information for a storm game event.
/// </summary>
public class StormGameEventData
{
    private readonly ulong _value;
    private readonly object? _reference;

    /// <summary>
    /// Initializes a new instance of the <see cref="StormGameEventData"/> class.
    /// </summary>
    /// <param name="value">Sets a boolean value.</param>
    public StormGameEventData(bool value)
    {
        DataType = StormGameEventDataType.Bool;
        _value = value ? 1UL : 0UL;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StormGameEventData"/> class.
    /// </summary>
    /// <param name="value">Sets an integer.</param>
    public StormGameEventData(int value)
    {
        DataType = StormGameEventDataType.Integer32;
        _value = (uint)value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StormGameEventData"/> class.
    /// </summary>
    /// <param name="value">Sets an unsigned integer.</param>
    public StormGameEventData(uint value)
    {
        DataType = StormGameEventDataType.UnsignedInteger32;
        _value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StormGameEventData"/> class.
    /// </summary>
    /// <param name="value">Sets a 64-bit unsigned integer.</param>
    public StormGameEventData(ulong value)
    {
        DataType = StormGameEventDataType.UnsignedInteger64;
        _value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StormGameEventData"/> class.
    /// </summary>
    /// <param name="value">Sets a string.</param>
    public StormGameEventData(string? value)
    {
        DataType = StormGameEventDataType.Blob;
        _reference = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StormGameEventData"/> class.
    /// </summary>
    /// <param name="structure">Sets a structure.</param>
    public StormGameEventData(StormDataStructure<StormGameEventData>? structure)
    {
        DataType = StormGameEventDataType.Structure;
        _reference = structure;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StormGameEventData"/> class.
    /// </summary>
    /// <param name="array">Sets an array.</param>
    public StormGameEventData(StormGameEventData[]? array)
    {
        DataType = StormGameEventDataType.Array;
        _reference = array;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StormGameEventData"/> class.
    /// </summary>
    /// <param name="bitArray">Sets an array of booleans.</param>
    public StormGameEventData(bool[] bitArray)
    {
        DataType = StormGameEventDataType.BitArray;
        _reference = bitArray;
    }

    /// <summary>
    /// Gets the data type.
    /// </summary>
    public StormGameEventDataType DataType { get; }

    /// <summary>
    /// Gets the structure.
    /// </summary>
    public StormDataStructure<StormGameEventData>? Structure => DataType == StormGameEventDataType.Structure ? (StormDataStructure<StormGameEventData>?)_reference : null;

    /// <summary>
    /// Gets the boolean value.
    /// </summary>
    public bool? Boolean => DataType == StormGameEventDataType.Bool ? _value != 0 : null;

    /// <summary>
    /// Gets the string value.
    /// </summary>
    public string? Blob => DataType == StormGameEventDataType.Blob ? (string?)_reference : null;

    /// <summary>
    /// Gets the 32-bit signed integer.
    /// </summary>
    public int? Integer32 => DataType == StormGameEventDataType.Integer32 ? (int)_value : null;

    /// <summary>
    /// Gets the 32-bit unsigned integer.
    /// </summary>
    public uint? UnsignedInteger32 => DataType == StormGameEventDataType.UnsignedInteger32 ? (uint)_value : null;

    /// <summary>
    /// Gets the 64-bit unsigned integer.
    /// </summary>
    public ulong? UnsignedInteger64 => DataType == StormGameEventDataType.UnsignedInteger64 ? _value : null;

    /// <summary>
    /// Gets the array.
    /// </summary>
    public StormGameEventData[]? Array => DataType == StormGameEventDataType.Array ? (StormGameEventData[]?)_reference : null;

    /// <summary>
    /// Gets the array of boooleans.
    /// </summary>
    public bool[]? BitArray => DataType == StormGameEventDataType.BitArray ? (bool[]?)_reference : null;

    /// <inheritdoc/>
    public override string? ToString()
    {
        return DataType switch
        {
            StormGameEventDataType.Bool => Boolean?.ToString(),
            StormGameEventDataType.Integer32 => Integer32?.ToString(),
            StormGameEventDataType.UnsignedInteger32 => UnsignedInteger32?.ToString(),
            StormGameEventDataType.UnsignedInteger64 => UnsignedInteger64?.ToString(),
            StormGameEventDataType.Blob => Blob is not null ? @$"""{Blob}""" : null,
            StormGameEventDataType.Array => Array is not null ? $"[{string.Join(", ", Array.Select(i => i?.ToString()))}]" : null,
            StormGameEventDataType.Structure => Structure is not null ? $"{{{string.Join(", ", Structure.Select(i => i?.ToString()))}}}" : null,
            StormGameEventDataType.BitArray => BitArray is not null ? $"[{string.Join(", ", BitArray.Select(i => i.ToString()))}]" : null,

            _ => string.Empty,
        };
    }

    /// <summary>
    /// Returns the current object as a json string.
    /// </summary>
    /// <returns>A json string.</returns>
    public string ToJson()
    {
        return DataType switch
        {
            StormGameEventDataType.Bool => $"\"{Boolean?.ToString()}\"",
            StormGameEventDataType.Integer32 => $"\"{Integer32?.ToString()}\"",
            StormGameEventDataType.UnsignedInteger32 => $"\"{UnsignedInteger32?.ToString()}\"",
            StormGameEventDataType.UnsignedInteger64 => $"\"{UnsignedInteger64?.ToString()}\"",
            StormGameEventDataType.Blob => Blob is not null ? $"\"{Blob}\"" : "null",
            StormGameEventDataType.Array => Array is not null ? GetArrayAsJson(Array) : "null",
            StormGameEventDataType.Structure => Structure is not null ? GetStructureAsJson(Structure) : "null",
            StormGameEventDataType.BitArray => BitArray is not null ? GetBitArrayAsJson(BitArray) : "null",

            _ => string.Empty,
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string GetArrayAsJson(StormGameEventData[] arrayData)
    {
        StringBuilder sb = new("[{");

        for (int i = 0; i < arrayData.Length; i++)
        {
            sb.Append($"\"{i}\": {arrayData[i].ToJson()}");

            if (i < arrayData.Length - 1)
                sb.Append(',');
        }

        sb.Append("}]");

        return sb.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string GetStructureAsJson(StormDataStructure<StormGameEventData> structure)
    {
        StringBuilder sb = new("{");

        for (int i = 0; i < structure.Count; i++)
        {
            sb.Append($"\"{i}\": {structure[i].ToJson()}");

            if (i < structure.Count - 1)
                sb.Append(',');
        }

        sb.Append('}');

        return sb.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string GetBitArrayAsJson(bool[] bitArray)
    {
        StringBuilder sb = new("[");

        for (int i = 0; i < bitArray.Length; i++)
        {
            sb.Append($"{bitArray[i]}");

            if (i < bitArray.Length - 1)
                sb.Append(',');
        }

        sb.Append(']');

        return sb.ToString();
    }
}
