using Heroes.StormReplayParser.Pregame.Player;

namespace Heroes.StormReplayParser.Tests;

[TestClass]
public class BattleLobbyARAM1Tests
{
    private readonly string _replaysFolder = "BattleLobbys";
    private readonly string _replayFile = "replay.server.ARAM1.battlelobby";
    private readonly StormReplayPregame _stormReplay;
    private readonly StormReplayPregameParseStatus _result;

    public BattleLobbyARAM1Tests()
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
        Assert.AreEqual(StormBanMode.NotUsingBans, _stormReplay.BanMode);
        Assert.AreEqual(StormFirstDraftTeam.CoinToss, _stormReplay.FirstDraftTeam);
        Assert.AreEqual(StormGameMode.ARAM, _stormReplay.GameMode);
        Assert.AreEqual(StormGamePrivacy.Normal, _stormReplay.GamePrivacy);
        Assert.AreEqual(StormGameSpeed.Faster, _stormReplay.GameSpeed);
        Assert.AreEqual(StormLobbyMode.Standard, _stormReplay.LobbyMode);
        Assert.AreEqual(2002111530, _stormReplay.RandomValue);
        Assert.AreEqual(StormReadyMode.FCFS, _stormReplay.ReadyMode);
        Assert.AreEqual(StormRegion.US, _stormReplay.Region);
        Assert.AreEqual(89754, _stormReplay.ReplayBuild);
    }

    [TestMethod]
    public void PlayerTest()
    {
        PregameStormPlayer player4 = _stormReplay.StormPlayers.ToList()[4];

        Assert.IsTrue(player4.BattleTagName.StartsWith("Blimphead316"));
        Assert.AreEqual(PlayerType.Human, player4.PlayerType);
        Assert.AreEqual(PlayerSlotType.Human, player4.PlayerSlotType);
        Assert.AreEqual(739, player4.AccountLevel);
        Assert.IsFalse(player4.HasActiveBoost);
        Assert.IsFalse(player4.IsBlizzardStaff);
        Assert.IsFalse(player4.IsSilenced);
        Assert.IsFalse(player4.IsVoiceSilenced);
        Assert.AreEqual("Blimphead316", player4.Name);
        Assert.IsNull(player4.PartyValue);
        Assert.AreEqual(PlayerDifficulty.Beginner, player4.PlayerDifficulty);

        PregameStormPlayer player8 = _stormReplay.StormPlayers.ToList()[8];
        Assert.IsTrue(player8.HasActiveBoost);
        Assert.IsFalse(player8.IsBlizzardStaff);
        Assert.IsFalse(player8.IsSilenced);
        Assert.IsFalse(player8.IsVoiceSilenced);
    }

    [TestMethod]
    public void PlayerHero()
    {
        PregamePlayerHero hero4 = _stormReplay.StormPlayers.ToList()[4]!.PlayerHero!;

        Assert.AreEqual("Ragn", hero4!.HeroAttributeId);
        Assert.AreEqual(1, hero4.HeroLevel);
        Assert.IsNull(hero4.HeroMasteryTier);
    }

    [TestMethod]
    public void PlayerLoadout()
    {
        PregamePlayerLoadout loadout4 = _stormReplay.StormPlayers.ToList()[4]!.PlayerLoadout!;

        Assert.AreEqual(string.Empty, loadout4.AnnouncerPackAttributeId);
        Assert.AreEqual(string.Empty, loadout4.BannerAttributeId);
        Assert.AreEqual("Rand", loadout4.MountAndMountTintAttributeId);
        Assert.AreEqual("Rand", loadout4.SkinAndSkinTintAttributeId);
        Assert.AreEqual(string.Empty, loadout4.SprayAttributeId);
        Assert.AreEqual(string.Empty, loadout4.VoiceLineAttributeId);
        Assert.AreEqual(string.Empty, loadout4.AnnouncerPackAttributeId);
    }

    [TestMethod]
    public void PlayerToon()
    {
        ToonHandle toon4 = _stormReplay.StormPlayers.ToList()[4]!.ToonHandle!;

        Assert.IsTrue(toon4.Id.ToString().StartsWith("12519"));
        Assert.AreEqual(1214607983, toon4.ProgramId);
        Assert.AreEqual(1, toon4.Realm);
        Assert.AreEqual(1, toon4.Region);
        Assert.AreEqual(StormRegion.US, toon4.StormRegion);
        Assert.IsTrue(toon4.ShortcutId.StartsWith("T:5705"));
        Assert.IsTrue(toon4.ShortcutId.EndsWith("#989"));
    }

    [TestMethod]
    public void HeroBans()
    {
        var blueBans = _stormReplay.GetTeamBans(StormTeam.Blue);

        Assert.AreEqual(string.Empty, blueBans[0]);
        Assert.AreEqual(string.Empty, blueBans[1]);
        Assert.AreEqual(string.Empty, blueBans[2]);

        var redBans = _stormReplay.GetTeamBans(StormTeam.Red);

        Assert.AreEqual(string.Empty, redBans[0]);
        Assert.AreEqual(string.Empty, redBans[1]);
        Assert.AreEqual(string.Empty, redBans[2]);
    }
}
