namespace Heroes.StormReplayParser.Tests;

[TestClass]
public class SilverCity2ReplayParserTests
{
    private readonly string _replaysFolder = "Replays";
    private readonly string _replayFile = "SilverCity2_98348.StormR";
    private readonly StormReplay _stormReplay;
    private readonly StormReplayParseStatus _result;

    public SilverCity2ReplayParserTests()
    {
        StormReplayResult result = StormReplay.Parse(Path.Combine(_replaysFolder, _replayFile));
        _stormReplay = result.Replay;
        _result = result.Status;
    }

    [TestMethod]
    public void ParseResult()
    {
        Assert.AreEqual(StormReplayParseStatus.Success, _result);
    }

    [TestMethod]
    public void PortraitLicenseParsingTest()
    {
        using FileStream stream = File.OpenRead(Path.Combine(_replaysFolder, _replayFile));
        StormReplayResult result = StormReplay.Parse(stream, new ParseOptions()
        {
            ShouldParseLicenseCatalog = true,
        });

        StormReplay replay = result.Replay;

        Assert.AreEqual(StormReplayParseStatus.Success, result.Status);

        List<StormPlayer> players = [.. replay.StormPlayers];

        Assert.IsNull(players[0].PortraitLicenseId); // however is DeathwingPortraitMastery
        Assert.IsNull(players[1].PortraitLicenseId); // however is 2019Season3SLMaster
        Assert.IsNull(players[2].PortraitLicenseId); // however is CallofKelThuzadPortrait
        Assert.AreEqual(32677, players[3].PortraitLicenseId); // ButcherCarbotsPortrait
        Assert.AreEqual(30619, players[4].PortraitLicenseId); // RexxarCarbotsPortrait
        Assert.AreEqual(23905, players[5].PortraitLicenseId); // MrBigglesworthPortrait

        Assert.IsNotNull(replay.LicenseIdCatalog);
        Assert.HasCount(5418, replay.LicenseIdCatalog);
    }
}
