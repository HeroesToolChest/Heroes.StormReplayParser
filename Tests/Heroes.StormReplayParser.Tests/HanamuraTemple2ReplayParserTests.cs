using Heroes.StormReplayParser.Player;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Heroes.StormReplayParser.Tests
{
    [TestClass]
    public class HanamuraTemple2ReplayParserTests
    {
        private readonly string _replaysFolder = "Replays";
        private readonly string _replayFile = "HanamuraTemple2_67679.StormR";
        private readonly StormReplay _stormReplay;

        public HanamuraTemple2ReplayParserTests()
        {
            _stormReplay = StormReplay.Parse(Path.Combine(_replaysFolder, _replayFile)).Replay;
        }

        [TestMethod]
        public void RegionTest()
        {
            Assert.AreEqual(StormRegion.XX, _stormReplay.Region);
        }

        [TestMethod]
        public void StormReplayInitDataTest()
        {
            List<StormPlayer> players = _stormReplay.StormPlayers.ToList();
            StormPlayer player0 = players[0];

            Assert.IsFalse(player0.IsSilenced);
            Assert.IsNull(player0.IsVoiceSilenced);
            Assert.IsTrue(player0.IsBlizzardStaff!.Value);
            Assert.IsNull(player0.HasActiveBoost);
        }
    }
}
