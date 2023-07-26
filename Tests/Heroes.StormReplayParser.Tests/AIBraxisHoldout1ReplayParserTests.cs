namespace Heroes.StormReplayParser.Tests;

[TestClass]
public class AIBraxisHoldout1ReplayParserTests
{
    private readonly string _replaysFolder = "Replays";
    private readonly string _replayFile = "AIBraxisHoldout1_90670.StormR";
    private readonly StormReplay _stormReplay;
    private readonly StormReplayParseStatus _result;

    public AIBraxisHoldout1ReplayParserTests()
    {
        StormReplayResult result = StormReplay.Parse(Path.Combine(_replaysFolder, _replayFile));
        _stormReplay = result.Replay;
        _result = result.Status;
    }

    [TestMethod]
    public void PlayerCountTest()
    {
        Assert.AreEqual(1, _stormReplay.PlayersWithObserversCount);
        Assert.AreEqual(10, _stormReplay.PlayersCount);
    }

    [TestMethod]
    public void PlayerListTest()
    {
        Assert.AreEqual(10, _stormReplay.StormPlayers.Count());
        Assert.AreEqual(1, _stormReplay.StormPlayersWithObservers.Count());
    }

    [TestMethod]
    public void ParseResult()
    {
        Assert.AreEqual(StormReplayParseStatus.Success, _result);
    }

    [TestMethod]
    public void StormReplayHeaderTest()
    {
        Assert.AreEqual(2, _stormReplay.ReplayVersion.Major);
        Assert.AreEqual(55, _stormReplay.ReplayVersion.Minor);
        Assert.AreEqual(3, _stormReplay.ReplayVersion.Revision);
        Assert.AreEqual(90670, _stormReplay.ReplayVersion.Build);
        Assert.AreEqual(90670, _stormReplay.ReplayVersion.BaseBuild);

        Assert.AreEqual(90670, _stormReplay.ReplayBuild);
        Assert.AreEqual(16267, _stormReplay.ElapsedGamesLoops);
    }

    [TestMethod]
    public void StormReplayDetailsTest()
    {
        List<StormPlayer> players = _stormReplay.StormPlayers.ToList();
        StormPlayer player0 = players[0];

        Assert.AreEqual("Player 1", player0.Name);
        Assert.AreEqual(StormTeam.Blue, player0.Team);
        Assert.IsFalse(player0.IsWinner);
        Assert.AreEqual("Auriel", player0.PlayerHero!.HeroName);

        StormPlayer player5 = players[5];

        Assert.AreEqual("lavakill", player5.Name);
        Assert.AreEqual(1214607983, player5.ToonHandle!.ProgramId);
        Assert.AreEqual(StormTeam.Red, player5.Team);
        Assert.IsTrue(player5.IsWinner);
        Assert.AreEqual("Azmodan", player5.PlayerHero!.HeroName);

        Assert.AreEqual("Braxis Holdout", _stormReplay.MapInfo.MapName);

        Assert.IsFalse(_stormReplay.HasObservers);
        Assert.IsTrue(_stormReplay.HasAI);
    }

    [TestMethod]
    public void StormReplayAttributeEventsTest()
    {
        List<StormPlayer> players = _stormReplay.StormPlayers.ToList();
        StormPlayer player = players[9];

        Assert.AreEqual("5v5", _stormReplay.TeamSize);
        Assert.AreEqual(PlayerDifficulty.Elite, player.PlayerDifficulty);
        Assert.AreEqual(StormGameSpeed.Faster, _stormReplay.GameSpeed);
        Assert.AreEqual(StormGameMode.Cooperative, _stormReplay.GameMode);
        Assert.AreEqual(string.Empty, player.PlayerHero!.HeroAttributeId);
        Assert.AreEqual(string.Empty, player.PlayerLoadout.SkinAndSkinTintAttributeId);
        Assert.AreEqual(string.Empty, player.PlayerLoadout.MountAndMountTintAttributeId);
        Assert.AreEqual(string.Empty, player.PlayerLoadout.BannerAttributeId);
        Assert.AreEqual(string.Empty, player.PlayerLoadout.SprayAttributeId);
        Assert.AreEqual(string.Empty, player.PlayerLoadout.VoiceLineAttributeId);
        Assert.AreEqual(string.Empty, player.PlayerLoadout.AnnouncerPackAttributeId);
        Assert.AreEqual(1, player.PlayerHero.HeroLevel);

        List<string?> ban0List = _stormReplay.GetTeamBans(StormTeam.Blue).ToList();
        List<string?> ban1List = _stormReplay.GetTeamBans(StormTeam.Red).ToList();

        Assert.AreEqual(string.Empty, ban0List[1]);
        Assert.AreEqual(string.Empty, ban1List[1]);
    }

    [TestMethod]
    public void DraftOrderTest()
    {
        var draft = _stormReplay.DraftPicks.ToList();

        Assert.AreEqual(0, draft.Count);
    }

    [TestMethod]
    public void TeamsFinalLevelTest()
    {
        Assert.AreEqual(16, _stormReplay.GetTeamFinalLevel(StormTeam.Blue));
        Assert.AreEqual(18, _stormReplay.GetTeamFinalLevel(StormTeam.Red));
        Assert.IsNull(_stormReplay.GetTeamFinalLevel(StormTeam.Observer));
    }

    [TestMethod]
    public void ChatMessagesTest()
    {
        List<IStormMessage> messages = _stormReplay.ChatMessages.ToList();

        Assert.AreEqual(0, messages.Count);
        Assert.IsTrue(messages.All(x => !string.IsNullOrEmpty(x.Message)));
    }

    [TestMethod]
    public void AttributeTest()
    {
        Assert.AreEqual(StormBanMode.NotUsingBans, _stormReplay.BanMode);
        Assert.AreEqual(StormFirstDraftTeam.CoinToss, _stormReplay.FirstDraftTeam);
        Assert.AreEqual(StormGamePrivacy.Normal, _stormReplay.GamePrivacy);
        Assert.AreEqual(StormLobbyMode.Standard, _stormReplay.LobbyMode);
        Assert.AreEqual(StormReadyMode.FCFS, _stormReplay.ReadyMode);
    }
}
