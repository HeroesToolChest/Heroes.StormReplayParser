using Heroes.StormReplayParser.Replay;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace Heroes.StormReplayParser.Tests
{
    [TestClass]
    public class SilverCity1ReplayParserTests
    {
        private readonly string _replaysFolder = "Replays";
        private readonly string _replayFile = "SilverCity1_85267.StormR";
        private readonly StormReplay _stormReplay;
        private readonly StormReplayParseStatus _result;

        public SilverCity1ReplayParserTests()
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
        public void StormReplayInitDataTest()
        {
            Assert.AreEqual(3845356360, _stormReplay.RandomValue);
            Assert.AreEqual(StormGameMode.ARAM, _stormReplay.GameMode);

            var disabledHeroes = _stormReplay.DisabledHeroes.ToList();

            Assert.AreEqual(1, disabledHeroes.Count);
            Assert.AreEqual("STUK", disabledHeroes[0]);
        }
    }
}
