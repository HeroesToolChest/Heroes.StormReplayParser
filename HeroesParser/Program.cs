using Heroes.StormReplayParser;

namespace HeroesParser;

public static class Program
{
    public static void Main(string[] args)
    {
        if (args is not null && args.Length == 1 && File.Exists(args[0]))
        {
            StormReplayResult stormReplayResult = StormReplay.Parse(args[0], new ParseOptions()
            {
                AllowPTR = true,
                ShouldParseGameEvents = true,
                ShouldParseMessageEvents = true,
                ShouldParseTrackerEvents = true,
            });

            Console.WriteLine(stormReplayResult.Status);
        }
        else
        {
            Console.WriteLine("No file.");
        }
    }
}
