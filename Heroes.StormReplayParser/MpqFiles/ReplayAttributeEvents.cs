namespace Heroes.StormReplayParser.MpqFiles;

internal static class ReplayAttributeEvents
{
    public static string FileName { get; } = "replay.attributes.events";

    public static void Parse(StormReplay replay, ReadOnlySpan<byte> source)
    {
        BitReader bitReader = new(source, EndianType.LittleEndian);

        bitReader.ReadAlignedByte();
        bitReader.ReadUInt32Aligned();
        int count = bitReader.ReadInt32Aligned();

        // The 'PlayerID' in attributes does not seem to match any existing player array
        // It almost matches the 'Replay.Player' array, except for games with less than 10 players
        int replayPlayersWithOpenSlotsIndex = 1;

        ReplayAttributeEventType attribute;
        int playerId;

        for (int i = 0; i < count; i++)
        {
            bitReader.ReadUInt32Aligned(); // namespace

            attribute = (ReplayAttributeEventType)bitReader.ReadUInt32Aligned(); // attrid
            playerId = bitReader.ReadAlignedByte();

            string value = bitReader.ReadStringFromAlignedBytes(4);

            switch (attribute)
            {
                case ReplayAttributeEventType.PlayerType:
                    {
                        if (value == "Comp" || value == "Humn")
                        {
                            replay.PlayersWithOpenSlots[playerId - 1] = replay.Players[playerId - replayPlayersWithOpenSlotsIndex];
                        }

                        if (value == "Comp")
                            replay.PlayersWithOpenSlots[playerId - 1]!.PlayerType = PlayerType.Computer;
                        else if (value == "Humn")
                            replay.PlayersWithOpenSlots[playerId - 1]!.PlayerType = PlayerType.Human;
                        else if (value == "Open")
                            replayPlayersWithOpenSlotsIndex++; // Less than 10 players in a Custom game
                        else
                            throw new StormParseException($"Unexpected value for PlayerTypeAttribute: {value}");

                        break;
                    }

                case ReplayAttributeEventType.TeamSize:
                    {
                        replay.TeamSize = value;
                        break;
                    }

                case ReplayAttributeEventType.DifficultyLevel:
                    {
                        StormPlayer player = replay.PlayersWithOpenSlots[playerId - 1]!;

                        player.ComputerDifficulty = GetComputerDifficultyLevel(value);

                        break;
                    }

                case ReplayAttributeEventType.GameSpeed:
                    {
                        replay.GameSpeed = GetGameSpeed(value);

                        break;
                    }

                case ReplayAttributeEventType.GameMode:
                    {
                        switch (value)
                        {
                            case "Priv":
                                replay.GameMode = StormGameMode.Custom;
                                break;
                            case "Amm":
                                if (replay.ReplayBuild < 33684)
                                    replay.GameMode = StormGameMode.QuickMatch;
                                break;
                            case "Pub":
                                break;
                            default:
                                throw new StormParseException($"Unexpected GameMode attribute: {value}");
                        }

                        break;
                    }

                case ReplayAttributeEventType.HeroAttributeId:
                    {
                        StormPlayer player = replay.PlayersWithOpenSlots[playerId - 1]!;

                        if (!player.IsAutoSelect)
                            player.IsAutoSelect = value == "rand";

                        player.PlayerHero!.HeroAttributeId = value;

                        break;
                    }

                case ReplayAttributeEventType.SkinAndSkinTint:
                    {
                        StormPlayer player = replay.PlayersWithOpenSlots[playerId - 1]!;

                        if (!player.IsAutoSelect)
                            player.IsAutoSelect = value == "rand";

                        player.PlayerLoadout.SkinAndSkinTintAttributeId = value;

                        break;
                    }

                case ReplayAttributeEventType.MountAndMountTint:
                    {
                        StormPlayer player = replay.PlayersWithOpenSlots[playerId - 1]!;

                        player.PlayerLoadout.MountAndMountTintAttributeId = value;

                        break;
                    }

                case ReplayAttributeEventType.Banner:
                    {
                        StormPlayer player = replay.PlayersWithOpenSlots[playerId - 1]!;

                        player.PlayerLoadout.BannerAttributeId = value;

                        break;
                    }

                case ReplayAttributeEventType.Spray:
                    {
                        StormPlayer player = replay.PlayersWithOpenSlots[playerId - 1]!;

                        player.PlayerLoadout.SprayAttributeId = value;

                        break;
                    }

                case ReplayAttributeEventType.VoiceLine:
                    {
                        StormPlayer player = replay.PlayersWithOpenSlots[playerId - 1]!;

                        player.PlayerLoadout.VoiceLineAttributeId = value;

                        break;
                    }

                case ReplayAttributeEventType.Announcer:
                    {
                        StormPlayer player = replay.PlayersWithOpenSlots[playerId - 1]!;

                        player.PlayerLoadout.AnnouncerPackAttributeId = value;

                        break;
                    }

                case ReplayAttributeEventType.HeroLevel:
                    {
                        StormPlayer player = replay.PlayersWithOpenSlots[playerId - 1]!;

                        player.PlayerHero!.HeroLevel = int.Parse(value);

                        if (player.IsAutoSelect && player.PlayerHero.HeroLevel > 1)
                            player.IsAutoSelect = false;

                        break;
                    }

                case ReplayAttributeEventType.LobbyMode:
                    {
                        if (replay.ReplayBuild < 43905 && replay.GameMode != StormGameMode.Custom)
                        {
                            replay.GameMode = value switch
                            {
                                "stan" => StormGameMode.QuickMatch,
                                "drft" => StormGameMode.HeroLeague,

                                _ => StormGameMode.Unknown,
                            };
                        }
                        else
                        {
                            replay.LobbyMode = GetLobbyMode(value);
                        }

                        break;
                    }

                case ReplayAttributeEventType.ReadyMode:
                    {
                        if (replay.ReplayBuild < 43905 && replay.GameMode == StormGameMode.HeroLeague && value == "fcfs")
                            replay.GameMode = StormGameMode.TeamLeague;
                        else
                            replay.ReadyMode = GetReadyMode(value);

                        break;
                    }

                case ReplayAttributeEventType.DraftTeam1Ban1:
                case ReplayAttributeEventType.DraftTeam1Ban2:
                case ReplayAttributeEventType.DraftTeam1Ban3:
                case ReplayAttributeEventType.DraftTeam2Ban1:
                case ReplayAttributeEventType.DraftTeam2Ban2:
                case ReplayAttributeEventType.DraftTeam2Ban3:
                    switch (attribute)
                    {
                        case ReplayAttributeEventType.DraftTeam1Ban1:
                            replay.TeamHeroAttributeIdBans[0][0] = value;
                            break;
                        case ReplayAttributeEventType.DraftTeam1Ban2:
                            replay.TeamHeroAttributeIdBans[0][1] = value;
                            break;
                        case ReplayAttributeEventType.DraftTeam1Ban3:
                            replay.TeamHeroAttributeIdBans[0][2] = value;
                            break;
                        case ReplayAttributeEventType.DraftTeam2Ban1:
                            replay.TeamHeroAttributeIdBans[1][0] = value;
                            break;
                        case ReplayAttributeEventType.DraftTeam2Ban2:
                            replay.TeamHeroAttributeIdBans[1][1] = value;
                            break;
                        case ReplayAttributeEventType.DraftTeam2Ban3:
                            replay.TeamHeroAttributeIdBans[1][2] = value;
                            break;
                    }

                    break;

                case ReplayAttributeEventType.FirstReadyingTeam:
                    {
                        replay.FirstDraftTeam = GetFirstReadyingTeam(value);

                        break;
                    }

                case ReplayAttributeEventType.PrivacyOption:
                    {
                        replay.GamePrivacy = GetPrivacyOption(value);

                        break;
                    }

                case ReplayAttributeEventType.DraftBanMode:
                    {
                        replay.BanMode = GetDraftBanMode(value);

                        break;
                    }

                default:
                    break;
            }
        }
    }

    internal static StormLobbyMode GetLobbyMode(string value) => value switch
    {
        "stan" => StormLobbyMode.Standard,
        "drft" => StormLobbyMode.Draft,
        "tour" => StormLobbyMode.TournamentDraft,

        _ => StormLobbyMode.Unknown,
    };

    internal static StormGameSpeed GetGameSpeed(string value) => value switch
    {
        "Slor" => StormGameSpeed.Slower,
        "Slow" => StormGameSpeed.Slow,
        "Norm" => StormGameSpeed.Normal,
        "Fast" => StormGameSpeed.Fast,
        "Fasr" => StormGameSpeed.Faster,

        _ => StormGameSpeed.Unknown,
    };

    internal static StormGamePrivacy GetPrivacyOption(string value) => value switch
    {
        "Norm" => StormGamePrivacy.Normal,
        "NoMH" => StormGamePrivacy.NoMatchHistory,

        _ => StormGamePrivacy.Unknown,
    };

    internal static StormReadyMode GetReadyMode(string value) => value switch
    {
        "fcfs" => StormReadyMode.FCFS,
        "pred" => StormReadyMode.Predetermined,

        _ => StormReadyMode.Unknown,
    };

    internal static StormBanMode GetDraftBanMode(string value) => value switch
    {
        "" => StormBanMode.NotUsingBans,
        "1ban" => StormBanMode.OneBan,
        "2ban" => StormBanMode.TwoBan,
        "Mban" => StormBanMode.MidBan,
        "3ban" => StormBanMode.ThreeBan,

        _ => StormBanMode.Unknown,
    };

    internal static StormFirstDraftTeam GetFirstReadyingTeam(string value) => value switch
    {
        "" => StormFirstDraftTeam.CoinToss,
        "T1" => StormFirstDraftTeam.Team1,
        "T2" => StormFirstDraftTeam.Team2,

        _ => StormFirstDraftTeam.Unknown,
    };

    internal static PlayerSlotType GetPlayerType(string value) => value switch
    {
        "Clsd" => PlayerSlotType.Closed,
        "Open" => PlayerSlotType.Open,
        "Humn" => PlayerSlotType.Human,
        "Comp" => PlayerSlotType.Computer,

        _ => PlayerSlotType.Unknown,
    };

    internal static PlayerType GetParticipantRole(string value) => value switch
    {
        "Part" => PlayerType.Human,
        "Watc" => PlayerType.Observer,

        _ => PlayerType.Unknown,
    };

    internal static ComputerDifficulty GetComputerDifficultyLevel(string value) => value switch
    {
        "VyEy" => ComputerDifficulty.Beginner,
        "Easy" => ComputerDifficulty.Recruit,
        "Medi" => ComputerDifficulty.Adept,
        "HdVH" => ComputerDifficulty.Veteran,
        "VyHd" => ComputerDifficulty.Elite,

        _ => ComputerDifficulty.Unknown,
    };
}
