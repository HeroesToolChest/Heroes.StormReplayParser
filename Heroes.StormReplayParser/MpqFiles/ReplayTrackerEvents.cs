using Heroes.StormReplayParser.Decoders;
using Heroes.StormReplayParser.MpqHeroesTool;
using Heroes.StormReplayParser.Replay;
using System;

namespace Heroes.StormReplayParser.MpqFiles
{
    internal static class ReplayTrackerEvents
    {
        public static string FileName { get; } = "replay.tracker.events";

        public static void Parse(StormReplay replay, ReadOnlySpan<byte> source)
        {
            BitReader.ResetIndex();
            BitReader.EndianType = EndianType.BigEndian;

            uint gameLoop = 0;

            while (BitReader.Index < source.Length)
            {
                gameLoop += new VersionedDecoder(source).ChoiceData!.GetValueAsUInt32();

                TimeSpan timeSpan = TimeSpan.FromSeconds(gameLoop / 16.0);
                TrackerEventType type = (TrackerEventType)new VersionedDecoder(source).GetValueAsUInt32();
                VersionedDecoder decoder = new VersionedDecoder(source);

                replay.TrackerEventsInternal.Add(new TrackerEvent(type, timeSpan, decoder));
            }
        }
    }
}
