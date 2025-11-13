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
        List<StormPlayer> players = [.. _stormReplay.StormPlayers];

        Assert.IsEmpty(players[0].PlayerDisconnects);
        Assert.IsEmpty(players[1].PlayerDisconnects);
        Assert.IsEmpty(players[2].PlayerDisconnects);
        Assert.IsEmpty(players[3].PlayerDisconnects);
        Assert.IsEmpty(players[4].PlayerDisconnects);

        Assert.IsEmpty(players[5].PlayerDisconnects);
        Assert.HasCount(10, players[6].PlayerDisconnects);
        Assert.IsTrue(players[6].PlayerDisconnects.All(x => x.To is not null));
        Assert.HasCount(1, players[7].PlayerDisconnects);
        Assert.IsNull(players[7].PlayerDisconnects[0].To);
        Assert.IsEmpty(players[8].PlayerDisconnects);
        Assert.IsEmpty(players[9].PlayerDisconnects);
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
