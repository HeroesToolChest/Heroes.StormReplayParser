namespace Heroes.StormReplayParser.Tests;

[TestClass]
public class CustomInfernalShrines3ReplayParserTests
{
    private readonly string _replaysFolder = "Replays";
    private readonly string _replayFile = "CustomInfernalShrines3_98348.StormR";
    private readonly StormReplay _stormReplay;
    private readonly StormReplayParseStatus _result;

    public CustomInfernalShrines3ReplayParserTests()
    {
        StormReplayResult result = StormReplay.Parse(Path.Combine(_replaysFolder, _replayFile));
        _stormReplay = result.Replay;
        _result = result.Status;
    }

    [TestMethod]
    public void ParseResultTest()
    {
        Assert.AreEqual(StormReplayParseStatus.Success, _result);
    }

    [TestMethod]
    public void PlayerCountTest()
    {
        Assert.AreEqual(13, _stormReplay.PlayersWithObserversCount);
        Assert.AreEqual(10, _stormReplay.PlayersCount);
    }

    [TestMethod]
    public void BanChooserTest()
    {
        Assert.AreEqual(StormBanChooserType.Player, _stormReplay.GetTeamBanChooserType(StormTeam.Blue));
        Assert.AreEqual(StormBanChooserType.Player, _stormReplay.GetTeamBanChooserType(StormTeam.Red));

        List<StormPlayer> players = [.. _stormReplay.StormPlayers];

        Assert.IsTrue(players[5].IsBanChooser);
        Assert.AreEqual(StormTeam.Red, players[5].Team);
        Assert.StartsWith("MasonB", players[5].Name);

        Assert.IsTrue(players[9].IsBanChooser);
        Assert.AreEqual(StormTeam.Blue, players[9].Team);
        Assert.StartsWith("Player", players[9].Name);

        Assert.AreSame(players[5], _stormReplay.GetTeamBanPlayerChooser(StormTeam.Red));
        Assert.AreSame(players[9], _stormReplay.GetTeamBanPlayerChooser(StormTeam.Blue));

        Assert.AreEqual(2, _stormReplay.StormPlayersWithObservers.Count(x => x.IsBanChooser));
    }
}
