namespace Heroes.StormReplayParser.Replay;

/// <summary>
/// Indicates the first draft team setting.
/// </summary>
public enum StormFirstDraftTeam
{
    /// <summary>
    /// Indicates an unknown first draft team mode.
    /// </summary>
    Unknown,

    /// <summary>
    /// Indicates a coin toss will determine the first draft team.
    /// </summary>
    CoinToss,

    /// <summary>
    /// Indicates team 1 (blue) will go first.
    /// </summary>
    Team1,

    /// <summary>
    /// Indicates team 2 (red) will go first.
    /// </summary>
    Team2,
}
