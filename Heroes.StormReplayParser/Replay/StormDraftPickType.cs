namespace Heroes.StormReplayParser.Replay;

/// <summary>
/// Specifies the type of draft pick.
/// </summary>
public enum StormDraftPickType
{
    /// <summary>
    /// Indicates a banned type.
    /// </summary>
    Banned = 0,

    /// <summary>
    /// Indicates a picked type.
    /// </summary>
    Picked = 1,

    /// <summary>
    /// Indicates a swapped type.
    /// </summary>
    Swapped = 2,
}
