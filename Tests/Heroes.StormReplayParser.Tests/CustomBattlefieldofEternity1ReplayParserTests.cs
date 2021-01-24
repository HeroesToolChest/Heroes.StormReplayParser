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
    public class CustomBattlefieldofEternity1ReplayParserTests
    {
        private readonly string _replaysFolder = "Replays";
        private readonly string _replayFile = "CustomBattlefieldofEternity1_65006.StormR";
        private readonly StormReplay _stormReplay;

        public CustomBattlefieldofEternity1ReplayParserTests()
        {
            _stormReplay = StormReplay.Parse(Path.Combine(_replaysFolder, _replayFile)).Replay;
        }

        [TestMethod]
        public void PlayerCountTest()
        {
            Assert.AreEqual(11, _stormReplay.PlayersWithObserversCount);
            Assert.AreEqual(10, _stormReplay.PlayersCount);
        }

        [TestMethod]
        public void PlayerListTest()
        {
            Assert.AreEqual(10, _stormReplay.StormPlayers.Count());
            Assert.AreEqual(11, _stormReplay.StormPlayersWithObservers.Count());
        }

        [TestMethod]
        public void StormReplayHeaderTest()
        {
            Assert.AreEqual(2, _stormReplay.ReplayVersion.Major);
            Assert.AreEqual(32, _stormReplay.ReplayVersion.Minor);
            Assert.AreEqual(3, _stormReplay.ReplayVersion.Revision);
            Assert.AreEqual(65006, _stormReplay.ReplayVersion.Build);
            Assert.AreEqual(65006, _stormReplay.ReplayVersion.BaseBuild);

            Assert.AreEqual(65006, _stormReplay.ReplayBuild);
            Assert.AreEqual(19306, _stormReplay.ElapsedGamesLoops);
        }

        [TestMethod]
        public void StormReplayDetailsTest()
        {
            List<StormPlayer> players = _stormReplay.StormPlayers.ToList();
            StormPlayer player0 = players[0];

            Assert.AreEqual("AZTDoubt", player0.Name);
            Assert.AreEqual(1, player0.ToonHandle!.Region);
            Assert.AreEqual(1, player0.ToonHandle.Realm);
            Assert.AreEqual(7027042, player0.ToonHandle.Id);
            Assert.AreEqual(1869768008, player0.ToonHandle.ProgramId);
            Assert.AreEqual(StormTeam.Blue, player0.Team);
            Assert.IsFalse(player0.IsWinner);
            Assert.AreEqual("Greymane", player0.PlayerHero!.HeroName);
            Assert.AreEqual(StormRegion.US, player0.ToonHandle.StormRegion);
            Assert.AreEqual(StormTeam.Red, _stormReplay.WinningTeam);

            StormPlayer player = players[9];

            Assert.AreEqual("FiZiX", player.Name);
            Assert.AreEqual(1, player.ToonHandle!.Region);
            Assert.AreEqual(1, player.ToonHandle.Realm);
            Assert.AreEqual(StormTeam.Red, player.Team);
            Assert.IsTrue(player.IsWinner);
            Assert.AreEqual("Li-Ming", player.PlayerHero!.HeroName);

            Assert.AreEqual("Battlefield of Eternity", _stormReplay.MapInfo.MapName);
            Assert.AreEqual(636619794857150779, _stormReplay.Timestamp.Ticks);

            List<StormPlayer> playersWithObs = _stormReplay.StormPlayersWithObservers.ToList();
            StormPlayer player8 = playersWithObs[8];

            Assert.AreEqual(StormTeam.Observer, player8.Team);

            Assert.IsTrue(_stormReplay.HasObservers);
            Assert.IsFalse(_stormReplay.HasAI);
            Assert.AreEqual(1, _stormReplay.PlayersObserversCount);
            Assert.IsNull(_stormReplay.StormObservers.ToList()[0].PlayerHero);
        }

        [TestMethod]
        public void StormReplayInitDataTest()
        {
            Assert.AreEqual(36047320, _stormReplay.RandomValue);
            Assert.AreEqual(StormGameMode.Custom, _stormReplay.GameMode);

            List<StormPlayer> players = _stormReplay.StormPlayers.ToList();
            StormPlayer player0 = players[0];

            Assert.AreEqual("GreymaneDoctorVar3", player0.PlayerLoadout.SkinAndSkinTint);
            Assert.AreEqual("MountCloudWhimsy", player0.PlayerLoadout.MountAndMountTint);
            Assert.IsFalse(player0.IsSilenced);
            Assert.IsNull(player0.IsVoiceSilenced);
            Assert.IsNull(player0.IsBlizzardStaff);
            Assert.IsNull(player0.HasActiveBoost);
            Assert.AreEqual("BannerDFEsportsWarChestRareDignitas", player0.PlayerLoadout.Banner);
            Assert.AreEqual("SprayStaticHGC2017EUDignitas", player0.PlayerLoadout.Spray);
            Assert.AreEqual("JainaA", player0.PlayerLoadout.AnnouncerPack);
            Assert.AreEqual("GreymaneBase_VoiceLine01", player0.PlayerLoadout.VoiceLine);
            Assert.AreEqual(15, player0.HeroMasteryTiersCount);
            Assert.AreEqual("Barb", player0.HeroMasteryTiers.ToList()[2].HeroAttributeId);
            Assert.AreEqual(1, player0.HeroMasteryTiers.ToList()[2].TierLevel);
            Assert.AreEqual(PlayerType.Human, player0.PlayerType);

            List<StormPlayer> playersWithObs = _stormReplay.StormPlayersWithObservers.ToList();
            StormPlayer player8 = playersWithObs[8];

            Assert.AreEqual(PlayerType.Observer, player8.PlayerType);
            Assert.AreEqual(PlayerDifficulty.Unknown, player8.PlayerDifficulty);
        }

        [TestMethod]
        public void StormReplayAttributeEventsTest()
        {
            List<StormPlayer> players = _stormReplay.StormPlayers.ToList();
            StormPlayer player = players[9];

            Assert.AreEqual("5v5", _stormReplay.TeamSize);
            Assert.AreEqual(PlayerDifficulty.Elite, player.PlayerDifficulty);
            Assert.AreEqual(StormGameSpeed.Faster, _stormReplay.GameSpeed);
            Assert.AreEqual(StormGameMode.Custom, _stormReplay.GameMode);
            Assert.AreEqual("Wiza", player.PlayerHero!.HeroAttributeId);
            Assert.AreEqual("WizI", player.PlayerLoadout.SkinAndSkinTintAttributeId);
            Assert.AreEqual("CLO0", player.PlayerLoadout.MountAndMountTintAttributeId);
            Assert.AreEqual("BN6d", player.PlayerLoadout.BannerAttributeId);
            Assert.AreEqual("SY81", player.PlayerLoadout.SprayAttributeId);
            Assert.AreEqual("WZ04", player.PlayerLoadout.VoiceLineAttributeId);
            Assert.AreEqual("AFIR", player.PlayerLoadout.AnnouncerPackAttributeId);
            Assert.AreEqual(20, player.PlayerHero.HeroLevel);

            List<string?> ban0List = _stormReplay.GetTeamBans(StormTeam.Blue).ToList();
            List<string?> ban1List = _stormReplay.GetTeamBans(StormTeam.Red).ToList();

            Assert.AreEqual("Diab", ban0List[1]);
            Assert.AreEqual("Tra0", ban1List[1]);
        }

        [TestMethod]
        public void DraftOrderTest()
        {
            List<StormDraftPick> draft = _stormReplay.DraftPicks.ToList();

            Assert.AreEqual(14, draft.Count);

            Assert.AreEqual("Maiev", draft[0].HeroSelected);
            Assert.AreEqual(StormDraftPickType.Banned, draft[0].PickType);
            Assert.IsNull(draft[0].Player);
            Assert.AreEqual(StormTeam.Red, draft[0].Team);

            Assert.AreEqual("Kaelthas", draft[13].HeroSelected);
            Assert.AreEqual(StormDraftPickType.Picked, draft[13].PickType);
            Assert.AreEqual("AZTLucker", draft[13].Player!.Name);
            Assert.AreEqual(StormTeam.Blue, draft[13].Team);
        }

        [TestMethod]
        public void TeamLevelsTest()
        {
            List<StormTeamLevel> levelsBlue = _stormReplay.GetTeamLevels(StormTeam.Blue)!.ToList();
            List<StormTeamLevel> levelsRed = _stormReplay.GetTeamLevels(StormTeam.Red)!.ToList();

            Assert.AreEqual(18, levelsBlue.Count);
            Assert.AreEqual(20, levelsRed.Count);

            Assert.AreEqual(1, levelsBlue[0].Level);
            Assert.AreEqual(new TimeSpan(32500000), levelsBlue[0].Time);

            Assert.AreEqual(8, levelsBlue[7].Level);
            Assert.AreEqual(new TimeSpan(3701875000), levelsBlue[7].Time);

            Assert.AreEqual(18, levelsBlue[17].Level);
            Assert.AreEqual(new TimeSpan(11347500000), levelsBlue[17].Time);

            Assert.AreEqual(1, levelsRed[0].Level);
            Assert.AreEqual(new TimeSpan(32500000), levelsRed[0].Time);

            Assert.AreEqual(10, levelsRed[9].Level);
            Assert.AreEqual(new TimeSpan(5124375000), levelsRed[9].Time);

            Assert.AreEqual(20, levelsRed[19].Level);
            Assert.AreEqual(new TimeSpan(11850625000), levelsRed[19].Time);
        }

        [TestMethod]
        public void TeamsFinalLevelTest()
        {
            Assert.AreEqual(18, _stormReplay.GetTeamFinalLevel(StormTeam.Blue));
            Assert.AreEqual(20, _stormReplay.GetTeamFinalLevel(StormTeam.Red));
            Assert.IsNull(_stormReplay.GetTeamFinalLevel(StormTeam.Observer));
        }

        [TestMethod]
        public void TeamXpBreakdownTest()
        {
            List<StormTeamXPBreakdown>? xpBlue = _stormReplay.GetTeamXPBreakdown(StormTeam.Blue)?.ToList();
            List<StormTeamXPBreakdown>? xpRed = _stormReplay.GetTeamXPBreakdown(StormTeam.Red)?.ToList();
            List<StormTeamXPBreakdown>? xpOther = _stormReplay.GetTeamXPBreakdown(StormTeam.Observer)?.ToList();

            Assert.AreEqual(20, xpBlue!.Count);
            Assert.AreEqual(20, xpRed!.Count);
            Assert.IsNull(xpOther);

            StormTeamXPBreakdown blue = xpBlue[3];

            Assert.AreEqual(1272, blue.HeroXP);
            Assert.AreEqual(5, blue.Level);
            Assert.AreEqual(360, blue.CreepXP);
            Assert.AreEqual(4868, blue.MinionXP);
            Assert.AreEqual(4100, blue.PassiveXP);
            Assert.AreEqual(0, blue.StructureXP);
            Assert.AreEqual(new TimeSpan(2781250000), blue.Time);
            Assert.AreEqual(10600, blue.TotalXP);

            blue = xpBlue[19];
            Assert.AreEqual(6037, blue.HeroXP);
            Assert.AreEqual(18, blue.Level);
            Assert.AreEqual(4668, blue.CreepXP);
            Assert.AreEqual(21883, blue.MinionXP);
            Assert.AreEqual(22520, blue.PassiveXP);
            Assert.AreEqual(7250, blue.StructureXP);
            Assert.AreEqual(new TimeSpan(12065000000), blue.Time);
            Assert.AreEqual(62358, blue.TotalXP);

            StormTeamXPBreakdown red = xpRed[3];

            Assert.AreEqual(0, red.HeroXP);
            Assert.AreEqual(5, red.Level);
            Assert.AreEqual(225, red.CreepXP);
            Assert.AreEqual(5082, red.MinionXP);
            Assert.AreEqual(4100, red.PassiveXP);
            Assert.AreEqual(0, red.StructureXP);
            Assert.AreEqual(new TimeSpan(2781250000), red.Time);
            Assert.AreEqual(9407, red.TotalXP);

            red = xpRed[19];
            Assert.AreEqual(12729, red.HeroXP);
            Assert.AreEqual(20, red.Level);
            Assert.AreEqual(6083, red.CreepXP);
            Assert.AreEqual(23551, red.MinionXP);
            Assert.AreEqual(22520, red.PassiveXP);
            Assert.AreEqual(8850, red.StructureXP);
            Assert.AreEqual(new TimeSpan(12065000000), red.Time);
            Assert.AreEqual(73733, red.TotalXP);
        }

        [TestMethod]
        public void PlayersScoreResultTest()
        {
            StormPlayer player = _stormReplay.StormPlayers.ToList()[8];

            Assert.AreEqual("Malfurion", player.PlayerHero!.HeroName);

            ScoreResult? scoreResult = player.ScoreResult;

            Assert.AreEqual(8, scoreResult!.Assists);
            Assert.AreEqual(3, scoreResult.ClutchHealsPerformed);
            Assert.AreEqual(8266, scoreResult.CreepDamage);
            Assert.AreEqual(18772, scoreResult.DamageSoaked);
            Assert.AreEqual(21863, scoreResult.DamageTaken);
            Assert.AreEqual(1, scoreResult.Deaths);
            Assert.AreEqual(0, scoreResult.EscapesPerformed);
            Assert.AreEqual(7123, scoreResult.ExperienceContribution);
            Assert.AreEqual(65166, scoreResult.Healing);
            Assert.AreEqual(13986, scoreResult.HeroDamage);
            Assert.AreEqual(8, scoreResult.HighestKillStreak);
            Assert.AreEqual(18, scoreResult.Level);
            Assert.AreEqual(0, scoreResult.MercCampCaptures);
            Assert.AreEqual(62359, scoreResult.MetaExperience);
            Assert.AreEqual(12804, scoreResult.MinionDamage);
            Assert.AreEqual(0, scoreResult.Multikill);
            Assert.AreEqual(0, scoreResult.OutnumberedDeaths);
            Assert.AreEqual(null, scoreResult.PhysicalDamage);
            Assert.AreEqual(0, scoreResult.ProtectionGivenToAllies);
            Assert.AreEqual(0, scoreResult.SelfHealing);
            Assert.AreEqual(22012, scoreResult.SiegeDamage);
            Assert.AreEqual(0, scoreResult.SoloKills);
            Assert.AreEqual(null, scoreResult.SpellDamage);
            Assert.AreEqual(9208, scoreResult.StructureDamage);
            Assert.AreEqual(0, scoreResult.SummonDamage);
            Assert.AreEqual(8, scoreResult.Takedowns);
            Assert.AreEqual(12196, scoreResult.TeamfightDamageTaken);
            Assert.AreEqual(0, scoreResult.TeamfightEscapesPerformed);
            Assert.AreEqual(16378, scoreResult.TeamfightHealingDone);
            Assert.AreEqual(5228, scoreResult.TeamfightHeroDamage);
            Assert.AreEqual(new TimeSpan(0, 0, 28), scoreResult.TimeCCdEnemyHeroes);
            Assert.AreEqual(new TimeSpan(0, 0, 25), scoreResult.TimeRootingEnemyHeroes);
            Assert.AreEqual(new TimeSpan(0, 0, 0), scoreResult.TimeSilencingEnemyHeroes);
            Assert.AreEqual(new TimeSpan(0, 0, 56), scoreResult.TimeSpentDead);
            Assert.AreEqual(new TimeSpan(0, 0, 0), scoreResult.TimeStunningEnemyHeroes);
            Assert.AreEqual(0, scoreResult.TownKills);
            Assert.AreEqual(0, scoreResult.VengeancesPerformed);
            Assert.AreEqual(0, scoreResult.WatchTowerCaptures);
            Assert.AreEqual(1, scoreResult.Tier1Talent);
            Assert.AreEqual(3, scoreResult.Tier4Talent);
            Assert.AreEqual(3, scoreResult.Tier7Talent);
            Assert.AreEqual(1, scoreResult.Tier10Talent);
            Assert.AreEqual(3, scoreResult.Tier13Talent);
            Assert.AreEqual(2, scoreResult.Tier16Talent);
            Assert.AreEqual(null, scoreResult.Tier20Talent);
        }

        [TestMethod]
        public void PlayersMatchAwardsTest()
        {
            List<MatchAwardType> matchAwards = _stormReplay.StormPlayers.ToList()[8].MatchAwards!.ToList();

            Assert.AreEqual(0, matchAwards.Count);
        }

        [TestMethod]
        public void MessagesTest()
        {
            List<IStormMessage> messages = _stormReplay.Messages.ToList();

            IStormMessage stormMessage = messages[144];

            Assert.AreEqual(StormMessageEventType.SPlayerAnnounceMessage, stormMessage.MessageEventType);
            Assert.AreEqual("Li Li", stormMessage.MessageSender!.PlayerHero!.HeroName);
            Assert.AreEqual(0, ((PlayerAnnounceMessage)stormMessage).AbilityAnnouncement!.Value.AbilityIndex);
            Assert.AreEqual(423, ((PlayerAnnounceMessage)stormMessage).AbilityAnnouncement!.Value.AbilityLink);
            Assert.AreEqual(954, ((PlayerAnnounceMessage)stormMessage).AbilityAnnouncement!.Value.ButtonLink);
            Assert.AreEqual(new TimeSpan(7110000000), stormMessage.Timestamp);

            stormMessage = messages.Last();

            Assert.AreEqual(StormMessageEventType.SChatMessage, stormMessage.MessageEventType);
            Assert.AreEqual("Li-Ming", stormMessage.MessageSender!.PlayerHero!.HeroName);
            Assert.AreEqual(new TimeSpan(12031875000), stormMessage.Timestamp);
            Assert.AreEqual(StormMessageTarget.All, ((ChatMessage)stormMessage).MessageTarget);
        }

        [TestMethod]
        public void ChatMessagesTest()
        {
            List<IStormMessage> messages = _stormReplay.ChatMessages.ToList();

            Assert.AreEqual(4, messages.Count);
            Assert.IsTrue(messages.All(x => !string.IsNullOrEmpty(x.Message)));
        }

        [TestMethod]
        public void BattleLobbyDataTest()
        {
            List<StormPlayer> players = _stormReplay.StormPlayersWithObservers.ToList();

            Assert.IsTrue(_stormReplay.IsBattleLobbyPlayerInfoParsed);

            Assert.IsNull(players[1].AccountLevel);
            Assert.AreEqual(null, players[1].PartyValue);
            Assert.IsNull(players[9].AccountLevel);
            Assert.AreEqual(null, players[9].PartyValue);

            Assert.IsTrue(players[0].BattleTagName.StartsWith(players[0].Name));
            Assert.IsTrue(players[0].BattleTagName.Contains('#'));
            Assert.IsTrue(players[0].BattleTagName.EndsWith("34"));

            Assert.AreEqual(6462480, players[8].ToonHandle!.Id);
            Assert.AreEqual(1869768008, players[8].ToonHandle!.ProgramId);
            Assert.AreEqual(1, players[8].ToonHandle!.Realm);
            Assert.AreEqual(1, players[8].ToonHandle!.Region);
            Assert.AreEqual(StormRegion.US, players[8].ToonHandle!.StormRegion);
            Assert.AreEqual("T:93796888#558", players[8].ToonHandle!.ShortcutId);
            Assert.AreEqual("1-Hero-1-6462480", players[8].ToonHandle!.ToString());
            Assert.IsTrue(players[8].BattleTagName.StartsWith(players[8].Name));
            Assert.IsTrue(players[8].BattleTagName.Contains('#'));
            Assert.IsTrue(players[8].BattleTagName.EndsWith("27"));
        }

        [TestMethod]
        public void TrackerEventsTest()
        {
            Assert.AreEqual(5239, _stormReplay.TrackerEvents.Count);
            Assert.AreEqual("BattlefieldOfEternity", _stormReplay.MapInfo.MapId);
        }

        [TestMethod]
        public void GameEventsTest()
        {
            Assert.AreEqual(347248, _stormReplay.GameEvents.Count);
            Assert.AreEqual("Hanzo", _stormReplay.Owner!.PlayerHero!.HeroName);
        }

        [TestMethod]
        public void PlayerTalentsTest()
        {
            List<StormPlayer> players = _stormReplay.StormPlayers.ToList();

            // sonya
            Assert.AreEqual(2, players[1].Talents[0].TalentSlotId);
            Assert.AreEqual("BarbarianToughAsNails", players[1].Talents[0].TalentNameId);
            Assert.AreEqual(20, players[1].Talents[0].Timestamp!.Value.Seconds);

            Assert.AreEqual(5, players[1].Talents[1].TalentSlotId);
            Assert.AreEqual("BarbarianShotOfFury", players[1].Talents[1].TalentNameId);
            Assert.AreEqual(3, players[1].Talents[1].Timestamp!.Value.Minutes);
            Assert.AreEqual(16, players[1].Talents[1].Timestamp!.Value.Seconds);

            Assert.AreEqual(8, players[1].Talents[2].TalentSlotId);
            Assert.AreEqual("BarbarianBattleRage", players[1].Talents[2].TalentNameId);
            Assert.AreEqual(6, players[1].Talents[2].Timestamp!.Value.Minutes);
            Assert.AreEqual(23, players[1].Talents[2].Timestamp!.Value.Seconds);

            Assert.AreEqual(10, players[1].Talents[3].TalentSlotId);
            Assert.AreEqual("BarbarianHeroicAbilityWrathoftheBerserker", players[1].Talents[3].TalentNameId);
            Assert.AreEqual(8, players[1].Talents[3].Timestamp!.Value.Minutes);
            Assert.AreEqual(37, players[1].Talents[3].Timestamp!.Value.Seconds);

            Assert.AreEqual(11, players[1].Talents[4].TalentSlotId);
            Assert.AreEqual("BarbarianMysticalSpear", players[1].Talents[4].TalentNameId);
            Assert.AreEqual(11, players[1].Talents[4].Timestamp!.Value.Minutes);
            Assert.AreEqual(20, players[1].Talents[4].Timestamp!.Value.Seconds);

            Assert.AreEqual(15, players[1].Talents[5].TalentSlotId);
            Assert.AreEqual("BarbarianRampage", players[1].Talents[5].TalentNameId);
            Assert.AreEqual(15, players[1].Talents[5].Timestamp!.Value.Minutes);
            Assert.AreEqual(30, players[1].Talents[5].Timestamp!.Value.Seconds);

            Assert.AreEqual(19, players[1].Talents[6].TalentSlotId);
            Assert.AreEqual("BarbarianCompositeSpear", players[1].Talents[6].TalentNameId);
            Assert.AreEqual(19, players[1].Talents[6].Timestamp!.Value.Minutes);
            Assert.AreEqual(52, players[1].Talents[6].Timestamp!.Value.Seconds);
        }

        [TestMethod]
        [TestCategory("Parsing Options")]
        public void NoTrackerEventsParsingTests()
        {
            StormReplayResult result = StormReplay.Parse(Path.Combine(_replaysFolder, _replayFile), new ParseOptions()
            {
                AllowPTR = false,
                ShouldParseGameEvents = true,
                ShouldParseMessageEvents = true,
                ShouldParseTrackerEvents = false,
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
                ShouldParseGameEvents = false,
                ShouldParseMessageEvents = true,
                ShouldParseTrackerEvents = true,
            });

            Assert.AreEqual(StormReplayParseStatus.Success, result.Status);
            NoGameEvents(result);

            List<StormPlayer> players = result.Replay.StormPlayers.ToList();
            Assert.AreEqual(6, players[0].Talents.Count);

            Assert.AreEqual("GreymaneInnerBeastViciousness", players[0].Talents[0].TalentNameId);
            Assert.IsNull(players[0].Talents[0].TalentSlotId);
            Assert.IsNull(players[0].Talents[0].Timestamp);

            Assert.AreEqual("GreymaneInnerBeastInsatiable", players[0].Talents[1].TalentNameId);
            Assert.IsNull(players[0].Talents[1].TalentSlotId);
            Assert.IsNull(players[0].Talents[1].Timestamp);

            Assert.AreEqual("GreymaneWorgenFormQuicksilverBullets", players[0].Talents[2].TalentNameId);
            Assert.IsNull(players[0].Talents[2].TalentSlotId);
            Assert.IsNull(players[0].Talents[2].Timestamp);

            Assert.AreEqual("GreymaneHeroicAbilityGoForTheThroat", players[0].Talents[3].TalentNameId);
            Assert.IsNull(players[0].Talents[3].TalentSlotId);
            Assert.IsNull(players[0].Talents[3].Timestamp);

            Assert.AreEqual("GreymaneInnerBeastOnTheProwl", players[0].Talents[4].TalentNameId);
            Assert.IsNull(players[0].Talents[4].TalentSlotId);
            Assert.IsNull(players[0].Talents[4].Timestamp);

            Assert.AreEqual("HeroGenericExecutionerPassive", players[0].Talents[5].TalentNameId);
            Assert.IsNull(players[0].Talents[5].TalentSlotId);
            Assert.IsNull(players[0].Talents[5].Timestamp);
        }

        private static void NoTrackerEvents(StormReplayResult result)
        {
            StormReplay replay = result.Replay!;

            Assert.IsNull(result.Replay.MapInfo.MapId);

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
            Assert.IsNull(replay.Owner?.PlayerHero?.HeroName);
        }
    }
}
