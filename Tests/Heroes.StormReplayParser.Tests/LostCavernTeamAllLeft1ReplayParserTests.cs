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
        Assert.HasCount(1, _stormReplay.GetTeamLevels(StormTeam.Blue)!);
        Assert.HasCount(1, _stormReplay.GetTeamLevels(StormTeam.Red)!);
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
        List<StormPlayer> players = [.. _stormReplay.StormPlayers];

        Assert.HasCount(1, players[0].PlayerDisconnects);
        Assert.IsEmpty(players[1].PlayerDisconnects);
        Assert.IsEmpty(players[2].PlayerDisconnects);
        Assert.IsEmpty(players[3].PlayerDisconnects);
        Assert.IsEmpty(players[4].PlayerDisconnects);

        Assert.HasCount(1, players[5].PlayerDisconnects);
        Assert.AreEqual(new TimeSpan(168125000), players[5].PlayerDisconnects[0].From);
        Assert.IsNull(players[5].PlayerDisconnects[0].To);
        Assert.HasCount(1, players[6].PlayerDisconnects);
        Assert.HasCount(1, players[7].PlayerDisconnects);
        Assert.HasCount(1, players[8].PlayerDisconnects);
        Assert.AreEqual(new TimeSpan(426250000), players[9].PlayerDisconnects[0].From);
        Assert.IsNull(players[9].PlayerDisconnects[0].To);
    }
}
