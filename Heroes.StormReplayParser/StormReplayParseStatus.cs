namespace Heroes.StormReplayParser;

/// <summary>
/// Specifies the status of the parse attempt.
/// </summary>
public enum StormReplayParseStatus
{
    /// <summary>
    /// Indicates an unknown parse.
    /// <para>
    /// This is not returned by the parser.
    /// </para>
    /// </summary>
    Unknonwn = -1,

    /// <summary>
    /// Indicates a successful parse.
    /// </summary>
    Success = 0,

    /// <summary>
    /// Indicates an incomplete parse, where the game did not finish, such as when there is no winner or the game is shorter than 45 seconds.
    /// <para>
    /// This is also the status of technical alpha replays (before October 6, 2014) and replays without a timestamp, which are not fully parsed.
    /// </para>
    /// </summary>
    Incomplete = 2,

    /// <summary>
    /// Indicates the replay is a Try Mode game, which has only one player.
    /// </summary>
    TryMeMode = 5,

    /// <summary>
    /// Indicates the replay was parsed, but it is not a regular 5 vs 5 game, such as a game without 10 players or 5 winners, or with an unknown game mode.
    /// </summary>
    UnexpectedResult = 9,

    /// <summary>
    /// Indicates an exception occurred while opening or parsing the replay. The exception is in <see cref="StormReplayResult.Exception"/>.
    /// </summary>
    Exception = 10,

    /// <summary>
    /// Indicates the replay was not found.
    /// <para>
    /// This is not returned by the parser. A replay that is not found has the status <see cref="Exception"/>.
    /// </para>
    /// </summary>
    FileNotFound = 11,

    /// <summary>
    /// Indicates the replay is from before the technical alpha wipe, which is a build older than 32455.
    /// </summary>
    PreAlphaWipe = 13,

    /// <summary>
    /// Indicates the replay size is too large.
    /// <para>
    /// This is not returned by the parser.
    /// </para>
    /// </summary>
    FileSizeTooLarge = 14,

    /// <summary>
    /// Indicates a successful parse and that the replay is from the PTR region, which is when a player's <see cref="ToonHandle.Region"/> is 90 or higher.
    /// <para>
    /// This is only used when <see cref="ParseOptions.AllowPTR"/> is <see langword="false"/>.
    /// </para>
    /// </summary>
    PTRRegion = 15,
}
