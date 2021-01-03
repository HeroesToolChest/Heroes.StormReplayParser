using Heroes.StormReplayParser.Player;
using Heroes.StormReplayParser.Replay;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Heroes.StormReplayParser.Tests
{
    [TestClass]
    public class MissingWorkSetSlotId1ReplayParserTests
    {
        private readonly string _replaysFolder = "Replays";
        private readonly string _replayFile = "MissingWorkSetSlotId1.StormR";
        private readonly StormReplay _stormReplay;
        private readonly StormReplayParseStatus _result;

        public MissingWorkSetSlotId1ReplayParserTests()
        {
            StormReplayResult result = StormReplay.Parse(Path.Combine(_replaysFolder, _replayFile));
            _stormReplay = result.Replay;
            _result = result.Status;
        }

        [TestMethod]
        public void ParseResultTest()
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
        public void StormReplayDetailsTest()
        {
            List<StormPlayer> players = _stormReplay.StormPlayers.ToList();
            StormPlayer player0 = players[0];

            Assert.AreEqual("kpaxchaos", player0.Name);
            Assert.AreEqual(2, player0.ToonHandle!.Region);
            Assert.AreEqual(1, player0.ToonHandle.Realm);
            Assert.AreEqual(StormRegion.EU, player0.ToonHandle.StormRegion);
            Assert.AreEqual(StormTeam.Blue, _stormReplay.WinningTeam);
        }

        [TestMethod]
        public void StormReplayInitDataTest()
        {
            Assert.AreEqual(2079817235, _stormReplay.RandomValue);
            Assert.AreEqual(StormGameMode.QuickMatch, _stormReplay.GameMode);

            List<StormPlayer> players = _stormReplay.StormPlayers.ToList();
            StormPlayer player0 = players[0];

            Assert.AreEqual("KaelthasRobotVar1", player0.PlayerLoadout.SkinAndSkinTint);
            Assert.AreEqual("RainbowUnicornGreen", player0.PlayerLoadout.MountAndMountTint);
            Assert.IsFalse(player0.IsSilenced);
            Assert.IsNull(player0.IsVoiceSilenced);
            Assert.IsNull(player0.IsBlizzardStaff);
            Assert.IsNull(player0.HasActiveBoost);
            Assert.AreEqual("BannerWCAllianceRare", player0.PlayerLoadout.Banner);
            Assert.AreEqual(6, player0.HeroMasteryTiersCount);
            Assert.AreEqual(PlayerType.Human, player0.PlayerType);

            StormPlayer player8 = players[8];

            Assert.AreEqual("ZagaraInsectoidBlue", player8.PlayerLoadout.SkinAndSkinTint);
            Assert.AreEqual("ZagaraWings", player8.PlayerLoadout.MountAndMountTint);
            Assert.IsFalse(player8.IsSilenced);
            Assert.IsNull(player8.IsVoiceSilenced);
            Assert.IsNull(player8.IsBlizzardStaff);
            Assert.IsNull(player8.HasActiveBoost);
            Assert.AreEqual("BannerDefault", player8.PlayerLoadout.Banner);
            Assert.AreEqual(1, player8.HeroMasteryTiersCount);
            Assert.AreEqual(PlayerType.Human, player8.PlayerType);
        }

        [TestMethod]
        public void PlayerScoreResultsTest()
        {
            List<StormPlayer> players = _stormReplay.StormPlayersWithObservers.ToList();

            Assert.IsNotNull(players[0].ScoreResult);
            Assert.IsNotNull(players[1].ScoreResult);
            Assert.IsNotNull(players[2].ScoreResult);
            Assert.IsNotNull(players[3].ScoreResult);
        }
    }
}
