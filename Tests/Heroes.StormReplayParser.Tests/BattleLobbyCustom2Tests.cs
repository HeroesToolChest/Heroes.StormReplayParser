using Heroes.StormReplayParser.Pregame.Player;

namespace Heroes.StormReplayParser.Tests;

[TestClass]
public class BattleLobbyCustom2Tests
{
    private readonly string _replaysFolder = "BattleLobbys";
    private readonly string _replayFile = "replay.server.Custom2_98348.battlelobby";
    private readonly StormReplayPregame _stormReplay;
    private readonly StormReplayPregameParseStatus _result;

    public BattleLobbyCustom2Tests()
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
        Assert.AreEqual(13, _stormReplay.PlayersWithObserversCount);
        Assert.AreEqual(10, _stormReplay.PlayersCount);
    }

    [TestMethod]
    public void ReplayOwnerTest()
    {
        PregameStormPlayer player8 = _stormReplay.StormPlayersWithObservers.ToList()[8];

        Assert.IsTrue(player8.IsReplayOwner);
    }

    [TestMethod]
    public void HeroMasteryRingTierTest()
    {
        // only (Wiza) has a mastery ring equipped, observers do not have one
        int?[] expected = [null, null, null, null, null, null, 3, null, null, null, null, null, null];

        Assert.AreSequenceEqual(expected, _stormReplay.StormPlayersWithObservers.Select(x => x.PlayerHero?.HeroMasteryRingTier).ToArray());
    }

    [TestMethod]
    public void PlayerTeamTest()
    {
        List<PregameStormPlayer> players = [.. _stormReplay.StormPlayers];
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

        List<PregameStormPlayer> playersWithObs = [.. _stormReplay.StormPlayersWithObservers];
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
        Assert.AreEqual(StormBanChooserType.Player, _stormReplay.GetTeamBanChooser(StormTeam.Blue));
        Assert.AreEqual(StormBanChooserType.Player, _stormReplay.GetTeamBanChooser(StormTeam.Red));

        List<PregameStormPlayer> players = [.. _stormReplay.StormPlayers];

        Assert.IsTrue(players[3].IsBanChooser);
        Assert.AreEqual(StormTeam.Blue, players[3].Team);
        Assert.StartsWith("Player", players[3].BattleTagName);

        Assert.IsTrue(players[6].IsBanChooser);
        Assert.AreEqual(StormTeam.Red, players[6].Team);
        Assert.StartsWith("MasonBlaze", players[6].BattleTagName);

        Assert.AreEqual(2, _stormReplay.StormPlayersWithObservers.Count(x => x.IsBanChooser));
    }

    [TestMethod]
    public void PortraitUnlockableIdTest()
    {
        List<PregameStormPlayer> players = [.. _stormReplay.StormPlayers];

        Assert.AreEqual(2247611436, players[0].PortraitUnlockableId);
        Assert.AreEqual(956270533, players[0].PortraitBorderUnlockableId); // basic border

        Assert.AreEqual(2082266764, players[3].PortraitUnlockableId); // portrait without a license
        Assert.AreEqual(2008600194, players[3].PortraitBorderUnlockableId); // storm border

        Assert.AreEqual(3385697865, players[9].PortraitUnlockableId);
        Assert.AreEqual(2008600194, players[9].PortraitBorderUnlockableId);

        Assert.IsTrue(_stormReplay.StormObservers.All(x => x.PortraitUnlockableId is null && x.PortraitBorderUnlockableId is null));
    }

    [TestMethod]
    public void PortraitLicenseWithNoLicenseParsingTest()
    {
        Assert.IsNull(_stormReplay.LicenseIdCatalog);
    }

    [TestMethod]
    public void PortraitLicenseParsingTest()
    {
        using FileStream stream = File.OpenRead(Path.Combine(_replaysFolder, _replayFile));
        StormReplayPregameResult result = StormReplayPregame.Parse(stream, new ParsePregameOptions()
        {
            ShouldParseLicenseCatalog = true,
        });

        StormReplayPregame replay = result.ReplayBattleLobby;

        Assert.AreEqual(StormReplayPregameParseStatus.Success, result.Status);

        List<PregameStormPlayer> players = [.. replay.StormPlayers];

        Assert.IsNotNull(replay.LicenseIdCatalog);
        Assert.HasCount(5418, replay.LicenseIdCatalog);
    }

    [TestMethod]
    public void PortraitLicenseNoParsingTest()
    {
        using FileStream stream = File.OpenRead(Path.Combine(_replaysFolder, _replayFile));
        StormReplayPregameResult result = StormReplayPregame.Parse(stream, new ParsePregameOptions()
        {
            ShouldParseLicenseCatalog = false,
        });

        StormReplayPregame replay = result.ReplayBattleLobby;

        Assert.AreEqual(StormReplayPregameParseStatus.Success, result.Status);

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
        StormReplayPregameResult result = StormReplayPregame.Parse(
            stream,
            new ParsePregameOptions()
            {
                ShouldParseLicenseCatalog = catalogParsing,
            },
            licenseCatalog);

        StormReplayPregame replay = result.ReplayBattleLobby;

        Assert.AreEqual(StormReplayPregameParseStatus.Success, result.Status);

        List<PregameStormPlayer> players = [.. replay.StormPlayers];

        Assert.IsNotNull(replay.LicenseIdCatalog);
        Assert.HasCount(5418, replay.LicenseIdCatalog);
    }

    [TestMethod]
    public void ParseStreamNotAtStartTest()
    {
        byte[] battlelobby = File.ReadAllBytes(Path.Combine(_replaysFolder, _replayFile));

        using MemoryStream stream = new();
        stream.Write([1, 2, 3, 4]);
        stream.Write(battlelobby);
        stream.Position = 4;

        StormReplayPregameResult result = StormReplayPregame.Parse(stream);

        Assert.AreEqual(StormReplayPregameParseStatus.Success, result.Status);
        Assert.AreEqual(13, result.ReplayBattleLobby.PlayersWithObserversCount);
    }

    [TestMethod]
    public void ParseSmallStreamTest()
    {
        // small enough to use the stack buffer, but not a valid battlelobby
        using MemoryStream stream = new(new byte[16]);

        StormReplayPregameResult result = StormReplayPregame.Parse(stream);

        Assert.AreEqual(StormReplayPregameParseStatus.Exception, result.Status);
    }
}
