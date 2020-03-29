using Heroes.StormReplayParser.GameEvent;
using Heroes.StormReplayParser.MpqHeroesTool;
using System;

namespace Heroes.StormReplayParser.MpqFiles
{
    public static class ReplayGameEvents
    {
        public static string FileName { get; } = "replay.game.events";

        public static void Parse(StormReplay replay, ReadOnlySpan<byte> source)
        {
            BitReader.ResetIndex();
            BitReader.EndianType = EndianType.BigEndian;

            uint ticksElapsed = 0;

            while (BitReader.Index < source.Length)
            {
                ticksElapsed += source.ReadBits(6 + ((int)source.ReadBits(2) << 3));
                TimeSpan timeStamp = TimeSpan.FromSeconds(ticksElapsed / 16.0);

                int playerIndex = (int)source.ReadBits(5);
                StormGameEventType gameEventType = (StormGameEventType)source.ReadBits(7);

                StormGameEvent? gameEvent = null;

                switch (gameEventType)
                {

                }

                //if (gameEvent != null)
                //{
                //    gameEvent.MessageEventType = messageEventType;
                //    gameEvent.Timestamp = timeStamp;

                //    if (playerIndex != 16)
                //    {
                //        gameEvent.MessageSender = replay.ClientListByUserID[playerIndex];
                //    }

                //    replay.MessagesInternal.Add(gameEvent);
                //}

                BitReader.AlignToByte();

            }
        }
    }
}
