namespace Heroes.StormReplayParser.Tests;

[TestClass]
public class NoPlayerNamesNoTag1ReplayParserTests
{
    private readonly string _replaysFolder = "Replays";
    private readonly string _replayFile = "NoPlayerNamesNoTag1_77548.StormR";
    private readonly StormReplay _stormReplay;
    private readonly StormReplayParseStatus _result;

    public NoPlayerNamesNoTag1ReplayParserTests()
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
    public void PlayerNoNameNoTag()
    {
        StormPlayer player = _stormReplay.StormPlayers.ToList()[0];
        Assert.AreEqual(string.Empty, player.BattleTagName);
        Assert.IsTrue(player.ToonHandle!.Id.ToString().StartsWith("1289"));
    }
}
