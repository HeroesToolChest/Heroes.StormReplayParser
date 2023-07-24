namespace Heroes.StormReplayParser;

/// <summary>
/// Contains the properties and methods for the battle lobby.
/// Used to obtain data from the loading screen part of the match.
/// </summary>
public class StormReplayPregame
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
    /// Gets or sets the map link, which can be used to determine the map.
    /// </summary>
    public string? MapLink { get; set; }

    /// <summary>
    /// Gets a collection of disabled heroes as attributeIds.
    /// </summary>
    public IReadOnlyCollection<string> DisabledHeroes => DisabledHeroAttributeIdList;

    /// <summary>
    /// Gets a collection of playing players (no observers, has AI).
    /// </summary>
    public IEnumerable<StormPregamePlayer> StormPlayers => ClientListByUserID.Where(PlayersFunc());

    /// <summary>
    /// Gets a collection of players (no AI, has observers).
    /// </summary>
    public IEnumerable<StormPregamePlayer> StormPlayersWithObservers => ClientListByUserID.Where(PlayersWithObserversFunc());

    /// <summary>
    /// Gets a collection of observer players.
    /// </summary>
    public IEnumerable<StormPregamePlayer> StormObservers => ClientListByUserID.Where(ObserversFunc());

    /// <summary>
    /// Gets the total number of playing players (no observers, has AI).
    /// </summary>
    public int PlayersCount => ClientListByUserID.Count(PlayersFunc());

    /// <summary>
    /// Gets the total number of playing players in the game (no AI, has observers).
    /// </summary>
    public int PlayersWithObserversCount => ClientListByUserID.Count(PlayersWithObserversFunc());

    /// <summary>
    /// Gets the total number of observers in the game.
    /// </summary>
    public int PlayersObserversCount => ClientListByUserID.Count(ObserversFunc());

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
    /// Gets the list of all players connected to the game, using 'm_userId' as index.
    /// </summary>
    /// <remarks>Contains observers. No AI.</remarks>
    internal StormPregamePlayer[] ClientListByUserID { get; private set; } = new StormPregamePlayer[16];

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal List<string> DisabledHeroAttributeIdList { get; private set; } = new List<string>();

    private static Func<StormPregamePlayer, bool> PlayersFunc() => x => x?.PlayerType != PlayerType.Observer && (x?.PlayerSlotType == PlayerSlotType.Human || x?.PlayerSlotType == PlayerSlotType.Computer);

    private static Func<StormPregamePlayer, bool> PlayersWithObserversFunc() => x => x?.PlayerSlotType == PlayerSlotType.Human;

    private static Func<StormPregamePlayer, bool> ObserversFunc() => x => x?.PlayerType == PlayerType.Observer && x.PlayerSlotType == PlayerSlotType.Human;
}
