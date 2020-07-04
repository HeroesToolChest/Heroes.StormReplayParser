using Heroes.StormReplayParser.MpqHeroesTool;
using Heroes.StormReplayParser.Player;
using Heroes.StormReplayParser.Replay;
using System;
using System.Text;

namespace Heroes.StormReplayParser.MpqFiles
{
    internal static class ReplayAttributeEvents
    {
        public static string FileName { get; } = "replay.attributes.events";

        public static void Parse(StormReplay replay, ReadOnlySpan<byte> source)
        {
            BitReader bitReader = new BitReader(source, EndianType.LittleEndian);

            bitReader.ReadAlignedByte();
            bitReader.ReadUInt32Aligned();
            int count = bitReader.ReadInt32Aligned();

            // The 'PlayerID' in attributes does not seem to match any existing player array
            // It almost matches the 'Replay.Player' array, except for games with less than 10 players
            int replayPlayersWithOpenSlotsIndex = 1;

            ReplayAttributeEventType attribute;
            int playerId;
            Span<char> value = stackalloc char[4];
            Span<char> upperValue = stackalloc char[4];

            for (int i = 0; i < count; i++)
            {
                bitReader.ReadUInt32Aligned(); // namespace

                attribute = (ReplayAttributeEventType)bitReader.ReadUInt32Aligned(); // attrid
                playerId = bitReader.ReadAlignedByte();
                Encoding.UTF8.GetChars(bitReader.ReadBytes(4), value);

                value.Reverse();

                for (int j = 0; j < value.Length; j++)
                {
                    upperValue[j] = char.ToUpperInvariant(value[j]);
                }

                switch (attribute)
                {
                    case ReplayAttributeEventType.PlayerTypeAttribute:
                        {
                            if (upperValue.SequenceEqual("COMP") || upperValue.SequenceEqual("HUMN"))
                            {
                                replay.PlayersWithOpenSlots[playerId - 1] = replay.Players[playerId - replayPlayersWithOpenSlotsIndex];
                            }

                            if (upperValue.SequenceEqual("COMP"))
                                replay.PlayersWithOpenSlots[playerId - 1].PlayerType = PlayerType.Computer;
                            else if (upperValue.SequenceEqual("HUMN"))
                                replay.PlayersWithOpenSlots[playerId - 1].PlayerType = PlayerType.Human;
                            else if (upperValue.SequenceEqual("OPEN"))
                                replayPlayersWithOpenSlotsIndex++; // Less than 10 players in a Custom game
                            else
                                throw new StormParseException($"Unexpected value for PlayerTypeAttribute: {value.ToString()}");

                            break;
                        }

                    case ReplayAttributeEventType.TeamSizeAttribute:
                        {
                            replay.TeamSize = value.Trim('\0').ToString();
                            break;
                        }

                    case ReplayAttributeEventType.DifficultyLevelAttribute:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[playerId - 1];

                            if (player != null)
                            {
                                player.PlayerDifficulty = value.ToString() switch
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
                            replay.GameSpeed = upperValue switch
                            {
                                Span<char> _ when upperValue.SequenceEqual("SLOR") => GameSpeed.Slower,
                                Span<char> _ when upperValue.SequenceEqual("SLOW") => GameSpeed.Slow,
                                Span<char> _ when upperValue.SequenceEqual("NORM") => GameSpeed.Normal,
                                Span<char> _ when upperValue.SequenceEqual("FAST") => GameSpeed.Fast,
                                Span<char> _ when upperValue.SequenceEqual("FASR") => GameSpeed.Faster,
                                _ => GameSpeed.Unknown,
                            };

                            break;
                        }

                    case ReplayAttributeEventType.GameModeAttribute:
                        {
                            switch (upperValue)
                            {
                                case Span<char> _ when upperValue.SequenceEqual("PRIV"):
                                    replay.GameMode = GameMode.Custom;
                                    break;
                                case Span<char> _ when upperValue.Slice(1, 3).SequenceEqual("AMM"):
                                    if (replay.ReplayBuild < 33684)
                                        replay.GameMode = GameMode.QuickMatch;
                                    break;
                                default:
                                    throw new StormParseException($"Unexpected GameTypeAttribute: {value.ToString()}");
                            }

                            break;
                        }

                    case ReplayAttributeEventType.HeroAttributeId:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[playerId - 1];

                            if (player != null)
                            {
                                player.IsAutoSelect = upperValue.SequenceEqual("RAND");
                                player.PlayerHero.HeroAttributeId = value.Trim('\0').ToString();
                            }

                            break;
                        }

                    case ReplayAttributeEventType.SkinAndSkinTintAttributeId:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[playerId - 1];

                            if (player != null)
                            {
                                player.IsAutoSelect = upperValue.SequenceEqual("RAND");
                                player.PlayerLoadout.SkinAndSkinTintAttributeId = value.Trim('\0').ToString();
                            }

                            break;
                        }

                    case ReplayAttributeEventType.MountAndMountTintAttributeId:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[playerId - 1];

                            if (player != null)
                            {
                                player.PlayerLoadout.MountAndMountTintAttributeId = value.Trim('\0').ToString();
                            }

                            break;
                        }

                    case ReplayAttributeEventType.BannerAttributeId:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[playerId - 1];

                            if (player != null)
                            {
                                player.PlayerLoadout.BannerAttributeId = value.Trim('\0').ToString();
                            }

                            break;
                        }

                    case ReplayAttributeEventType.SprayAttributeId:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[playerId - 1];

                            if (player != null)
                            {
                                player.PlayerLoadout.SprayAttributeId = value.Trim('\0').ToString();
                            }

                            break;
                        }

                    case ReplayAttributeEventType.VoiceLineAttributeId:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[playerId - 1];

                            if (player != null)
                            {
                                player.PlayerLoadout.VoiceLineAttributeId = value.Trim('\0').ToString();
                            }

                            break;
                        }

                    case ReplayAttributeEventType.AnnouncerAttributeId:
                        {
                            StormPlayer player = replay.PlayersWithOpenSlots[playerId - 1];

                            if (player != null)
                            {
                                player.PlayerLoadout.AnnouncerPackAttributeId = value.Trim('\0').ToString();
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
                                    case Span<char> _ when upperValue.SequenceEqual("STAN"):
                                        replay.GameMode = GameMode.QuickMatch;
                                        break;
                                    case Span<char> _ when upperValue.SequenceEqual("DRFT"):
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
                            if (replay.ReplayBuild < 43905 && replay.GameMode == GameMode.HeroLeague && upperValue.SequenceEqual("FCFS"))
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
                                replay.TeamHeroAttributeIdBans[0][0] = value.Trim('\0').ToString();
                                break;
                            case ReplayAttributeEventType.DraftTeam1Ban2:
                                replay.TeamHeroAttributeIdBans[0][1] = value.Trim('\0').ToString();
                                break;
                            case ReplayAttributeEventType.DraftTeam1Ban3:
                                replay.TeamHeroAttributeIdBans[0][2] = value.Trim('\0').ToString();
                                break;
                            case ReplayAttributeEventType.DraftTeam2Ban1:
                                replay.TeamHeroAttributeIdBans[1][0] = value.Trim('\0').ToString();
                                break;
                            case ReplayAttributeEventType.DraftTeam2Ban2:
                                replay.TeamHeroAttributeIdBans[1][1] = value.Trim('\0').ToString();
                                break;
                            case ReplayAttributeEventType.DraftTeam2Ban3:
                                replay.TeamHeroAttributeIdBans[1][2] = value.Trim('\0').ToString();
                                break;
                        }

                        break;
                }
            }
        }
    }
}
