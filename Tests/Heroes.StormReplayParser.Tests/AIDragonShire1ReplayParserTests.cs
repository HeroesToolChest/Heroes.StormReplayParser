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
    public class AIDragonShire1ReplayParserTests
    {
        private readonly string _replaysFolder = "Replays";
        private readonly StormReplay _stormReplay;
        private readonly StormReplayParseStatus _result;

        public AIDragonShire1ReplayParserTests()
        {
            StormReplayResult result = StormReplay.Parse(Path.Combine(_replaysFolder, "AIDragonShire1_75589.StormR"));
            _stormReplay = result.Replay;
            _result = result.Status;
        }

        [TestMethod]
        public void PlayerCountTest()
        {
            Assert.AreEqual(1, _stormReplay.PlayersWithObserversCount);
            Assert.AreEqual(10, _stormReplay.PlayersCount);
        }

        [TestMethod]
        public void PlayerListTest()
        {
            Assert.AreEqual(10, _stormReplay.StormPlayers.Count());
            Assert.AreEqual(1, _stormReplay.StormPlayersWithObservers.Count());
        }

        [TestMethod]
        public void ParseResult()
        {
            Assert.AreEqual(StormReplayParseStatus.Success, _result);
        }

        [TestMethod]
        public void StormReplayHeaderTest()
        {
            Assert.AreEqual(2, _stormReplay.ReplayVersion.Major);
            Assert.AreEqual(47, _stormReplay.ReplayVersion.Minor);
            Assert.AreEqual(0, _stormReplay.ReplayVersion.Revision);
            Assert.AreEqual(75589, _stormReplay.ReplayVersion.Build);
            Assert.AreEqual(75589, _stormReplay.ReplayVersion.BaseBuild);

            Assert.AreEqual(75589, _stormReplay.ReplayBuild);
            Assert.AreEqual(13231, _stormReplay.ElapsedGamesLoops);
        }

        [TestMethod]
        public void StormReplayDetailsTest()
        {
            List<StormPlayer> players = _stormReplay.StormPlayers.ToList();
            StormPlayer player0 = players[0];

            Assert.AreEqual("lavakill", player0.Name);
            Assert.AreEqual(1, player0.ToonHandle.Region);
            Assert.AreEqual(1, player0.ToonHandle.Realm);
            Assert.AreEqual(StormTeam.Blue, player0.Team);
            Assert.IsTrue(player0.IsWinner);
            Assert.AreEqual("Qhira", player0.PlayerHero.HeroName);

            StormPlayer player9 = players[9];

            Assert.AreEqual("Player 10", player9.Name);
            Assert.AreEqual(0, player9.ToonHandle.Region);
            Assert.AreEqual(0, player9.ToonHandle.Realm);
            Assert.AreEqual(StormTeam.Red, player9.Team);
            Assert.IsFalse(player9.IsWinner);
            Assert.AreEqual("Valeera", player9.PlayerHero.HeroName);

            Assert.AreEqual("Dragon Shire", _stormReplay.MapInfo.MapName);
            Assert.AreEqual(637010888527768698, _stormReplay.Timestamp.Ticks);

            Assert.IsFalse(_stormReplay.HasObservers);
            Assert.IsTrue(_stormReplay.HasAI);
        }

        [TestMethod]
        public void StormReplayAttributeEventsTest()
        {
            List<StormPlayer> players = _stormReplay.StormPlayers.ToList();
            StormPlayer player = players[9];

            Assert.AreEqual("5v5", _stormReplay.TeamSize);
            Assert.AreEqual(PlayerDifficulty.Veteran, player.PlayerDifficulty);
            Assert.AreEqual(GameSpeed.Faster, _stormReplay.GameSpeed);
            Assert.AreEqual(GameMode.Cooperative, _stormReplay.GameMode);
            Assert.AreEqual(string.Empty, player.PlayerHero.HeroAttributeId);
            Assert.AreEqual(string.Empty, player.PlayerLoadout.SkinAndSkinTintAttributeId);
            Assert.AreEqual(string.Empty, player.PlayerLoadout.MountAndMountTintAttributeId);
            Assert.AreEqual(string.Empty, player.PlayerLoadout.BannerAttributeId);
            Assert.AreEqual(string.Empty, player.PlayerLoadout.SprayAttributeId);
            Assert.AreEqual(string.Empty, player.PlayerLoadout.VoiceLineAttributeId);
            Assert.AreEqual(string.Empty, player.PlayerLoadout.AnnouncerPackAttributeId);
            Assert.AreEqual(1, player.PlayerHero.HeroLevel);

            List<string?> ban0List = _stormReplay.GetTeamBans(StormTeam.Blue).ToList();
            List<string?> ban1List = _stormReplay.GetTeamBans(StormTeam.Red).ToList();

            Assert.AreEqual(string.Empty, ban0List[1]);
            Assert.AreEqual(string.Empty, ban1List[1]);
        }

        [TestMethod]
        public void DraftOrderTest()
        {
            var draft = _stormReplay.GetDraftOrder().ToList();

            Assert.AreEqual(0, draft.Count);
        }

        [TestMethod]
        public void TeamLevelsTest()
        {
            List<TeamLevel> levelsBlue = _stormReplay.GetTeamLevels(StormTeam.Blue).ToList();
            List<TeamLevel> levelsRed = _stormReplay.GetTeamLevels(StormTeam.Red).ToList();

            Assert.AreEqual(15, levelsBlue.Count);
            Assert.AreEqual(15, levelsRed.Count);

            Assert.AreEqual(1, levelsBlue[0].Level);
            Assert.AreEqual(new TimeSpan(32500000), levelsBlue[0].Time);

            Assert.AreEqual(8, levelsBlue[7].Level);
            Assert.AreEqual(new TimeSpan(3390625000), levelsBlue[7].Time);

            Assert.AreEqual(1, levelsRed[0].Level);
            Assert.AreEqual(new TimeSpan(32500000), levelsRed[0].Time);

            Assert.AreEqual(10, levelsRed[9].Level);
            Assert.AreEqual(new TimeSpan(4350625000), levelsRed[9].Time);
        }

        [TestMethod]
        public void TeamsFinalLevelTest()
        {
            Assert.AreEqual(15, _stormReplay.GetTeamFinalLevel(StormTeam.Blue));
            Assert.AreEqual(15, _stormReplay.GetTeamFinalLevel(StormTeam.Red));
            Assert.AreEqual(0, _stormReplay.GetTeamFinalLevel(StormTeam.Observer));
        }

        [TestMethod]
        public void TeamXpBreakdownTest()
        {
            List<TeamXPBreakdown> xpBlue = _stormReplay.GetTeamXPBreakdown(StormTeam.Blue).ToList();
            List<TeamXPBreakdown> xpRed = _stormReplay.GetTeamXPBreakdown(StormTeam.Red).ToList();
            List<TeamXPBreakdown> xpOther = _stormReplay.GetTeamXPBreakdown(StormTeam.Observer).ToList();

            Assert.AreEqual(13, xpBlue.Count);
            Assert.AreEqual(13, xpRed.Count);
            Assert.AreEqual(0, xpOther.Count);

            TeamXPBreakdown blue = xpBlue[3];

            Assert.AreEqual(0, blue.HeroXP);
            Assert.AreEqual(6, blue.Level);
            Assert.AreEqual(0, blue.CreepXP);
            Assert.AreEqual(7724, blue.MinionXP);
            Assert.AreEqual(4715, blue.PassiveXP);
            Assert.AreEqual(250, blue.StructureXP);
            Assert.AreEqual(new TimeSpan(2781250000), blue.Time);
            Assert.AreEqual(12689, blue.TotalXP);

            blue = xpBlue[12];
            Assert.AreEqual(5849, blue.HeroXP);
            Assert.AreEqual(15, blue.Level);
            Assert.AreEqual(1159, blue.CreepXP);
            Assert.AreEqual(22001, blue.MinionXP);
            Assert.AreEqual(18514, blue.PassiveXP);
            Assert.AreEqual(1300, blue.StructureXP);
            Assert.AreEqual(new TimeSpan(8157500000), blue.Time);
            Assert.AreEqual(48823, blue.TotalXP);

            TeamXPBreakdown red = xpRed[3];

            Assert.AreEqual(2160, red.HeroXP);
            Assert.AreEqual(6, red.Level);
            Assert.AreEqual(0, red.CreepXP);
            Assert.AreEqual(6657, red.MinionXP);
            Assert.AreEqual(4715, red.PassiveXP);
            Assert.AreEqual(0, red.StructureXP);
            Assert.AreEqual(new TimeSpan(2781250000), red.Time);
            Assert.AreEqual(13532, red.TotalXP);

            red = xpRed[12];
            Assert.AreEqual(4214, red.HeroXP);
            Assert.AreEqual(15, red.Level);
            Assert.AreEqual(0, red.CreepXP);
            Assert.AreEqual(24203, red.MinionXP);
            Assert.AreEqual(17153, red.PassiveXP);
            Assert.AreEqual(500, red.StructureXP);
            Assert.AreEqual(new TimeSpan(8157500000), red.Time);
            Assert.AreEqual(46070, red.TotalXP);
        }

        [TestMethod]
        public void PlayersScoreResultTest()
        {
            StormPlayer player = _stormReplay.StormPlayers.ToList()[3];

            Assert.AreEqual("Kael'thas", player.PlayerHero.HeroName);

            ScoreResult scoreResult = player.ScoreResult;

            Assert.AreEqual(3, scoreResult.Assists);
            Assert.AreEqual(0, scoreResult.ClutchHealsPerformed);
            Assert.AreEqual(0, scoreResult.CreepDamage);
            Assert.AreEqual(10462, scoreResult.DamageSoaked);
            Assert.AreEqual(18385, scoreResult.DamageTaken);
            Assert.AreEqual(new TimeSpan(0, 0, 30), scoreResult.TimeCCdEnemyHeroes);
            Assert.AreEqual(new TimeSpan(0, 0, 0), scoreResult.TimeRootingEnemyHeroes);
            Assert.AreEqual(new TimeSpan(0, 0, 0), scoreResult.TimeSilencingEnemyHeroes);
            Assert.AreEqual(new TimeSpan(0, 1, 23), scoreResult.TimeSpentDead);
            Assert.AreEqual(new TimeSpan(0, 0, 12), scoreResult.TimeStunningEnemyHeroes);
            Assert.AreEqual(2, scoreResult.TownKills);
            Assert.AreEqual(1, scoreResult.VengeancesPerformed);
            Assert.AreEqual(0, scoreResult.WatchTowerCaptures);
            Assert.AreEqual(39, scoreResult.MinionKills);
            Assert.AreEqual(15, scoreResult.RegenGlobes);
            Assert.AreEqual(1, scoreResult.Tier1Talent);
            Assert.AreEqual(3, scoreResult.Tier4Talent);
            Assert.AreEqual(1, scoreResult.Tier7Talent);
            Assert.AreEqual(1, scoreResult.Tier10Talent);
            Assert.AreEqual(2, scoreResult.Tier13Talent);
            Assert.AreEqual(null, scoreResult.Tier16Talent);
            Assert.AreEqual(null, scoreResult.Tier20Talent);
        }

        [TestMethod]
        public void PlayersMatchAwardsTest()
        {
            List<MatchAwardType> matchAwards = _stormReplay.StormPlayers.ToList()[3].MatchAwards.ToList();

            Assert.AreEqual(0, matchAwards.Count);

            matchAwards = _stormReplay.StormPlayers.ToList()[1].MatchAwards.ToList();
            Assert.AreEqual(1, matchAwards.Count);
            Assert.AreEqual(MatchAwardType.MostDragonShrinesCaptured, matchAwards[0]);
        }

        [TestMethod]
        public void MessagesTest()
        {
            List<StormMessage> messages = _stormReplay.Messages.ToList();

            Assert.AreEqual(10, messages.Count);
        }

        [TestMethod]
        public void ChatMessagesTest()
        {
            List<StormMessage> messages = _stormReplay.ChatMessages.ToList();

            Assert.AreEqual(0, messages.Count);
            Assert.IsTrue(messages.All(x => x.ChatMessage != null && !string.IsNullOrEmpty(x.ChatMessage.Message)));
        }

        [TestMethod]
        public void BattleLobbyDataTest()
        {
            List<StormPlayer> players = _stormReplay.StormPlayers.ToList();

            Assert.AreEqual(2331, players[0].AccountLevel);
            Assert.AreEqual(null, players[0].PartyValue);
        }
    }
}
