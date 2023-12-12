namespace Heroes.StormReplayParser.Tests;

[TestClass]
public class SilverCity1ReplayParserTests
{
    private readonly string _replaysFolder = "Replays";
    private readonly string _replayFile = "SilverCity1_85267.StormR";
    private readonly StormReplay _stormReplay;
    private readonly StormReplayParseStatus _result;

    public SilverCity1ReplayParserTests()
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
    public void StormReplayInitDataTest()
    {
        Assert.AreEqual(3845356360, _stormReplay.RandomValue);
        Assert.AreEqual(StormGameMode.ARAM, _stormReplay.GameMode);

        var disabledHeroes = _stormReplay.DisabledHeroes.ToList();

        Assert.AreEqual(1, disabledHeroes.Count);
        Assert.AreEqual("STUK", disabledHeroes[0]);
    }

    [TestMethod]
    public void TrackerEventsAsJsonTest()
    {
        int i = 0;
        foreach (string? item in _stormReplay.TrackerEvents.Select(x => x.VersionedDecoder?.AsJson()))
        {
            if (item is null)
                continue;

            try
            {
                JsonDocument.Parse(item);
                i++;
            }
            catch (Exception ex)
            {
                Assert.Fail($"Expected no exception, but received: {ex.Message}");
            }
        }

        Assert.AreEqual(_stormReplay.TrackerEvents.Count, i);
    }
}
