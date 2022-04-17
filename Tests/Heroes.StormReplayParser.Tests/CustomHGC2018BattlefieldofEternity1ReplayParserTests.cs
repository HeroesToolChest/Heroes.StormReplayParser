namespace Heroes.StormReplayParser.Tests;

[TestClass]
public class CustomHGC2018BattlefieldofEternity1ReplayParserTests
{
    private readonly string _replaysFolder = "Replays";
    private readonly string _replayFile = "CustomHGC2018BattlefieldofEternity1_68509.StormR";
    private readonly StormReplay _stormReplay;

    public CustomHGC2018BattlefieldofEternity1ReplayParserTests()
    {
        _stormReplay = StormReplay.Parse(Path.Combine(_replaysFolder, _replayFile)).Replay;
    }

    [TestMethod]
    public void PlayerCountTest()
    {
        Assert.AreEqual(14, _stormReplay.PlayersWithObserversCount);
        Assert.AreEqual(10, _stormReplay.PlayersCount);
    }

    [TestMethod]
    public void DraftOrderTest()
    {
        var draft = _stormReplay.DraftPicks.ToList();

        Assert.AreEqual(20, draft.Count);

        Assert.AreEqual("Medivh", draft[0].HeroSelected);
        Assert.AreEqual(StormDraftPickType.Banned, draft[0].PickType);
        Assert.IsNull(draft[0].Player);
        Assert.AreEqual(StormTeam.Blue, draft[0].Team);

        Assert.AreEqual("Uther", draft[17].HeroSelected);
        Assert.AreEqual(StormDraftPickType.Swapped, draft[17].PickType);
        Assert.AreEqual("TFYoDa", draft[17].Player!.Name);
        Assert.AreEqual(StormTeam.Red, draft[17].Team);

        Assert.AreEqual("Tassadar", draft[18].HeroSelected);
        Assert.AreEqual(StormDraftPickType.Swapped, draft[18].PickType);
        Assert.AreEqual("TSViN", draft[18].Player!.Name);
        Assert.AreEqual(StormTeam.Blue, draft[18].Team);

        Assert.AreEqual("Raynor", draft[19].HeroSelected);
        Assert.AreEqual(StormDraftPickType.Swapped, draft[19].PickType);
        Assert.AreEqual("TSFan", draft[19].Player!.Name);
        Assert.AreEqual(StormTeam.Blue, draft[19].Team);
    }

    [TestMethod]
    public void BattleLobbyDataTest()
    {
        List<StormPlayer> players = _stormReplay.StormPlayersWithObservers.ToList();

        Assert.IsNull(players[1].AccountLevel);
        Assert.AreEqual(1369706131243, players[1].PartyValue);
        Assert.IsNull(players[2].AccountLevel);
        Assert.AreEqual(1369706131243, players[2].PartyValue);

        Assert.IsTrue(players[0].BattleTagName.StartsWith(players[0].Name));
        Assert.IsTrue(players[0].BattleTagName.Contains('#'));
        Assert.IsTrue(players[0].BattleTagName.EndsWith("41"));

        Assert.AreEqual(485855, players[0].ToonHandle!.Id);
        Assert.AreEqual(1869768008, players[0].ToonHandle!.ProgramId);
        Assert.AreEqual(1, players[0].ToonHandle!.Realm);
        Assert.AreEqual(98, players[0].ToonHandle!.Region);
        Assert.AreEqual(StormRegion.XX, players[0].ToonHandle!.StormRegion);
        Assert.AreEqual("T:39037232#704", players[0].ToonHandle!.ShortcutId);

        Assert.IsFalse(players[0].IsBlizzardStaff!.Value);
        Assert.IsFalse(players[0].IsVoiceSilenced!.Value);
        Assert.IsFalse(players[0].IsSilenced);
        Assert.IsNull(players[0].HasActiveBoost);
    }

    [TestMethod]
    public void PlayerScoreResultsTest()
    {
        List<StormPlayer> players = _stormReplay.StormPlayersWithObservers.ToList();

        Assert.IsNull(players[0].ScoreResult);
        Assert.IsNull(players[1].ScoreResult);
        Assert.IsNull(players[2].ScoreResult);
        Assert.IsNull(players[3].ScoreResult);
        Assert.IsNotNull(players[5].ScoreResult);
        Assert.IsNotNull(players[6].ScoreResult);
    }
}
