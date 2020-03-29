using Heroes.StormReplayParser.Replay;
using System;

namespace Heroes.ReplayParser.MpqFiles
{
    internal interface IMpqFiles
    {
        public string FileName { get; }

        public void Parse(StormReplay stormReplay, ReadOnlySpan<byte> source);
    }
}
