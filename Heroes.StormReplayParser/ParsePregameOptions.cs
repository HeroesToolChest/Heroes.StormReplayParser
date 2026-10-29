namespace Heroes.StormReplayParser;

/// <summary>
/// Contains the options for parsing a <c>replay.server.battlelobby</c> file with <see cref="StormReplayPregame"/>.
/// </summary>
public class ParsePregameOptions
{
    /// <summary>
    /// Gets the default parse options, which are the same as a new <see cref="ParsePregameOptions"/>.
    /// <para>
    /// PTR battlelobby files are allowed, and the license id catalog is not parsed.
    /// </para>
    /// </summary>
    public static ParsePregameOptions DefaultParsing => new();

    /// <summary>
    /// Gets or sets a value indicating whether battlelobby files from the PTR (public test realm) are allowed.
    /// <para>
    /// A battlelobby file is from the PTR when a player's <see cref="ToonHandle.Region"/> is 90 or higher.
    /// If <see langword="false"/>, a PTR battlelobby file is still parsed, but if it parses successfully its status is <see cref="StormReplayPregameParseStatus.PTRRegion"/> instead of <see cref="StormReplayPregameParseStatus.Success"/>.
    /// </para>
    /// </summary>
    public bool AllowPTR { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to parse the license id catalog and the players' license ids when no license id catalog is passed to the <c>Parse</c> methods of <see cref="StormReplayPregame"/>.
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
