namespace Heroes.StormReplayParser.MpqFiles;

internal static class ReplayTrackerEvents
{
    public static string FileName { get; } = "replay.tracker.events";

    public static void Parse(StormReplay replay, ReadOnlySpan<byte> source)
    {
        BitReader bitReader = new(source, EndianType.BigEndian);

        uint gameLoop = 0;

        while (bitReader.Index < source.Length)
        {
            gameLoop += new VersionedDecoder(ref bitReader).ChoiceData!.GetValueAsUInt32();

            TimeSpan timeSpan = TimeSpan.FromSeconds(gameLoop / 16.0);
            StormTrackerEventType type = (StormTrackerEventType)new VersionedDecoder(ref bitReader).GetValueAsUInt32();
            VersionedDecoder decoder = new(ref bitReader);

            replay.TrackerEventsInternal.Add(new StormTrackerEvent(type, timeSpan, decoder));
        }

        SetParsedData(replay);
    }

    private static void SetParsedData(StormReplay replay)
    {
        foreach (StormTrackerEvent stormTrackerEvent in replay.TrackerEventsInternal)
        {
            switch (stormTrackerEvent.TrackerEventType)
            {
                case StormTrackerEventType.PlayerSetupEvent:
                    if (replay.NoWorkingSetSlotID)
                    {
                        uint playerId = stormTrackerEvent.VersionedDecoder!.Structure![3].OptionalData!.GetValueAsUInt32();
                        uint workingSetSlotId = stormTrackerEvent.VersionedDecoder!.Structure![2].OptionalData!.GetValueAsUInt32();

                        replay.ClientListByWorkingSetSlotID[workingSetSlotId] = replay.Players[playerId];
                    }

                    break;
                case StormTrackerEventType.ScoreResultEvent:
                    if (stormTrackerEvent.VersionedDecoder != null)
                    {
                        Dictionary<string, int?[]> scoreResultsByScoreName = stormTrackerEvent.VersionedDecoder.Structure![0].ArrayData!
                            .ToDictionary(x => x.Structure![0].GetValueAsString(), x => x.Structure![1].ArrayData!.Select(i => i.ArrayData?.Length == 1 ? (int)i.ArrayData![0].Structure![0].GetValueAsInt64() : (int?)null).ToArray());

                        for (int i = 0; i < replay.ClientListByWorkingSetSlotID.Length; i++)
                        {
                            replay.ClientListByWorkingSetSlotID[i]?.SetScoreResult(i, (i) => GetScoreResult(i, scoreResultsByScoreName));
                        }
                    }

                    break;
                case StormTrackerEventType.HeroBannedEvent:
                    replay.DraftPicksInternal.Add(new StormDraftPick()
                    {
                        HeroSelected = stormTrackerEvent.VersionedDecoder!.Structure![0].GetValueAsString(),
                        Team = (StormTeam)(stormTrackerEvent.VersionedDecoder.Structure[1].GetValueAsUInt32() - 1),
                        PickType = StormDraftPickType.Banned,
                    });
                    break;
                case StormTrackerEventType.HeroPickedEvent:
                    replay.DraftPicksInternal.Add(new StormDraftPick()
                    {
                        HeroSelected = stormTrackerEvent.VersionedDecoder!.Structure![0].GetValueAsString(),
                        Player = replay.ClientListByUserID[stormTrackerEvent.VersionedDecoder.Structure[1].GetValueAsUInt32()],
                        PickType = StormDraftPickType.Picked,
                    });

                    break;
                case StormTrackerEventType.HeroSwappedEvent:
                    replay.DraftPicksInternal.Add(new StormDraftPick()
                    {
                        HeroSelected = stormTrackerEvent.VersionedDecoder!.Structure![0].GetValueAsString(),
                        Player = replay.ClientListByUserID[stormTrackerEvent.VersionedDecoder.Structure[1].GetValueAsUInt32()],
                        PickType = StormDraftPickType.Swapped,
                    });
                    break;
                case StormTrackerEventType.StatGameEvent:
                    if (stormTrackerEvent.VersionedDecoder != null)
                    {
                        ParseStatGameEvent(replay, stormTrackerEvent);
                    }

                    break;
                default:
                    break;
            }
        }
    }

    private static void ParseStatGameEvent(StormReplay replay, StormTrackerEvent stormTrackerEvent)
    {
        byte[]? value = stormTrackerEvent.VersionedDecoder!.Structure?[0].Value;
        if (value != null)
        {
            Span<char> valueSpan = stackalloc char[value.Length];
            Encoding.UTF8.GetChars(value, valueSpan);

            if (valueSpan.SequenceEqual("LevelUp"))
            {
                StatLevelUp(replay, stormTrackerEvent);
            }
            else if (valueSpan.SequenceEqual("PeriodicXPBreakdown"))
            {
                StatPeriodXPBreakdown(replay, stormTrackerEvent);
            }
            else if (valueSpan.SequenceEqual("EndOfGameXPBreakdown"))
            {
                StatEndOfGameXPBreakdown(replay, stormTrackerEvent);
            }
            else if (valueSpan.SequenceEqual("StatEndOfGameTalentChoices"))
            {
                StatEndOfGameTalentChoices(replay, stormTrackerEvent);
            }
            else if (valueSpan.SequenceEqual("EndOfGameTalentChoices"))
            {
                StatEndOfGameTalentChoices(replay, stormTrackerEvent);
            }
        }
    }

    private static void StatLevelUp(StormReplay replay, StormTrackerEvent stormTrackerEvent)
    {
        byte[]? playerIdValue = stormTrackerEvent.VersionedDecoder!.Structure?[2].OptionalData?.ArrayData?[0].Structure?[0].Structure?[0].Value;
        byte[]? levelValue = stormTrackerEvent.VersionedDecoder!.Structure?[2].OptionalData?.ArrayData?[1].Structure?[0].Structure?[0].Value;

        if (playerIdValue != null && levelValue != null)
        {
            Span<char> playerIdValueSpan = stackalloc char[playerIdValue.Length];
            Encoding.UTF8.GetChars(playerIdValue, playerIdValueSpan);

            Span<char> levelValueSpan = stackalloc char[levelValue.Length];
            Encoding.UTF8.GetChars(levelValue, levelValueSpan);

            if (playerIdValueSpan.SequenceEqual("PlayerID") &&
                levelValueSpan.SequenceEqual("Level"))
            {
                int playerId = (int)stormTrackerEvent.VersionedDecoder!.Structure![2].OptionalData!.ArrayData![0].Structure![1].GetValueAsUInt32();
                int level = (int)stormTrackerEvent.VersionedDecoder!.Structure![2].OptionalData!.ArrayData![1].Structure![1].GetValueAsUInt32();
                StormTeam team = replay.PlayersWithOpenSlots[playerId - 1].Team;

                Dictionary<int, StormTeamLevel>? teamLevel = replay.TeamLevelsInternal[(int)team] ??= new Dictionary<int, StormTeamLevel>();

                if (!teamLevel.ContainsKey(level))
                {
                    teamLevel.Add(level, new StormTeamLevel()
                    {
                        Level = level,
                        Time = stormTrackerEvent.Timestamp,
                    });
                }
            }
        }
    }

    private static void StatPeriodXPBreakdown(StormReplay replay, StormTrackerEvent stormTrackerEvent)
    {
        byte[]? teamLevelValue = stormTrackerEvent.VersionedDecoder?.Structure?[2].OptionalData?.ArrayData?[1].Structure?[0].Structure?[0].Value;
        byte[]? gameTimeValue = stormTrackerEvent.VersionedDecoder?.Structure?[3].OptionalData?.ArrayData?[0].Structure?[0].Structure?[0].Value;
        byte[]? previousGameTimeValue = stormTrackerEvent.VersionedDecoder?.Structure?[3].OptionalData?.ArrayData?[1].Structure?[0].Structure?[0].Value;
        byte[]? minionXPValue = stormTrackerEvent.VersionedDecoder?.Structure?[3].OptionalData?.ArrayData?[2].Structure?[0].Structure?[0].Value;
        byte[]? creepXPValue = stormTrackerEvent.VersionedDecoder?.Structure?[3].OptionalData?.ArrayData?[3].Structure?[0].Structure?[0].Value;
        byte[]? structureXPValue = stormTrackerEvent.VersionedDecoder?.Structure?[3].OptionalData?.ArrayData?[4].Structure?[0].Structure?[0].Value;
        byte[]? heroXPValue = stormTrackerEvent.VersionedDecoder?.Structure?[3].OptionalData?.ArrayData?[5].Structure?[0].Structure?[0].Value;
        byte[]? trickleXPValue = stormTrackerEvent.VersionedDecoder?.Structure?[3].OptionalData?.ArrayData?[6].Structure?[0].Structure?[0].Value;

        if (teamLevelValue != null && gameTimeValue != null && previousGameTimeValue != null && minionXPValue != null &&
            creepXPValue != null && structureXPValue != null && heroXPValue != null && trickleXPValue != null)
        {
            Span<char> teamLevelValueSpan = stackalloc char[teamLevelValue.Length];
            Encoding.UTF8.GetChars(teamLevelValue, teamLevelValueSpan);

            Span<char> gameTimeValueSpan = stackalloc char[gameTimeValue.Length];
            Encoding.UTF8.GetChars(gameTimeValue, gameTimeValueSpan);

            Span<char> previousGameTimeValueSpan = stackalloc char[previousGameTimeValue.Length];
            Encoding.UTF8.GetChars(previousGameTimeValue, previousGameTimeValueSpan);

            Span<char> minionXPValueSpan = stackalloc char[minionXPValue.Length];
            Encoding.UTF8.GetChars(minionXPValue, minionXPValueSpan);

            Span<char> creepXPValueSpan = stackalloc char[creepXPValue.Length];
            Encoding.UTF8.GetChars(creepXPValue, creepXPValueSpan);

            Span<char> structureXPValueSpan = stackalloc char[structureXPValue.Length];
            Encoding.UTF8.GetChars(structureXPValue, structureXPValueSpan);

            Span<char> heroXPValueSpan = stackalloc char[heroXPValue.Length];
            Encoding.UTF8.GetChars(heroXPValue, heroXPValueSpan);

            Span<char> trickleXPValueSpan = stackalloc char[trickleXPValue.Length];
            Encoding.UTF8.GetChars(trickleXPValue, trickleXPValueSpan);

            if (teamLevelValueSpan.SequenceEqual("TeamLevel") &&
                gameTimeValueSpan.SequenceEqual("GameTime") &&
                previousGameTimeValueSpan.SequenceEqual("PreviousGameTime") &&
                minionXPValueSpan.SequenceEqual("MinionXP") &&
                creepXPValueSpan.SequenceEqual("CreepXP") &&
                structureXPValueSpan.SequenceEqual("StructureXP") &&
                heroXPValueSpan.SequenceEqual("HeroXP") &&
                trickleXPValueSpan.SequenceEqual("TrickleXP"))
            {
                uint team = stormTrackerEvent.VersionedDecoder!.Structure![2].OptionalData!.ArrayData![0].Structure![1].GetValueAsUInt32() - 1;

                replay.TeamXPBreakdownInternal[team] ??= new List<StormTeamXPBreakdown>();
                replay.TeamXPBreakdownInternal[team].Add(new StormTeamXPBreakdown()
                {
                    Level = (int)stormTrackerEvent.VersionedDecoder!.Structure![2].OptionalData!.ArrayData![1].Structure![1].GetValueAsUInt32(),
                    Time = stormTrackerEvent.Timestamp,
                    MinionXP = (int)(stormTrackerEvent.VersionedDecoder!.Structure![3].OptionalData!.ArrayData![2].Structure![1].GetValueAsInt64() / 4096),
                    CreepXP = (int)(stormTrackerEvent.VersionedDecoder!.Structure![3].OptionalData!.ArrayData![3].Structure![1].GetValueAsInt64() / 4096),
                    StructureXP = (int)(stormTrackerEvent.VersionedDecoder!.Structure![3].OptionalData!.ArrayData![4].Structure![1].GetValueAsInt64() / 4096),
                    HeroXP = (int)(stormTrackerEvent.VersionedDecoder!.Structure![3].OptionalData!.ArrayData![5].Structure![1].GetValueAsInt64() / 4096),
                    PassiveXP = (int)(stormTrackerEvent.VersionedDecoder!.Structure![3].OptionalData!.ArrayData![6].Structure![1].GetValueAsInt64() / 4096),
                });
            }
        }
    }

    private static void StatEndOfGameXPBreakdown(StormReplay replay, StormTrackerEvent stormTrackerEvent)
    {
        byte[]? playerIdValue = stormTrackerEvent.VersionedDecoder?.Structure?[2].OptionalData?.ArrayData?[0].Structure?[0].Structure?[0].Value;
        byte[]? minionXPValue = stormTrackerEvent.VersionedDecoder?.Structure?[3].OptionalData?.ArrayData?[0].Structure?[0].Structure?[0].Value;
        byte[]? creepXPValue = stormTrackerEvent.VersionedDecoder?.Structure?[3].OptionalData?.ArrayData?[1].Structure?[0].Structure?[0].Value;
        byte[]? structureXPValue = stormTrackerEvent.VersionedDecoder?.Structure?[3].OptionalData?.ArrayData?[2].Structure?[0].Structure?[0].Value;
        byte[]? heroXPValue = stormTrackerEvent.VersionedDecoder?.Structure?[3].OptionalData?.ArrayData?[3].Structure?[0].Structure?[0].Value;
        byte[]? trickleXPValue = stormTrackerEvent.VersionedDecoder?.Structure?[3].OptionalData?.ArrayData?[4].Structure?[0].Structure?[0].Value;

        if (playerIdValue != null && minionXPValue != null && creepXPValue != null && structureXPValue != null &&
            heroXPValue != null && trickleXPValue != null)
        {
            Span<char> playerIdValueSpan = stackalloc char[playerIdValue.Length];
            Encoding.UTF8.GetChars(playerIdValue, playerIdValueSpan);

            Span<char> minionXPValueSpan = stackalloc char[minionXPValue.Length];
            Encoding.UTF8.GetChars(minionXPValue, minionXPValueSpan);

            Span<char> creepXPValueSpan = stackalloc char[creepXPValue.Length];
            Encoding.UTF8.GetChars(creepXPValue, creepXPValueSpan);

            Span<char> structureXPValueSpan = stackalloc char[structureXPValue.Length];
            Encoding.UTF8.GetChars(structureXPValue, structureXPValueSpan);

            Span<char> heroXPValueSpan = stackalloc char[heroXPValue.Length];
            Encoding.UTF8.GetChars(heroXPValue, heroXPValueSpan);

            Span<char> trickleXPValueSpan = stackalloc char[trickleXPValue.Length];
            Encoding.UTF8.GetChars(trickleXPValue, trickleXPValueSpan);

            if (playerIdValueSpan.SequenceEqual("PlayerID") &&
                minionXPValueSpan.SequenceEqual("MinionXP") &&
                creepXPValueSpan.SequenceEqual("CreepXP") &&
                structureXPValueSpan.SequenceEqual("StructureXP") &&
                heroXPValueSpan.SequenceEqual("HeroXP") &&
                trickleXPValueSpan.SequenceEqual("TrickleXP"))
            {
                uint playerId = stormTrackerEvent.VersionedDecoder!.Structure![2].OptionalData!.ArrayData![0].Structure![1].GetValueAsUInt32();
                StormTeam team = replay.PlayersWithOpenSlots[playerId - 1].Team;
                int teamNumber = (int)team;

                StormTeamXPBreakdown lastBreakdown = replay.TeamXPBreakdownInternal[teamNumber].Last();

                if (lastBreakdown.Time != stormTrackerEvent.Timestamp)
                {
                    replay.TeamXPBreakdownInternal[teamNumber] ??= new List<StormTeamXPBreakdown>();

                    replay.TeamXPBreakdownInternal[teamNumber].Add(new StormTeamXPBreakdown()
                    {
                        Level = replay.TeamLevelsInternal[teamNumber].Last().Key,
                        Time = stormTrackerEvent.Timestamp,
                        MinionXP = (int)(stormTrackerEvent.VersionedDecoder!.Structure![3].OptionalData!.ArrayData![0].Structure![1].GetValueAsInt64() / 4096),
                        CreepXP = (int)(stormTrackerEvent.VersionedDecoder!.Structure![3].OptionalData!.ArrayData![1].Structure![1].GetValueAsInt64() / 4096),
                        StructureXP = (int)(stormTrackerEvent.VersionedDecoder!.Structure![3].OptionalData!.ArrayData![2].Structure![1].GetValueAsInt64() / 4096),
                        HeroXP = (int)(stormTrackerEvent.VersionedDecoder!.Structure![3].OptionalData!.ArrayData![3].Structure![1].GetValueAsInt64() / 4096),
                        PassiveXP = (int)(stormTrackerEvent.VersionedDecoder!.Structure![3].OptionalData!.ArrayData![4].Structure![1].GetValueAsInt64() / 4096),
                    });
                }
            }
        }
    }

    private static void StatEndOfGameTalentChoices(StormReplay replay, StormTrackerEvent stormTrackerEvent)
    {
        if (!string.IsNullOrEmpty(replay.MapInfo.MapName) && stormTrackerEvent.VersionedDecoder!.Structure![1].OptionalData!.ArrayData!.Length > 2)
        {
            replay.MapInfo.MapId = stormTrackerEvent.VersionedDecoder!.Structure[1]!.OptionalData!.ArrayData![2]!.Structure![1].GetValueAsString();
        }

        byte[]? playerIdValue = stormTrackerEvent.VersionedDecoder?.Structure?[2].OptionalData?.ArrayData?[0].Structure?[0].Structure?[0].Value;
        byte[]? heroValue = stormTrackerEvent.VersionedDecoder?.Structure?[1].OptionalData?.ArrayData?[0].Structure?[0].Structure?[0].Value;

        if (playerIdValue != null && heroValue != null)
        {
            Span<char> playerIdValueSpan = stackalloc char[playerIdValue.Length];
            Encoding.UTF8.GetChars(playerIdValue, playerIdValueSpan);

            Span<char> heroValueSpan = stackalloc char[heroValue.Length];
            Encoding.UTF8.GetChars(heroValue, heroValueSpan);

            if (playerIdValueSpan.SequenceEqual("PlayerID") && heroValueSpan.SequenceEqual("Hero"))
            {
                StormPlayer player = replay.PlayersWithOpenSlots[stormTrackerEvent.VersionedDecoder!.Structure![2].OptionalData!.ArrayData![0].Structure![1].GetValueAsUInt32() - 1];

                player.PlayerHero!.HeroUnitId = stormTrackerEvent.VersionedDecoder!.Structure![1].OptionalData!.ArrayData![0].Structure![1].GetValueAsString();

                int arrayLength = stormTrackerEvent.VersionedDecoder!.Structure![1].OptionalData!.ArrayData!.Length;

                if (arrayLength >= 4)
                    AddTalentInfo(stormTrackerEvent, player, 3, "Tier 1 Choice", 3, 0);

                if (arrayLength >= 5)
                    AddTalentInfo(stormTrackerEvent, player, 4, "Tier 2 Choice", 4, 1);

                if (arrayLength >= 6)
                    AddTalentInfo(stormTrackerEvent, player, 5, "Tier 3 Choice", 5, 2);

                if (arrayLength >= 7)
                    AddTalentInfo(stormTrackerEvent, player, 6, "Tier 4 Choice", 6, 3);

                if (arrayLength >= 8)
                    AddTalentInfo(stormTrackerEvent, player, 7, "Tier 5 Choice", 7, 4);

                if (arrayLength >= 9)
                    AddTalentInfo(stormTrackerEvent, player, 8, "Tier 6 Choice", 8, 5);

                if (arrayLength >= 10)
                    AddTalentInfo(stormTrackerEvent, player, 9, "Tier 7 Choice", 9, 6);
            }
        }
    }

    private static void StatTalentChosen(StormReplay replay, StormTrackerEvent stormTrackerEvent)
    {
        byte[]? playerIdValue = stormTrackerEvent.VersionedDecoder?.Structure?[2].OptionalData?.ArrayData?[0].Structure?[0].Structure?[0].Value;
        byte[]? purchaseNameValue = stormTrackerEvent.VersionedDecoder?.Structure?[1].OptionalData?.ArrayData?[0].Structure?[0].Structure?[0].Value;

        if (playerIdValue != null && purchaseNameValue != null)
        {
            Span<char> playerIdValueSpan = stackalloc char[playerIdValue.Length];
            Encoding.UTF8.GetChars(playerIdValue, playerIdValueSpan);

            Span<char> purchaseNameValueSpan = stackalloc char[purchaseNameValue.Length];
            Encoding.UTF8.GetChars(purchaseNameValue, purchaseNameValueSpan);

            if (playerIdValueSpan.SequenceEqual("PlayerID") && purchaseNameValueSpan.SequenceEqual("PurchaseName"))
            {
                uint playerId = stormTrackerEvent.VersionedDecoder!.Structure![2].OptionalData!.ArrayData![0].Structure![1].GetValueAsUInt32();

                StormPlayer player = replay.PlayersWithOpenSlots[playerId - 1];

                if (player.TalentsInternal.Count > player.TalentSetCount)
                {
                    player.TalentsInternal[player.TalentSetCount].TalentNameId = stormTrackerEvent.VersionedDecoder.Structure[1].OptionalData!.ArrayData![0].Structure![1].GetValueAsString();
                    player.TalentSetCount++;
                }
            }
        }
    }

    private static ScoreResult GetScoreResult(int player, Dictionary<string, int?[]> scoreResultsByScoreName)
    {
        ScoreResult scoreResult = new();

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AddTalentInfo(StormTrackerEvent stormTrackerEvent, StormPlayer player, int tierStringIndex, string value, int talentNameIndex, int heroTalentIndex)
    {
        byte[] tierChoiceValue = stormTrackerEvent.VersionedDecoder!.Structure![1].OptionalData!.ArrayData![tierStringIndex].Structure![0].Structure![0].Value!;
        Span<char> tierChoiceValueSpan = stackalloc char[tierChoiceValue.Length];
        Encoding.UTF8.GetChars(tierChoiceValue, tierChoiceValueSpan);

        if (tierChoiceValueSpan.SequenceEqual(value))
        {
            if (player.TalentsInternal.Count <= heroTalentIndex)
                player.TalentsInternal.Add(new HeroTalent());

            player.Talents[heroTalentIndex].TalentNameId = stormTrackerEvent.VersionedDecoder.Structure![1].OptionalData!.ArrayData![talentNameIndex].Structure![1].GetValueAsString();
        }
    }
}
