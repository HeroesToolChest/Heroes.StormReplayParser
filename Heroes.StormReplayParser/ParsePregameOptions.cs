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

    /// <summary>
    /// Gets or sets a value indicating whether to parse the license id catalog and the players' license ids when no license id catalog is passed to <see cref="StormReplayPregame.Parse(string, ParsePregameOptions, IReadOnlySet{long})"/> or <see cref="StormReplayPregame.Parse(Stream, ParsePregameOptions, IReadOnlySet{long})"/>.
    /// <para>
    /// If <see langword="false"/> and no license id catalog is passed, <see cref="StormReplayPregame.LicenseIdCatalog"/> will be <see langword="null"/>
    /// and <see cref="PregameStormPlayer.LicenseIds"/> will be empty.
    /// </para>
    /// <para>
    /// If a license id catalog is passed, these are always parsed and this option is ignored.
    /// </para>
    /// </summary>
    public bool ShouldParseLicenseCatalog { get; set; } = false;
}
