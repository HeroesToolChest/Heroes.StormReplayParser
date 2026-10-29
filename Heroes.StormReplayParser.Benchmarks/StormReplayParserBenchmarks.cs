using BenchmarkDotNet.Jobs;

namespace Heroes.StormReplayParser.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class StormReplayParserBenchmarks
{
    private readonly string _replaysFolder = "Replays";
    private readonly string _replayFile = "SilverCity1_85267.StormR";
    private readonly string _replayFile2 = "LostCavernNonSingleUnit1_76517.StormR";

    public StormReplayParserBenchmarks()
    {
    }

    // | Method                                 | Mean      | Error     | StdDev    | Median    | Gen0       | Gen1       | Gen2      | Allocated |
    // |--------------------------------------- |----------:|----------:|----------:|----------:|-----------:|-----------:|----------:|----------:|
    // | ParseReplay1                           |  82.18 ms |  1.628 ms |  2.674 ms |  83.57 ms |  3285.7143 |  3142.8571 | 1142.8571 |  45.87 MB |
    // | ParseReplay1WithLicenseGen             |  82.96 ms |  1.617 ms |  2.102 ms |  82.73 ms |  3333.3333 |  3166.6667 | 1000.0000 |  46.55 MB |
    // | ParseReplayNonSingleUnit               | 778.65 ms | 13.256 ms | 12.400 ms | 776.28 ms | 27000.0000 | 26000.0000 | 4000.0000 | 375.85 MB |
    // | ParseReplayNonSingleUnitWithLicenseGen | 773.08 ms |  8.634 ms |  8.076 ms | 771.31 ms | 27000.0000 | 26000.0000 | 4000.0000 | 376.33 MB |

    [Benchmark]
    public void ParseReplay1()
    {
        _ = StormReplay.Parse(Path.Join(_replaysFolder, _replayFile), new ParseOptions());
    }
    [Benchmark]
    public void ParseReplay1WithLicenseGen()
    {
        _ = StormReplay.Parse(Path.Join(_replaysFolder, _replayFile), new ParseOptions()
        {
             ShouldParseLicenseCatalog = true,
        });
    }

    [Benchmark]
    public void ParseReplayNonSingleUnit()
    {
        _ = StormReplay.Parse(Path.Join(_replaysFolder, _replayFile2), new ParseOptions());
    }

    [Benchmark]
    public void ParseReplayNonSingleUnitWithLicenseGen()
    {
        _ = StormReplay.Parse(Path.Join(_replaysFolder, _replayFile2), new ParseOptions()
        {
             ShouldParseLicenseCatalog = true,
        });
    }
}
