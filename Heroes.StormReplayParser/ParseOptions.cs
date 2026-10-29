namespace Heroes.StormReplayParser;

/// <summary>
/// Contains the options for parsing a replay with <see cref="StormReplay"/>.
/// </summary>
public class ParseOptions
{
    /// <summary>
    /// Gets the default parse options, which are the same as a new <see cref="ParseOptions"/>.
    /// <para>
    /// PTR replays are not allowed, all of the tracker, game, and message events are parsed and kept, and the license id catalog is not parsed.
    /// </para>
    /// </summary>
    public static ParseOptions DefaultParsing => new();

    /// <summary>
    /// Gets the minimal parse options, which parse a replay the fastest.
    /// <para>
    /// PTR replays are not allowed, and none of the tracker, game, or message events or the license id catalog are parsed.
    /// The replay still has its basic data, such as the players and their heroes, the map, the game mode, and the replay length.
    /// </para>
    /// </summary>
    public static ParseOptions MinimalParsing => new()
    {
        AllowPTR = false,
        ShouldParseTrackerEvents = false,
        ShouldParseTrackerUnitEvents = false,
        ShouldParseGameEvents = false,
        ShouldKeepGameEvents = false,
        ShouldParseMessageEvents = false,
        ShouldParseLicenseCatalog = false,
    };

    /// <summary>
    /// Gets or sets a value indicating whether replays from the PTR (public test realm) are allowed.
    /// <para>
    /// A replay is from the PTR when a player's <see cref="ToonHandle.Region"/> is 90 or higher.
    /// If <see langword="false"/>, a PTR replay is still parsed, but if it parses successfully its status is <see cref="StormReplayParseStatus.PTRRegion"/> instead of <see cref="StormReplayParseStatus.Success"/>.
    /// </para>
    /// </summary>
    public bool AllowPTR { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the tracker events should be parsed.
    /// <para>
    /// If <see langword="false"/>, these come from the tracker events, so they will be empty or not set:
    /// </para>
    /// <list type="bullet">
    /// <item><description><see cref="StormReplay.TrackerEvents"/></description></item>
    /// <item><description><see cref="StormReplay.DraftPicks"/></description></item>
    /// <item><description><see cref="StormPlayer.ScoreResult"/> and <see cref="StormPlayer.MatchAwards"/></description></item>
    /// <item><description><see cref="PlayerHero.HeroUnitId"/></description></item>
    /// <item><description>the talent names (<see cref="HeroTalent.TalentNameId"/>) of <see cref="StormPlayer.Talents"/></description></item>
    /// <item><description>the team levels of <see cref="StormReplay.GetTeamLevels(StormTeam)"/> and <see cref="StormReplay.GetTeamFinalLevel(StormTeam)"/></description></item>
    /// <item><description>the team XP of <see cref="StormReplay.GetTeamXPBreakdown(StormTeam)"/></description></item>
    /// <item><description>the <see cref="StormMapInfo.MapId"/> of <see cref="StormReplay.MapInfo"/></description></item>
    /// </list>
    /// </summary>
    public bool ShouldParseTrackerEvents { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the unit tracker events should be parsed.
    /// <para>
    /// These are the unit tracker events:
    /// </para>
    /// <list type="bullet">
    /// <item><description><see cref="StormTrackerEventType.UnitBornEvent"/></description></item>
    /// <item><description><see cref="StormTrackerEventType.UnitDiedEvent"/></description></item>
    /// <item><description><see cref="StormTrackerEventType.UnitOwnerChangeEvent"/></description></item>
    /// <item><description><see cref="StormTrackerEventType.UnitTypeChangeEvent"/></description></item>
    /// <item><description><see cref="StormTrackerEventType.UnitInitEvent"/></description></item>
    /// <item><description><see cref="StormTrackerEventType.UnitDoneEvent"/></description></item>
    /// <item><description><see cref="StormTrackerEventType.UnitPositionsEvent"/></description></item>
    /// <item><description><see cref="StormTrackerEventType.UnitRevivedEvent"/></description></item>
    /// </list>
    /// <para>
    /// If <see langword="false"/>, these are skipped and left out of <see cref="StormReplay.TrackerEvents"/>. None of the other replay data comes from them.
    /// </para>
    /// <para>
    /// Having this as <see langword="false"/> makes parsing faster and uses less memory.
    /// </para>
    /// <para>
    /// This has no effect if <see cref="ShouldParseTrackerEvents"/> is <see langword="false"/>.
    /// </para>
    /// </summary>
    public bool ShouldParseTrackerUnitEvents { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the game events should be parsed.
    /// <para>
    /// If <see langword="false"/>, these come from the game events, so they will be empty or not set:
    /// </para>
    /// <list type="bullet">
    /// <item><description><see cref="StormReplay.GameEvents"/></description></item>
    /// <item><description><see cref="StormReplay.TeamChatMessages"/></description></item>
    /// <item><description>the talent slot ids (<see cref="HeroTalent.TalentSlotId"/>) and timestamps (<see cref="HeroTalent.Timestamp"/>) of <see cref="StormPlayer.Talents"/></description></item>
    /// <item><description><see cref="StormPlayer.PlayerDisconnects"/></description></item>
    /// <item><description><see cref="StormPlayer.IsPlatformMac"/></description></item>
    /// </list>
    /// <para>
    /// <see cref="StormReplay.Owner"/> is also read from the battle lobby, so it is usually set either way.
    /// </para>
    /// <para>
    /// The game events take most of the time and memory of a parse. If only the data that comes from them is needed, set <see cref="ShouldKeepGameEvents"/> to <see langword="false"/> instead.
    /// </para>
    /// </summary>
    public bool ShouldParseGameEvents { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the game events should be kept in <see cref="StormReplay.GameEvents"/>.
    /// <para>
    /// If <see langword="false"/>, the game events are still parsed for the data that comes from them:
    /// </para>
    /// <list type="bullet">
    /// <item><description>the talent slot ids (<see cref="HeroTalent.TalentSlotId"/>) and timestamps (<see cref="HeroTalent.Timestamp"/>) of <see cref="StormPlayer.Talents"/></description></item>
    /// <item><description><see cref="StormPlayer.PlayerDisconnects"/></description></item>
    /// <item><description><see cref="StormPlayer.IsPlatformMac"/></description></item>
    /// <item><description><see cref="StormReplay.TeamChatMessages"/></description></item>
    /// <item><description><see cref="StormReplay.Owner"/></description></item>
    /// </list>
    /// <para>
    /// The game events are not kept, so <see cref="StormReplay.GameEvents"/> will be empty.
    /// </para>
    /// <para>
    /// Having this as <see langword="false"/> makes parsing faster and uses less memory.
    /// </para>
    /// <para>
    /// This has no effect if <see cref="ShouldParseGameEvents"/> is <see langword="false"/>.
    /// </para>
    /// </summary>
    public bool ShouldKeepGameEvents { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the message events should be parsed.
    /// <para>
    /// If <see langword="false"/>, <see cref="StormReplay.Messages"/> and <see cref="StormReplay.ChatMessages"/> will be empty.
    /// </para>
    /// <para>
    /// The message events are:
    /// </para>
    /// <list type="bullet">
    /// <item><description><see cref="StormMessageEventType.SChatMessage"/></description></item>
    /// <item><description><see cref="StormMessageEventType.SPingMessage"/></description></item>
    /// <item><description><see cref="StormMessageEventType.SLoadingProgressMessage"/></description></item>
    /// <item><description><see cref="StormMessageEventType.SServerPingMessage"/></description></item>
    /// <item><description><see cref="StormMessageEventType.SReconnectNotifyMessage"/></description></item>
    /// <item><description><see cref="StormMessageEventType.SPlayerAnnounceMessage"/></description></item>
    /// </list>
    /// <para>
    /// <see cref="StormReplay.TeamChatMessages"/> comes from the game events, so it is not affected by this option.
    /// </para>
    /// </summary>
    public bool ShouldParseMessageEvents { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to parse the license id catalog and the players' license ids when no license id catalog is passed to the <c>Parse</c> methods of <see cref="StormReplay"/>.
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
