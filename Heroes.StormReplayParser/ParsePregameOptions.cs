namespace Heroes.StormReplayParser;

/// <summary>
/// Contains the parsing options.
/// </summary>
public class ParsePregameOptions
{
    /// <summary>
    /// Gets the default parse options. Allows ptr.
    /// </summary>
    public static ParsePregameOptions DefaultParsing => new();

    /// <summary>
    /// Gets or sets a value indicating whether ptr replays should be parsed.
    /// </summary>
    public bool AllowPTR { get; set; } = true;
}
