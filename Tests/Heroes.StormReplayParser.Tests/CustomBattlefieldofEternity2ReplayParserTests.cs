namespace Heroes.StormReplayParser.Tests;

[TestClass]
public class CustomBattlefieldofEternity2ReplayParserTests
{
    private readonly string _replaysFolder = "Replays";
    private readonly string _replayFile = "CustomBattlefieldofEternity2_98348.StormR";
    private readonly StormReplay _stormReplay;
    private readonly StormReplayParseStatus _result;

    public CustomBattlefieldofEternity2ReplayParserTests()
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
    public void PlayerListTest()
    {
        Assert.HasCount(10, _stormReplay.StormPlayers);
        Assert.HasCount(13, _stormReplay.StormPlayersWithObservers);

        List<StormPlayer> players = [.. _stormReplay.StormPlayers];
        Assert.AreEqual(StormTeam.Blue, players[0].Team);
        Assert.AreEqual(StormTeam.Blue, players[1].Team);
        Assert.AreEqual(StormTeam.Red, players[2].Team);
        Assert.AreEqual(StormTeam.Blue, players[3].Team);
        Assert.AreEqual(StormTeam.Red, players[4].Team);
        Assert.AreEqual(StormTeam.Red, players[5].Team);
        Assert.AreEqual(StormTeam.Red, players[6].Team);
        Assert.AreEqual(StormTeam.Blue, players[7].Team);
        Assert.AreEqual(StormTeam.Blue, players[8].Team);
        Assert.AreEqual(StormTeam.Red, players[9].Team);

        List<StormPlayer> playersWithObs = [.. _stormReplay.StormPlayersWithObservers];
        Assert.AreEqual(StormTeam.Blue, playersWithObs[0].Team);
        Assert.AreEqual(StormTeam.Blue, playersWithObs[1].Team);
        Assert.AreEqual(StormTeam.Observer, playersWithObs[2].Team);
        Assert.AreEqual(StormTeam.Red, playersWithObs[3].Team);
        Assert.AreEqual(StormTeam.Blue, playersWithObs[4].Team);
        Assert.AreEqual(StormTeam.Red, playersWithObs[5].Team);
        Assert.AreEqual(StormTeam.Red, playersWithObs[6].Team);
        Assert.AreEqual(StormTeam.Red, playersWithObs[7].Team);
        Assert.AreEqual(StormTeam.Blue, playersWithObs[8].Team);
        Assert.AreEqual(StormTeam.Blue, playersWithObs[9].Team);
        Assert.AreEqual(StormTeam.Observer, playersWithObs[10].Team);
        Assert.AreEqual(StormTeam.Observer, playersWithObs[11].Team);
        Assert.AreEqual(StormTeam.Red, playersWithObs[12].Team);
    }

    [TestMethod]
    public void BanChooserTest()
    {
        Assert.AreEqual(StormBanChooserType.Player, _stormReplay.GetTeamBanChooserType(StormTeam.Blue));
        Assert.AreEqual(StormBanChooserType.Player, _stormReplay.GetTeamBanChooserType(StormTeam.Red));

        List<StormPlayer> players = [.. _stormReplay.StormPlayers];

        Assert.IsTrue(players[3].IsBanChooser);
        Assert.AreEqual(StormTeam.Blue, players[3].Team);
        Assert.AreEqual("Player", players[3].Name);

        Assert.IsTrue(players[6].IsBanChooser);
        Assert.AreEqual(StormTeam.Red, players[6].Team);
        Assert.AreEqual("MasonBlaze", players[6].Name);

        Assert.AreSame(players[3], _stormReplay.GetTeamBanPlayerChooser(StormTeam.Blue));
        Assert.AreSame(players[6], _stormReplay.GetTeamBanPlayerChooser(StormTeam.Red));

        Assert.AreEqual(2, _stormReplay.StormPlayersWithObservers.Count(x => x.IsBanChooser));
    }
}
