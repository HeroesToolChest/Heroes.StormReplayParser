namespace Heroes.StormReplayParser.Replay;

/// <summary>
/// Indicates the ban mode setting.
/// </summary>
public enum StormBanMode
{
    /// <summary>
    /// Indicates an unknown ban mode.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Indicates there are no bans.
    /// </summary>
    NotUsingBans = 1,

    /// <summary>
    /// Indicates a one ban mode.
    /// </summary>
    OneBan = 2,

    /// <summary>
    /// Indicates a two ban mode.
    /// </summary>
    TwoBan = 3,

    /// <summary>
    /// Indicates a mid ban mode.
    /// </summary>
    MidBan = 4,

    /// <summary>
    /// Indicates a three ban mode.
    /// </summary>
    ThreeBan = 5,
}
