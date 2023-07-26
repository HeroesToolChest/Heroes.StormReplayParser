namespace Heroes.StormReplayParser.Replay;

/// <summary>
/// Indicates the game privacy setting.
/// </summary>
public enum StormGamePrivacy
{
    /// <summary>
    /// Indicates an unknown privacy type.
    /// </summary>
    Unknown = 0,

    /// <summary>
    ///  Indicates that the match will appear in match history.
    /// </summary>
    Normal = 1,

    /// <summary>
    /// Indicates that the match will not appear in match history.
    /// </summary>
    NoMatchHistory = 2,
}
