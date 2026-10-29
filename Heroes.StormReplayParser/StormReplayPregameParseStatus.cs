namespace Heroes.StormReplayParser;

/// <summary>
/// Specifies the status of the parse attempt.
/// </summary>
public enum StormReplayPregameParseStatus
{
    /// <summary>
    /// Indicates an unknown parse.
    /// <para>
    /// This is not returned by the parser.
    /// </para>
    /// </summary>
    Unknown = -1,

    /// <summary>
    /// Indicates a successful parse.
    /// </summary>
    Success = 0,

    /// <summary>
    /// Indicates an exception occurred while opening or parsing the battlelobby file, which includes a file that is not a battlelobby file.
    /// The exception is in <see cref="StormReplayPregameResult.Exception"/>.
    /// </summary>
    Exception = 10,

    /// <summary>
    /// Indicates a successful parse and that the battlelobby file is from the PTR region, which is when a player's <see cref="ToonHandle.Region"/> is 90 or higher.
    /// <para>
    /// This is only used when <see cref="ParsePregameOptions.AllowPTR"/> is <see langword="false"/>.
    /// </para>
    /// </summary>
    PTRRegion = 15,
}
