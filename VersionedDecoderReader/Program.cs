// for .s2mh and .s2mi files

using Heroes.MpqTool;
using Heroes.StormReplayParser.Decoders;

if (args.Length < 1)
{
    Console.WriteLine("Usage: VersionedDecoderReader <file>");
    return;
}

if (!File.Exists(args[0]))
{
    Console.WriteLine($"File not found: {args[0]}");
    return;
}

ReadOnlySpan<byte> source = File.ReadAllBytes(args[0]);

BitReader bitReader = new(source, EndianType.BigEndian);

VersionedDecoder versionedDecoder = new(ref bitReader);

// string json = versionedDecoder.ToJson();

Console.WriteLine("Done.");