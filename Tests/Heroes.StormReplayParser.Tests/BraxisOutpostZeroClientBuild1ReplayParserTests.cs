namespace Heroes.StormReplayParser.Tests;

[TestClass]
public class BraxisOutpostZeroClientBuild1ReplayParserTests
{
    private readonly string _replaysFolder = "Replays";
    private readonly string _replayFile = "BraxisOutpostZeroClientBuild1_96477.StormR";
    private readonly StormReplay _stormReplay;
    private readonly StormReplayParseStatus _result;

    public BraxisOutpostZeroClientBuild1ReplayParserTests()
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
        Assert.AreEqual(10, _stormReplay.PlayersWithObserversCount);
        Assert.AreEqual(10, _stormReplay.PlayersCount);
    }

    [TestMethod]
    public void BattleLobbyPlayersAfterZeroClientBuildTest()
    {
        List<StormPlayer> players = [.. _stormReplay.StormPlayers];

        // the player with the client base build of 0 in the replay.server.battlelobby
        Assert.StartsWith("ARM#", players[1].BattleTagName);
        Assert.AreEqual(1936, players[1].AccountLevel);

        Assert.StartsWith("bdomo#", players[2].BattleTagName);
        Assert.AreEqual(3555, players[2].AccountLevel);

        Assert.StartsWith("goldSpear#", players[9].BattleTagName);
        Assert.AreEqual(1708, players[9].AccountLevel);
    }
}
