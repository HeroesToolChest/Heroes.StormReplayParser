using BenchmarkDotNet.Jobs;

namespace Heroes.StormReplayParser.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Net70)]
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
        for (int i = 0; i < 10; i++)
        {
            _ = StormReplay.Parse(Path.Join(_replaysFolder, _replayFile));
        }
    }
}
