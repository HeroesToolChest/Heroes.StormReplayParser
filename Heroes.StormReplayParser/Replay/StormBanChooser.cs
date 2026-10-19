namespace Heroes.StormReplayParser.Replay;

/// <summary>
/// Specifies how a team's ban chooser is determined.
/// </summary>
public enum StormBanChooser
{
    /// <summary>
    /// Indicates an unknown ban chooser.
    /// </summary>
    Unknown,

    /// <summary>
    /// Indicates there is no ban chooser, bans are not being used.
    /// </summary>
    None,

    /// <summary>
    /// Indicates the player with the highest MMR on the team chooses the bans. Used in ranked games. Which player this is, is currently not known.
    /// </summary>
    HighestMmr,

    /// <summary>
    /// Indicates a specific player chooses the bans. Used in custom games.
    /// </summary>
    Player,
}
