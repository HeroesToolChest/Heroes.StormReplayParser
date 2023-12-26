namespace Heroes.StormReplayParser.Tests;

[TestClass]
public class LostCavernTeamAllLeft1ReplayParserTests
{
    private readonly string _replaysFolder = "Replays";
    private readonly StormReplay _stormReplay;
    private readonly StormReplayParseStatus _result;

    public LostCavernTeamAllLeft1ReplayParserTests()
    {
        StormReplayResult result = StormReplay.Parse(Path.Combine(_replaysFolder, "LostCavernTeamAllLeft1.StormR"));
        _stormReplay = result.Replay;
        _result = result.Status;
    }

    [TestMethod]
    public void ParseResult()
    {
        Assert.AreEqual(StormReplayParseStatus.Success, _result);
    }

    [TestMethod]
    public void TeamFinalLevelsTest()
    {
        Assert.AreEqual(1, _stormReplay.GetTeamFinalLevel(StormTeam.Blue));
        Assert.AreEqual(1, _stormReplay.GetTeamFinalLevel(StormTeam.Red));
    }

    [TestMethod]
    public void TeamLevelsTest()
    {
        Assert.AreEqual(1, _stormReplay.GetTeamLevels(StormTeam.Blue)!.Count);
        Assert.AreEqual(1, _stormReplay.GetTeamLevels(StormTeam.Red)!.Count);
    }

    [TestMethod]
    public void TeamXPBreakdownTest()
    {
        Assert.IsNull(_stormReplay.GetTeamXPBreakdown(StormTeam.Blue));
        Assert.IsNull(_stormReplay.GetTeamXPBreakdown(StormTeam.Red));
    }

    [TestMethod]
    public void PlayerDisconnectsTest()
    {
        List<StormPlayer> players = _stormReplay.StormPlayers.ToList();

        Assert.AreEqual(1, players[0].PlayerDisconnects.Count);
        Assert.AreEqual(0, players[1].PlayerDisconnects.Count);
        Assert.AreEqual(0, players[2].PlayerDisconnects.Count);
        Assert.AreEqual(0, players[3].PlayerDisconnects.Count);
        Assert.AreEqual(0, players[4].PlayerDisconnects.Count);

        Assert.AreEqual(1, players[5].PlayerDisconnects.Count);
        Assert.AreEqual(new TimeSpan(168125000), players[5].PlayerDisconnects[0].From);
        Assert.IsNull(players[5].PlayerDisconnects[0].To);
        Assert.AreEqual(1, players[6].PlayerDisconnects.Count);
        Assert.AreEqual(1, players[7].PlayerDisconnects.Count);
        Assert.AreEqual(1, players[8].PlayerDisconnects.Count);
        Assert.AreEqual(new TimeSpan(426250000), players[9].PlayerDisconnects[0].From);
        Assert.IsNull(players[9].PlayerDisconnects[0].To);
    }
}
