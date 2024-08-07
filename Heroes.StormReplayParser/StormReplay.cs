﻿namespace Heroes.StormReplayParser;

/// <summary>
/// Contains the properties and methods for the parsed replay.
/// </summary>
public partial class StormReplay
{
    /// <summary>
    /// Gets a value indicating whether there is at least one observer.
    /// </summary>
    public bool HasObservers => ClientListByUserID.Any(x => x?.PlayerType == PlayerType.Observer);

    /// <summary>
    /// Gets a value indicating whether there is at least one AI.
    /// </summary>
    public bool HasAI => Players.Any(x => x?.PlayerType == PlayerType.Computer);

    /// <summary>
    /// Gets or sets the version of the replay.
    /// </summary>
    public StormReplayVersion ReplayVersion { get; set; } = new StormReplayVersion();

    /// <summary>
    /// Gets the build number of the replay.
    /// </summary>
    public int ReplayBuild => ReplayVersion.BaseBuild;

    /// <summary>
    /// Gets or sets the total number of elapsed game loops / frames.
    /// </summary>
    public int ElapsedGamesLoops { get; set; }

    /// <summary>
    /// Gets the length of the replay.
    /// </summary>
    public TimeSpan ReplayLength => new(0, 0, ElapsedGamesLoops / 16);

    /// <summary>
    /// Gets or sets the map info.
    /// </summary>
    public StormMapInfo MapInfo { get; set; } = new StormMapInfo();

    /// <summary>
    /// Gets or sets the date and time of the when the replay was created.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the random value.
    /// </summary>
    public long RandomValue { get; set; }

    /// <summary>
    /// Gets or sets the game mode.
    /// </summary>
    public StormGameMode GameMode { get; set; } = StormGameMode.TryMe;

    /// <summary>
    /// Gets or sets the team size of the selected game type.
    /// </summary>
    public string TeamSize { get; set; } = string.Empty;

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
    /// Gets or sets the owner of this replay.
    /// </summary>
    public StormPlayer? Owner { get; set; } = null;

    /// <summary>
    /// Gets a collection of playing players (no observers, has AI).
    /// </summary>
    public IEnumerable<StormPlayer> StormPlayers => Players.Where(x => x is not null)!;

    /// <summary>
    /// Gets a collection of players (no AI, has observers).
    /// </summary>
    public IEnumerable<StormPlayer> StormPlayersWithObservers => ClientListByUserID.Where(x => x is not null)!;

    /// <summary>
    /// Gets a collection of observer players.
    /// </summary>
    public IEnumerable<StormPlayer> StormObservers => ClientListByUserID.Where(ObserversFunc()!)!;

    /// <summary>
    /// Gets the total number of playing players (no observers, has AI).
    /// </summary>
    public int PlayersCount => Players.Count(x => x is not null);

    /// <summary>
    /// Gets the total number of playing players in the game (no AI, has observers).
    /// </summary>
    public int PlayersWithObserversCount => ClientListByUserID.Count(x => x is not null);

    /// <summary>
    /// Gets the total number of observers in the game.
    /// </summary>
    public int PlayersObserversCount => ClientListByUserID.Count(x => x?.PlayerType == PlayerType.Observer);

    /// <summary>
    /// Gets the region of this replay.
    /// </summary>
    public StormRegion Region
    {
        get
        {
            StormPlayer? player = StormPlayersWithObservers.FirstOrDefault();
            if (player is not null && player.ToonHandle is not null)
                return player.ToonHandle.StormRegion;
            else
                return StormRegion.Unknown;
        }
    }

    /// <summary>
    /// Gets the winning team.
    /// </summary>
    public StormTeam WinningTeam
    {
        get
        {
            StormPlayer? player = Players[0];
            if (player is null)
                return StormTeam.Unknown;
            else
                return player.IsWinner ? StormTeam.Blue : StormTeam.Red;
        }
    }

    /// <summary>
    /// Gets a collection of tracker events.
    /// </summary>
    public IReadOnlyList<StormTrackerEvent> TrackerEvents => TrackerEventsInternal.AsReadOnly();

    /// <summary>
    /// Gets a collection of game events.
    /// </summary>
    public IReadOnlyList<StormGameEvent> GameEvents => GameEventsInternal.AsReadOnly();

    /// <summary>
    /// Gets a collection of all messages.
    /// </summary>
    public IReadOnlyList<IStormMessage> Messages => MessagesInternal.AsReadOnly();

    /// <summary>
    /// Gets a collection of only chat messages.
    /// </summary>
    public IEnumerable<IStormMessage> ChatMessages => MessagesInternal.AsReadOnly().Where(x => x.MessageEventType == StormMessageEventType.SChatMessage);

    /// <summary>
    /// Gets a collection of the draft order.
    /// </summary>
    public IReadOnlyList<StormDraftPick> DraftPicks => DraftPicksInternal.AsReadOnly();

    /// <summary>
    /// Gets or sets a value indicating whether battle lobby data was parsed successfully.
    /// </summary>
    public bool IsBattleLobbyPlayerInfoParsed { get; set; }

    /// <summary>
    /// Gets a collection of disabled heroes as attributeIds.
    /// </summary>
    public IReadOnlyCollection<string> DisabledHeroes => DisabledHeroAttributeIdList;

    /// <summary>
    /// Gets or sets the list of all players (no observers).
    /// </summary>
    /// <remarks>Contains AI.</remarks>
    internal StormPlayer?[] Players { get; set; } = new StormPlayer?[10];

    /// <summary>
    /// Gets the list of all players connected to the game, using 'm_userId' as index.
    /// </summary>
    /// <remarks>Contains observers. No AI.</remarks>
    internal StormPlayer?[] ClientListByUserID { get; private set; } = new StormPlayer?[16];

    /// <summary>
    /// Gets the list of all players connected to the game, using 'm_workingSetSlotId' as index.
    /// </summary>
    /// <remarks>Contains AI. No observers.</remarks>
    internal StormPlayer?[] ClientListByWorkingSetSlotID { get; private set; } = new StormPlayer?[16];

    /// <summary>
    /// Gets or sets a value indicating whether the replay does not contain the working set slot ids and as such <see cref=" ClientListByWorkingSetSlotID"/>
    /// will contain all <see langword="null"/>s.
    /// </summary>
    internal bool NoWorkingSetSlotID { get; set; } = false;

    /// <summary>
    /// Gets the collection of open slot players. In some places, this is used instead of the 'Player' array, in games with less than 10 players.
    /// </summary>
    /// <remarks>Contains AI. No observers.</remarks>
    internal StormPlayer?[] PlayersWithOpenSlots { get; private set; } = new StormPlayer?[10];

#if NET8_0_OR_GREATER
    internal string?[][] TeamHeroAttributeIdBans { get; private set; } = [[null, null, null], [null, null, null]];
#else
    internal string?[][] TeamHeroAttributeIdBans { get; private set; } = new string?[2][] { new string?[3] { null, null, null }, new string?[3] { null, null, null } };
#endif

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal List<StormGameEvent> GameEventsInternal { get; private set; } = new List<StormGameEvent>();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal List<StormTrackerEvent> TrackerEventsInternal { get; private set; } = new List<StormTrackerEvent>();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal List<IStormMessage> MessagesInternal { get; private set; } = new List<IStormMessage>();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal List<StormDraftPick> DraftPicksInternal { get; private set; } = new List<StormDraftPick>();

    internal Dictionary<int, StormTeamLevel>?[] TeamLevelsInternal { get; private set; } = new Dictionary<int, StormTeamLevel>[2];

    internal List<StormTeamXPBreakdown>?[] TeamXPBreakdownInternal { get; private set; } = new List<StormTeamXPBreakdown>[2];

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal List<string> DisabledHeroAttributeIdList { get; private set; } = new List<string>();

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

    /// <summary>
    /// Gets a team's final level at the end of the game.
    /// </summary>
    /// <param name="team">The team value.</param>
    /// <returns>The team's level.</returns>
    public int? GetTeamFinalLevel(StormTeam team)
    {
        if (team == StormTeam.Blue)
            return TeamLevelsInternal[0]?.Values?.LastOrDefault()?.Level;
        else if (team == StormTeam.Red)
            return TeamLevelsInternal[1]?.Values?.LastOrDefault()?.Level;
        else
            return null;
    }

    /// <summary>
    /// Gets a collection of a <see cref="StormTeam"/>'s levels.
    /// </summary>
    /// <param name="team">The team value.</param>
    /// <returns>A collection of the team's levels or <see langword="null"/> if it does not exists.</returns>
    public IReadOnlyList<StormTeamLevel>? GetTeamLevels(StormTeam team)
    {
        if (team == StormTeam.Blue)
            return TeamLevelsInternal[0]?.Values?.ToList() ?? null;
        else if (team == StormTeam.Red)
            return TeamLevelsInternal[1]?.Values?.ToList() ?? null;
        else
            return null;
    }

    /// <summary>
    /// Gets a collection of a team's experience breakdown that occurs during periodic intervals.
    /// </summary>
    /// <param name="team">The team value.</param>
    /// <returns>A collection of the team's xp or <see langword="null"/> if it does not exist.</returns>
    public IReadOnlyList<StormTeamXPBreakdown>? GetTeamXPBreakdown(StormTeam team)
    {
        if (team == StormTeam.Blue)
            return TeamXPBreakdownInternal[0] ?? null;
        else if (team == StormTeam.Red)
            return TeamXPBreakdownInternal[1] ?? null;
        else
            return null;
    }

    private static Func<StormPlayer, bool> ObserversFunc() => x => x?.PlayerType == PlayerType.Observer;
}
