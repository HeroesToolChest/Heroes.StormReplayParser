namespace Heroes.StormReplayParser.Player;

/// <summary>
/// Specifies the type of player.
/// </summary>
public enum PlayerType
{
    /// <summary>
    /// Indicates an unknown type of player.
    /// </summary>
    Unknown = -1,

    /// <summary>
    /// Indicates a human player.
    /// </summary>
    Human = 0,

    /// <summary>
    /// Indicates an AI player.
    /// </summary>
    Computer = 1,

    /// <summary>
    /// Indicates a non-player who is spectating.
    /// </summary>
    Observer = 2,
}
