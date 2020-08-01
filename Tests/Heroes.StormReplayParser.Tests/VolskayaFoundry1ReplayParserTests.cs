using Heroes.StormReplayParser.GameEvent;
using Heroes.StormReplayParser.MessageEvent;
using Heroes.StormReplayParser.Player;
using Heroes.StormReplayParser.Replay;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Heroes.StormReplayParser.Tests
{
    [TestClass]
    public class VolskayaFoundry1ReplayParserTests
    {
        private readonly string _replaysFolder = "Replays";
        private readonly string _replayFile = "VolskayaFoundry1_77548.StormR";
        private readonly StormReplay _stormReplay;
        private readonly StormReplayParseStatus _result;

        public VolskayaFoundry1ReplayParserTests()
        {
            StormReplayResult result = StormReplay.Parse(Path.Combine(_replaysFolder, _replayFile));
            _stormReplay = result.Replay;
            _result = result.Status;
        }

        [TestMethod]
        public void ParseResult()
        {
            Assert.AreEqual(StormReplayParseStatus.Success, _result);
        }

        [TestMethod]
        public void PlayerCountTest()
        {
            Assert.AreEqual(10, _stormReplay.PlayersWithObserversCount);
            Assert.AreEqual(10, _stormReplay.PlayersCount);
        }

        [TestMethod]
        public void PlayerListTest()
        {
            Assert.AreEqual(10, _stormReplay.StormPlayers.Count());
            Assert.AreEqual(10, _stormReplay.StormPlayersWithObservers.Count());
        }

        [TestMethod]
        public void StormReplayHeaderTest()
        {
            Assert.AreEqual(2, _stormReplay.ReplayVersion.Major);
            Assert.AreEqual(49, _stormReplay.ReplayVersion.Minor);
            Assert.AreEqual(0, _stormReplay.ReplayVersion.Revision);
            Assert.AreEqual(77548, _stormReplay.ReplayVersion.Build);
            Assert.AreEqual(77548, _stormReplay.ReplayVersion.BaseBuild);

            Assert.AreEqual(77548, _stormReplay.ReplayBuild);
            Assert.AreEqual(17919, _stormReplay.ElapsedGamesLoops);
            Assert.AreEqual(new TimeSpan(0, 0, 18, 39, 0), _stormReplay.ReplayLength);
        }

        [TestMethod]
        public void StormReplayDetailsTest()
        {
            List<StormPlayer> players = _stormReplay.StormPlayers.ToList();
            StormPlayer player0 = players[0];

            Assert.AreEqual("Steph", player0.Name);
            Assert.AreEqual(1, player0.ToonHandle.Region);
            Assert.AreEqual(1, player0.ToonHandle.Realm);
            Assert.AreEqual(StormTeam.Blue, player0.Team);
            Assert.IsFalse(player0.IsWinner);
            Assert.AreEqual("Greymane", player0.PlayerHero.HeroName);

            StormPlayer player1 = players[9];

            Assert.AreEqual("Pinzs", player1.Name);
            Assert.AreEqual(1, player1.ToonHandle.Region);
            Assert.AreEqual(1, player1.ToonHandle.Realm);
            Assert.AreEqual(StormTeam.Red, player1.Team);
            Assert.IsTrue(player1.IsWinner);
            Assert.AreEqual("Rehgar", player1.PlayerHero.HeroName);

            Assert.AreEqual("Volskaya Foundry", _stormReplay.MapInfo.MapName);
            Assert.AreEqual(637120547862521860, _stormReplay.Timestamp.Ticks);

            Assert.IsFalse(_stormReplay.HasAI);
            Assert.IsFalse(_stormReplay.HasObservers);

            Assert.AreEqual(0, _stormReplay.StormObservers.ToList().Count);
            Assert.AreEqual(0, _stormReplay.PlayersObserversCount);
        }

        [TestMethod]
        public void StormReplayInitDataTest()
        {
            Assert.AreEqual(1102687070, _stormReplay.RandomValue);
            Assert.AreEqual(StormGameMode.StormLeague, _stormReplay.GameMode);

            List<StormPlayer> players = _stormReplay.StormPlayers.ToList();
            StormPlayer player0 = players[0];

            Assert.AreEqual("GreymaneDoctorVar1", player0.PlayerLoadout.SkinAndSkinTint);
            Assert.AreEqual("MountCloud", player0.PlayerLoadout.MountAndMountTint);
            Assert.IsFalse(player0.IsSilenced);
            Assert.IsFalse(player0.IsVoiceSilenced);
            Assert.IsFalse(player0.IsBlizzardStaff);
            Assert.IsFalse(player0.HasActiveBoost);
            Assert.AreEqual("BannerOWDVaIconicRare", player0.PlayerLoadout.Banner);
            Assert.AreEqual("SprayStaticComicSweetChromie", player0.PlayerLoadout.Spray);
            Assert.AreEqual("DeckardA", player0.PlayerLoadout.AnnouncerPack);
            Assert.AreEqual("GreymaneBase_VoiceLine04", player0.PlayerLoadout.VoiceLine);
            Assert.AreEqual(24, player0.HeroMasteryTiersCount);
            Assert.AreEqual("Auri", player0.HeroMasteryTiers.ToList()[2].HeroAttributeId);
            Assert.AreEqual(1, player0.HeroMasteryTiers.ToList()[2].TierLevel);
        }

        [TestMethod]
        public void StormReplayAttributeEventsTest()
        {
            List<StormPlayer> players = _stormReplay.StormPlayers.ToList();
            StormPlayer player = players[9];

            Assert.AreEqual("5v5", _stormReplay.TeamSize);
            Assert.AreEqual(PlayerDifficulty.Elite, player.PlayerDifficulty);
            Assert.AreEqual(StormGameSpeed.Faster, _stormReplay.GameSpeed);
            Assert.AreEqual(StormGameMode.StormLeague, _stormReplay.GameMode);
            Assert.AreEqual("Rehg", player.PlayerHero.HeroAttributeId);
            Assert.AreEqual("Reh8", player.PlayerLoadout.SkinAndSkinTintAttributeId);
            Assert.AreEqual(string.Empty, player.PlayerLoadout.MountAndMountTintAttributeId);
            Assert.AreEqual("BN03", player.PlayerLoadout.BannerAttributeId);
            Assert.AreEqual("SY3K", player.PlayerLoadout.SprayAttributeId);
            Assert.AreEqual("RE05", player.PlayerLoadout.VoiceLineAttributeId);
            Assert.AreEqual("DEA0", player.PlayerLoadout.AnnouncerPackAttributeId);
            Assert.AreEqual(20, player.PlayerHero.HeroLevel);

            List<string?> ban0List = _stormReplay.GetTeamBans(StormTeam.Blue).ToList();
            List<string?> ban1List = _stormReplay.GetTeamBans(StormTeam.Red).ToList();

            Assert.AreEqual("Garr", ban0List[1]);
            Assert.AreEqual("DEAT", ban1List[1]);
        }

        [TestMethod]
        public void DraftOrderTest()
        {
            var draft = _stormReplay.DraftPicks.ToList();

            Assert.AreEqual(16, draft.Count);

            Assert.AreEqual("Crusader", draft[0].HeroSelected);
            Assert.AreEqual(StormDraftPickType.Banned, draft[0].PickType);
            Assert.AreEqual(2, draft[0].SelectedPlayerSlotId);

            Assert.AreEqual("Auriel", draft[15].HeroSelected);
            Assert.AreEqual(StormDraftPickType.Picked, draft[15].PickType);
            Assert.AreEqual(2, draft[15].SelectedPlayerSlotId);
        }

        [TestMethod]
        public void TeamLevelsTest()
        {
            List<StormTeamLevel>? levelsBlue = _stormReplay.GetTeamLevels(StormTeam.Blue)?.ToList();
            List<StormTeamLevel>? levelsBlue2 = _stormReplay.GetTeamLevels(StormTeam.Blue)?.ToList();
            List<StormTeamLevel>? levelsRed = _stormReplay.GetTeamLevels(StormTeam.Red)?.ToList();
            List<StormTeamLevel>? levelsRed2 = _stormReplay.GetTeamLevels(StormTeam.Red)?.ToList();
            List<StormTeamLevel>? levelsOther = _stormReplay.GetTeamLevels(StormTeam.Observer)?.ToList();

            Assert.AreEqual(19, levelsBlue!.Count);
            Assert.AreEqual(19, levelsBlue2!.Count);
            Assert.AreEqual(21, levelsRed!.Count);
            Assert.AreEqual(21, levelsRed2!.Count);
            Assert.IsNull(levelsOther);

            Assert.AreEqual(1, levelsBlue[0].Level);
            Assert.AreEqual(new TimeSpan(32500000), levelsBlue[0].Time);

            Assert.AreEqual(8, levelsBlue[7].Level);
            Assert.AreEqual(new TimeSpan(3283125000), levelsBlue[7].Time);

            Assert.AreEqual(18, levelsBlue[17].Level);
            Assert.AreEqual(new TimeSpan(9587500000), levelsBlue[17].Time);

            Assert.AreEqual(1, levelsRed[0].Level);
            Assert.AreEqual(new TimeSpan(32500000), levelsRed[0].Time);

            Assert.AreEqual(10, levelsRed[9].Level);
            Assert.AreEqual(new TimeSpan(3693750000), levelsRed[9].Time);

            Assert.AreEqual(20, levelsRed[19].Level);
            Assert.AreEqual(new TimeSpan(9930625000), levelsRed[19].Time);
        }

        [TestMethod]
        public void TeamsFinalLevelTest()
        {
            Assert.AreEqual(19, _stormReplay.GetTeamFinalLevel(StormTeam.Blue));
            Assert.AreEqual(21, _stormReplay.GetTeamFinalLevel(StormTeam.Red));
            Assert.IsNull(_stormReplay.GetTeamFinalLevel(StormTeam.Observer));
        }

        [TestMethod]
        public void TeamXpBreakdownTest()
        {
            List<StormTeamXPBreakdown>? xpBlue = _stormReplay.GetTeamXPBreakdown(StormTeam.Blue)?.ToList();
            List<StormTeamXPBreakdown>? xpRed = _stormReplay.GetTeamXPBreakdown(StormTeam.Red)?.ToList();
            List<StormTeamXPBreakdown>? xpOther = _stormReplay.GetTeamXPBreakdown(StormTeam.Observer)?.ToList();

            Assert.AreEqual(18, xpBlue!.Count);
            Assert.AreEqual(18, xpRed!.Count);
            Assert.IsNull(xpOther);

            StormTeamXPBreakdown blue = xpBlue[0];

            Assert.AreEqual(0, blue.HeroXP);
            Assert.AreEqual(1, blue.Level);
            Assert.AreEqual(0, blue.CreepXP);
            Assert.AreEqual(1200, blue.MinionXP);
            Assert.AreEqual(575, blue.PassiveXP);
            Assert.AreEqual(0, blue.StructureXP);
            Assert.AreEqual(new TimeSpan(981250000), blue.Time);
            Assert.AreEqual(1775, blue.TotalXP);

            blue = xpBlue[13];
            Assert.AreEqual(5470, blue.HeroXP);
            Assert.AreEqual(16, blue.Level);
            Assert.AreEqual(3773, blue.CreepXP);
            Assert.AreEqual(25754, blue.MinionXP);
            Assert.AreEqual(18515, blue.PassiveXP);
            Assert.AreEqual(375, blue.StructureXP);
            Assert.AreEqual(new TimeSpan(8781250000), blue.Time);
            Assert.AreEqual(53887, blue.TotalXP);

            blue = xpBlue[17];
            Assert.AreEqual(10164, blue.HeroXP);
            Assert.AreEqual(19, blue.Level);
            Assert.AreEqual(4121, blue.CreepXP);
            Assert.AreEqual(32032, blue.MinionXP);
            Assert.AreEqual(23621, blue.PassiveXP);
            Assert.AreEqual(375, blue.StructureXP);
            Assert.AreEqual(new TimeSpan(11078750000), blue.Time);
            Assert.AreEqual(70313, blue.TotalXP);

            StormTeamXPBreakdown red = xpRed[0];

            Assert.AreEqual(592, red.HeroXP);
            Assert.AreEqual(2, red.Level);
            Assert.AreEqual(0, red.CreepXP);
            Assert.AreEqual(1360, red.MinionXP);
            Assert.AreEqual(575, red.PassiveXP);
            Assert.AreEqual(0, red.StructureXP);
            Assert.AreEqual(new TimeSpan(981250000), red.Time);
            Assert.AreEqual(2527, red.TotalXP);

            red = xpRed[13];
            Assert.AreEqual(6596, red.HeroXP);
            Assert.AreEqual(18, red.Level);
            Assert.AreEqual(3661, red.CreepXP);
            Assert.AreEqual(31496, red.MinionXP);
            Assert.AreEqual(21288, red.PassiveXP);
            Assert.AreEqual(1425, red.StructureXP);
            Assert.AreEqual(new TimeSpan(8781250000), red.Time);
            Assert.AreEqual(64466, red.TotalXP);

            red = xpRed[17];
            Assert.AreEqual(11379, red.HeroXP);
            Assert.AreEqual(21, red.Level);
            Assert.AreEqual(3661, red.CreepXP);
            Assert.AreEqual(35918, red.MinionXP);
            Assert.AreEqual(29392, red.PassiveXP);
            Assert.AreEqual(2350, red.StructureXP);
            Assert.AreEqual(new TimeSpan(11078750000), red.Time);
            Assert.AreEqual(82700, red.TotalXP);
        }

        [TestMethod]
        public void PlayersScoreResultTest()
        {
            ScoreResult? scoreResult = _stormReplay.StormPlayers.ToList()[0].ScoreResult;

            Assert.AreEqual(3, scoreResult!.Assists);
            Assert.AreEqual(0, scoreResult.ClutchHealsPerformed);
            Assert.AreEqual(33369, scoreResult.CreepDamage);
            Assert.AreEqual(28369, scoreResult.DamageSoaked);
            Assert.AreEqual(43030, scoreResult.DamageTaken);
            Assert.AreEqual(5, scoreResult.Deaths);
            Assert.AreEqual(0, scoreResult.EscapesPerformed);
            Assert.AreEqual(8968, scoreResult.ExperienceContribution);
            Assert.AreEqual(0, scoreResult.Healing);
            Assert.AreEqual(47875, scoreResult.HeroDamage);
            Assert.AreEqual(5, scoreResult.HighestKillStreak);
            Assert.AreEqual(19, scoreResult.Level);
            Assert.AreEqual(7, scoreResult.MercCampCaptures);
            Assert.AreEqual(70313, scoreResult.MetaExperience);
            Assert.AreEqual(50333, scoreResult.MinionDamage);
            Assert.AreEqual(0, scoreResult.Multikill);
            Assert.AreEqual(2, scoreResult.OutnumberedDeaths);
            Assert.AreEqual(101982, scoreResult.PhysicalDamage);
            Assert.AreEqual(0, scoreResult.ProtectionGivenToAllies);
            Assert.AreEqual(0, scoreResult.SelfHealing);
            Assert.AreEqual(61382, scoreResult.SiegeDamage);
            Assert.AreEqual(3, scoreResult.SoloKills);
            Assert.AreEqual(49618, scoreResult.SpellDamage);
            Assert.AreEqual(5864, scoreResult.StructureDamage);
            Assert.AreEqual(2986, scoreResult.SummonDamage);
            Assert.AreEqual(6, scoreResult.Takedowns);
            Assert.AreEqual(34692, scoreResult.TeamfightDamageTaken);
            Assert.AreEqual(0, scoreResult.TeamfightEscapesPerformed);
            Assert.AreEqual(2531, scoreResult.TeamfightHealingDone);
            Assert.AreEqual(35818, scoreResult.TeamfightHeroDamage);
            Assert.AreEqual(new TimeSpan(0, 0, 0), scoreResult.TimeCCdEnemyHeroes);
            Assert.AreEqual(new TimeSpan(0, 0, 0), scoreResult.TimeRootingEnemyHeroes);
            Assert.AreEqual(new TimeSpan(0, 0, 0), scoreResult.TimeSilencingEnemyHeroes);
            Assert.AreEqual(new TimeSpan(0, 2, 27), scoreResult.TimeSpentDead);
            Assert.AreEqual(new TimeSpan(0, 0, 0), scoreResult.TimeStunningEnemyHeroes);
            Assert.AreEqual(0, scoreResult.TownKills);
            Assert.AreEqual(2, scoreResult.VengeancesPerformed);
            Assert.AreEqual(0, scoreResult.WatchTowerCaptures);
            Assert.AreEqual(56, scoreResult.MinionKills);
            Assert.AreEqual(33, scoreResult.RegenGlobes);
            Assert.AreEqual(3, scoreResult.Tier1Talent);
            Assert.AreEqual(2, scoreResult.Tier4Talent);
            Assert.AreEqual(3, scoreResult.Tier7Talent);
            Assert.AreEqual(1, scoreResult.Tier10Talent);
            Assert.AreEqual(1, scoreResult.Tier13Talent);
            Assert.AreEqual(3, scoreResult.Tier16Talent);
            Assert.AreEqual(null, scoreResult.Tier20Talent);
        }

        [TestMethod]
        public void PlayersMatchAwardsTest()
        {
            List<MatchAwardType> matchAwards = _stormReplay.StormPlayers.ToList()[0].MatchAwards.ToList();

            Assert.AreEqual(1, _stormReplay.StormPlayers.ToList()[0].MatchAwardsCount);
            Assert.AreEqual(MatchAwardType.MostMercCampsCaptured, matchAwards[0]);

            matchAwards = _stormReplay.StormPlayers.ToList()[9].MatchAwards.ToList();

            Assert.AreEqual(0, matchAwards.Count);
        }

        [TestMethod]
        public void MessagesTest()
        {
            List<StormMessage> messages = _stormReplay.Messages.ToList();

            StormMessage stormMessage = messages.Last();

            Assert.AreEqual(StormMessageEventType.SChatMessage, stormMessage.MessageEventType);
            Assert.AreEqual("Rehgar", stormMessage.MessageSender!.PlayerHero.HeroName);
            Assert.AreEqual(StormMessageTarget.Allies, stormMessage.ChatMessage!.MessageTarget);
            Assert.IsTrue(stormMessage.ChatMessage!.Message!.StartsWith("https:"));
            Assert.IsTrue(stormMessage.ChatMessage!.Message!.EndsWith("nzs"));
            Assert.AreEqual(new TimeSpan(11055625000), stormMessage.Timestamp);

            stormMessage = messages.First();

            Assert.AreEqual(StormMessageEventType.SLoadingProgressMessage, stormMessage.MessageEventType);
            Assert.AreEqual("Rehgar", stormMessage.MessageSender!.PlayerHero.HeroName);
            Assert.AreEqual(12, stormMessage.LoadingProgressMessage!.LoadingProgress);
            Assert.AreEqual(new TimeSpan(0), stormMessage.Timestamp);

            stormMessage = messages[97];

            Assert.AreEqual(StormMessageEventType.SPingMessage, stormMessage.MessageEventType);
            Assert.AreEqual("Arthas", stormMessage.MessageSender!.PlayerHero.HeroName);
            Assert.AreEqual(StormMessageTarget.Allies, stormMessage.PingMessage!.MessageTarget);
            Assert.AreEqual(146.725830078125, stormMessage.PingMessage!.Point!.Value.X);
            Assert.AreEqual(73.9296875, stormMessage.PingMessage!.Point!.Value.Y);
            Assert.AreEqual(new TimeSpan(1091250000), stormMessage.Timestamp);
        }

        [TestMethod]
        public void ChatMessagesTest()
        {
            List<StormMessage> messages = _stormReplay.ChatMessages.ToList();

            Assert.AreEqual(15, messages.Count);
            Assert.IsTrue(messages.All(x => x.ChatMessage != null && !string.IsNullOrEmpty(x.ChatMessage.Message)));
        }

        [TestMethod]
        public void BattleLobbyDataTest()
        {
            List<StormPlayer> players = _stormReplay.StormPlayers.ToList();

            Assert.AreEqual(1145, players[1].AccountLevel);
            Assert.AreEqual(null, players[1].PartyValue);
            Assert.AreEqual(1201, players[9].AccountLevel);
            Assert.AreEqual(null, players[9].PartyValue);
        }

        [TestMethod]
        public void TrackerEventsTest()
        {
            Assert.AreEqual(8319, _stormReplay.TrackerEvents.Count);

            StormTrackerEvent unitBornEvent = _stormReplay.TrackerEvents[8145];

            Assert.AreEqual(StormTrackerEventType.UnitBornEvent, unitBornEvent.TrackerEventType);
            Assert.AreEqual(18, unitBornEvent.Timestamp.Minutes);

            Assert.AreEqual(23U, unitBornEvent.VersionedDecoder!.Structure![0].GetValueAsUInt32());
            Assert.AreEqual(19U, unitBornEvent.VersionedDecoder!.Structure![1].GetValueAsUInt32());
            Assert.AreEqual("ExperienceGlobeMinion", unitBornEvent.VersionedDecoder!.Structure![2].GetValueAsString());
            Assert.AreEqual(12U, unitBornEvent.VersionedDecoder!.Structure![3].GetValueAsUInt32());
            Assert.AreEqual(12U, unitBornEvent.VersionedDecoder!.Structure![4].GetValueAsUInt32());
            Assert.AreEqual(90U, unitBornEvent.VersionedDecoder!.Structure![5].GetValueAsUInt32());
            Assert.AreEqual(96U, unitBornEvent.VersionedDecoder!.Structure![6].GetValueAsUInt32());
        }

        [TestMethod]
        public void GameEventsTest()
        {
            Assert.AreEqual(122839, _stormReplay.GameEvents.Count);

            StormGameEvent updateTargetPointEvent = _stormReplay.GameEvents[100741];

            Assert.AreEqual(StormGameEventType.SCmdUpdateTargetPointEvent, updateTargetPointEvent.GameEventType);
            Assert.AreEqual(14, updateTargetPointEvent.Timestamp.Minutes);
            Assert.AreEqual("Li-Ming", updateTargetPointEvent!.MessageSender!.PlayerHero.HeroName);

            Assert.AreEqual(2479U, updateTargetPointEvent.Data!.Structure![0].UnsignedInteger32);
            Assert.AreEqual(506409U, updateTargetPointEvent.Data!.Structure![1].Structure![0].UnsignedInteger32);
            Assert.AreEqual(149373U, updateTargetPointEvent.Data!.Structure![1].Structure![1].UnsignedInteger32);
            Assert.AreEqual(33550, updateTargetPointEvent.Data!.Structure![1].Structure![2].Integer32);

            StormGameEvent selectionDeltaEvent = _stormReplay.GameEvents[74221];

            Assert.AreEqual(StormGameEventType.SSelectionDeltaEvent, selectionDeltaEvent.GameEventType);
            Assert.AreEqual(11, selectionDeltaEvent.Timestamp.Minutes);
            Assert.AreEqual("Arthas", selectionDeltaEvent!.MessageSender!.PlayerHero.HeroName);

            Assert.AreEqual(10U, selectionDeltaEvent.Data!.Structure![0].UnsignedInteger32);
            Assert.AreEqual(0U, selectionDeltaEvent.Data!.Structure![1].Structure![0].UnsignedInteger32);
            Assert.AreEqual(30U, selectionDeltaEvent.Data!.Structure![1].Structure![2].Array![0].Structure![0].UnsignedInteger32);
            Assert.AreEqual(15U, selectionDeltaEvent.Data!.Structure![1].Structure![2].Array![0].Structure![1].UnsignedInteger32);
            Assert.AreEqual(1U, selectionDeltaEvent.Data!.Structure![1].Structure![2].Array![0].Structure![2].UnsignedInteger32);
            Assert.AreEqual(1U, selectionDeltaEvent.Data!.Structure![1].Structure![2].Array![0].Structure![3].UnsignedInteger32);
            Assert.AreEqual(1195376641U, selectionDeltaEvent.Data!.Structure![1].Structure![3].Array![0].UnsignedInteger32);
        }

        [TestMethod]
        [TestCategory("Parsing Options")]
        public void NoTrackerEventsParsingTests()
        {
            StormReplayResult result = StormReplay.Parse(Path.Combine(_replaysFolder, _replayFile), new ParseOptions()
            {
                AllowPTR = false,
                ShouldGameEvents = true,
                ShouldParseMessageEvents = true,
                ShouldTrackerEvents = false,
            });

            Assert.AreEqual(StormReplayParseStatus.Success, result.Status);

            NoTrackerEvents(result);
        }

        [TestMethod]
        [TestCategory("Parsing Options")]
        public void NoGameEventsParsingTests()
        {
            StormReplayResult result = StormReplay.Parse(Path.Combine(_replaysFolder, _replayFile), new ParseOptions()
            {
                AllowPTR = false,
                ShouldGameEvents = false,
                ShouldParseMessageEvents = true,
                ShouldTrackerEvents = true,
            });

            Assert.AreEqual(StormReplayParseStatus.Success, result.Status);
            NoGameEvents(result);
        }

        [TestMethod]
        [TestCategory("Parsing Options")]
        public void NoMessageEventsParsingTests()
        {
            StormReplayResult result = StormReplay.Parse(Path.Combine(_replaysFolder, _replayFile), new ParseOptions()
            {
                AllowPTR = false,
                ShouldGameEvents = true,
                ShouldParseMessageEvents = false,
                ShouldTrackerEvents = true,
            });

            Assert.AreEqual(StormReplayParseStatus.Success, result.Status);
            NoMessageEvents(result);
        }

        [TestMethod]
        [TestCategory("Parsing Options")]
        public void MinimalParsingTests()
        {
            StormReplayResult result = StormReplay.Parse(Path.Combine(_replaysFolder, _replayFile), ParseOptions.MinimalParsing);

            StormReplay replay = result.Replay!;

            Assert.AreEqual(StormReplayParseStatus.Success, result.Status);

            Assert.AreEqual(0, replay.TrackerEvents.Count);
            Assert.IsNull(replay.GetTeamLevels(StormTeam.Blue));
            Assert.IsNull(replay.GetTeamLevels(StormTeam.Red));
            Assert.IsNull(replay.GetTeamXPBreakdown(StormTeam.Blue));
            Assert.IsNull(replay.GetTeamXPBreakdown(StormTeam.Red));
            Assert.AreEqual(0, replay.DraftPicks.Count);

            List<StormPlayer> players = replay.StormPlayers.ToList();
            Assert.AreEqual(0, players[0].Talents.Count);
            Assert.IsNull(players[0].ScoreResult);
            Assert.IsNull(players[0].MatchAwards);
            Assert.IsNull(players[0].MatchAwardsCount);

            NoGameEvents(result);
            NoMessageEvents(result);
        }

        private static void NoTrackerEvents(StormReplayResult result)
        {
            StormReplay replay = result.Replay!;

            Assert.AreEqual(0, replay.TrackerEvents.Count);
            Assert.IsNull(replay.GetTeamLevels(StormTeam.Blue));
            Assert.IsNull(replay.GetTeamLevels(StormTeam.Red));
            Assert.IsNull(replay.GetTeamXPBreakdown(StormTeam.Blue));
            Assert.IsNull(replay.GetTeamXPBreakdown(StormTeam.Red));
            Assert.AreEqual(0, replay.DraftPicks.Count);

            List<StormPlayer> players = replay.StormPlayers.ToList();
            Assert.IsNull(players[0].Talents[0].TalentNameId);
            Assert.IsNull(players[0].ScoreResult);
            Assert.IsNull(players[0].MatchAwards);
            Assert.IsNull(players[0].MatchAwardsCount);
        }

        private static void NoGameEvents(StormReplayResult result)
        {
            StormReplay replay = result.Replay!;

            Assert.AreEqual(0, replay.GameEvents.Count);

            List<StormPlayer> players = replay.StormPlayers.ToList();
            Assert.AreEqual(0, players[0].Talents.Count);
        }

        private static void NoMessageEvents(StormReplayResult result)
        {
            StormReplay replay = result.Replay!;

            Assert.AreEqual(0, replay.Messages.Count);
            Assert.AreEqual(0, replay.ChatMessages.ToList().Count);
        }
    }
}
