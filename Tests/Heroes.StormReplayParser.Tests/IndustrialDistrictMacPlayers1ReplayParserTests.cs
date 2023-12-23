namespace Heroes.StormReplayParser.Tests;

[TestClass]
public class IndustrialDistrictMacPlayers1ReplayParserTests
{
    private readonly string _replaysFolder = "Replays";
    private readonly StormReplay _stormReplay;
    private readonly StormReplayParseStatus _result;

    public IndustrialDistrictMacPlayers1ReplayParserTests()
    {
        StormReplayResult result = StormReplay.Parse(Path.Combine(_replaysFolder, "IndustrialDistrictMacPlayers1.StormR"));
        _stormReplay = result.Replay;
        _result = result.Status;
    }

    [TestMethod]
    public void ParseResult()
    {
        Assert.AreEqual(StormReplayParseStatus.Success, _result);
    }

    [TestMethod]
    public void PlayerOnMacPlatformTest()
    {
        List<StormPlayer> players = _stormReplay.StormPlayers.ToList();
        Assert.IsFalse(players[0].IsPlatformMac);
        Assert.IsTrue(players[1].IsPlatformMac);
        Assert.IsFalse(players[2].IsPlatformMac);
        Assert.IsFalse(players[3].IsPlatformMac);
        Assert.IsFalse(players[4].IsPlatformMac);
        Assert.IsFalse(players[5].IsPlatformMac);
        Assert.IsFalse(players[6].IsPlatformMac);
        Assert.IsFalse(players[7].IsPlatformMac);
        Assert.IsTrue(players[8].IsPlatformMac);
        Assert.IsFalse(players[9].IsPlatformMac);
    }
}
