using Heroes.StormReplayParser;
using Heroes.StormReplayParser.Player;
using Heroes.StormReplayParser.Replay;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Text;

namespace HeroesDecode
{
    public static class Program
    {
        private static bool _showPlayerTalents = false;

        public static int Main(string[] args)
        {
            RootCommand rootCommand = new RootCommand()
            {
                new Option<string>(
                    "--replay-file-path",
                    description: "File path of a Heroes of the Storm .StormReplay file")
                {
                    IsRequired = true,
                },
                new Option<bool>(
                    "--show-player-talents",
                    getDefaultValue: () => false,
                    description: "Shows the player's talent information"),
            };

            rootCommand.Description = "Parses Heroes of the Storm replay files";

            rootCommand.Handler = CommandHandler.Create<string, bool>((replayFilePath, showPlayerTalents) =>
            {
                _showPlayerTalents = showPlayerTalents;

                StormReplayResult stormReplayResult = StormReplay.Parse(replayFilePath, new ParseOptions()
                {
                    AllowPTR = true,
                    ShouldParseTrackerEvents = true,
                    ShouldParseGameEvents = true,
                    ShouldParseMessageEvents = true,
                });

                if (stormReplayResult.Status == StormReplayParseStatus.Success)
                    Console.ForegroundColor = ConsoleColor.Green;
                else
                    Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine(stormReplayResult.Status);
                Console.ResetColor();

                if (stormReplayResult.Status != StormReplayParseStatus.Success && stormReplayResult.Exception != null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(stormReplayResult.Exception.StackTrace);
                    Console.ResetColor();
                }

                GetInfo(stormReplayResult.Replay);
            });

            return rootCommand.InvokeAsync(args).Result;
        }

        private static void GetInfo(StormReplay replay)
        {
            List<StormPlayer> players = replay.StormPlayers.ToList();

            Console.WriteLine($"{"Game Mode: ",11}{replay.GameMode}");
            Console.WriteLine($"{"Map: ",11}{replay.MapInfo.MapName} [ID:{replay.MapInfo.MapId}]");
            Console.WriteLine($"{"Version: ",11}{replay.ReplayVersion}");
            Console.WriteLine($"{"Region: ",11}{replay.Region}");
            Console.WriteLine($"{"Game Time: ",11}{replay.ReplayLength}");

            IEnumerable<StormPlayer> blueTeam = players.Where(x => x.Team == StormTeam.Blue);
            IEnumerable<StormPlayer> redTeam = players.Where(x => x.Team == StormTeam.Red);

            List<StormPlayer> playersWithObservers = replay.StormObservers.ToList();

            StormTeamDisplay(replay, blueTeam, StormTeam.Blue);
            StormTeamDisplay(replay, redTeam, StormTeam.Red);
            StormTeamDisplay(replay, playersWithObservers, StormTeam.Observer);
        }

        private static void StormTeamDisplay(StormReplay replay, IEnumerable<StormPlayer> players, StormTeam team)
        {
            Dictionary<long, PartyIconColor> partyPlayers = new Dictionary<long, PartyIconColor>();
            bool partyPurpleUsed = false;
            bool partyRedUsed = false;

            Console.WriteLine();

            string teamName = string.Empty;

            if (team == StormTeam.Blue)
                teamName = "Team Blue";
            else if (team == StormTeam.Red)
                teamName = "Team Red";
            else if (team == StormTeam.Observer)
                teamName = "Observers";

            if ((replay.WinningTeam == StormTeam.Blue && team == StormTeam.Blue) ||
                (replay.WinningTeam == StormTeam.Red && team == StormTeam.Red))
                teamName += " (Winner)";

            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine(teamName);
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            if (players.Any())
            {
                foreach (StormPlayer player in players)
                {
                    PartyIconColor? partyIcon = null;

                    if (player.PartyValue.HasValue)
                    {
                        if (partyPlayers.TryGetValue(player.PartyValue.Value, out PartyIconColor partyIconColor))
                        {
                            partyIcon = partyIconColor;
                        }
                        else
                        {
                            if (player.Team == StormTeam.Blue)
                            {
                                if (partyPurpleUsed)
                                    partyIcon = PartyIconColor.Blue;
                                else
                                    partyIcon = PartyIconColor.Purple;

                                partyPurpleUsed = true;
                            }
                            else if (player.Team == StormTeam.Red)
                            {
                                if (partyRedUsed)
                                    partyIcon = PartyIconColor.Orange;
                                else
                                    partyIcon = PartyIconColor.Red;

                                partyRedUsed = true;
                            }

                            partyPlayers.Add(player.PartyValue.Value, partyIcon.Value);
                        }
                    }

                    PlayerInfo(player, partyIcon);
                }
            }
            else
            {
                Console.WriteLine("(NONE)");
            }
        }

        private static void PlayerInfo(StormPlayer player, PartyIconColor? partyIcon)
        {
            if (player.PlayerType != PlayerType.Computer)
            {
                StringBuilder playerBuilder = new StringBuilder();

                // party
                if (partyIcon.HasValue)
                    playerBuilder.Append($"[{(int)partyIcon}]");
                else
                    playerBuilder.Append($"{"[-]"}");

                // battletag
                playerBuilder.Append($" {player.BattleTagName,-22}");

                // account level
                if (player.AccountLevel.HasValue && player.AccountLevel.Value > 0)
                {
                    string level = $" [Level:{player.AccountLevel.Value}]";
                    playerBuilder.Append($"{level,-14}");
                }
                else
                {
                    playerBuilder.Append($"{" [Level:???]",-14}");
                }

                // toon handle
                playerBuilder.Append($" [Toon:{player.ToonHandle}]");

                Console.WriteLine(playerBuilder);
            }
            else if (player.PlayerType == PlayerType.Computer)
            {
                Console.WriteLine($"{player.Name}");
            }

            if (player.HeroMasteryTiers.ToDictionary(x => x.HeroAttributeId, x => x.TierLevel).TryGetValue(player.PlayerHero.HeroAttributeId, out int tierLevel))
            {
                if (tierLevel == 2 && player.PlayerHero.HeroLevel < 25)
                    player.PlayerHero.HeroLevel = 25;
                else if (tierLevel == 3 && player.PlayerHero.HeroLevel < 50)
                    player.PlayerHero.HeroLevel = 50;
                else if (tierLevel == 4 && player.PlayerHero.HeroLevel < 75)
                    player.PlayerHero.HeroLevel = 75;
                else if (tierLevel == 5 && player.PlayerHero.HeroLevel < 100)
                    player.PlayerHero.HeroLevel = 100;
            }

            // hero name
            StringBuilder heroBuilder = new StringBuilder($"{player.PlayerHero.HeroName,-22}");

            // hero level
            if (player.IsAutoSelect)
                heroBuilder.Append($" [Level:Auto Select]");
            else
                heroBuilder.Append($" [Level:{player.PlayerHero.HeroLevel}]");

            // hero unit id
            heroBuilder.Append($" [ID:{player.PlayerHero.HeroUnitId}]");

            Console.WriteLine($"    {heroBuilder}");

            if (_showPlayerTalents)
            {
                Console.WriteLine();
                Console.WriteLine("Talents");

                Console.Write($"{"Level 1:",10}");
                if (player.Talents.Count >= 1)
                    Console.WriteLine($" {player.Talents[0].TalentNameId}");

                Console.Write($"{"Level 4:",10}");
                if (player.Talents.Count >= 2)
                    Console.WriteLine($" {player.Talents[1].TalentNameId}");

                Console.Write($"{"Level 7:",10}");
                if (player.Talents.Count >= 3)
                    Console.WriteLine($" {player.Talents[2].TalentNameId}");

                Console.Write($"{"Level 10:",10}");
                if (player.Talents.Count >= 4)
                    Console.WriteLine($" {player.Talents[3].TalentNameId}");

                Console.Write($"{"Level 13:",10}");
                if (player.Talents.Count >= 5)
                    Console.WriteLine($" {player.Talents[4].TalentNameId}");

                Console.Write($"{"Level 16:",10}");
                if (player.Talents.Count >= 6)
                    Console.WriteLine($" {player.Talents[5].TalentNameId}");

                Console.Write($"{"Level 20:",10}");
                if (player.Talents.Count >= 7)
                    Console.WriteLine($" {player.Talents[6].TalentNameId}");
                else
                    Console.WriteLine();
            }
        }
    }
}
