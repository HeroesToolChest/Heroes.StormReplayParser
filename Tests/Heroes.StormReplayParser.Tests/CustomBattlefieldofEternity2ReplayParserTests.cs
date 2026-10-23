namespace Heroes.StormReplayParser.Tests;

[TestClass]
public class CustomBattlefieldofEternity2ReplayParserTests
{
    private readonly string _replaysFolder = "Replays";
    private readonly string _replayFile = "CustomBattlefieldofEternity2_98348.StormR";
    private readonly StormReplay _stormReplay;
    private readonly StormReplayParseStatus _result;

    public CustomBattlefieldofEternity2ReplayParserTests()
    {
        StormReplayResult result = StormReplay.Parse(Path.Combine(_replaysFolder, _replayFile));
        _stormReplay = result.Replay;
        _result = result.Status;
    }

    [TestMethod]
    public void ParseResultTest()
    {
        Assert.AreEqual(StormReplayParseStatus.Success, _result);
    }

    [TestMethod]
    public void PlayerCountTest()
    {
        Assert.AreEqual(13, _stormReplay.PlayersWithObserversCount);
        Assert.AreEqual(10, _stormReplay.PlayersCount);
    }

    [TestMethod]
    public void PlayerListTest()
    {
        Assert.HasCount(10, _stormReplay.StormPlayers);
        Assert.HasCount(13, _stormReplay.StormPlayersWithObservers);

        List<StormPlayer> players = [.. _stormReplay.StormPlayers];
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

        List<StormPlayer> playersWithObs = [.. _stormReplay.StormPlayersWithObservers];
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
        Assert.AreEqual(StormBanChooserType.Player, _stormReplay.GetTeamBanChooserType(StormTeam.Blue));
        Assert.AreEqual(StormBanChooserType.Player, _stormReplay.GetTeamBanChooserType(StormTeam.Red));

        List<StormPlayer> players = [.. _stormReplay.StormPlayers];

        Assert.IsTrue(players[3].IsBanChooser);
        Assert.AreEqual(StormTeam.Blue, players[3].Team);
        Assert.AreEqual("Player", players[3].Name);

        Assert.IsTrue(players[6].IsBanChooser);
        Assert.AreEqual(StormTeam.Red, players[6].Team);
        Assert.AreEqual("MasonBlaze", players[6].Name);

        Assert.AreSame(players[3], _stormReplay.GetTeamBanPlayerChooser(StormTeam.Blue));
        Assert.AreSame(players[6], _stormReplay.GetTeamBanPlayerChooser(StormTeam.Red));

        Assert.AreEqual(2, _stormReplay.StormPlayersWithObservers.Count(x => x.IsBanChooser));
    }

    [TestMethod]
    public void PortraitLicenseWithNoLicenseParsingTest()
    {
        Assert.IsTrue(_stormReplay.StormPlayersWithObservers.All(x => x.PortraitLicenseId is null));
        Assert.IsNull(_stormReplay.LicenseIdCatalog);
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

        Assert.AreEqual(52780, players[0].PortraitLicenseId); // ToonThunderGoddessCassiaPortrait
        Assert.AreEqual(92261, players[6].PortraitLicenseId); // ClassicRexxarPortrait03
        Assert.AreEqual(26912, players[9].PortraitLicenseId); // HGC2018P1TeamFreedomPortrait
        Assert.IsNull(players[3].PortraitLicenseId); // portrait without a license

        Assert.IsNotNull(replay.LicenseIdCatalog);
        Assert.HasCount(5418, replay.LicenseIdCatalog);

        // no collection data for observers
        Assert.IsTrue(replay.StormObservers.All(x => x.PortraitLicenseId is null));
    }

    [TestMethod]
    public void PortraitLicenseNoParsingTest()
    {
        using FileStream stream = File.OpenRead(Path.Combine(_replaysFolder, _replayFile));
        StormReplayResult result = StormReplay.Parse(stream, new ParseOptions()
        {
            ShouldParseLicenseCatalog = false,
        });

        StormReplay replay = result.Replay;

        Assert.AreEqual(StormReplayParseStatus.Success, result.Status);

        Assert.IsTrue(replay.StormPlayersWithObservers.All(x => x.PortraitLicenseId is null));
        Assert.IsNull(replay.LicenseIdCatalog);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void PortraitLicenseWithProvidedCatalogTestWithParsing(bool catalogParsing)
    {
        HashSet<long> licenseCatalog =
        [
            52780,
            92261,
            26912,
        ];

        using FileStream stream = File.OpenRead(Path.Combine(_replaysFolder, _replayFile));
        StormReplayResult result = StormReplay.Parse(
            stream,
            new ParseOptions()
            {
                ShouldParseLicenseCatalog = catalogParsing,
            },
            licenseCatalog);

        StormReplay replay = result.Replay;

        Assert.AreEqual(StormReplayParseStatus.Success, result.Status);

        List<StormPlayer> players = [.. replay.StormPlayers];

        Assert.AreEqual(52780, players[0].PortraitLicenseId); // ToonThunderGoddessCassiaPortrait
        Assert.AreEqual(92261, players[6].PortraitLicenseId); // ClassicRexxarPortrait03
        Assert.AreEqual(26912, players[9].PortraitLicenseId); // HGC2018P1TeamFreedomPortrait
        Assert.IsNull(players[3].PortraitLicenseId); // portrait without a license

        Assert.IsNotNull(replay.LicenseIdCatalog);
        Assert.HasCount(5418, replay.LicenseIdCatalog);

        // no collection data for observers
        Assert.IsTrue(replay.StormObservers.All(x => x.PortraitLicenseId is null));
    }
}
