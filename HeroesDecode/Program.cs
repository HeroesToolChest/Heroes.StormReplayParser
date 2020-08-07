using Heroes.StormReplayParser;
using Heroes.StormReplayParser.Player;
using Heroes.StormReplayParser.Replay;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Text;

namespace HeroesDecode
{
    public static class Program
    {
        private const int _statisticsFieldWidth = 21;

        private static bool _resultOnly = false;
        private static bool _showPlayerTalents = false;
        private static bool _showPlayerStats = false;

        private static bool _failed = false;

        public static int Main(string[] args)
        {
            RootCommand rootCommand = new RootCommand()
            {
                new Option<string>(
                    "--replay-path",
                    description: "File path of a Heroes of the Storm .StormReplay file or a directory")
                {
                    IsRequired = true,
                },
                new Option<bool>(
                    "--result-only",
                    getDefaultValue: () => false,
                    description: "Will only show result of parsing, no map info or player info; --show-player-talents and --show-player-stats options will be overridden to false"),
                new Option<bool>(
                    "--show-player-talents",
                    getDefaultValue: () => false,
                    description: "Shows the player's talent information"),
                new Option<bool>(
                    "--show-player-stats",
                    getDefaultValue: () => false,
                    description: "Shows the player's stats"),
            };

            rootCommand.Description = "Parses Heroes of the Storm replay files";

            rootCommand.Handler = CommandHandler.Create<string, bool, bool, bool>((replayPath, resultOnly, showPlayerTalents, showPlayerStats) =>
            {
                _resultOnly = resultOnly;
                if (!resultOnly)
                {
                    _showPlayerTalents = showPlayerTalents;
                    _showPlayerStats = showPlayerStats;
                }

                if (File.Exists(replayPath))
                {
                    Parse(replayPath, resultOnly);
                }
                else if (Directory.Exists(replayPath))
                {
                    foreach (string? replayFile in Directory.EnumerateFiles(replayPath, "*.StormReplay", SearchOption.AllDirectories))
                    {
                        if (!string.IsNullOrEmpty(replayFile) && File.Exists(replayFile))
                            Parse(replayFile, resultOnly);
                    }
                }
            });

            return rootCommand.InvokeAsync(args).Result;
        }

        private static void Parse(string replayPath, bool onlyResult)
        {
            StormReplayResult stormReplayResult = StormReplay.Parse(replayPath, new ParseOptions()
            {
                AllowPTR = true,
                ShouldParseTrackerEvents = true,
                ShouldParseGameEvents = true,
                ShouldParseMessageEvents = true,
            });

            ResultLine(stormReplayResult);

            if (!onlyResult)
            {
                GetInfo(stormReplayResult);
                Console.WriteLine();
            }
        }

        private static void ResultLine(StormReplayResult stormReplayResult)
        {
            if (stormReplayResult.Status == StormReplayParseStatus.Success)
                Console.ForegroundColor = ConsoleColor.Green;
            else
                Console.ForegroundColor = ConsoleColor.Red;

            if (!_resultOnly)
            {
                Console.WriteLine(stormReplayResult.Status);
                Console.ResetColor();

                if (stormReplayResult.Status != StormReplayParseStatus.Success && stormReplayResult.Exception != null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(stormReplayResult.Exception.StackTrace);
                    Console.ResetColor();
                    _failed = true;
                }
            }
            else
            {
                Console.Write(stormReplayResult.Status);
                Console.ResetColor();
                Console.WriteLine($" [{Path.GetFileName(stormReplayResult.FileName)}] [{stormReplayResult.Replay.ReplayVersion}]");
            }
        }

        private static void GetInfo(StormReplayResult stormReplayResult)
        {
            StormReplay replay = stormReplayResult.Replay;

            List<StormPlayer> players = replay.StormPlayers.ToList();

            Console.WriteLine($"{"File Name: ",11}{Path.GetFileName(stormReplayResult.FileName)}");
            Console.WriteLine($"{"Game Mode: ",11}{replay.GameMode}");
            Console.WriteLine($"{"Map: ",11}{replay.MapInfo.MapName} [ID:{replay.MapInfo.MapId}]");
            Console.WriteLine($"{"Version: ",11}{replay.ReplayVersion}");
            Console.WriteLine($"{"Region: ",11}{replay.Region}");
            Console.WriteLine($"{"Game Time: ",11}{replay.ReplayLength}");

            if (_failed)
                Environment.Exit(1);

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

                            partyPlayers.Add(player.PartyValue.Value, partyIcon!.Value);
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
            StringBuilder heroBuilder = new StringBuilder($"{player.PlayerHero.HeroName,-16}");

            // hero level
            if (player.IsAutoSelect)
            {
                heroBuilder.Append($"{" [Level:Auto]",-14}");
            }
            else
            {
                string level = $" [Level:{player.PlayerHero.HeroLevel}]";
                heroBuilder.Append($"{level,-14}");
            }

            // hero unit id
            heroBuilder.Append($" [ID:{player.PlayerHero.HeroUnitId}]");

            Console.WriteLine($"    Hero: {heroBuilder}");

            foreach (MatchAwardType matchAwardType in player.MatchAwards!)
            {
                Console.WriteLine($"    Award: {matchAwardType}");
            }

            // talents
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

                Console.WriteLine();
            }

            // stats
            if (_showPlayerStats)
            {
                Console.WriteLine();
                Console.WriteLine("Statistics");

                Console.WriteLine("Combat");
                Console.WriteLine($"{"Hero Kills:",_statisticsFieldWidth} {player.ScoreResult!.SoloKills}");
                Console.WriteLine($"{"Assists:",_statisticsFieldWidth} {player.ScoreResult.Assists}");
                Console.WriteLine($"{"Takedowns:",_statisticsFieldWidth} {player.ScoreResult.Takedowns}");
                Console.WriteLine($"{"Deaths:",_statisticsFieldWidth} {player.ScoreResult.Deaths}");

                Console.WriteLine("Siege");
                Console.WriteLine($"{"Minion Damage:",_statisticsFieldWidth} {player.ScoreResult.MinionDamage}");
                Console.WriteLine($"{"Summon Damage:",_statisticsFieldWidth} {player.ScoreResult.SummonDamage}");
                Console.WriteLine($"{"Structure Damage:",_statisticsFieldWidth} {player.ScoreResult.StructureDamage}");
                Console.WriteLine($"{"Total Siege Damage:",_statisticsFieldWidth} {player.ScoreResult.SiegeDamage}");

                Console.WriteLine("Hero");
                Console.WriteLine($"{"Hero Damage:",_statisticsFieldWidth} {player.ScoreResult.HeroDamage}");

                if (player.ScoreResult.DamageTaken > 0)
                    Console.WriteLine($"{"Damage Taken:",_statisticsFieldWidth} {player.ScoreResult.DamageTaken}");
                else
                    Console.WriteLine($"{"Damage Taken:",_statisticsFieldWidth}");

                if (player.ScoreResult.DamageTaken > 0)
                    Console.WriteLine($"{"Healing/Shielding:",_statisticsFieldWidth} {player.ScoreResult.Healing}");
                else
                    Console.WriteLine($"{"Healing/Shielding:",_statisticsFieldWidth}");

                if (player.ScoreResult.SelfHealing > 0)
                    Console.WriteLine($"{"Self Healing:",_statisticsFieldWidth} {player.ScoreResult.SelfHealing}");
                else
                    Console.WriteLine($"{"Self Healing:",_statisticsFieldWidth}");

                Console.WriteLine($"{"Experience:",_statisticsFieldWidth} {player.ScoreResult.ExperienceContribution}");

                Console.WriteLine("Time");
                Console.WriteLine($"{"Spent Dead:",_statisticsFieldWidth} {player.ScoreResult.TimeSpentDead}");
                Console.WriteLine($"{"Rooting Heroes:",_statisticsFieldWidth} {player.ScoreResult.TimeRootingEnemyHeroes}");
                Console.WriteLine($"{"Silence Heroes:",_statisticsFieldWidth} {player.ScoreResult.TimeSilencingEnemyHeroes}");
                Console.WriteLine($"{"Stun Heroes:",_statisticsFieldWidth} {player.ScoreResult.TimeStunningEnemyHeroes}");
                Console.WriteLine($"{"CC Heroes:",_statisticsFieldWidth} {player.ScoreResult.TimeCCdEnemyHeroes}");
                Console.WriteLine($"{"On Fire:",_statisticsFieldWidth} {player.ScoreResult.OnFireTimeonFire}");

                Console.WriteLine("Other");
                if (player.ScoreResult.SpellDamage.HasValue && player.ScoreResult.SpellDamage.Value > 0)
                    Console.WriteLine($"{"Spell Damage:",_statisticsFieldWidth} {player.ScoreResult.SpellDamage}");
                else
                    Console.WriteLine($"{"Spell Damage:",_statisticsFieldWidth}");

                if (player.ScoreResult.PhysicalDamage.HasValue && player.ScoreResult.PhysicalDamage.Value > 0)
                    Console.WriteLine($"{"Physical Damage:",_statisticsFieldWidth} {player.ScoreResult.PhysicalDamage}");
                else
                    Console.WriteLine($"{"Physical Damage:",_statisticsFieldWidth}");

                Console.WriteLine($"{"Merc Damage:",_statisticsFieldWidth} {player.ScoreResult.CreepDamage}");
                Console.WriteLine($"{"Merc Camp Captures:",_statisticsFieldWidth} {player.ScoreResult.MercCampCaptures}");
                Console.WriteLine($"{"Watch Tower Captures:",_statisticsFieldWidth} {player.ScoreResult.WatchTowerCaptures}");
                Console.WriteLine($"{"Town Kills:",_statisticsFieldWidth} {player.ScoreResult.TownKills}");
                Console.WriteLine($"{"Town Kills:",_statisticsFieldWidth} {player.ScoreResult.TownKills}");
                Console.WriteLine($"{"Minion Kills:",_statisticsFieldWidth} {player.ScoreResult.MinionKills}");
                Console.WriteLine($"{"Regen Globes:",_statisticsFieldWidth} {player.ScoreResult.RegenGlobes}");

                Console.WriteLine();
            }
        }
    }
}
