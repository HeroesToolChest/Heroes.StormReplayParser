namespace Heroes.StormReplayParser.MpqFiles.Tests;

[TestClass]
public class ReplayAttributeEventsTests
{
    [TestMethod]
    [DataRow("HMT1", 1)]
    [DataRow("HMT2", 2)]
    [DataRow("HMT3", 3)]
    public void GetHeroMasteryRingTierTest(string value, int tier)
    {
        Assert.AreEqual(tier, ReplayAttributeEvents.GetHeroMasteryRingTier(value));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("HMT0")]
    [DataRow("HMT")]
    [DataRow("Humn")]
    public void GetHeroMasteryRingTierNoRingTest(string value)
    {
        Assert.IsNull(ReplayAttributeEvents.GetHeroMasteryRingTier(value));
    }
}
