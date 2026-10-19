using Heroes.StormReplayParser.Pregame.Player;

namespace Heroes.StormReplayParser.Tests;

[TestClass]
public class BattleLobbyCustom1Tests
{
    private readonly string _replaysFolder = "BattleLobbys";
    private readonly string _replayFile = "replay.server.Custom1_90670.battlelobby";
    private readonly StormReplayPregame _stormReplay;
    private readonly StormReplayPregameParseStatus _result;

    public BattleLobbyCustom1Tests()
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
        Assert.AreEqual(1, _stormReplay.PlayersWithObserversCount);
        Assert.AreEqual(10, _stormReplay.PlayersCount);
    }

    [TestMethod]
    public void GamePropertiesTest()
    {
        Assert.AreEqual(StormBanMode.NotUsingBans, _stormReplay.BanMode);
        Assert.AreEqual(StormFirstDraftTeam.CoinToss, _stormReplay.FirstDraftTeam);
        Assert.AreEqual(StormGameMode.Custom, _stormReplay.GameMode);
        Assert.AreEqual(StormGamePrivacy.Normal, _stormReplay.GamePrivacy);
        Assert.AreEqual(StormGameSpeed.Faster, _stormReplay.GameSpeed);
        Assert.AreEqual(StormLobbyMode.Standard, _stormReplay.LobbyMode);
        Assert.AreEqual(243203581, _stormReplay.RandomValue);
        Assert.AreEqual(StormReadyMode.FCFS, _stormReplay.ReadyMode);
        Assert.AreEqual(StormRegion.US, _stormReplay.Region);
        Assert.AreEqual(90670, _stormReplay.ReplayBuild);
    }

    [TestMethod]
    public void PlayerTest()
    {
        PregameStormPlayer player0 = _stormReplay.StormObservers.ToList()[0];

        Assert.AreEqual(PlayerType.Observer, player0.PlayerType);
        Assert.AreEqual(PlayerSlotType.Human, player0.PlayerSlotType);
        Assert.IsNull(player0.AccountLevel);
        Assert.IsFalse(player0.HasActiveBoost);
        Assert.IsFalse(player0.IsBlizzardStaff);
        Assert.IsFalse(player0.IsSilenced);
        Assert.IsFalse(player0.IsVoiceSilenced);
        Assert.StartsWith("la", player0.Name);
        Assert.IsNull(player0.PartyValue);
        Assert.AreEqual(ComputerDifficulty.Elite, player0.ComputerDifficulty);
        Assert.AreEqual(1214607983, player0.ToonHandle!.ProgramId);
        Assert.StartsWith("152", player0.ToonHandle!.Id.ToString());
        Assert.StartsWith("T:563", player0.ToonHandle!.ShortcutId.ToString());
        Assert.EndsWith("#167", player0.ToonHandle!.ShortcutId.ToString());
    }

    [TestMethod]
    public void ObserverReplayOwnerTest()
    {
        PregameStormPlayer player0 = _stormReplay.StormObservers.ToList()[0];

        Assert.IsTrue(player0.IsReplayOwner);
        Assert.DoesNotContain(x => x.IsReplayOwner, _stormReplay.StormPlayers);
    }

    [TestMethod]
    public void ObserverHeroMasteryTiersTest()
    {
        PregameStormPlayer player0 = _stormReplay.StormObservers.ToList()[0];

        Assert.AreEqual(83, player0.HeroMasteryTiersCount);
        Assert.AreEqual("Abat", player0.HeroMasteryTiers[0].HeroAttributeId);
        Assert.AreEqual(3, player0.HeroMasteryTiers[0].TierLevel);
    }

    [TestMethod]
    public void PlayerTeamTest()
    {
        Assert.AreEqual(StormTeam.Observer, _stormReplay.StormObservers.ToList()[0].Team);

        // the observer is in the first slot, so the first 5 players are not all on the same team
        List<PregameStormPlayer> players = [.. _stormReplay.StormPlayers];

        Assert.IsTrue(players[..4].All(x => x.Team == StormTeam.Blue));
        Assert.IsTrue(players[4..9].All(x => x.Team == StormTeam.Red));
        Assert.AreEqual(StormTeam.Blue, players[9].Team);
    }
}
