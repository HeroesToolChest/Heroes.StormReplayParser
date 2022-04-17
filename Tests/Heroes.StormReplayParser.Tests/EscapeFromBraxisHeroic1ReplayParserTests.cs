namespace Heroes.StormReplayParser.Tests;

[TestClass]
public class EscapeFromBraxisHeroic1ReplayParserTests
{
    private readonly string _replaysFolder = "Replays";
    private readonly StormReplay _stormReplay;
    private readonly StormReplayParseStatus _result;

    public EscapeFromBraxisHeroic1ReplayParserTests()
    {
        StormReplayResult result = StormReplay.Parse(Path.Combine(_replaysFolder, "EscapeFromBraxis(Heroic)1_70200.StormR"));
        _stormReplay = result.Replay;
        _result = result.Status;
    }

    [TestMethod]
    public void PlayerCountTest()
    {
        Assert.AreEqual(5, _stormReplay.PlayersWithObserversCount);
        Assert.AreEqual(10, _stormReplay.PlayersCount);
    }

    [TestMethod]
    public void PlayerListTest()
    {
        Assert.AreEqual(10, _stormReplay.StormPlayers.Count());
        Assert.AreEqual(5, _stormReplay.StormPlayersWithObservers.Count());
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
        Assert.AreEqual(40, _stormReplay.ReplayVersion.Minor);
        Assert.AreEqual(0, _stormReplay.ReplayVersion.Revision);
        Assert.AreEqual(70200, _stormReplay.ReplayVersion.Build);
        Assert.AreEqual(70200, _stormReplay.ReplayVersion.BaseBuild);

        Assert.AreEqual(70200, _stormReplay.ReplayBuild);
    }
}
