namespace Heroes.StormReplayParser.Benchmarks;

[MemoryDiagnoser]
public class StormReplayParserBenchmarks
{
    private readonly string _replaysFolder = "Replays";
    private readonly string _replayFile = "SilverCity1_85267.StormR";

    public StormReplayParserBenchmarks()
    {
    }

    [Benchmark]
    public void ParseReplay10()
    {
        for (int i = 0; i < 1; i++)
        {
            _ = StormReplay.Parse(Path.Join(_replaysFolder, _replayFile));
        }
    }
}
