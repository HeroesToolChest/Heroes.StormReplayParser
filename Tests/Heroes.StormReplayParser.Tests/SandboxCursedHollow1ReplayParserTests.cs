namespace Heroes.StormReplayParser.Tests;

// the open slots still have per player attributes in the replay.attributes.events
[TestClass]
public class SandboxCursedHollow1ReplayParserTests
{
    private readonly string _replaysFolder = "Replays";
    private readonly string _replayFile = "SandboxCursedHollow1_97771.StormR";
    private readonly StormReplay _stormReplay;
    private readonly StormReplayParseStatus _result;

    public SandboxCursedHollow1ReplayParserTests()
    {
        StormReplayResult result = StormReplay.Parse(Path.Combine(_replaysFolder, _replayFile));
        _stormReplay = result.Replay;
        _result = result.Status;
    }

    [TestMethod]
    public void ParseResultTest()
    {
        Assert.AreEqual(StormReplayParseStatus.Incomplete, _result); // because of no winner
    }

    [TestMethod]
    public void PlayerCountTest()
    {
        Assert.AreEqual(8, _stormReplay.PlayersCount);
    }

    [TestMethod]
    public void ComputerPlayersTest()
    {
        List<StormPlayer> players = [.. _stormReplay.StormPlayers];

        Assert.IsTrue(players.All(x => x.PlayerType == PlayerType.Computer));
        Assert.AreEqual(StormGameMode.Custom, _stormReplay.GameMode);
    }
}
