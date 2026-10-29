namespace Heroes.StormReplayParser.Tests;

[TestClass]
public class LostCavernNonSingleUnit1ReplayParserTests
{
    private readonly string _replaysFolder = "Replays";
    private readonly StormReplay _stormReplay;
    private readonly StormReplayParseStatus _result;

    public LostCavernNonSingleUnit1ReplayParserTests()
    {
        StormReplayResult result = StormReplay.Parse(Path.Combine(_replaysFolder, "LostCavernNonSingleUnit1_76517.StormR"));
        _stormReplay = result.Replay;
        _result = result.Status;
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
        Assert.HasCount(10, _stormReplay.StormPlayers);
        Assert.HasCount(10, _stormReplay.StormPlayersWithObservers);
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
        Assert.AreEqual(48, _stormReplay.ReplayVersion.Minor);
        Assert.AreEqual(1, _stormReplay.ReplayVersion.Revision);
        Assert.AreEqual(76517, _stormReplay.ReplayVersion.Build);
        Assert.AreEqual(76517, _stormReplay.ReplayVersion.BaseBuild);

        Assert.AreEqual(76517, _stormReplay.ReplayBuild);
    }
}
