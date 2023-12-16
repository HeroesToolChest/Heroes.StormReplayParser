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
    public void TrackerEventsVersionedDecoderToJsonTest()
    {
        int i = 0;
        foreach (string? item in _stormReplay.TrackerEvents.Select(x => x.VersionedDecoder?.ToJson()))
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
        Assert.AreEqual("{\"0\": 67,\"1\": 124,\"2\": \"FootmanMinion\",\"3\": 12,\"4\": 12,\"5\": 188,\"6\": 105}", _stormReplay.TrackerEvents[2434].VersionedDecoder!.ToJson());
    }

    [TestMethod]
    public void GameEventsDataToJsonTest()
    {
        int i = 0;
        foreach (string? item in _stormReplay.GameEvents.Select(x => x.Data?.ToJson()))
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

        Assert.AreEqual(_stormReplay.GameEvents.Count, i);
        Assert.AreEqual("{\"0\": \"87558\"}", _stormReplay.GameEvents[3567].Data!.ToJson());
    }
}
