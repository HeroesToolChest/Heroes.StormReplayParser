namespace Heroes.StormReplayParser.Player;

/// <summary>
/// Specifies the slot type.
/// </summary>
public enum PlayerSlotType
{
    /// <summary>
    /// Indicates an unknown type of player.
    /// </summary>
    Unknown = -1,

    /// <summary>
    /// Indicates a human slot.
    /// </summary>
    Human = 0,

    /// <summary>
    /// Indicates an AI slot.
    /// </summary>
    Computer = 1,

    /// <summary>
    /// Indicates a closed slot.
    /// </summary>
    Closed = 2,

    /// <summary>
    /// Indicates an opened slot.
    /// </summary>
    Open = 3,
}
