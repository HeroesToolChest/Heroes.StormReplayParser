using Heroes.StormReplayParser.MpqHeroesTool;
using Heroes.StormReplayParser.Player;
using Heroes.StormReplayParser.Replay;
using System;
using System.Linq;

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

            Span<ReplayAttribute> attributes = new ReplayAttribute[count];

            for (int i = 0; i < count; i++)
            {
                source.ReadUInt32Aligned(); // namespace

                attributes[i].AttributeType = (ReplayAttributeEventType)source.ReadUInt32Aligned(); // attrid
                attributes[i].PlayerId = source.ReadAlignedByte();
                attributes[i].Value = source.ReadStringFromBytes(4);
            }

            // The 'PlayerID' in attributes does not seem to match any existing player array
            // It almost matches the 'Replay.Player' array, except for games with less than 10 players
            int replayPlayersWithOpenSlotsIndex = 1;

            Span<char> valueAsLoweredSpan = stackalloc char[4]; // hold a lowered attribute value

            foreach (ReplayAttribute attribute in attributes)
            {
                valueAsLoweredSpan.Clear();
                attribute.Value.AsSpan().ToLowerInvariant(valueAsLoweredSpan);

                switch (attribute.AttributeType)
                {
                    case ReplayAttributeEventType.PlayerTypeAttribute:
                        {
                            if (valueAsLoweredSpan.SequenceEqual("comp") || valueAsLoweredSpan.SequenceEqual("humn"))
                            {
                                replay.PlayersWithOpenSlots[attribute.PlayerId - 1] = replay.Players[attribute.PlayerId - replayPlayersWithOpenSlotsIndex];
                            }

                            if (valueAsLoweredSpan.SequenceEqual("comp"))
                                replay.PlayersWithOpenSlots[attribute.PlayerId - 1].PlayerType = PlayerType.Computer;
                            else if (valueAsLoweredSpan.SequenceEqual("humn"))
                                replay.PlayersWithOpenSlots[attribute.PlayerId - 1].PlayerType = PlayerType.Human;
                            else if (valueAsLoweredSpan.SequenceEqual("open"))
                                replayPlayersWithOpenSlotsIndex++; // Less than 10 players in a Custom game
                            else
                                throw new StormParseException($"Unexpected value for PlayerTypeAttribute: {attribute.Value}");

                            break;
                        }

                    case ReplayAttributeEventType.TeamSizeAttribute:
                        {
                            replay.TeamSize = attribute.Value;
                            break;
                        }

                    case ReplayAttributeEventType.DifficultyLevelAttribute:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[attribute.PlayerId - 1];

                            if (player != null)
                            {
                                player.PlayerDifficulty = attribute.Value switch
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
                            replay.GameSpeed = valueAsLoweredSpan switch
                            {
                                Span<char> _ when valueAsLoweredSpan.SequenceEqual("slor") => GameSpeed.Slower,
                                Span<char> _ when valueAsLoweredSpan.SequenceEqual("slow") => GameSpeed.Slow,
                                Span<char> _ when valueAsLoweredSpan.SequenceEqual("norm") => GameSpeed.Normal,
                                Span<char> _ when valueAsLoweredSpan.SequenceEqual("fast") => GameSpeed.Fast,
                                Span<char> _ when valueAsLoweredSpan.SequenceEqual("fasr") => GameSpeed.Faster,
                                _ => GameSpeed.Unknown,
                            };

                            break;
                        }

                    case ReplayAttributeEventType.GameModeAttribute:
                        {
                            switch (valueAsLoweredSpan)
                            {
                                case Span<char> _ when valueAsLoweredSpan.SequenceEqual("priv"):
                                    replay.GameMode = GameMode.Custom;
                                    break;
                                case Span<char> _ when valueAsLoweredSpan.SequenceEqual("amm\0"):
                                    if (replay.ReplayBuild < 33684)
                                        replay.GameMode = GameMode.QuickMatch;
                                    break;
                                default:
                                    throw new StormParseException($"Unexpected GameTypeAttribute: {attribute.Value}");
                            }

                            break;
                        }

                    case ReplayAttributeEventType.HeroAttributeId:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[attribute.PlayerId - 1];

                            if (player != null)
                            {
                                player.IsAutoSelect = attribute.Value == "Rand";
                                player.PlayerHero.HeroAttributeId = attribute.Value;
                            }

                            break;
                        }

                    case ReplayAttributeEventType.SkinAndSkinTintAttributeId:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[attribute.PlayerId - 1];

                            if (player != null)
                            {
                                player.IsAutoSelect = attribute.Value == "Rand";
                                player.PlayerLoadout.SkinAndSkinTintAttributeId = attribute.Value;
                            }

                            break;
                        }

                    case ReplayAttributeEventType.MountAndMountTintAttributeId:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[attribute.PlayerId - 1];

                            if (player != null)
                            {
                                player.PlayerLoadout.MountAndMountTintAttributeId = attribute.Value;
                            }

                            break;
                        }

                    case ReplayAttributeEventType.BannerAttributeId:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[attribute.PlayerId - 1];

                            if (player != null)
                            {
                                player.PlayerLoadout.BannerAttributeId = attribute.Value;
                            }

                            break;
                        }

                    case ReplayAttributeEventType.SprayAttributeId:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[attribute.PlayerId - 1];

                            if (player != null)
                            {
                                player.PlayerLoadout.SprayAttributeId = attribute.Value;
                            }

                            break;
                        }

                    case ReplayAttributeEventType.VoiceLineAttributeId:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[attribute.PlayerId - 1];

                            if (player != null)
                            {
                                player.PlayerLoadout.VoiceLineAttributeId = attribute.Value;
                            }

                            break;
                        }

                    case ReplayAttributeEventType.AnnouncerAttributeId:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[attribute.PlayerId - 1];

                            if (player != null)
                            {
                                player.PlayerLoadout.AnnouncerPackAttributeId = attribute.Value;
                            }

                            break;
                        }

                    case ReplayAttributeEventType.HeroLevel:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[attribute.PlayerId - 1];

                            if (player != null)
                            {
                                player.PlayerHero.HeroLevel = int.Parse(attribute.Value);

                                if (player.IsAutoSelect && player.PlayerHero.HeroLevel > 1)
                                    player.IsAutoSelect = false;
                            }

                            break;
                        }

                    case ReplayAttributeEventType.LobbyMode:
                        {
                            if (replay.ReplayBuild < 43905 && replay.GameMode != GameMode.Custom)
                            {
                                switch (valueAsLoweredSpan)
                                {
                                    case Span<char> _ when valueAsLoweredSpan.SequenceEqual("stan"):
                                        replay.GameMode = GameMode.QuickMatch;
                                        break;
                                    case Span<char> _ when valueAsLoweredSpan.SequenceEqual("drft"):
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
                            if (replay.ReplayBuild < 43905 && replay.GameMode == GameMode.HeroLeague && valueAsLoweredSpan.SequenceEqual("fcfs"))
                                replay.GameMode = GameMode.TeamLeague;
                            break;
                        }

                    case ReplayAttributeEventType.DraftTeam1Ban1:
                    case ReplayAttributeEventType.DraftTeam1Ban2:
                    case ReplayAttributeEventType.DraftTeam1Ban3:
                    case ReplayAttributeEventType.DraftTeam2Ban1:
                    case ReplayAttributeEventType.DraftTeam2Ban2:
                    case ReplayAttributeEventType.DraftTeam2Ban3:
                        switch (attribute.AttributeType)
                        {
                            case ReplayAttributeEventType.DraftTeam1Ban1:
                                replay.TeamHeroAttributeIdBans[0][0] = attribute.Value;
                                break;
                            case ReplayAttributeEventType.DraftTeam1Ban2:
                                replay.TeamHeroAttributeIdBans[0][1] = attribute.Value;
                                break;
                            case ReplayAttributeEventType.DraftTeam1Ban3:
                                replay.TeamHeroAttributeIdBans[0][2] = attribute.Value;
                                break;
                            case ReplayAttributeEventType.DraftTeam2Ban1:
                                replay.TeamHeroAttributeIdBans[1][0] = attribute.Value;
                                break;
                            case ReplayAttributeEventType.DraftTeam2Ban2:
                                replay.TeamHeroAttributeIdBans[1][1] = attribute.Value;
                                break;
                            case ReplayAttributeEventType.DraftTeam2Ban3:
                                replay.TeamHeroAttributeIdBans[1][2] = attribute.Value;
                                break;
                        }

                        break;
                }
            }
        }
    }
}
