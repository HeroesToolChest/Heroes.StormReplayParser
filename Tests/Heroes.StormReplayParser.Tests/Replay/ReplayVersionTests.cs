using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Heroes.StormReplayParser.Replay.Tests
{
    [TestClass]
    public class ReplayVersionTests
    {
        [TestMethod]
        public void EqualsTest()
        {
            StormReplayVersion replayVersion1 = new StormReplayVersion()
            {
                Major = 1,
                Minor = 3,
                Revision = 23,
                Build = 33444,
                BaseBuild = 33444,
            };

            StormReplayVersion replayVersion2 = new StormReplayVersion()
            {
                Major = 1,
                Minor = 3,
                Revision = 23,
                Build = 33444,
                BaseBuild = 33444,
            };

            Assert.AreEqual(replayVersion1, replayVersion2);
            Assert.AreEqual(replayVersion2, replayVersion1);
        }

        [TestMethod]
        public void NotEqualsTest()
        {
            StormReplayVersion replayVersion1 = new StormReplayVersion()
            {
                Major = 1,
                Minor = 3,
                Revision = 23,
                Build = 33443,
                BaseBuild = 33444,
            };

            StormReplayVersion replayVersionMajor = new StormReplayVersion()
            {
                Major = 2,
                Minor = 3,
                Revision = 23,
                Build = 33443,
                BaseBuild = 33444,
            };

            StormReplayVersion replayVersionMinor = new StormReplayVersion()
            {
                Major = 1,
                Minor = 4,
                Revision = 23,
                Build = 33443,
                BaseBuild = 33444,
            };

            StormReplayVersion replayVersionRevision = new StormReplayVersion()
            {
                Major = 1,
                Minor = 3,
                Revision = 24,
                Build = 33443,
                BaseBuild = 33444,
            };

            StormReplayVersion replayVersionBuild = new StormReplayVersion()
            {
                Major = 1,
                Minor = 3,
                Revision = 23,
                Build = 33444,
                BaseBuild = 33444,
            };

            StormReplayVersion replayVersionBaseBuild = new StormReplayVersion()
            {
                Major = 1,
                Minor = 3,
                Revision = 23,
                Build = 33443,
                BaseBuild = 33442,
            };

            Assert.AreNotEqual(replayVersion1, replayVersionMajor);
            Assert.AreNotEqual(replayVersion1, replayVersionMinor);
            Assert.AreNotEqual(replayVersion1, replayVersionRevision);
            Assert.AreNotEqual(replayVersion1, replayVersionBuild);
            Assert.AreNotEqual(replayVersion1, replayVersionBaseBuild);
        }

        [TestMethod]
        public void NotEqualsDiffObjects()
        {
            StormReplayVersion replayVersion1 = new StormReplayVersion()
            {
                Major = 1,
                Minor = 3,
                Revision = 23,
                Build = 33444,
                BaseBuild = 33444,
            };

            Assert.AreNotEqual(replayVersion1, new MapInfo());
        }
    }
}