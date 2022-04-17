using Heroes.StormReplayParser;

namespace Heroes.ReplayParser.MpqFiles;

internal interface IMpqFiles
{
    public string FileName { get; }

    public void Parse(StormReplay stormReplay, ReadOnlySpan<byte> source);
}
