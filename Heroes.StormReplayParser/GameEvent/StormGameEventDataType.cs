namespace Heroes.StormReplayParser.GameEvent;

/// <summary>
/// Specifies the game event data type.
/// </summary>
public enum StormGameEventDataType
{
    /// <summary>
    /// Indicates an array.
    /// </summary>
    Array,

    /// <summary>
    /// Indicates an array of booleans.
    /// </summary>
    BitArray,

    /// <summary>
    /// Indicates a string.
    /// </summary>
    Blob,

    /// <summary>
    /// Indicates a boolean.
    /// </summary>
    Bool,

    /// <summary>
    /// Indicates a 32-bit signed integer.
    /// </summary>
    Integer32,

    /// <summary>
    /// Indicates a 32-bit unsigned integer.
    /// </summary>
    UnsignedInteger32,

    /// <summary>
    /// Indicates a 64-bit unsigned integer.
    /// </summary>
    UnsignedInteger64,

    /// <summary>
    /// Indicates a structure.
    /// </summary>
    Structure,
}
