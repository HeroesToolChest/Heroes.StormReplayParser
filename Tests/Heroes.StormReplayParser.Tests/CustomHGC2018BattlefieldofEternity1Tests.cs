using Heroes.StormReplayParser.Replay;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace Heroes.StormReplayParser.Tests
{
    [TestClass]
    public class CustomHGC2018BattlefieldofEternity1Tests
    {
        private readonly string _replaysFolder = "Replays";
        private readonly string _replayFile = "CustomHGC2018BattlefieldofEternity1_68509.StormR";
        private readonly StormReplay _stormReplay;

        public CustomHGC2018BattlefieldofEternity1Tests()
        {
            _stormReplay = StormReplay.Parse(Path.Combine(_replaysFolder, _replayFile)).Replay;
        }

        [TestMethod]
        public void PlayerCountTest()
        {
            Assert.AreEqual(14, _stormReplay.PlayersWithObserversCount);
            Assert.AreEqual(10, _stormReplay.PlayersCount);
        }

        [TestMethod]
        public void DraftOrderTest()
        {
            var draft = _stormReplay.DraftPicks.ToList();

            Assert.AreEqual(20, draft.Count);

            Assert.AreEqual("Medivh", draft[0].HeroSelected);
            Assert.AreEqual(StormDraftPickType.Banned, draft[0].PickType);
            Assert.IsNull(draft[0].Player);
            Assert.AreEqual(StormTeam.Blue, draft[0].Team);

            Assert.AreEqual("Uther", draft[17].HeroSelected);
            Assert.AreEqual(StormDraftPickType.Swapped, draft[17].PickType);
            Assert.AreEqual("TFYoDa", draft[17].Player!.Name);
            Assert.AreEqual(StormTeam.Red, draft[17].Team);

            Assert.AreEqual("Tassadar", draft[18].HeroSelected);
            Assert.AreEqual(StormDraftPickType.Swapped, draft[18].PickType);
            Assert.AreEqual("TSViN", draft[18].Player!.Name);
            Assert.AreEqual(StormTeam.Blue, draft[18].Team);

            Assert.AreEqual("Raynor", draft[19].HeroSelected);
            Assert.AreEqual(StormDraftPickType.Swapped, draft[19].PickType);
            Assert.AreEqual("TSFan", draft[19].Player!.Name);
            Assert.AreEqual(StormTeam.Blue, draft[19].Team);
        }
    }
}
