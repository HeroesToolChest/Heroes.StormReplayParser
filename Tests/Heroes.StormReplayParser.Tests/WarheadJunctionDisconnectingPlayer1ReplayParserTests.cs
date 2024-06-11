namespace Heroes.StormReplayParser.Tests;

[TestClass]
public class WarheadJunctionDisconnectingPlayer1ReplayParserTests
{
    private readonly string _replaysFolder = "Replays";
    private readonly StormReplay _stormReplay;
    private readonly StormReplayParseStatus _result;

    public WarheadJunctionDisconnectingPlayer1ReplayParserTests()
    {
        StormReplayResult result = StormReplay.Parse(Path.Combine(_replaysFolder, "WarheadJunctionDisconnectingPlayer1_91418.StormR"));
        _stormReplay = result.Replay;
        _result = result.Status;
    }

    [TestMethod]
    public void ParseResult()
    {
        Assert.AreEqual(StormReplayParseStatus.Success, _result);
    }

    [TestMethod]
    public void PlayerDisconnectsTest()
    {
        List<StormPlayer> players = _stormReplay.StormPlayers.ToList();

        Assert.AreEqual(0, players[0].PlayerDisconnects.Count);
        Assert.AreEqual(0, players[1].PlayerDisconnects.Count);
        Assert.AreEqual(0, players[2].PlayerDisconnects.Count);
        Assert.AreEqual(0, players[3].PlayerDisconnects.Count);
        Assert.AreEqual(0, players[4].PlayerDisconnects.Count);

        Assert.AreEqual(0, players[5].PlayerDisconnects.Count);
        Assert.AreEqual(10, players[6].PlayerDisconnects.Count);
        Assert.IsTrue(players[6].PlayerDisconnects.All(x => x.To is not null));
        Assert.AreEqual(1, players[7].PlayerDisconnects.Count);
        Assert.IsNull(players[7].PlayerDisconnects[0].To);
        Assert.AreEqual(0, players[8].PlayerDisconnects.Count);
        Assert.AreEqual(0, players[9].PlayerDisconnects.Count);
    }

    [TestMethod]
    public void NoGameEventsParsingNoPlayerDisconnectsTest()
    {
        StormReplayResult result = StormReplay.Parse(
            Path.Combine(_replaysFolder, "WarheadJunctionDisconnectingPlayer1_91418.StormR"),
            new ParseOptions()
            {
                ShouldParseGameEvents = false,
            });

        Assert.IsTrue(result.Replay.StormPlayers.All(x => x.PlayerDisconnects.Count == 0));
    }
}
