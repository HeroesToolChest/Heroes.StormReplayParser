namespace Heroes.StormReplayParser.Tests;

[TestClass]
public class HanamuraSl1ReplayParserTests
{
    private readonly string _replaysFolder = "Replays";
    private readonly StormReplay _stormReplay;

    public HanamuraSl1ReplayParserTests()
    {
        StormReplayResult result = StormReplay.Parse(Path.Combine(_replaysFolder, "HanamuraSL1_89566.StormR"));
        _stormReplay = result.Replay;
    }

    [TestMethod]
    public void StormReplayHeaderTest()
    {
        Assert.AreEqual(2, _stormReplay.ReplayVersion.Major);
        Assert.AreEqual(55, _stormReplay.ReplayVersion.Minor);
        Assert.AreEqual(3, _stormReplay.ReplayVersion.Revision);
        Assert.AreEqual(89566, _stormReplay.ReplayVersion.Build);
        Assert.AreEqual(89566, _stormReplay.ReplayVersion.BaseBuild);

        Assert.AreEqual(89566, _stormReplay.ReplayBuild);
        Assert.AreEqual(16097, _stormReplay.ElapsedGamesLoops);
    }

    [TestMethod]
    public void AttributeTest()
    {
        Assert.AreEqual(StormBanMode.ThreeBan, _stormReplay.BanMode);
        Assert.AreEqual(StormFirstDraftTeam.CoinToss, _stormReplay.FirstDraftTeam);
        Assert.AreEqual(StormGamePrivacy.Normal, _stormReplay.GamePrivacy);
        Assert.AreEqual(StormLobbyMode.Draft, _stormReplay.LobbyMode);
        Assert.AreEqual(StormReadyMode.FCFS, _stormReplay.ReadyMode);
    }

    [TestMethod]
    public void BanChooserTest()
    {
        Assert.AreEqual(StormBanChooserType.HighestMmr, _stormReplay.GetTeamBanChooserType(StormTeam.Blue));
        Assert.AreEqual(StormBanChooserType.HighestMmr, _stormReplay.GetTeamBanChooserType(StormTeam.Red));
        Assert.DoesNotContain(x => x.IsBanChooser, _stormReplay.StormPlayers);

        Assert.IsNull(_stormReplay.GetTeamBanPlayerChooser(StormTeam.Blue));
        Assert.IsNull(_stormReplay.GetTeamBanPlayerChooser(StormTeam.Red));
        Assert.IsNull(_stormReplay.GetTeamBanPlayerChooser(StormTeam.Observer));
    }

    [TestMethod]
    public void HeroMasteryRingTierTest()
    {
        // (Fenix)
        int?[] expected = [null, null, null, 3, null, null, null, null, null, null];

        Assert.AreSequenceEqual(expected, _stormReplay.StormPlayers.Select(x => x.PlayerHero!.HeroMasteryRingTier).ToArray());
    }
}
