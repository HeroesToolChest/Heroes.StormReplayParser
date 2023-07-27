namespace Heroes.StormReplayParser;

/// <summary>
/// Specifies the status of the parse attempt.
/// </summary>
public enum StormReplayPregameParseStatus
{
    /// <summary>
    /// Indicates an unknown parse.
    /// </summary>
    Unknown = -1,

    /// <summary>
    /// Indicates a successful parse.
    /// </summary>
    Success = 0,

    /// <summary>
    /// Indicates an exception occured while parsing.
    /// </summary>
    Exception = 10,

    /// <summary>
    /// Indicates a successful parse and that the replay is from the PTR region.
    /// </summary>
    PTRRegion = 15,
}
