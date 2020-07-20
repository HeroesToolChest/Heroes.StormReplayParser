using Heroes.StormReplayParser.Decoders;
using Heroes.StormReplayParser.MpqHeroesTool;
using System;

namespace Heroes.StormReplayParser.MpqFiles
{
    internal static class ReplayTrackerEvents
    {
        public static string FileName { get; } = "replay.tracker.events";

        public static void Parse(StormReplay replay, ReadOnlySpan<byte> source)
        {
            BitReader bitReader = new BitReader(source, EndianType.BigEndian);

            uint gameLoop = 0;

            while (bitReader.Index < source.Length)
            {
                gameLoop += new VersionedDecoder(ref bitReader).ChoiceData!.GetValueAsUInt32();

                TimeSpan timeSpan = TimeSpan.FromSeconds(gameLoop / 16.0);
                TrackerEventType type = (TrackerEventType)new VersionedDecoder(ref bitReader).GetValueAsUInt32();
                VersionedDecoder decoder = new VersionedDecoder(ref bitReader);

                replay.TrackerEventsInternal.Add(new TrackerEvent(type, timeSpan, decoder));
            }
        }
    }
}
