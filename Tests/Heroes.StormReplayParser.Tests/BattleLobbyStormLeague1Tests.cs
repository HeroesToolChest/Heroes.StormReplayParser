using Heroes.StormReplayParser.Pregame.Player;

namespace Heroes.StormReplayParser.Tests;

[TestClass]
public class BattleLobbyStormLeague1Tests
{
    private readonly string _replaysFolder = "BattleLobbys";
    private readonly string _replayFile = "replay.server.StormLeague1.battlelobby";
    private readonly StormReplayPregame _stormReplay;
    private readonly StormReplayPregameParseStatus _result;

    public BattleLobbyStormLeague1Tests()
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
        Assert.AreEqual(10, _stormReplay.PlayersWithObserversCount);
        Assert.AreEqual(10, _stormReplay.PlayersCount);
    }

    [TestMethod]
    public void GamePropertiesTest()
    {
        Assert.AreEqual(StormBanMode.ThreeBan, _stormReplay.BanMode);
        Assert.AreEqual(StormFirstDraftTeam.CoinToss, _stormReplay.FirstDraftTeam);
        Assert.AreEqual(StormGameMode.StormLeague, _stormReplay.GameMode);
        Assert.AreEqual(StormGamePrivacy.Normal, _stormReplay.GamePrivacy);
        Assert.AreEqual(StormGameSpeed.Faster, _stormReplay.GameSpeed);
        Assert.AreEqual(StormLobbyMode.Draft, _stormReplay.LobbyMode);
        Assert.AreEqual(3685014112, _stormReplay.RandomValue);
        Assert.AreEqual(StormReadyMode.FCFS, _stormReplay.ReadyMode);
        Assert.AreEqual(StormRegion.US, _stormReplay.Region);
        Assert.AreEqual(89566, _stormReplay.ReplayBuild);
        Assert.AreEqual("5v5", _stormReplay.TeamSize);
    }

    [TestMethod]
    public void PlayerTest()
    {
        StormPregamePlayer player4 = _stormReplay.StormPlayers.ToList()[4];

        Assert.AreEqual(PlayerType.Human, player4.PlayerType);
        Assert.AreEqual(PlayerSlotType.Human, player4.PlayerSlotType);
        Assert.AreEqual(1062, player4.AccountLevel);
        Assert.IsFalse(player4.HasActiveBoost);
        Assert.IsFalse(player4.IsBlizzardStaff);
        Assert.IsFalse(player4.IsSilenced);
        Assert.IsFalse(player4.IsVoiceSilenced);
        Assert.IsNull(player4.PartyValue);
        Assert.AreEqual(PlayerDifficulty.Elite, player4.PlayerDifficulty);

        StormPregamePlayer player5 = _stormReplay.StormPlayers.ToList()[5];
        Assert.IsFalse(player5.HasActiveBoost);
        Assert.IsFalse(player5.IsBlizzardStaff);
        Assert.IsFalse(player5.IsSilenced);
        Assert.IsFalse(player5.IsVoiceSilenced);
        Assert.AreEqual(6794873790121, player5.PartyValue);

        StormPregamePlayer player6 = _stormReplay.StormPlayers.ToList()[6];
        Assert.AreEqual(player5.PartyValue, player6.PartyValue);

        StormPregamePlayer player3 = _stormReplay.StormPlayers.ToList()[3];
        Assert.AreEqual(3, player3.PlayerHero!.HeroMasteryTier);
    }

    [TestMethod]
    public void PlayerHero()
    {
        PregamePlayerHero hero4 = _stormReplay.StormPlayers.ToList()[4]!.PlayerHero!;

        Assert.AreEqual("Mura", hero4!.HeroAttributeId);
        Assert.AreEqual(20, hero4.HeroLevel);
        Assert.IsNull(hero4.HeroMasteryTier);
    }

    [TestMethod]
    public void PlayerLoadout()
    {
        PlayerPregameLoadout loadout4 = _stormReplay.StormPlayers.ToList()[4]!.PlayerLoadout!;

        Assert.AreEqual("AJAN", loadout4.AnnouncerPackAttributeId);
        Assert.AreEqual("BNaf", loadout4.BannerAttributeId);
        Assert.AreEqual("MMJF", loadout4.MountAndMountTintAttributeId);
        Assert.AreEqual("Mur9", loadout4.SkinAndSkinTintAttributeId);
        Assert.AreEqual("0001", loadout4.SprayAttributeId);
        Assert.AreEqual("MRM5", loadout4.VoiceLineAttributeId);
        Assert.AreEqual("AJAN", loadout4.AnnouncerPackAttributeId);
    }

    [TestMethod]
    public void HeroBans()
    {
        var blueBans = _stormReplay.GetTeamBans(StormTeam.Blue);

        Assert.AreEqual(string.Empty, blueBans[0]);
        Assert.AreEqual("Anub", blueBans[1]);
        Assert.AreEqual(string.Empty, blueBans[2]);

        var redBans = _stormReplay.GetTeamBans(StormTeam.Red);

        Assert.AreEqual("Jain", redBans[0]);
        Assert.AreEqual("Tych", redBans[1]);
        Assert.AreEqual("Faer", redBans[2]);
    }
}
