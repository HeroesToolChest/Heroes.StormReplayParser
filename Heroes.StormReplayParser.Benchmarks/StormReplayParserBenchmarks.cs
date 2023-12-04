using BenchmarkDotNet.Jobs;

namespace Heroes.StormReplayParser.Benchmarks;

[MemoryDiagnoser]
public class StormReplayParserBenchmarks
{
    private readonly string _replaysFolder = "Replays";
    private readonly string _replayFile = "SilverCity1_85267.StormR";
    private readonly string _replayFile2 = "LostCavernNonSingleUnit1_76517.StormR";

    public StormReplayParserBenchmarks()
    {
    }

    [Benchmark]
    public void ParseReplay1()
    {
        _ = StormReplay.Parse(Path.Join(_replaysFolder, _replayFile));
    }

    [Benchmark]
    public void ParseReplayNonSingleUnit()
    {
        _ = StormReplay.Parse(Path.Join(_replaysFolder, _replayFile2));
    }
}
