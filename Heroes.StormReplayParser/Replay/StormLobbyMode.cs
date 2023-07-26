namespace Heroes.StormReplayParser.Replay;

/// <summary>
/// Indicates the lobby mode setting.
/// </summary>
public enum StormLobbyMode
{
    /// <summary>
    /// Indicates an unknown lobby mode.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Indicates a standard lobby mode.
    /// </summary>
    Standard = 1,

    /// <summary>
    /// Indicates a draft lobby mode.
    /// </summary>
    Draft = 2,

    /// <summary>
    /// Indicates a tournament draft lobby mode.
    /// </summary>
    TournamentDraft = 3,
}
