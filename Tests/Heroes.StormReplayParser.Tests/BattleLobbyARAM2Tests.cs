namespace Heroes.StormReplayParser.Tests;

[TestClass]
public class BattleLobbyARAM2Tests
{
    private readonly string _replaysFolder = "BattleLobbys";
    private readonly string _replayFile = "replay.server.ARAM2_93640.battlelobby";
    private readonly StormReplayPregame _stormReplay;
    private readonly StormReplayPregameParseStatus _result;

    public BattleLobbyARAM2Tests()
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
    public void PlayerIsVoicedSilencedTest()
    {
        PregameStormPlayer player0 = _stormReplay.StormPlayers.ToList()[0];
        Assert.IsFalse(player0.IsVoiceSilenced);
        PregameStormPlayer player1 = _stormReplay.StormPlayers.ToList()[1];
        Assert.IsFalse(player1.IsVoiceSilenced);
        PregameStormPlayer player2 = _stormReplay.StormPlayers.ToList()[2];
        Assert.IsFalse(player2.IsVoiceSilenced);
        PregameStormPlayer player3 = _stormReplay.StormPlayers.ToList()[3];
        Assert.IsFalse(player3.IsVoiceSilenced);
        PregameStormPlayer player4 = _stormReplay.StormPlayers.ToList()[4];
        Assert.IsTrue(player4.IsVoiceSilenced);
        PregameStormPlayer player5 = _stormReplay.StormPlayers.ToList()[5];
        Assert.IsFalse(player5.IsVoiceSilenced);
        PregameStormPlayer player6 = _stormReplay.StormPlayers.ToList()[6];
        Assert.IsFalse(player6.IsVoiceSilenced);
        PregameStormPlayer player7 = _stormReplay.StormPlayers.ToList()[7];
        Assert.IsFalse(player7.IsVoiceSilenced);
        PregameStormPlayer player8 = _stormReplay.StormPlayers.ToList()[8];
        Assert.IsFalse(player8.IsVoiceSilenced);
        PregameStormPlayer player9 = _stormReplay.StormPlayers.ToList()[9];
        Assert.IsFalse(player9.IsVoiceSilenced);
    }

    [TestMethod]
    public void ReplayOwnerTest()
    {
        List<PregameStormPlayer> players = [.. _stormReplay.StormPlayers];

        Assert.IsTrue(players[3].IsReplayOwner);
        Assert.StartsWith("lavakill", players[3].BattleTagName);
        Assert.ContainsSingle(x => x.IsReplayOwner, players);
    }

    [TestMethod]
    public void PlayerHeroMasteryTiersTest()
    {
        PregameStormPlayer player3 = _stormReplay.StormPlayers.ToList()[3];

        Assert.AreEqual(85, player3.HeroMasteryTiersCount);
        Assert.AreEqual("Abat", player3.HeroMasteryTiers[0].HeroAttributeId);
        Assert.AreEqual(3, player3.HeroMasteryTiers[0].TierLevel);
        Assert.AreEqual("Zera", player3.HeroMasteryTiers[^1].HeroAttributeId);
        Assert.AreEqual(3, player3.HeroMasteryTiers[^1].TierLevel);
    }
}
