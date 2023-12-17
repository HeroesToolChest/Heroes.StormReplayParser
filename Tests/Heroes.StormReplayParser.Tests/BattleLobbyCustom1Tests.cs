using Heroes.StormReplayParser.Pregame.Player;

namespace Heroes.StormReplayParser.Tests;

[TestClass]
public class BattleLobbyCustom1Tests
{
    private readonly string _replaysFolder = "BattleLobbys";
    private readonly string _replayFile = "replay.server.Custom1.battlelobby";
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
        Assert.IsTrue(player0.Name.StartsWith("la"));
        Assert.IsNull(player0.PartyValue);
        Assert.AreEqual(ComputerDifficulty.Elite, player0.ComputerDifficulty);
        Assert.AreEqual(1214607983, player0.ToonHandle!.ProgramId);
        Assert.IsTrue(player0.ToonHandle!.Id.ToString().StartsWith("152"));
        Assert.IsTrue(player0.ToonHandle!.ShortcutId.ToString().StartsWith("T:563"));
        Assert.IsTrue(player0.ToonHandle!.ShortcutId.ToString().EndsWith("#167"));
    }
}
