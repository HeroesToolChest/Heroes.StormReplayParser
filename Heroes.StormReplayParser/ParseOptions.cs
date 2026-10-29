namespace Heroes.StormReplayParser;

/// <summary>
/// Contains the parsing options.
/// </summary>
public class ParseOptions
{
    /// <summary>
    /// Gets the default parse options. Disallows ptr and allows parsing of tracker, game, and message events.
    /// </summary>
    public static ParseOptions DefaultParsing => new();

    /// <summary>
    /// Gets the minimal parse options. Disallows ptr and no parsing of tracker, game, or message events.
    /// </summary>
    public static ParseOptions MinimalParsing => new()
    {
        AllowPTR = false,
        ShouldParseTrackerEvents = false,
        ShouldParseGameEvents = false,
        ShouldParseMessageEvents = false,
        ShouldParseLicenseCatalog = false,
    };

    /// <summary>
    /// Gets or sets a value indicating whether ptr replays should be parsed.
    /// </summary>
    public bool AllowPTR { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the tracker events should be parsed.
    /// </summary>
    public bool ShouldParseTrackerEvents { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the game events should be parsed.
    /// </summary>
    public bool ShouldParseGameEvents { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether message events should be parsed.
    /// </summary>
    public bool ShouldParseMessageEvents { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to parse the license id catalog and the players' license ids when no license id catalog is passed to <see cref="StormReplay.Parse(string, ParseOptions, IReadOnlySet{long})"/> or <see cref="StormReplay.Parse(Stream, ParseOptions, IReadOnlySet{long})"/>.
    /// <para>
    /// If <see langword="false"/> and no license id catalog is passed, <see cref="StormReplay.LicenseIdCatalog"/> and <see cref="StormPlayer.PortraitLicenseId"/> will be <see langword="null"/>,
    /// and <see cref="StormPlayer.LicenseIds"/> will be empty.
    /// </para>
    /// <para>
    /// If a license id catalog is passed, these are always parsed and this option is ignored.
    /// </para>
    /// </summary>
    public bool ShouldParseLicenseCatalog { get; set; } = false;
}
