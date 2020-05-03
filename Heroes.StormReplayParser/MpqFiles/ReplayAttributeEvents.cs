using Heroes.StormReplayParser.MpqHeroesTool;
using Heroes.StormReplayParser.Player;
using Heroes.StormReplayParser.Replay;
using System;

namespace Heroes.StormReplayParser.MpqFiles
{
    internal static class ReplayAttributeEvents
    {
        public static string FileName { get; } = "replay.attributes.events";

        public static void Parse(StormReplay replay, ReadOnlySpan<byte> source)
        {
            BitReader.ResetIndex();
            BitReader.EndianType = EndianType.LittleEndian;

            source.ReadAlignedByte();
            source.ReadUInt32Aligned();
            int count = source.ReadInt32Aligned();

            // The 'PlayerID' in attributes does not seem to match any existing player array
            // It almost matches the 'Replay.Player' array, except for games with less than 10 players
            int replayPlayersWithOpenSlotsIndex = 1;

            ReplayAttributeEventType attribute;
            int playerId;
            string value;

            for (int i = 0; i < count; i++)
            {
                source.ReadUInt32Aligned(); // namespace

                attribute = (ReplayAttributeEventType)source.ReadUInt32Aligned(); // attrid
                playerId = source.ReadAlignedByte();
                value = source.ReadStringFromBytes(4);

                switch (attribute)
                {
                    case ReplayAttributeEventType.PlayerTypeAttribute:
                        {
                            if (value.Equals("comp", StringComparison.OrdinalIgnoreCase) || value.Equals("humn", StringComparison.OrdinalIgnoreCase))
                            {
                                replay.PlayersWithOpenSlots[playerId - 1] = replay.Players[playerId - replayPlayersWithOpenSlotsIndex];
                            }

                            if (value.Equals("comp", StringComparison.OrdinalIgnoreCase))
                                replay.PlayersWithOpenSlots[playerId - 1].PlayerType = PlayerType.Computer;
                            else if (value.Equals("humn", StringComparison.OrdinalIgnoreCase))
                                replay.PlayersWithOpenSlots[playerId - 1].PlayerType = PlayerType.Human;
                            else if (value.Equals("open", StringComparison.OrdinalIgnoreCase))
                                replayPlayersWithOpenSlotsIndex++; // Less than 10 players in a Custom game
                            else
                                throw new StormParseException($"Unexpected value for PlayerTypeAttribute: {value}");

                            break;
                        }

                    case ReplayAttributeEventType.TeamSizeAttribute:
                        {
                            replay.TeamSize = value;
                            break;
                        }

                    case ReplayAttributeEventType.DifficultyLevelAttribute:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[playerId - 1];

                            if (player != null)
                            {
                                player.PlayerDifficulty = value switch
                                {
                                    "VyEy" => PlayerDifficulty.Beginner,
                                    "Easy" => PlayerDifficulty.Recruit,
                                    "Medi" => PlayerDifficulty.Adept,
                                    "HdVH" => PlayerDifficulty.Veteran,
                                    "VyHd" => PlayerDifficulty.Elite,
                                    _ => PlayerDifficulty.Unknown,
                                };
                            }

                            break;
                        }

                    case ReplayAttributeEventType.GameSpeedAttribute:
                        {
                            replay.GameSpeed = value switch
                            {
                                string _ when value.Equals("slor", StringComparison.OrdinalIgnoreCase) => GameSpeed.Slower,
                                string _ when value.Equals("slow", StringComparison.OrdinalIgnoreCase) => GameSpeed.Slow,
                                string _ when value.Equals("norm", StringComparison.OrdinalIgnoreCase) => GameSpeed.Normal,
                                string _ when value.Equals("fast", StringComparison.OrdinalIgnoreCase) => GameSpeed.Fast,
                                string _ when value.Equals("fasr", StringComparison.OrdinalIgnoreCase) => GameSpeed.Faster,
                                _ => GameSpeed.Unknown,
                            };

                            break;
                        }

                    case ReplayAttributeEventType.GameModeAttribute:
                        {
                            switch (value)
                            {
                                case string _ when value.Equals("priv", StringComparison.OrdinalIgnoreCase):
                                    replay.GameMode = GameMode.Custom;
                                    break;
                                case string _ when value.AsSpan(0, 3).Equals("amm", StringComparison.OrdinalIgnoreCase):
                                    if (replay.ReplayBuild < 33684)
                                        replay.GameMode = GameMode.QuickMatch;
                                    break;
                                default:
                                    throw new StormParseException($"Unexpected GameTypeAttribute: {value}");
                            }

                            break;
                        }

                    case ReplayAttributeEventType.HeroAttributeId:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[playerId - 1];

                            if (player != null)
                            {
                                player.IsAutoSelect = value == "Rand";
                                player.PlayerHero.HeroAttributeId = value;
                            }

                            break;
                        }

                    case ReplayAttributeEventType.SkinAndSkinTintAttributeId:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[playerId - 1];

                            if (player != null)
                            {
                                player.IsAutoSelect = value == "Rand";
                                player.PlayerLoadout.SkinAndSkinTintAttributeId = value;
                            }

                            break;
                        }

                    case ReplayAttributeEventType.MountAndMountTintAttributeId:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[playerId - 1];

                            if (player != null)
                            {
                                player.PlayerLoadout.MountAndMountTintAttributeId = value;
                            }

                            break;
                        }

                    case ReplayAttributeEventType.BannerAttributeId:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[playerId - 1];

                            if (player != null)
                            {
                                player.PlayerLoadout.BannerAttributeId = value;
                            }

                            break;
                        }

                    case ReplayAttributeEventType.SprayAttributeId:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[playerId - 1];

                            if (player != null)
                            {
                                player.PlayerLoadout.SprayAttributeId = value;
                            }

                            break;
                        }

                    case ReplayAttributeEventType.VoiceLineAttributeId:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[playerId - 1];

                            if (player != null)
                            {
                                player.PlayerLoadout.VoiceLineAttributeId = value;
                            }

                            break;
                        }

                    case ReplayAttributeEventType.AnnouncerAttributeId:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[playerId - 1];

                            if (player != null)
                            {
                                player.PlayerLoadout.AnnouncerPackAttributeId = value;
                            }

                            break;
                        }

                    case ReplayAttributeEventType.HeroLevel:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[playerId - 1];

                            if (player != null)
                            {
                                player.PlayerHero.HeroLevel = int.Parse(value);

                                if (player.IsAutoSelect && player.PlayerHero.HeroLevel > 1)
                                    player.IsAutoSelect = false;
                            }

                            break;
                        }

                    case ReplayAttributeEventType.LobbyMode:
                        {
                            if (replay.ReplayBuild < 43905 && replay.GameMode != GameMode.Custom)
                            {
                                switch (value)
                                {
                                    case string _ when value.Equals("stan", StringComparison.OrdinalIgnoreCase):
                                        replay.GameMode = GameMode.QuickMatch;
                                        break;
                                    case string _ when value.Equals("drft", StringComparison.OrdinalIgnoreCase):
                                        replay.GameMode = GameMode.HeroLeague;
                                        break;
                                    default:
                                        break;
                                }
                            }

                            break;
                        }

                    case ReplayAttributeEventType.ReadyMode:
                        {
                            if (replay.ReplayBuild < 43905 && replay.GameMode == GameMode.HeroLeague && value.Equals("fcfs", StringComparison.OrdinalIgnoreCase))
                                replay.GameMode = GameMode.TeamLeague;
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
                }
            }
        }
    }
}
