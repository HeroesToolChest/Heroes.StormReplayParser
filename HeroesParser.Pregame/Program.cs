using Heroes.StormReplayParser;

if (args is not null && args.Length == 1 && File.Exists(args[0]))
{
    StormReplayPregameResult result = StormReplayPregame.Parse(args[0], new ParsePregameOptions());

    Console.WriteLine(result.Status);
}
else
{
    Console.WriteLine("No file.");
}
