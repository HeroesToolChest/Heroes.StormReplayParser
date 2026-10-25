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
                case ReplayAttributeEventType.Controller:
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
                        if (GetSlotPlayer(replay, playerId) is not StormPlayer player)
                            break;

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
                        if (GetSlotPlayer(replay, playerId) is not StormPlayer player)
                            break;

                        if (!player.IsAutoSelect)
                            player.IsAutoSelect = value == "rand";

                        player.PlayerHero!.HeroAttributeId = value;

                        break;
                    }

                case ReplayAttributeEventType.Skin:
                    {
                        if (GetSlotPlayer(replay, playerId) is not StormPlayer player)
                            break;

                        if (!player.IsAutoSelect)
                            player.IsAutoSelect = value == "rand";

                        player.PlayerLoadout.SkinAndSkinTintAttributeId = value;

                        break;
                    }

                case ReplayAttributeEventType.Mount:
                    {
                        if (GetSlotPlayer(replay, playerId) is not StormPlayer player)
                            break;

                        player.PlayerLoadout.MountAndMountTintAttributeId = value;

                        break;
                    }

                case ReplayAttributeEventType.Banner:
                    {
                        if (GetSlotPlayer(replay, playerId) is not StormPlayer player)
                            break;

                        player.PlayerLoadout.BannerAttributeId = value;

                        break;
                    }

                case ReplayAttributeEventType.Spray:
                    {
                        if (GetSlotPlayer(replay, playerId) is not StormPlayer player)
                            break;

                        player.PlayerLoadout.SprayAttributeId = value;

                        break;
                    }

                case ReplayAttributeEventType.VoiceLine:
                    {
                        if (GetSlotPlayer(replay, playerId) is not StormPlayer player)
                            break;

                        player.PlayerLoadout.VoiceLineAttributeId = value;

                        break;
                    }

                case ReplayAttributeEventType.AnnouncerPack:
                    {
                        if (GetSlotPlayer(replay, playerId) is not StormPlayer player)
                            break;

                        player.PlayerLoadout.AnnouncerPackAttributeId = value;

                        break;
                    }

                case ReplayAttributeEventType.HeroLevel:
                    {
                        if (GetSlotPlayer(replay, playerId) is not StormPlayer player)
                            break;

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

                case ReplayAttributeEventType.Team1Ban1HeroChoice:
                case ReplayAttributeEventType.Team1Ban2HeroChoice:
                case ReplayAttributeEventType.Team1Ban3HeroChoice:
                case ReplayAttributeEventType.Team2Ban1HeroChoice:
                case ReplayAttributeEventType.Team2Ba2HeroChoice:
                case ReplayAttributeEventType.Team2Ban3HeroChoice:
                    switch (attribute)
                    {
                        case ReplayAttributeEventType.Team1Ban1HeroChoice:
                            replay.TeamHeroAttributeIdBans[0][0] = value;
                            break;
                        case ReplayAttributeEventType.Team1Ban2HeroChoice:
                            replay.TeamHeroAttributeIdBans[0][1] = value;
                            break;
                        case ReplayAttributeEventType.Team1Ban3HeroChoice:
                            replay.TeamHeroAttributeIdBans[0][2] = value;
                            break;
                        case ReplayAttributeEventType.Team2Ban1HeroChoice:
                            replay.TeamHeroAttributeIdBans[1][0] = value;
                            break;
                        case ReplayAttributeEventType.Team2Ba2HeroChoice:
                            replay.TeamHeroAttributeIdBans[1][1] = value;
                            break;
                        case ReplayAttributeEventType.Team2Ban3HeroChoice:
                            replay.TeamHeroAttributeIdBans[1][2] = value;
                            break;
                    }

                    break;

                case ReplayAttributeEventType.Team1BanChooser:
                    {
                        replay.TeamBanChooserTypes[0] = SetBanChooser(replay, value, out StormPlayer? stormPlayer);
                        replay.TeamBanPlayerChooser[0] = stormPlayer;

                        break;
                    }

                case ReplayAttributeEventType.Team2BanChooser:
                    {
                        replay.TeamBanChooserTypes[1] = SetBanChooser(replay, value, out StormPlayer? stormPlayer);
                        replay.TeamBanPlayerChooser[1] = stormPlayer;

                        break;
                    }

                case ReplayAttributeEventType.FirstDraftTeam:
                    {
                        replay.FirstDraftTeam = GetFirstDraftTeam(value);

                        break;
                    }

                case ReplayAttributeEventType.GamePrivacy:
                    {
                        replay.GamePrivacy = GetGamePrivacy(value);

                        break;
                    }

                case ReplayAttributeEventType.BanMode:
                    {
                        replay.BanMode = GetBanMode(value);

                        break;
                    }

                default:
                    break;
            }
        }
    }

    // the value is "Hmmr" (ranked games), a player slot (m_workingSetSlotId) for custom games, or 22 when bans are not used
    internal static StormBanChooserType GetBanChooser(string value, out int? playerSlot)
    {
        playerSlot = null;

        if (value == "Hmmr")
            return StormBanChooserType.HighestMmr;

        if (!int.TryParse(value, out int slot))
            return StormBanChooserType.Unknown;

        if (slot == 22)
            return StormBanChooserType.None;

        // 16 total player slots
        if (slot < 0 || slot >= 16)
            return StormBanChooserType.Unknown;

        playerSlot = slot;

        return StormBanChooserType.Player;
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

    internal static StormGamePrivacy GetGamePrivacy(string value) => value switch
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

    internal static StormBanMode GetBanMode(string value) => value switch
    {
        "" => StormBanMode.NotUsingBans,
        "1ban" => StormBanMode.OneBan,
        "2ban" => StormBanMode.TwoBan,
        "Mban" => StormBanMode.MidBan,
        "3ban" => StormBanMode.ThreeBan,

        _ => StormBanMode.Unknown,
    };

    internal static StormFirstDraftTeam GetFirstDraftTeam(string value) => value switch
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

    private static StormBanChooserType SetBanChooser(StormReplay replay, string value, out StormPlayer? stormPlayer)
    {
        StormBanChooserType banChooser = GetBanChooser(value, out int? playerSlot);

        if (playerSlot is null)
        {
            stormPlayer = null;
            return banChooser;
        }

        // the ban chooser is a participant, so they will be in the replay.details
        StormPlayer? player = replay.ClientListByWorkingSetSlotID[playerSlot.Value];

        if (player is not null)
            player.IsBanChooser = true;

        stormPlayer = player;
        return banChooser;
    }

    // open slots do not have a player, but can still have per player attributes, such as in Sandbox games
    private static StormPlayer? GetSlotPlayer(StormReplay replay, int playerId)
    {
        if (playerId >= 1 && playerId <= replay.PlayersWithOpenSlots.Length)
            return replay.PlayersWithOpenSlots[playerId - 1];
        else
            return null;
    }
}
