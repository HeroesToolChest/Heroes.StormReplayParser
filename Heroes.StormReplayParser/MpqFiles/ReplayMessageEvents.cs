using Heroes.StormReplayParser.MessageEvent;
using Heroes.StormReplayParser.MpqHeroesTool;
using System;

namespace Heroes.StormReplayParser.MpqFiles
{
    internal static class ReplayMessageEvents
    {
        public static string FileName { get; } = "replay.message.events";

        public static void Parse(StormReplay replay, ReadOnlySpan<byte> source)
        {
            if (source.Length <= 1)
                return;

            BitReader bitReader = new BitReader(source, EndianType.BigEndian);

            uint ticksElapsed = 0;

            while (bitReader.Index < source.Length)
            {
                ticksElapsed += bitReader.ReadBits(6 + ((int)bitReader.ReadBits(2) << 3));
                TimeSpan timeStamp = TimeSpan.FromSeconds(ticksElapsed / 16.0);

                int playerIndex = (int)bitReader.ReadBits(5);

                StormMessageEventType messageEventType = (StormMessageEventType)bitReader.ReadBits(4);

                IStormMessage? stormMessage = null;

                switch (messageEventType)
                {
                    case StormMessageEventType.SChatMessage:
                        stormMessage = new ChatMessage
                        {
                            MessageTarget = (StormMessageTarget)bitReader.ReadBits(3), // m_recipient (the target)
                            Text = bitReader.ReadBlobAsString(11), // m_string
                        };

                        break;
                    case StormMessageEventType.SPingMessage:
                        stormMessage = new PingMessage()
                        {
                            MessageTarget = (StormMessageTarget)bitReader.ReadBits(3), // m_recipient (the target)
                            Point = new Point((double)(bitReader.ReadInt32Unaligned() - (-2147483648)) / 4096, ((double)bitReader.ReadInt32Unaligned() - (-2147483648)) / 4096), // m_point x and m_point y
                        };

                        break;
                    case StormMessageEventType.SLoadingProgressMessage:
                        stormMessage = new LoadingProgressMessage()
                        {
                            LoadingProgress = bitReader.ReadInt32Unaligned() - (-2147483648), // m_progress
                        };

                        break;
                    case StormMessageEventType.SServerPingMessage:
                        break;
                    case StormMessageEventType.SReconnectNotifyMessage:
                        bitReader.ReadBits(2); // m_status; is either a 1 or a 2
                        break;
                    case StormMessageEventType.SPlayerAnnounceMessage:
                        PlayerAnnounceMessage playerAnnounceMessage = new PlayerAnnounceMessage()
                        {
                            AnnouncementType = (AnnouncementType)bitReader.ReadBits(2),
                        };

                        switch (playerAnnounceMessage.AnnouncementType)
                        {
                            case AnnouncementType.None:
                                break;
                            case AnnouncementType.Ability:
                                int abilityLink = bitReader.ReadInt16Unaligned(); // m_abilLink
                                int abilityIndex = (int)bitReader.ReadBits(5); // m_abilCmdIndex
                                int buttonLink = bitReader.ReadInt16Unaligned(); // m_buttonLink
                                playerAnnounceMessage.AbilityAnnouncement = new AbilityAnnouncement(abilityIndex, abilityLink, buttonLink);

                                break;
                            case AnnouncementType.Behavior:
                                bitReader.ReadInt16Unaligned(); // m_behaviorLink
                                bitReader.ReadInt16Unaligned(); // m_buttonLink
                                break;
                            case AnnouncementType.Vitals:
                                playerAnnounceMessage.VitalAnnouncement = new VitalAnnouncement((VitalType)(bitReader.ReadInt16Unaligned() - (-32768)));
                                break;
                            default:
                                throw new NotImplementedException();
                        }

                        if (replay.ReplayBuild > 45635)
                        {
                            // m_announceLink
                            bitReader.ReadInt16Unaligned();
                        }

                        bitReader.ReadInt32Unaligned(); // m_otherUnitTag
                        bitReader.ReadInt32Unaligned(); // m_unitTag

                        stormMessage = playerAnnounceMessage;
                        break;

                    default:
                        throw new NotImplementedException();
                }

                if (stormMessage is not null)
                {
                    stormMessage.MessageEventType = messageEventType;
                    stormMessage.Timestamp = timeStamp;

                    if (playerIndex != 16)
                    {
                        stormMessage.MessageSender = replay.ClientListByUserID[playerIndex];
                    }

                    replay.MessagesInternal.Add(stormMessage);
                }

                bitReader.AlignToByte();
            }
        }
    }
}
