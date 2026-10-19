using Heroes.StormReplayParser.Pregame.Player;

namespace Heroes.StormReplayParser.Tests;

[TestClass]
public class BattleLobbyCustom2Tests
{
    private readonly string _replaysFolder = "BattleLobbys";
    private readonly string _replayFile = "replay.server.Custom2_98348.battlelobby";
    private readonly StormReplayPregame _stormReplay;
    private readonly StormReplayPregameParseStatus _result;

    public BattleLobbyCustom2Tests()
    {
        StormReplayPregameResult result = StormReplayPregame.Parse(Path.Combine(_replaysFolder, _replayFile));
        _stormReplay = result.ReplayBattleLobby;
        _result = result.Status;
    }

    [TestMethod]
    public void ParseStatusTest()
    {
        Assert.AreEqual(StormReplayPregameParseStatus.Success, _result);
    }

    [TestMethod]
    public void PlayerCountTest()
    {
        Assert.AreEqual(13, _stormReplay.PlayersWithObserversCount);
        Assert.AreEqual(10, _stormReplay.PlayersCount);
    }

    [TestMethod]
    public void ReplayOwnerTest()
    {
        PregameStormPlayer player8 = _stormReplay.StormPlayersWithObservers.ToList()[8];

        Assert.IsTrue(player8.IsReplayOwner);
    }

    [TestMethod]
    public void PlayerTeamTest()
    {
        List<PregameStormPlayer> players = [.. _stormReplay.StormPlayers];
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

        List<PregameStormPlayer> playersWithObs = [.. _stormReplay.StormPlayersWithObservers];
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
        Assert.AreEqual(StormBanChooser.Player, _stormReplay.GetTeamBanChooser(StormTeam.Blue));
        Assert.AreEqual(StormBanChooser.Player, _stormReplay.GetTeamBanChooser(StormTeam.Red));

        List<PregameStormPlayer> players = [.. _stormReplay.StormPlayers];

        Assert.IsTrue(players[3].IsBanChooser);
        Assert.AreEqual(StormTeam.Blue, players[3].Team);
        Assert.StartsWith("Player", players[3].BattleTagName);

        Assert.IsTrue(players[6].IsBanChooser);
        Assert.AreEqual(StormTeam.Red, players[6].Team);
        Assert.StartsWith("MasonBlaze", players[6].BattleTagName);

        Assert.AreEqual(2, _stormReplay.StormPlayersWithObservers.Count(x => x.IsBanChooser));
    }
}
