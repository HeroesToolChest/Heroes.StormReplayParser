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

            BitReader.ResetIndex();
            BitReader.EndianType = EndianType.BigEndian;

            uint ticksElapsed = 0;

            while (BitReader.Index < source.Length)
            {
                ticksElapsed += source.ReadBits(6 + ((int)source.ReadBits(2) << 3));
                TimeSpan timeStamp = TimeSpan.FromSeconds(ticksElapsed / 16.0);

                int playerIndex = (int)source.ReadBits(5);
                StormMessageEventType messageEventType = (StormMessageEventType)source.ReadBits(4);

                StormMessage? message = null;

                switch (messageEventType)
                {
                    case StormMessageEventType.SChatMessage:
                        ChatMessage chatMessage = new ChatMessage
                        {
                            MessageTarget = (StormMessageTarget)source.ReadBits(3), // m_recipient (the target)
                            Message = source.ReadBlobAsString(11), // m_string
                        };

                        message = new StormMessage(chatMessage);

                        break;
                    case StormMessageEventType.SPingMessage:
                        PingMessage pingMessage = new PingMessage()
                        {
                            MessageTarget = (StormMessageTarget)source.ReadBits(3), // m_recipient (the target)
                            Point = new Point((double)(source.ReadInt32Unaligned() - (-2147483648)) / 4096, ((double)source.ReadInt32Unaligned() - (-2147483648)) / 4096), // m_point x and m_point y
                        };

                        message = new StormMessage(pingMessage);

                        break;
                    case StormMessageEventType.SLoadingProgressMessage:
                        LoadingProgressMessage loadingProgressMessage = new LoadingProgressMessage()
                        {
                            LoadingProgress = source.ReadInt32Unaligned() - (-2147483648), // m_progress
                        };

                        message = new StormMessage(loadingProgressMessage);

                        break;
                    case StormMessageEventType.SServerPingMessage:
                        break;
                    case StormMessageEventType.SReconnectNotifyMessage:
                        source.ReadBits(2); // m_status; is either a 1 or a 2
                        break;
                    case StormMessageEventType.SPlayerAnnounceMessage:
                        PlayerAnnounceMessage playerAnnounceMessage = new PlayerAnnounceMessage()
                        {
                            AnnouncementType = (AnnouncementType)source.ReadBits(2),
                        };

                        switch (playerAnnounceMessage.AnnouncementType)
                        {
                            case AnnouncementType.None:
                                break;
                            case AnnouncementType.Ability:
                                int abilityLink = source.ReadInt16Unaligned(); // m_abilLink
                                int abilityIndex = (int)source.ReadBits(5); // m_abilCmdIndex
                                int buttonLink = source.ReadInt16Unaligned(); // m_buttonLink
                                playerAnnounceMessage.AbilityAnnouncement = new AbilityAnnouncement(abilityIndex, abilityLink, buttonLink);

                                break;
                            case AnnouncementType.Behavior:
                                source.ReadInt16Unaligned(); // m_behaviorLink
                                source.ReadInt16Unaligned(); // m_buttonLink
                                break;
                            case AnnouncementType.Vitals:
                                playerAnnounceMessage.VitalAnnouncement = new VitalAnnouncement((VitalType)(source.ReadInt16Unaligned() - (-32768)));
                                break;
                            default:
                                throw new NotImplementedException();
                        }

                        if (replay.ReplayBuild > 45635)
                        {
                            // m_announceLink
                            source.ReadInt16Unaligned();
                        }

                        source.ReadInt32Unaligned(); // m_otherUnitTag
                        source.ReadInt32Unaligned(); // m_unitTag

                        message = new StormMessage(playerAnnounceMessage);
                        break;

                    default:
                        throw new NotImplementedException();
                }

                if (message != null)
                {
                    message.MessageEventType = messageEventType;
                    message.Timestamp = timeStamp;

                    if (playerIndex != 16)
                    {
                        message.MessageSender = replay.ClientListByUserID[playerIndex];
                    }

                    replay.MessagesInternal.Add(message);
                }

                BitReader.AlignToByte();
            }
        }
    }
}
