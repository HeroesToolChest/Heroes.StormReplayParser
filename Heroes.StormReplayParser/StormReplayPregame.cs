namespace Heroes.StormReplayParser;

/// <summary>
/// Contains the properties and methods for the battle lobby.
/// Used to obtain data from the loading screen part of the match.
/// </summary>
public partial class StormReplayPregame
{
    /// <summary>
    /// Gets or sets the replay build.
    /// </summary>
    public int ReplayBuild { get; set; } = 9999999;

    /// <summary>
    /// Gets or sets the random value.
    /// </summary>
    public long RandomValue { get; set; }

    /// <summary>
    /// Gets or sets the game mode.
    /// </summary>
    public StormGameMode GameMode { get; set; } = StormGameMode.Unknown;

    /// <summary>
    /// Gets or sets the team size of the selected game type.
    /// </summary>
    public string TeamSize { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the map link, which can be used to determine the map.
    /// </summary>
    public string? MapLink { get; set; }

    /// <summary>
    /// Gets or sets the map id name.
    /// </summary>
    public string? MapId { get; set; }

    /// <summary>
    /// Gets or sets the speed the game was played at.
    /// </summary>
    public StormGameSpeed GameSpeed { get; set; } = StormGameSpeed.Unknown;

    /// <summary>
    /// Gets or sets the game privacy setting (appears in match history or not).
    /// </summary>
    public StormGamePrivacy GamePrivacy { get; set; } = StormGamePrivacy.Unknown;

    /// <summary>
    /// Gets or sets the ready mode. This is the player order for selecting a hero in draft.
    /// </summary>
    public StormReadyMode ReadyMode { get; set; } = StormReadyMode.Unknown;

    /// <summary>
    /// Gets or sets the lobby mode. This determines the type of draft or no draft.
    /// </summary>
    public StormLobbyMode LobbyMode { get; set; } = StormLobbyMode.Unknown;

    /// <summary>
    /// Gets or sets the ban mode.
    /// </summary>
    public StormBanMode BanMode { get; set; } = StormBanMode.Unknown;

    /// <summary>
    /// Gets or sets the first draft team.
    /// </summary>
    public StormFirstDraftTeam FirstDraftTeam { get; set; } = StormFirstDraftTeam.Unknown;

    /// <summary>
    /// Gets a collection of disabled heroes as attributeIds.
    /// </summary>
    public IReadOnlyCollection<string> DisabledHeroes => DisabledHeroAttributeIdList;

    /// <summary>
    /// Gets a collection of playing players (no observers, has AI).
    /// </summary>
    public IEnumerable<StormPregamePlayer> StormPlayers => ClientListByWorkingSetSlotID.Where(PlayersFunc());

    /// <summary>
    /// Gets a collection of players (no AI, has observers).
    /// </summary>
    public IEnumerable<StormPregamePlayer> StormPlayersWithObservers => ClientListByWorkingSetSlotID.Where(PlayersWithObserversFunc());

    /// <summary>
    /// Gets a collection of observer players.
    /// </summary>
    public IEnumerable<StormPregamePlayer> StormObservers => ClientListByWorkingSetSlotID.Where(ObserversFunc());

    /// <summary>
    /// Gets the total number of playing players (no observers, has AI).
    /// </summary>
    public int PlayersCount => ClientListByWorkingSetSlotID.Count(PlayersFunc());

    /// <summary>
    /// Gets the total number of playing players in the game (no AI, has observers).
    /// </summary>
    public int PlayersWithObserversCount => ClientListByWorkingSetSlotID.Count(PlayersWithObserversFunc());

    /// <summary>
    /// Gets the total number of observers in the game.
    /// </summary>
    public int PlayersObserversCount => ClientListByWorkingSetSlotID.Count(ObserversFunc());

    /// <summary>
    /// Gets the region of this replay.
    /// </summary>
    public StormRegion Region
    {
        get
        {
            StormPregamePlayer? player = StormPlayersWithObservers.FirstOrDefault();
            if (player is not null && player.ToonHandle is not null)
                return player.ToonHandle.StormRegion;
            else
                return StormRegion.Unknown;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the battle lobby data was parsed successfully.
    /// </summary>
    public bool IsBattleLobbyPlayerInfoParsed { get; set; }

    /// <summary>
    /// Gets the list of all players connected to the game, using 'm_workingSetSlotId' as index.
    /// </summary>
    /// <remarks>Contains AI. No observers.</remarks>
    internal StormPregamePlayer[] ClientListByWorkingSetSlotID { get; private set; } = new StormPregamePlayer[16];

    internal string?[][] TeamHeroAttributeIdBans { get; private set; } = new string?[2][] { new string?[3] { null, null, null }, new string?[3] { null, null, null } };

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal List<string> DisabledHeroAttributeIdList { get; private set; } = new();

    /// <summary>
    /// Gets a collection of a team's bans (as attribute ids).
    /// </summary>
    /// <param name="stormTeam">The <see cref="StormTeam"/> to obtain the bans for.</param>
    /// <returns>A collection of hero bans as attribute ids.</returns>
    public IReadOnlyList<string?> GetTeamBans(StormTeam stormTeam)
    {
        if (!(stormTeam == StormTeam.Blue || stormTeam == StormTeam.Red))
            return new List<string>();

        return TeamHeroAttributeIdBans[(int)stormTeam];
    }

    private static Func<StormPregamePlayer, bool> PlayersFunc() => x => x?.PlayerType != PlayerType.Observer && (x?.PlayerSlotType == PlayerSlotType.Human || x?.PlayerSlotType == PlayerSlotType.Computer);

    private static Func<StormPregamePlayer, bool> PlayersWithObserversFunc() => x => x?.PlayerSlotType == PlayerSlotType.Human;

    private static Func<StormPregamePlayer, bool> ObserversFunc() => x => x?.PlayerType == PlayerType.Observer && x.PlayerSlotType == PlayerSlotType.Human;
}
