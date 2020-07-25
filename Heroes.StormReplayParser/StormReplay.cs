using Heroes.StormReplayParser.GameEvent;
using Heroes.StormReplayParser.MessageEvent;
using Heroes.StormReplayParser.Player;
using Heroes.StormReplayParser.Replay;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Heroes.StormReplayParser
{
    /// <summary>
    /// Contains the properties and methods for the parsed replay.
    /// </summary>
    public partial class StormReplay
    {
        private readonly Lazy<Dictionary<int, TeamLevel>> _teamBlueLevels = new Lazy<Dictionary<int, TeamLevel>>();
        private readonly Lazy<Dictionary<int, TeamLevel>> _teamRedLevels = new Lazy<Dictionary<int, TeamLevel>>();

        /// <summary>
        /// Gets the latest build that the parser was updated for.
        /// </summary>
        public static int LatestUpdatedBuild => 73016;

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
        public TimeSpan ReplayLength => new TimeSpan(0, 0, ElapsedGamesLoops / 16);

        /// <summary>
        /// Gets or sets the map info.
        /// </summary>
        public MapInfo MapInfo { get; set; } = new MapInfo();

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
        public GameMode GameMode { get; set; } = GameMode.TryMe;

        /// <summary>
        /// Gets or sets the team size of the selected game type.
        /// </summary>
        public string TeamSize { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the speed the game was played at.
        /// </summary>
        public GameSpeed GameSpeed { get; set; } = GameSpeed.Unknown;

        /// <summary>
        /// Gets a collection of playing players (no observers, has AI).
        /// </summary>
        public IEnumerable<StormPlayer> StormPlayers => Players.Where(x => x != null);

        /// <summary>
        /// Gets a collection of players (contains observers, no AI).
        /// </summary>
        public IEnumerable<StormPlayer> StormPlayersWithObservers => ClientListByUserID.Where(x => x != null);

        /// <summary>
        /// Gets a collection of observer players.
        /// </summary>
        public IEnumerable<StormPlayer> StormObservers => ClientListByUserID.Where(x => x?.PlayerType == PlayerType.Observer);

        /// <summary>
        /// Gets the total number of playing players (includes AI). Use <see cref="PlayersWithObserversCount"/> instead to include observers.
        /// </summary>
        public int PlayersCount => Players.Count(x => x != null);

        /// <summary>
        /// Gets the total number of playing players, including observers, in the game. Does not include AI.
        /// </summary>
        public int PlayersWithObserversCount => ClientListByUserID.Count(x => x != null);

        /// <summary>
        /// Gets the total number of observers in the game.
        /// </summary>
        public int PlayersObserversCount => StormObservers.Count();

        /// <summary>
        /// Gets a collection of tracker events.
        /// </summary>
        public IReadOnlyList<StormTrackerEvent> TrackerEvents => TrackerEventsInternal;

        /// <summary>
        /// Gets a collection of game events.
        /// </summary>
        public IReadOnlyList<StormGameEvent> GameEvents => GameEventsInternal;

        /// <summary>
        /// Gets a collection of all messages.
        /// </summary>
        public IReadOnlyList<StormMessage> Messages => MessagesInternal;

        /// <summary>
        /// Gets a collection of only chat messages.
        /// </summary>
        public IEnumerable<StormMessage> ChatMessages => MessagesInternal.Where(x => x.MessageEventType.HasValue && x.MessageEventType.Value == StormMessageEventType.SChatMessage);

        /// <summary>
        /// Gets or sets the list of all players (no observers).
        /// </summary>
        /// <remarks>Contains AI.</remarks>
        internal StormPlayer[] Players { get; set; } = new StormPlayer[10];

        /// <summary>
        /// Gets the list of all players connected to the game, using 'm_userId' as index.
        /// </summary>
        /// <remarks>Contains observers. No AI.</remarks>
        internal StormPlayer[] ClientListByUserID { get; private set; } = new StormPlayer[16];

        /// <summary>
        /// Gets the list of all players connected to the game, using 'm_workingSetSlotId' as index.
        /// </summary>
        /// <remarks>Contains AI. No observers.</remarks>
        internal StormPlayer[] ClientListByWorkingSetSlotID { get; private set; } = new StormPlayer[16];

        /// <summary>
        /// Gets the collection of open slot players. In some places, this is used instead of the 'Player' array, in games with less than 10 players.
        /// </summary>
        /// <remarks>Contains AI. No observers.</remarks>
        internal StormPlayer[] PlayersWithOpenSlots { get; private set; } = new StormPlayer[10];

        internal string?[][] TeamHeroAttributeIdBans { get; private set; } = new string?[2][] { new string?[3] { null, null, null }, new string?[3] { null, null, null } };

        internal List<StormGameEvent> GameEventsInternal { get; private set; } = new List<StormGameEvent>();
        internal List<StormTrackerEvent> TrackerEventsInternal { get; private set; } = new List<StormTrackerEvent>();

        internal List<StormMessage> MessagesInternal { get; private set; } = new List<StormMessage>();

        /// <summary>
        /// Gets a collection of a team's bans.
        /// </summary>
        /// <param name="stormTeam">The team.</param>
        /// <returns>The collection of bans.</returns>
        public IEnumerable<string?> GetTeamBans(StormTeam stormTeam)
        {
            if (!(stormTeam == StormTeam.Blue || stormTeam == StormTeam.Red))
                return Enumerable.Empty<string>();

            return TeamHeroAttributeIdBans[(int)stormTeam];
        }

        /// <summary>
        /// Gets a collection of the draft order.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DraftPick> GetDraftOrder()
        {
            foreach (StormTrackerEvent trackerEvent in TrackerEventsInternal.Where(x =>
                x.TrackerEventType == StormTrackerEventType.HeroBannedEvent ||
                x.TrackerEventType == StormTrackerEventType.HeroPickedEvent ||
                x.TrackerEventType == StormTrackerEventType.HeroSwappedEvent))
            {
                switch (trackerEvent.TrackerEventType)
                {
                    case StormTrackerEventType.HeroBannedEvent:
                        yield return new DraftPick()
                        {
                            HeroSelected = trackerEvent.VersionedDecoder!.Structure![0].GetValueAsString(),
                            SelectedPlayerSlotId = (int)trackerEvent.VersionedDecoder.Structure[1].GetValueAsUInt32(),
                            PickType = DraftPickType.Banned,
                        };
                        break;

                    case StormTrackerEventType.HeroPickedEvent:
                        yield return new DraftPick()
                        {
                            HeroSelected = trackerEvent.VersionedDecoder!.Structure![0].GetValueAsString(),
                            SelectedPlayerSlotId = (int)trackerEvent.VersionedDecoder.Structure[1].GetValueAsUInt32(),
                            PickType = DraftPickType.Picked,
                        };
                        break;

                    case StormTrackerEventType.HeroSwappedEvent:
                        yield return new DraftPick()
                        {
                            HeroSelected = trackerEvent.VersionedDecoder!.Structure![0].GetValueAsString(),
                            SelectedPlayerSlotId = (int)trackerEvent.VersionedDecoder.Structure[1].GetValueAsUInt32(),
                            PickType = DraftPickType.Swapped,
                        };
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Gets a team's final level at the end of the game.
        /// </summary>
        /// <param name="team">The team value.</param>
        /// <returns>The team's level.</returns>
        public int GetTeamFinalLevel(StormTeam team)
        {
            if (team == StormTeam.Blue)
            {
                if (!_teamBlueLevels.IsValueCreated)
                    GetTeamLevels(team);

                return _teamBlueLevels.Value.Values.LastOrDefault().Level;
            }
            else if (team == StormTeam.Red)
            {
                if (!_teamRedLevels.IsValueCreated)
                    GetTeamLevels(team);

                return _teamRedLevels.Value.Values.LastOrDefault().Level;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets a collection of a <see cref="StormTeam"/>'s levels.
        /// </summary>
        /// <param name="team">The team value.</param>
        /// <returns>A collection of the team's levels.</returns>
        public IEnumerable<TeamLevel> GetTeamLevels(StormTeam team)
        {
            if (team == StormTeam.Blue && _teamBlueLevels.IsValueCreated)
                return _teamBlueLevels.Value.Values;
            else if (team == StormTeam.Red && _teamRedLevels.IsValueCreated)
                return _teamRedLevels.Value.Values;

            foreach (StormTrackerEvent trackerEvent in TrackerEventsInternal.Where(x => x.TrackerEventType == StormTrackerEventType.StatGameEvent && x.VersionedDecoder?.Structure?[0].GetValueAsString() == "LevelUp"))
            {
                if (trackerEvent.VersionedDecoder?.Structure?[2].OptionalData?.ArrayData?[0].Structure?[0].Structure?[0].GetValueAsString() == "PlayerID" &&
                    trackerEvent.VersionedDecoder?.Structure?[2].OptionalData?.ArrayData?[1].Structure?[0].Structure?[0].GetValueAsString() == "Level")
                {
                    int playerId = (int)trackerEvent.VersionedDecoder!.Structure![2].OptionalData!.ArrayData![0].Structure![1].GetValueAsUInt32();

                    if (PlayersWithOpenSlots[playerId - 1].Team == team)
                    {
                        TeamLevel teamLevel = new TeamLevel()
                        {
                            Level = (int)trackerEvent.VersionedDecoder!.Structure![2].OptionalData!.ArrayData![1].Structure![1].GetValueAsUInt32(),
                            Time = trackerEvent.Timestamp,
                        };

                        if (team == StormTeam.Blue)
                            _teamBlueLevels.Value.TryAdd(teamLevel.Level, teamLevel);
                        else if (team == StormTeam.Red)
                            _teamRedLevels.Value.TryAdd(teamLevel.Level, teamLevel);
                    }
                }
            }

            if (team == StormTeam.Blue)
                return _teamBlueLevels.Value.Values;
            else if (team == StormTeam.Red)
                return _teamRedLevels.Value.Values;
            else
                return Enumerable.Empty<TeamLevel>();
        }

        /// <summary>
        /// Gets a collection of a team's experience breakdown that occurs during periodic intervals.
        /// </summary>
        /// <param name="team">The team value.</param>
        /// <returns>A collection fo the teams xp.</returns>
        public IEnumerable<TeamXPBreakdown> GetTeamXPBreakdown(StormTeam team)
        {
            TeamXPBreakdown teamXPBreakdown = new TeamXPBreakdown();

            foreach (StormTrackerEvent trackerEvent in TrackerEventsInternal.Where(x => x.TrackerEventType == StormTrackerEventType.StatGameEvent))
            {
                string value = trackerEvent.VersionedDecoder!.Structure![0].GetValueAsString();

                if (value == "PeriodicXPBreakdown")
                {
                    if (team == (StormTeam)(trackerEvent.VersionedDecoder!.Structure![2].OptionalData!.ArrayData![0].Structure![1].GetValueAsUInt32() - 1) &&
                        trackerEvent.VersionedDecoder?.Structure?[2].OptionalData?.ArrayData?[1].Structure?[0].Structure?[0].GetValueAsString() == "TeamLevel" &&
                        trackerEvent.VersionedDecoder?.Structure?[3].OptionalData?.ArrayData?[0].Structure?[0].Structure?[0].GetValueAsString() == "GameTime" &&
                        trackerEvent.VersionedDecoder?.Structure?[3].OptionalData?.ArrayData?[1].Structure?[0].Structure?[0].GetValueAsString() == "PreviousGameTime" &&
                        trackerEvent.VersionedDecoder?.Structure?[3].OptionalData?.ArrayData?[2].Structure?[0].Structure?[0].GetValueAsString() == "MinionXP" &&
                        trackerEvent.VersionedDecoder?.Structure?[3].OptionalData?.ArrayData?[3].Structure?[0].Structure?[0].GetValueAsString() == "CreepXP" &&
                        trackerEvent.VersionedDecoder?.Structure?[3].OptionalData?.ArrayData?[4].Structure?[0].Structure?[0].GetValueAsString() == "StructureXP" &&
                        trackerEvent.VersionedDecoder?.Structure?[3].OptionalData?.ArrayData?[5].Structure?[0].Structure?[0].GetValueAsString() == "HeroXP" &&
                        trackerEvent.VersionedDecoder?.Structure?[3].OptionalData?.ArrayData?[6].Structure?[0].Structure?[0].GetValueAsString() == "TrickleXP")
                    {
                        teamXPBreakdown = new TeamXPBreakdown()
                        {
                            Level = (int)trackerEvent.VersionedDecoder!.Structure![2].OptionalData!.ArrayData![1].Structure![1].GetValueAsUInt32(),
                            Time = trackerEvent.Timestamp,
                            MinionXP = (int)(trackerEvent.VersionedDecoder!.Structure![3].OptionalData!.ArrayData![2].Structure![1].GetValueAsInt64() / 4096),
                            CreepXP = (int)(trackerEvent.VersionedDecoder!.Structure![3].OptionalData!.ArrayData![3].Structure![1].GetValueAsInt64() / 4096),
                            StructureXP = (int)(trackerEvent.VersionedDecoder!.Structure![3].OptionalData!.ArrayData![4].Structure![1].GetValueAsInt64() / 4096),
                            HeroXP = (int)(trackerEvent.VersionedDecoder!.Structure![3].OptionalData!.ArrayData![5].Structure![1].GetValueAsInt64() / 4096),
                            PassiveXP = (int)(trackerEvent.VersionedDecoder!.Structure![3].OptionalData!.ArrayData![6].Structure![1].GetValueAsInt64() / 4096),
                        };

                        yield return teamXPBreakdown;
                    }
                }
                else if (value == "EndOfGameXPBreakdown")
                {
                    int playerId = (int)trackerEvent.VersionedDecoder!.Structure![2].OptionalData!.ArrayData![0].Structure![1].GetValueAsUInt32();

                    if (PlayersWithOpenSlots[playerId - 1].Team == team &&
                        teamXPBreakdown.Time != trackerEvent.Timestamp &&
                        trackerEvent.VersionedDecoder?.Structure?[2].OptionalData?.ArrayData?[0].Structure?[0].Structure?[0].GetValueAsString() == "PlayerID" &&
                        trackerEvent.VersionedDecoder?.Structure?[3].OptionalData?.ArrayData?[0].Structure?[0].Structure?[0].GetValueAsString() == "MinionXP" &&
                        trackerEvent.VersionedDecoder?.Structure?[3].OptionalData?.ArrayData?[1].Structure?[0].Structure?[0].GetValueAsString() == "CreepXP" &&
                        trackerEvent.VersionedDecoder?.Structure?[3].OptionalData?.ArrayData?[2].Structure?[0].Structure?[0].GetValueAsString() == "StructureXP" &&
                        trackerEvent.VersionedDecoder?.Structure?[3].OptionalData?.ArrayData?[3].Structure?[0].Structure?[0].GetValueAsString() == "HeroXP" &&
                        trackerEvent.VersionedDecoder?.Structure?[3].OptionalData?.ArrayData?[4].Structure?[0].Structure?[0].GetValueAsString() == "TrickleXP")
                    {
                        teamXPBreakdown = new TeamXPBreakdown()
                        {
                            Level = GetTeamFinalLevel(team),
                            Time = trackerEvent.Timestamp,
                            MinionXP = (int)(trackerEvent.VersionedDecoder!.Structure![3].OptionalData!.ArrayData![0].Structure![1].GetValueAsInt64() / 4096),
                            CreepXP = (int)(trackerEvent.VersionedDecoder!.Structure![3].OptionalData!.ArrayData![1].Structure![1].GetValueAsInt64() / 4096),
                            StructureXP = (int)(trackerEvent.VersionedDecoder!.Structure![3].OptionalData!.ArrayData![2].Structure![1].GetValueAsInt64() / 4096),
                            HeroXP = (int)(trackerEvent.VersionedDecoder!.Structure![3].OptionalData!.ArrayData![3].Structure![1].GetValueAsInt64() / 4096),
                            PassiveXP = (int)(trackerEvent.VersionedDecoder!.Structure![3].OptionalData!.ArrayData![4].Structure![1].GetValueAsInt64() / 4096),
                        };

                        yield return teamXPBreakdown;
                    }
                }
            }
        }

        internal void SetStormPlayerData()
        {
            StormTrackerEvent trackerEvent = TrackerEventsInternal.DefaultIfEmpty().LastOrDefault(x => x.TrackerEventType == StormTrackerEventType.ScoreResultEvent);

            if (trackerEvent.VersionedDecoder != null)
            {
                Dictionary<string, int?[]> scoreResultsByScoreName = trackerEvent.VersionedDecoder.Structure![0].ArrayData
                    .ToDictionary(x => x.Structure![0].GetValueAsString(), x => x.Structure![1].ArrayData.Select(i => i.ArrayData?.Length == 1 ? (int)i.ArrayData![0].Structure![0].GetValueAsInt64() : (int?)null).ToArray());

                for (int i = 0; i < ClientListByWorkingSetSlotID.Length; i++)
                {
                    ClientListByWorkingSetSlotID[i]?.SetScoreResult(i, (i) => GetScoreResult(i, scoreResultsByScoreName));
                }
            }
        }

        private static ScoreResult GetScoreResult(int player, Dictionary<string, int?[]> scoreResultsByScoreName)
        {
            ScoreResult scoreResult = new ScoreResult();

            foreach (string scoreResultEventKey in scoreResultsByScoreName.Keys)
            {
                int? value = scoreResultsByScoreName[scoreResultEventKey][player];

                if (value != null && value >= 0)
                {
                    switch (scoreResultEventKey)
                    {
                        case "Level":
                            scoreResult.Level = value.Value;
                            break;
                        case "Takedowns":
                            scoreResult.Takedowns = value.Value;
                            break;
                        case "SoloKill":
                            scoreResult.SoloKills = value.Value;
                            break;
                        case "Assists":
                            scoreResult.Assists = value.Value;
                            break;
                        case "Deaths":
                            scoreResult.Deaths = value.Value;
                            break;
                        case "HeroDamage":
                            scoreResult.HeroDamage = value.Value;
                            break;
                        case "SiegeDamage":
                            scoreResult.SiegeDamage = value.Value;
                            break;
                        case "StructureDamage":
                            scoreResult.StructureDamage = value.Value;
                            break;
                        case "MinionDamage":
                            scoreResult.MinionDamage = value.Value;
                            break;
                        case "CreepDamage":
                            scoreResult.CreepDamage = value.Value;
                            break;
                        case "SummonDamage":
                            scoreResult.SummonDamage = value.Value;
                            break;
                        case "TimeCCdEnemyHeroes":
                            scoreResult.TimeCCdEnemyHeroes = TimeSpan.FromSeconds(value.Value);
                            break;
                        case "Healing":
                            scoreResult.Healing = value.Value;
                            break;
                        case "SelfHealing":
                            scoreResult.SelfHealing = value.Value;
                            break;
                        case "DamageTaken":
                            scoreResult.DamageTaken = value.Value;
                            break;
                        case "DamageSoaked":
                            scoreResult.DamageSoaked = value.Value;
                            break;
                        case "ExperienceContribution":
                            scoreResult.ExperienceContribution = value.Value;
                            break;
                        case "TownKills":
                            scoreResult.TownKills = value.Value;
                            break;
                        case "TimeSpentDead":
                            scoreResult.TimeSpentDead = TimeSpan.FromSeconds(value.Value);
                            break;
                        case "MercCampCaptures":
                            scoreResult.MercCampCaptures = value.Value;
                            break;
                        case "WatchTowerCaptures":
                            scoreResult.WatchTowerCaptures = value.Value;
                            break;
                        case "AAA":
                            scoreResult.MercCampCaptures = value.Value;
                            break;
                        case "MetaExperience":
                            scoreResult.MetaExperience = value.Value;
                            break;
                        case "HighestKillStreak":
                            scoreResult.HighestKillStreak = value.Value;
                            break;
                        case "ProtectionGivenToAllies":
                            scoreResult.ProtectionGivenToAllies = value.Value;
                            break;
                        case "TimeSilencingEnemyHeroes":
                            scoreResult.TimeSilencingEnemyHeroes = TimeSpan.FromSeconds(value.Value);
                            break;
                        case "TimeRootingEnemyHeroes":
                            scoreResult.TimeRootingEnemyHeroes = TimeSpan.FromSeconds(value.Value);
                            break;
                        case "TimeStunningEnemyHeroes":
                            scoreResult.TimeStunningEnemyHeroes = TimeSpan.FromSeconds(value.Value);
                            break;
                        case "ClutchHealsPerformed":
                            scoreResult.ClutchHealsPerformed = value.Value;
                            break;
                        case "EscapesPerformed":
                            scoreResult.EscapesPerformed = value.Value;
                            break;
                        case "VengeancesPerformed":
                            scoreResult.VengeancesPerformed = value.Value;
                            break;
                        case "OutnumberedDeaths":
                            scoreResult.OutnumberedDeaths = value.Value;
                            break;
                        case "TeamfightEscapesPerformed":
                            scoreResult.TeamfightEscapesPerformed = value.Value;
                            break;
                        case "TeamfightHealingDone":
                            scoreResult.TeamfightHealingDone = value.Value;
                            break;
                        case "TeamfightDamageTaken":
                            scoreResult.TeamfightDamageTaken = value.Value;
                            break;
                        case "TeamfightHeroDamage":
                            scoreResult.TeamfightHeroDamage = value.Value;
                            break;
                        case "Multikill":
                            scoreResult.Multikill = value.Value;
                            break;
                        case "PhysicalDamage":
                            scoreResult.PhysicalDamage = value.Value;
                            break;
                        case "SpellDamage":
                            scoreResult.SpellDamage = value.Value;
                            break;
                        case "OnFireTimeOnFire":
                            scoreResult.OnFireTimeonFire = TimeSpan.FromSeconds(value.Value);
                            break;
                        case "MinionKills":
                            scoreResult.MinionKills = value.Value;
                            break;
                        case "RegenGlobes":
                            scoreResult.RegenGlobes = value.Value;
                            break;
                        case "Tier1Talent":
                            if (value.Value > 0)
                                scoreResult.Tier1Talent = value.Value;
                            break;
                        case "Tier2Talent":
                            if (value.Value > 0)
                                scoreResult.Tier4Talent = value.Value;
                            break;
                        case "Tier3Talent":
                            if (value.Value > 0)
                                scoreResult.Tier7Talent = value.Value;
                            break;
                        case "Tier4Talent":
                            if (value.Value > 0)
                                scoreResult.Tier10Talent = value.Value;
                            break;
                        case "Tier5Talent":
                            if (value.Value > 0)
                                scoreResult.Tier13Talent = value.Value;
                            break;
                        case "Tier6Talent":
                            if (value.Value > 0)
                                scoreResult.Tier16Talent = value.Value;
                            break;
                        case "Tier7Talent":
                            if (value.Value > 0)
                                scoreResult.Tier20Talent = value.Value;
                            break;
                        case "EndOfMatchAwardMVPBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MVP);
                            break;
                        case "EndOfMatchAwardHighestKillStreakBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.HighestKillStreak);
                            break;
                        case "EndOfMatchAwardMostXPContributionBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostXPContribution);
                            break;
                        case "EndOfMatchAwardMostHeroDamageDoneBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostHeroDamageDone);
                            break;
                        case "EndOfMatchAwardMostSiegeDamageDoneBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostSiegeDamageDone);
                            break;
                        case "EndOfMatchAwardMostDamageTakenBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostDamageTaken);
                            break;
                        case "EndOfMatchAwardMostHealingBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostHealing);
                            break;
                        case "EndOfMatchAwardMostStunsBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostStuns);
                            break;
                        case "EndOfMatchAwardMostMercCampsCapturedBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostMercCampsCaptured);
                            break;
                        case "EndOfMatchAwardMapSpecificBoolean":
                            // generic, check map specific instead
                            break;
                        case "EndOfMatchAwardMostDragonShrinesCapturedBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostDragonShrinesCaptured);
                            break;
                        case "EndOfMatchAwardMostCurseDamageDoneBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostCurseDamageDone);
                            break;
                        case "EndOfMatchAwardMostCoinsPaidBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostCoinsPaid);
                            break;
                        case "EndOfMatchAwardMostImmortalDamageBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostImmortalDamage);
                            break;
                        case "EndOfMatchAwardMostDamageDoneToZergBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostDamageDoneToZerg);
                            break;
                        case "EndOfMatchAwardMostDamageToPlantsBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostDamageToPlants);
                            break;
                        case "EndOfMatchAwardMostDamageToMinionsBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostDamageToMinions);
                            break;
                        case "EndOfMatchAwardMostTimeInTempleBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostTimeInTemple);
                            break;
                        case "EndOfMatchAwardMostGemsTurnedInBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostGemsTurnedIn);
                            break;
                        case "EndOfMatchAwardMostAltarDamageDone":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostAltarDamage);
                            break;
                        case "EndOfMatchAwardMostNukeDamageDoneBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostNukeDamageDone);
                            break;
                        case "EndOfMatchAwardMostSkullsCollectedBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostSkullsCollected);
                            break;
                        case "EndOfMatchAwardMostTimePushingBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostTimePushing);
                            break;
                        case "EndOfMatchAwardMostTimeOnPointBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostTimeOnPoint);
                            break;
                        case "EndOfMatchAwardMostInterruptedCageUnlocksBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostInterruptedCageUnlocks);
                            break;
                        case "EndOfMatchAwardMostSeedsCollectedBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostSeedsCollected);
                            break;
                        case "EndOfMatchAwardMostKillsBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostKills);
                            break;
                        case "EndOfMatchAwardHatTrickBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.HatTrick);
                            break;
                        case "EndOfMatchAwardClutchHealerBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.ClutchHealer);
                            break;
                        case "EndOfMatchAwardMostProtectionBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostProtection);
                            break;
                        case "EndOfMatchAward0DeathsBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.ZeroDeaths);
                            break;
                        case "EndOfMatchAwardMostRootsBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostRoots);
                            break;
                        case "EndOfMatchAward0OutnumberedDeathsBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.ZeroOutnumberedDeaths);
                            break;
                        case "EndOfMatchAwardMostDaredevilEscapesBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostDaredevilEscapes);
                            break;
                        case "EndOfMatchAwardMostEscapesBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostEscapes);
                            break;
                        case "EndOfMatchAwardMostSilencesBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostSilences);
                            break;
                        case "EndOfMatchAwardMostTeamfightDamageTakenBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostTeamfightDamageTaken);
                            break;
                        case "EndOfMatchAwardMostTeamfightHealingDoneBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostTeamfightHealingDone);
                            break;
                        case "EndOfMatchAwardMostTeamfightHeroDamageDoneBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostTeamfightHeroDamageDone);
                            break;
                        case "EndOfMatchAwardMostVengeancesPerformedBoolean":
                            if (value.Value == 1)
                                scoreResult.MatchAwards.Add(MatchAwardType.MostVengeancesPerformed);
                            break;

                        case "GameScore":
                        case "TeamLevel":
                        case "TeamTakedowns":
                        case "Role":
                        case "EndOfMatchAwardGivenToNonwinner":
                        case "TouchByBlightPlague":
                        case "Difficulty":
                        case "HeroRingMasteryUpgrade":
                        case "LessThan4Deaths":
                        case "LessThan3TownStructuresLost":

                        // Map Objectives
                        case "DamageDoneToZerg":
                        case "DamageDoneToShrineMinions":
                        case "DragonNumberOfDragonCaptures":
                        case "DragonShrinesCaptured":
                        case "TimeInTemple":
                        case "GemsTurnedIn":
                        case "AltarDamageDone":
                        case "CurseDamageDone":
                        case "GardensPlantDamage":
                        case "DamageDoneToImmortal":
                        case "RavenTributesCollected":
                        case "GardensSeedsCollected":
                        case "BlackheartDoubloonsCollected":
                        case "BlackheartDoubloonsTurnedIn":
                        case "MinesSkullsCollected":
                        case "NukeDamageDone":
                        case "TimeOnPayload":
                        case "TimeOnPoint":
                        case "CageUnlocksInterrupted":
                        case "GardenSeedsCollectedByPlayer":

                        // Special Events
                        case "LunarNewYearEventCompleted": // Early 2016
                        case "LunarNewYearSuccesfulArtifactTurnIns": // Early 2017
                        case "LunarNewYearRoosterEventCompleted": // Early 2017
                        case "KilledTreasureGoblin":
                        case "StarcraftDailyEventCompleted":
                        case "StarcraftPiecesCollected":
                        case "PachimariMania":

                        // Franchise Booleans
                        case "TeamWinsDiablo":
                        case "TeamWinsStarCraft":
                        case "TeamWinsWarcraft":
                        case "TeamWinsOverwatch":
                        case "WinsStarCraft":
                        case "WinsDiablo":
                        case "WinsWarcraft":
                        case "WinsOverwatch":
                        case "PlaysStarCraft":
                        case "PlaysDiablo":
                        case "PlaysWarCraft":
                        case "PlaysOverwatch":

                        // Gender Booleans
                        case "TeamWinsFemale":
                        case "TeamWinsMale":
                        case "WinsMale":
                        case "WinsFemale":
                        case "PlaysMale":
                        case "PlaysFemale":

                        // Role Booleans
                        case "WinsWarrior":
                        case "WinsAssassin":
                        case "WinsSupport":
                        case "WinsSpecialist":
                        case "PlaysWarrior":
                        case "PlaysAssassin":
                        case "PlaysSupport":
                        case "PlaysSpecialist":
                            scoreResult.MiscellaneousScoreResultEvents[scoreResultEventKey] = value.Value;
                            break;

                        default:
                            scoreResult.NewScoreResultEvents[scoreResultEventKey] = value.Value;
                            break;
                    }
                }
            }

            return scoreResult;
        }
    }
}
