using Heroes.StormReplayParser.GameEvent;
using Heroes.StormReplayParser.MpqHeroesTool;
using Heroes.StormReplayParser.Player;
using System;
using System.Collections.Generic;

namespace Heroes.StormReplayParser.MpqFiles
{
    internal static class ReplayGameEvents
    {
        public static string FileName { get; } = "replay.game.events";

        public static void Parse(StormReplay replay, ReadOnlySpan<byte> source)
        {
            BitReader bitReader = new BitReader(source, EndianType.BigEndian);

            uint ticksElapsed = 0;

            while (bitReader.Index < bitReader.Length)
            {
                ticksElapsed += bitReader.ReadBits(6 + ((int)bitReader.ReadBits(2) << 3));

                // time
                TimeSpan timeStamp = TimeSpan.FromSeconds(ticksElapsed / 16.0);

                int playerIndex = (int)bitReader.ReadBits(5);

                // player
                StormPlayer? player = null;
                if (playerIndex != 16)
                {
                    player = replay.ClientListByUserID[playerIndex];
                }

                // type
                StormGameEventType gameEventType = (StormGameEventType)bitReader.ReadBits(7);

                StormGameEvent? gameEvent = null;

                switch (gameEventType)
                {
                    case StormGameEventType.SStartGameEvent:
                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, null);
                        break;
                    case StormGameEventType.SUserFinishedLoadingSyncEvent:
                    case StormGameEventType.SSaveGameDoneEvent:
                    case StormGameEventType.SLoadGameDoneEvent:
                    case StormGameEventType.STriggerSkippedEvent:
                    case StormGameEventType.STriggerAbortMissionEvent:
                    case StormGameEventType.STriggerMovieStartedEvent:
                    case StormGameEventType.STriggerMovieFinishedEvent:
                    case StormGameEventType.STriggerGameCreditsFinishedEvent:
                    case StormGameEventType.STriggerProfilerLoggingFinishedEvent:
                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(new Dictionary<int, StormGameEventData>(0)));
                        break;
                    case StormGameEventType.SUserOptionsEvent:
                        Dictionary<int, StormGameEventData> structure = new Dictionary<int, StormGameEventData>(14)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBoolean()), // m_gameFullyDownloaded
                            [1] = new StormGameEventData(bitReader.ReadBoolean()), // m_developmentCheatsEnabled
                            [2] = new StormGameEventData(bitReader.ReadBoolean()), // m_testCheatsEnabled
                            [3] = new StormGameEventData(bitReader.ReadBoolean()), // m_multiplayerCheatsEnabled
                            [4] = new StormGameEventData(bitReader.ReadBoolean()), // m_syncChecksummingEnabled
                            [5] = new StormGameEventData(bitReader.ReadBoolean()), // m_isMapToMapTransition
                            [6] = new StormGameEventData(bitReader.ReadBoolean()), // m_debugPauseEnabled
                            [7] = new StormGameEventData(bitReader.ReadBoolean()), // m_useGalaxyAsserts
                            [8] = new StormGameEventData(bitReader.ReadBoolean()), // m_platformMac
                            [9] = new StormGameEventData(bitReader.ReadBoolean()), // m_cameraFollow
                            [10] = new StormGameEventData(bitReader.ReadBits(32)), // m_baseBuildNum
                            [11] = new StormGameEventData(bitReader.ReadBits(32)), // m_buildNum
                            [12] = new StormGameEventData(bitReader.ReadBits(32)), // m_versionFlags
                            [13] = new StormGameEventData(bitReader.ReadBlobAsString(9)), // m_hotkeyProfile
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SBankFileEvent:
                        structure = new Dictionary<int, StormGameEventData>(1)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBlobAsString(7)), // m_name
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SBankSectionEvent:
                        structure = new Dictionary<int, StormGameEventData>(1)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBlobAsString(6)), // m_name
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SBankKeyEvent:
                        structure = new Dictionary<int, StormGameEventData>(3)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBlobAsString(6)), // m_name
                            [1] = new StormGameEventData(bitReader.ReadBits(32)), // m_type
                            [2] = new StormGameEventData(bitReader.ReadBlobAsString(7)), // m_data
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SBankValueEvent:
                        structure = new Dictionary<int, StormGameEventData>(1)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBits(32)), // m_type
                            [1] = new StormGameEventData(bitReader.ReadBlobAsString(6)), // m_name
                            [2] = new StormGameEventData(bitReader.ReadBlobAsString(12)), // m_data
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SBankSignatureEvent:
                        StormGameEventData[] signatureArray = new StormGameEventData[bitReader.ReadBits(5)];
                        for (int i = 0; i < signatureArray.Length; i++)
                        {
                            signatureArray[i] = new StormGameEventData(bitReader.ReadBits(8));
                        }

                        structure = new Dictionary<int, StormGameEventData>(2)
                        {
                            [0] = new StormGameEventData(signatureArray), // m_signature
                            [1] = new StormGameEventData(bitReader.ReadBlobAsString(7)), // m_toonHandle
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SCameraSaveEvent:
                        structure = new Dictionary<int, StormGameEventData>(2)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBits(3)), // m_which
                            [1] = new StormGameEventData(new Dictionary<int, StormGameEventData>(2) // m_target
                            {
                                [0] = new StormGameEventData(bitReader.ReadBits(16)), // x
                                [1] = new StormGameEventData(bitReader.ReadBits(16)), // y
                            }),
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SSaveGameEvent:
                        structure = new Dictionary<int, StormGameEventData>(5)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBlobAsString(11)), // m_fileName
                            [1] = new StormGameEventData(bitReader.ReadBoolean()), // m_automatic
                            [2] = new StormGameEventData(bitReader.ReadBoolean()), // m_overwrite
                            [3] = new StormGameEventData(bitReader.ReadBlobAsString(8)), // m_name
                            [4] = new StormGameEventData(bitReader.ReadBlobAsString(10)), // m_description
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SCommandManagerResetEvent:
                        structure = new Dictionary<int, StormGameEventData>(1)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBits(32)), // m_sequence
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SGameCheatEvent:
                        Dictionary<int, StormGameEventData>? targetChoice = null;

                        switch (bitReader.ReadBits(2))
                        {
                            case 0: // None
                                break;
                            case 1: // TargetPoint
                                targetChoice = PointXYZStucture(ref bitReader);

                                break;
                            case 2: // TargetUnit
                                targetChoice = new Dictionary<int, StormGameEventData>(7)
                                {
                                    [0] = new StormGameEventData(bitReader.ReadBits(16)), // m_targetUnitFlags
                                    [1] = new StormGameEventData(bitReader.ReadBits(8)), // m_timer
                                    [2] = new StormGameEventData(bitReader.ReadBits(32)), // m_tag
                                    [3] = new StormGameEventData(bitReader.ReadBits(16)), // m_snapshotUnitLink
                                };

                                if (bitReader.ReadBoolean()) // m_snapshotControlPlayerId
                                    targetChoice[4] = new StormGameEventData(bitReader.ReadBits(4));
                                if (bitReader.ReadBoolean()) // m_snapshotUpkeepPlayerId
                                    targetChoice[5] = new StormGameEventData(bitReader.ReadBits(4));

                                // m_snapshotPoint
                                targetChoice[6] = new StormGameEventData(PointXYZStucture(ref bitReader));

                                break;
                            default:
                                throw new NotImplementedException();
                        }

                        structure = new Dictionary<int, StormGameEventData>(1)
                        {
                            [0] = new StormGameEventData(new Dictionary<int, StormGameEventData>(4) // m_data
                            {
                                [0] = new StormGameEventData(targetChoice), // m_target
                                [1] = new StormGameEventData((int)(bitReader.ReadBits(32) - 2147483648)), // m_time
                                [2] = new StormGameEventData(bitReader.ReadBlobAsString(10)), // m_verb
                                [3] = new StormGameEventData(bitReader.ReadBlobAsString(10)), // m_arguments
                            }),
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SCmdEvent:
                        if (replay.ReplayBuild < 33684)
                            structure = new Dictionary<int, StormGameEventData>(5);
                        else if (replay.ReplayBuild < 44256)
                            structure = new Dictionary<int, StormGameEventData>(6);
                        else
                            structure = new Dictionary<int, StormGameEventData>(7);

                        // m_cmdFlags
                        if (replay.ReplayBuild < 33684)
                            structure[0] = new StormGameEventData(bitReader.ReadBits(22));
                        else if (replay.ReplayBuild < 37117)
                            structure[0] = new StormGameEventData(bitReader.ReadBits(23));
                        else if (replay.ReplayBuild < 38236 || (replay.ReplayBuild >= 42958 && replay.ReplayBuild < 44256))
                            structure[0] = new StormGameEventData(bitReader.ReadBits(24));
                        else if (replay.ReplayBuild < 42958 || (replay.ReplayBuild > 45635 && replay.ReplayVersion.Major < 2))
                            structure[0] = new StormGameEventData(bitReader.ReadBits(25));
                        else if (replay.ReplayBuild <= 45635 || replay.ReplayBuild < 59837 || replay.ReplayBuild == 59988 || replay.ReplayBuild > 62424)
                            structure[0] = new StormGameEventData(bitReader.ReadBits(26));
                        else if (replay.ReplayBuild < 62833)
                            structure[0] = new StormGameEventData(bitReader.ReadBits(27));
                        else
                            throw new NotImplementedException();

                        // m_abil
                        if (bitReader.ReadBoolean())
                        {
                            Dictionary<int, StormGameEventData> abilStructure = new Dictionary<int, StormGameEventData>(3)
                            {
                                [0] = new StormGameEventData(bitReader.ReadBits(16)), // m_abilLink
                                [1] = new StormGameEventData(bitReader.ReadBits(5)), // m_abilCmdIndex
                            };

                            // m_abilCmdData
                            if (bitReader.ReadBoolean())
                                abilStructure[2] = new StormGameEventData(bitReader.ReadBits(8));

                            structure[1] = new StormGameEventData(abilStructure);
                        }

                        // m_data
                        Dictionary<int, StormGameEventData>? dataChoice;

                        switch (bitReader.ReadBits(2))
                        {
                            case 0: // None
                                dataChoice = null;
                                structure[2] = new StormGameEventData(dataChoice);
                                break;
                            case 1: // TargetPoint
                                dataChoice = PointXYZStucture(ref bitReader);
                                structure[2] = new StormGameEventData(dataChoice);

                                break;
                            case 2: // TargetUnit
                                dataChoice = new Dictionary<int, StormGameEventData>(7)
                                {
                                    [0] = new StormGameEventData(bitReader.ReadBits(16)), // m_targetUnitFlags
                                    [1] = new StormGameEventData(bitReader.ReadBits(8)), // m_timer
                                    [2] = new StormGameEventData(bitReader.ReadBits(32)), // m_tag
                                    [3] = new StormGameEventData(bitReader.ReadBits(16)), // m_snapshotUnitLink
                                };

                                // m_snapshotControlPlayerId
                                if (bitReader.ReadBoolean())
                                    dataChoice[4] = new StormGameEventData(bitReader.ReadBits(4));

                                // m_snapshotUpkeepPlayerId
                                if (bitReader.ReadBoolean())
                                    dataChoice[5] = new StormGameEventData(bitReader.ReadBits(4));

                                dataChoice[6] = new StormGameEventData(PointXYZStucture(ref bitReader)); // m_snapshotPoint
                                structure[2] = new StormGameEventData(dataChoice);
                                break;
                            case 3: // Data
                                structure[2] = new StormGameEventData(bitReader.ReadBits(32));
                                break;
                            default:
                                throw new NotImplementedException();
                        }

                        // m_vector
                        if (replay.ReplayBuild >= 44256 && bitReader.ReadBoolean())
                            structure[3] = new StormGameEventData(PointXYZStucture(ref bitReader));

                        // m_sequence
                        if (replay.ReplayBuild >= 33684 && replay.ReplayBuild < 44256)
                            structure[3] = new StormGameEventData(bitReader.ReadBits(32) + 1);
                        else if (replay.ReplayBuild >= 33684)
                            structure[4] = new StormGameEventData(bitReader.ReadBits(32) + 1);

                        // m_otherUnit
                        if (bitReader.ReadBoolean())
                        {
                            if (replay.ReplayBuild < 33684)
                                structure[3] = new StormGameEventData(bitReader.ReadBits(32));
                            else if (replay.ReplayBuild < 44256)
                                structure[4] = new StormGameEventData(bitReader.ReadBits(32));
                            else
                                structure[5] = new StormGameEventData(bitReader.ReadBits(32));
                        }

                        // m_unitGroup
                        if (bitReader.ReadBoolean())
                        {
                            if (replay.ReplayBuild < 33684)
                                structure[4] = new StormGameEventData(bitReader.ReadBits(32));
                            if (replay.ReplayBuild < 44256)
                                structure[5] = new StormGameEventData(bitReader.ReadBits(32));
                            else
                                structure[6] = new StormGameEventData(bitReader.ReadBits(32));
                        }

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SSelectionDeltaEvent:
                        structure = new Dictionary<int, StormGameEventData>(2)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBits(4)), // m_controlGroupId
                            [1] = new StormGameEventData(new Dictionary<int, StormGameEventData>(4) // m_delta
                            {
                                [0] = new StormGameEventData(bitReader.ReadBits(5)), // m_subgroupIndex
                                [1] = MaskChoice(replay, ref bitReader), // m_removeMask
                            }),
                        };

                        // m_addSubgroups
                        StormGameEventData[] addSubgroupsArray = new StormGameEventData[bitReader.ReadBits(6)];
                        for (int i = 0; i < addSubgroupsArray.Length; i++)
                        {
                            Dictionary<int, StormGameEventData> subGroupStructure = new Dictionary<int, StormGameEventData>(4)
                            {
                                [0] = new StormGameEventData(bitReader.ReadBits(16)), // m_unitLink
                                [1] = new StormGameEventData(bitReader.ReadBits(8)), // m_subgroupPriority
                                [2] = new StormGameEventData(bitReader.ReadBits(8)), // m_intraSubgroupPriority
                                [3] = new StormGameEventData(bitReader.ReadBits(replay.ReplayVersion.Major < 2 ? 9 : 6)), // m_count
                            };

                            addSubgroupsArray[i] = new StormGameEventData(subGroupStructure);
                        }

                        structure[1].StructureByIndex![2] = new StormGameEventData(addSubgroupsArray);

                        // m_addUnitTags
                        StormGameEventData[] addUnitTagsArray = new StormGameEventData[bitReader.ReadBits(6)];
                        for (int i = 0; i < addUnitTagsArray.Length; i++)
                        {
                            addUnitTagsArray[i] = new StormGameEventData(bitReader.ReadBits(32));
                        }

                        structure[1].StructureByIndex![3] = new StormGameEventData(addUnitTagsArray);

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SControlGroupUpdateEvent:
                        structure = new Dictionary<int, StormGameEventData>(3)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBits(4)), // m_controlGroupIndex
                        };

                        // m_controlGroupUpdate
                        if (replay.ReplayBuild < 37069)
                            structure[1] = new StormGameEventData(bitReader.ReadBits(2));
                        else
                            structure[1] = new StormGameEventData(bitReader.ReadBits(3));

                        structure[2] = MaskChoice(replay, ref bitReader); // m_mask

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SSelectionSyncCheckEvent:
                        structure = new Dictionary<int, StormGameEventData>(2)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBits(4)), // m_controlGroupId
                        };

                        Dictionary<int, StormGameEventData> selectionSyncDataStructure = new Dictionary<int, StormGameEventData>(6); // m_selectionSyncData

                        if (replay.ReplayVersion.Major < 2)
                        {
                            selectionSyncDataStructure[0] = new StormGameEventData(bitReader.ReadBits(9)); // m_count
                            selectionSyncDataStructure[1] = new StormGameEventData(bitReader.ReadBits(9)); // m_subgroupCount
                            selectionSyncDataStructure[2] = new StormGameEventData(bitReader.ReadBits(9)); // m_activeSubgroupIndex
                        }
                        else
                        {
                            selectionSyncDataStructure[0] = new StormGameEventData(bitReader.ReadBits(6)); // m_count
                            selectionSyncDataStructure[1] = new StormGameEventData(bitReader.ReadBits(6)); // m_subgroupCount
                            selectionSyncDataStructure[2] = new StormGameEventData(bitReader.ReadBits(5)); // m_activeSubgroupIndex
                        }

                        selectionSyncDataStructure[3] = new StormGameEventData(bitReader.ReadBits(32)); // m_unitTagsChecksum
                        selectionSyncDataStructure[4] = new StormGameEventData(bitReader.ReadBits(32)); // m_subgroupIndicesChecksum
                        selectionSyncDataStructure[5] = new StormGameEventData(bitReader.ReadBits(32)); // m_subgroupsChecksum

                        structure[1] = new StormGameEventData(selectionSyncDataStructure);

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.STriggerChatMessageEvent:
                        structure = new Dictionary<int, StormGameEventData>(1)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBlobAsString(10)), // m_chatMessage
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SDynamicButtonSwapEvent:
                        structure = new Dictionary<int, StormGameEventData>(3)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBits(32)), // m_unitTag
                            [1] = new StormGameEventData(bitReader.ReadBits(8)), // m_buttonSlotA
                            [2] = new StormGameEventData(bitReader.ReadBits(8)), // m_buttonSlotB
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SSetAbsoluteGameSpeedEvent:
                        structure = new Dictionary<int, StormGameEventData>(1)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBits(3)), // m_speed
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SAddAbsoluteGameSpeedEvent:
                        structure = new Dictionary<int, StormGameEventData>(1)
                        {
                            [0] = new StormGameEventData((int)(bitReader.ReadBits(8) - 128)), // m_delta
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.STriggerPingEvent:
                        structure = new Dictionary<int, StormGameEventData>(4)
                        {
                            [0] = new StormGameEventData(new Dictionary<int, StormGameEventData>(2) // m_point
                            {
                                [0] = new StormGameEventData((int)(bitReader.ReadBits(32) - 2147483648)), // x
                                [1] = new StormGameEventData((int)(bitReader.ReadBits(32) - 2147483648)), // y
                            }),
                            [1] = new StormGameEventData(bitReader.ReadBits(32)), // m_unit
                            [2] = new StormGameEventData(bitReader.ReadBoolean()), // m_pingedMinimap
                            [3] = new StormGameEventData((int)(bitReader.ReadBits(32) - 2147483648)), // m_option
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SBroadcastCheatEvent:
                        structure = new Dictionary<int, StormGameEventData>(2)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBlobAsString(10)), // m_verb
                            [1] = new StormGameEventData(bitReader.ReadBlobAsString(10)), // m_arguments
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SAllianceEvent:
                        structure = new Dictionary<int, StormGameEventData>(2)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBits(32)), // m_alliance
                            [1] = new StormGameEventData(bitReader.ReadBits(32)), // m_control
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SUnitClickEvent:
                        structure = new Dictionary<int, StormGameEventData>(1)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBits(32)), // m_unitTag
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SUnitHighlightEvent:
                        structure = new Dictionary<int, StormGameEventData>(2)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBits(32)), // m_unitTag
                            [1] = new StormGameEventData(bitReader.ReadBits(8)), // m_flags
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.STriggerReplySelectedEvent:
                        structure = new Dictionary<int, StormGameEventData>(2)
                        {
                            [0] = new StormGameEventData((int)(bitReader.ReadBits(32) - 2147483648)), // m_conversationId
                            [1] = new StormGameEventData((int)(bitReader.ReadBits(32) - 2147483648)), // m_replyId
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SHijackReplayGameEvent:
                        structure = new Dictionary<int, StormGameEventData>(2);

                        // m_userInfos
                        StormGameEventData[] userInfosArray = new StormGameEventData[bitReader.ReadBits(5)];
                        for (int i = 0; i < userInfosArray.Length; i++)
                        {
                            userInfosArray[i] = new StormGameEventData(new Dictionary<int, StormGameEventData>(6)
                            {
                                [0] = new StormGameEventData(bitReader.ReadBits(4)), // m_gameUserId
                                [1] = new StormGameEventData(bitReader.ReadBits(2)), // m_observe
                                [2] = new StormGameEventData(bitReader.ReadBlobAsString(8)), // m_name
                            });

                            // m_toonHandle
                            if (bitReader.ReadBoolean())
                            {
                                userInfosArray[i].StructureByIndex![3] = new StormGameEventData(bitReader.ReadBlobAsString(7));
                            }

                            // m_clanTag
                            if (bitReader.ReadBoolean())
                            {
                                userInfosArray[i].StructureByIndex![4] = new StormGameEventData(bitReader.ReadBlobAsString(8));
                            }

                            // m_clanLogo
                            if (bitReader.ReadBoolean())
                            {
                                userInfosArray[i].StructureByIndex![5] = new StormGameEventData(bitReader.ReadStringFromBytes(40));
                            }
                        }

                        structure[0] = new StormGameEventData(userInfosArray);
                        structure[1] = new StormGameEventData(bitReader.ReadBits(1)); // m_method

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.STriggerSoundLengthQueryEvent:
                        structure = new Dictionary<int, StormGameEventData>(2)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBits(32)), // m_soundHash
                            [1] = new StormGameEventData(bitReader.ReadBits(32)), // m_length
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.STriggerSoundOffsetEvent:
                        structure = new Dictionary<int, StormGameEventData>(1)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBits(32)), // m_sound
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.STriggerTransmissionOffsetEvent:
                        structure = new Dictionary<int, StormGameEventData>(2)
                        {
                            [0] = new StormGameEventData((int)(bitReader.ReadBits(32) - 2147483648)), // m_transmissionId
                            [1] = new StormGameEventData(bitReader.ReadBits(32)), // m_thread
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.STriggerTransmissionCompleteEvent:
                        structure = new Dictionary<int, StormGameEventData>(1)
                        {
                            [0] = new StormGameEventData((int)(bitReader.ReadBits(32) - 2147483648)), // m_transmissionId
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SCameraUpdateEvent:
                        structure = new Dictionary<int, StormGameEventData>(6);

                        // m_target
                        if (bitReader.ReadBoolean())
                        {
                            structure[0] = new StormGameEventData(new Dictionary<int, StormGameEventData>(2)
                            {
                                [0] = new StormGameEventData(bitReader.ReadBits(16)), // x
                                [1] = new StormGameEventData(bitReader.ReadBits(16)), // y
                            });
                        }

                        // m_distance
                        if (bitReader.ReadBoolean())
                        {
                            structure[1] = new StormGameEventData(bitReader.ReadBits(16));
                        }

                        // m_pitch
                        if (bitReader.ReadBoolean())
                        {
                            structure[2] = new StormGameEventData(bitReader.ReadBits(16));
                        }

                        // m_yaw
                        if (bitReader.ReadBoolean())
                        {
                            structure[3] = new StormGameEventData(bitReader.ReadBits(16));
                        }

                        // m_reason
                        if (bitReader.ReadBoolean())
                        {
                            structure[4] = new StormGameEventData((int)(bitReader.ReadBits(8) - 128));
                        }

                        structure[5] = new StormGameEventData(bitReader.ReadBoolean()); // m_follow

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.STriggerDialogControlEvent:
                        structure = new Dictionary<int, StormGameEventData>(3)
                        {
                            [0] = new StormGameEventData((int)(bitReader.ReadBits(32) - 2147483648)), // m_controlId
                            [1] = new StormGameEventData((int)(bitReader.ReadBits(32) - 2147483648)), // m_eventType
                        };

                        // m_eventData
                        switch (bitReader.ReadBits(3))
                        {
                            case 0: // None
                                structure[2] = new StormGameEventData(structureByIndex: null);
                                break;
                            case 1: // Checked
                                structure[2] = new StormGameEventData(bitReader.ReadBoolean());
                                break;
                            case 2: // ValueChanged
                                structure[2] = new StormGameEventData(bitReader.ReadBits(32));
                                break;
                            case 3: // SelectionChanged
                                structure[2] = new StormGameEventData((int)(bitReader.ReadBits(32) - 2147483648));
                                break;
                            case 4: // TextChanged
                                structure[2] = new StormGameEventData(bitReader.ReadBlobAsString(11));
                                break;
                            case 5: // MouseButton (old), MouseEvent
                                if (replay.ReplayBuild == 57547 || replay.ReplayBuild > 57589)
                                {
                                    structure[2] = new StormGameEventData(new Dictionary<int, StormGameEventData>(2) // MouseEvent
                                    {
                                        [0] = new StormGameEventData(bitReader.ReadBits(16)), // m_button
                                        [1] = new StormGameEventData(bitReader.ReadBits(16)), // m_metaKeyFlags
                                    });
                                }
                                else
                                {
                                    // MouseButton
                                    structure[2] = new StormGameEventData(bitReader.ReadBits(32));
                                }

                                break;
                            default:
                                throw new NotImplementedException();
                        }

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.STriggerSoundLengthSyncEvent:
                        StormGameEventData[] soundHashArray = new StormGameEventData[bitReader.ReadBits(7)];
                        for (int i = 0; i < soundHashArray.Length; i++)
                            soundHashArray[i] = new StormGameEventData(bitReader.ReadBits(32));

                        StormGameEventData[] mlengthArray = new StormGameEventData[bitReader.ReadBits(7)];
                        for (int i = 0; i < mlengthArray.Length; i++)
                            mlengthArray[i] = new StormGameEventData(bitReader.ReadBits(32));

                        structure = new Dictionary<int, StormGameEventData>(1)
                        {
                            [0] = new StormGameEventData(new Dictionary<int, StormGameEventData>(2) // m_syncInfo
                            {
                                [0] = new StormGameEventData(soundHashArray), // m_soundHash
                                [1] = new StormGameEventData(mlengthArray), // m_length
                            }),
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.STriggerConversationSkippedEvent:
                        structure = new Dictionary<int, StormGameEventData>(1)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBits(1)), // m_skipType
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.STriggerMouseClickedEvent:
                        structure = new Dictionary<int, StormGameEventData>(5)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBits(32)), // m_button
                            [1] = new StormGameEventData(bitReader.ReadBoolean()), // m_down
                            [2] = new StormGameEventData(new Dictionary<int, StormGameEventData>(2) // m_posUI
                            {
                                [0] = new StormGameEventData(bitReader.ReadBits(11)), // x
                                [1] = new StormGameEventData(bitReader.ReadBits(11)), // y
                            }),
                            [3] = new StormGameEventData(new Dictionary<int, StormGameEventData>(2) // m_posWorld
                            {
                                [0] = new StormGameEventData(bitReader.ReadBits(20)), // x
                                [1] = new StormGameEventData(bitReader.ReadBits(20)), // y
                                [2] = new StormGameEventData((int)(bitReader.ReadBits(32) - 2147483648)), // z
                            }),
                            [4] = new StormGameEventData((int)(bitReader.ReadBits(8) - 128)), // m_flags
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.STriggerMouseMovedEvent:
                        structure = new Dictionary<int, StormGameEventData>(3)
                        {
                            [0] = new StormGameEventData(new Dictionary<int, StormGameEventData>(2) // m_posUI
                            {
                                [0] = new StormGameEventData(bitReader.ReadBits(11)), // x
                                [1] = new StormGameEventData(bitReader.ReadBits(11)), // y
                            }),
                            [1] = new StormGameEventData(new Dictionary<int, StormGameEventData>(2) // m_posWorld
                            {
                                [0] = new StormGameEventData(bitReader.ReadBits(20)), // x
                                [1] = new StormGameEventData(bitReader.ReadBits(20)), // y
                                [2] = new StormGameEventData((int)(bitReader.ReadBits(32) - 2147483648)), // z
                            }),
                            [2] = new StormGameEventData((int)(bitReader.ReadBits(8) - 128)), // m_flags
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SAchievementAwardedEvent:
                        structure = new Dictionary<int, StormGameEventData>(1)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBits(16)), // m_achievementLink
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.STriggerHotkeyPressedEvent:
                        structure = new Dictionary<int, StormGameEventData>(2)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBits(32)), // m_hotkey
                            [1] = new StormGameEventData(bitReader.ReadBoolean()), // m_down
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.STriggerTargetModeUpdateEvent:
                        structure = new Dictionary<int, StormGameEventData>(3)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBits(16)), // m_abilLink
                            [1] = new StormGameEventData(bitReader.ReadBits(5)), // m_abilCmdIndex
                            [2] = new StormGameEventData((int)(bitReader.ReadBits(8) - 128)), // m_abilCmdIndex
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.STriggerSoundtrackDoneEvent:
                        structure = new Dictionary<int, StormGameEventData>(1)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBits(32)), // m_soundtrack
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.STriggerKeyPressedEvent:
                        structure = new Dictionary<int, StormGameEventData>(2)
                        {
                            [0] = new StormGameEventData((int)(bitReader.ReadBits(8) - 128)), // m_key
                            [1] = new StormGameEventData((int)(bitReader.ReadBits(8) - 128)), // m_flags
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.STriggerMovieFunctionEvent:
                        structure = new Dictionary<int, StormGameEventData>(1)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBlobAsString(7)), // m_functionName
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.STriggerCommandErrorEvent:
                        structure = new Dictionary<int, StormGameEventData>(2)
                        {
                            [0] = new StormGameEventData((int)(bitReader.ReadBits(32) - 2147483648)), // m_error
                        };

                        // m_abil
                        if (bitReader.ReadBoolean())
                        {
                            structure[1] = new StormGameEventData(new Dictionary<int, StormGameEventData>(3)
                            {
                                [0] = new StormGameEventData(bitReader.ReadBits(16)), // m_abilLink
                                [1] = new StormGameEventData(bitReader.ReadBits(5)), // m_abilCmdIndex
                            });

                            // m_abilCmdData
                            if (bitReader.ReadBoolean())
                            {
                                structure[1].StructureByIndex![2] = new StormGameEventData(bitReader.ReadBits(8));
                            }
                        }

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SDecrementGameTimeRemainingEvent:
                        structure = new Dictionary<int, StormGameEventData>(1)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBits(19)), // m_decrementMs
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.STriggerPortraitLoadedEvent:
                        structure = new Dictionary<int, StormGameEventData>(1)
                        {
                            [0] = new StormGameEventData((int)(bitReader.ReadBits(32) - 2147483648)), // m_portraitId
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.STriggerCustomDialogDismissedEvent:
                        structure = new Dictionary<int, StormGameEventData>(1)
                        {
                            [0] = new StormGameEventData((int)(bitReader.ReadBits(32) - 2147483648)), // m_result
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.STriggerGameMenuItemSelectedEvent:
                        structure = new Dictionary<int, StormGameEventData>(1)
                        {
                            [0] = new StormGameEventData((int)(bitReader.ReadBits(32) - 2147483648)), // m_gameMenuItemIndex
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.STriggerMouseWheelEvent:
                        structure = new Dictionary<int, StormGameEventData>(2)
                        {
                            [0] = new StormGameEventData((int)(bitReader.ReadBits(16) - 32768)), // m_wheelSpin
                            [1] = new StormGameEventData((int)(bitReader.ReadBits(8) - 128)), // m_flags
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.STriggerButtonPressedEvent:
                        structure = new Dictionary<int, StormGameEventData>(1)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBits(16)), // m_button
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.STriggerCutsceneBookmarkFiredEvent:
                        structure = new Dictionary<int, StormGameEventData>(2)
                        {
                            [0] = new StormGameEventData((int)(bitReader.ReadBits(32) - 2147483648)), // m_cutsceneId
                            [1] = new StormGameEventData(bitReader.ReadBlobAsString(7)), // m_bookmarkName
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.STriggerCutsceneEndSceneFiredEvent:
                        structure = new Dictionary<int, StormGameEventData>(1)
                        {
                            [0] = new StormGameEventData((int)(bitReader.ReadBits(32) - 2147483648)), // m_cutsceneId
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.STriggerCutsceneConversationLineEvent:
                        structure = new Dictionary<int, StormGameEventData>(3)
                        {
                            [0] = new StormGameEventData((int)(bitReader.ReadBits(32) - 2147483648)), // m_cutsceneId
                            [1] = new StormGameEventData(bitReader.ReadBlobAsString(7)), // m_conversationLine
                            [2] = new StormGameEventData(bitReader.ReadBlobAsString(7)), // m_altConversationLine
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.STriggerCutsceneConversationLineMissingEvent:
                        structure = new Dictionary<int, StormGameEventData>(2)
                        {
                            [0] = new StormGameEventData((int)(bitReader.ReadBits(32) - 2147483648)), // m_cutsceneId
                            [1] = new StormGameEventData(bitReader.ReadBlobAsString(7)), // m_conversationLine
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SGameUserLeaveEvent:
                        structure = new Dictionary<int, StormGameEventData>(1)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBits(5)),
                        };

                        // m_leaveReason
                        if (replay.ReplayBuild < 55929)
                            structure[0] = new StormGameEventData(bitReader.ReadBits(4));
                        else
                            structure[0] = new StormGameEventData(bitReader.ReadBits(5));

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SGameUserJoinEvent:
                        structure = new Dictionary<int, StormGameEventData>(7)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBits(2)), // m_observe
                            [1] = new StormGameEventData(bitReader.ReadBlobAsString(8)), // m_name
                        };

                        // m_toonHandle
                        if (bitReader.ReadBoolean())
                        {
                            structure[2] = new StormGameEventData(bitReader.ReadBlobAsString(7));
                        }

                        // m_clanTag
                        if (bitReader.ReadBoolean())
                        {
                            structure[3] = new StormGameEventData(bitReader.ReadBlobAsString(8));
                        }

                        // m_clanLogo
                        if (bitReader.ReadBoolean())
                        {
                            structure[4] = new StormGameEventData(bitReader.ReadStringFromBytes(40));
                        }

                        structure[5] = new StormGameEventData(bitReader.ReadBoolean()); // m_hijack

                        // m_hijackCloneGameUserId
                        if (bitReader.ReadBoolean())
                        {
                            structure[6] = new StormGameEventData(bitReader.ReadBits(4));
                        }

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SCommandManagerStateEvent:
                        structure = new Dictionary<int, StormGameEventData>(2)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBits(2)), // m_state
                        };

                        if (replay.ReplayBuild >= 33684)
                        {
                            // m_sequence
                            if (bitReader.ReadBoolean())
                            {
                                structure[1] = new StormGameEventData(bitReader.ReadBits(32) + 1);
                            }
                        }

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SCmdUpdateTargetPointEvent:
                        structure = new Dictionary<int, StormGameEventData>(2);

                        if (replay.ReplayBuild >= 40336)
                        {
                            // m_sequence
                            if (bitReader.ReadBoolean())
                            {
                                structure[0] = new StormGameEventData(bitReader.ReadBits(32) + 1);
                            }
                        }

                        if (replay.ReplayBuild >= 40336)
                            structure[1] = new StormGameEventData(PointXYZStucture(ref bitReader)); // m_target
                        else
                            structure[0] = new StormGameEventData(PointXYZStucture(ref bitReader)); // m_target

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SCmdUpdateTargetUnitEvent:
                        structure = new Dictionary<int, StormGameEventData>(2);

                        if (replay.ReplayBuild >= 33684)
                        {
                            // m_sequence
                            if (bitReader.ReadBoolean())
                            {
                                structure[0] = new StormGameEventData(bitReader.ReadBits(32) + 1);
                            }
                        }

                        // m_target
                        Dictionary<int, StormGameEventData> targetStructure = new Dictionary<int, StormGameEventData>(7);
                        targetStructure[0] = new StormGameEventData(bitReader.ReadBits(16)); // m_targetUnitFlags
                        targetStructure[1] = new StormGameEventData(bitReader.ReadBits(8)); // m_timer
                        targetStructure[2] = new StormGameEventData(bitReader.ReadBits(32)); // m_tag
                        targetStructure[3] = new StormGameEventData(bitReader.ReadBits(16)); // m_snapshotUnitLink

                        if (bitReader.ReadBoolean())
                            targetStructure[4] = new StormGameEventData(bitReader.ReadBits(4)); // m_snapshotControlPlayerId

                        if (bitReader.ReadBoolean())
                            targetStructure[5] = new StormGameEventData(bitReader.ReadBits(4)); // m_snapshotUpkeepPlayerId

                        targetStructure[6] = new StormGameEventData(PointXYZStucture(ref bitReader)); // m_snapshotPoint

                        structure[1] = new StormGameEventData(targetStructure);

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.STriggerAnimLengthQueryByNameEvent:
                        structure = new Dictionary<int, StormGameEventData>(3)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBits(16)), // m_queryId
                            [1] = new StormGameEventData(bitReader.ReadBits(32)), // m_lengthMs
                            [2] = new StormGameEventData(bitReader.ReadBits(32)), // m_finishGameLoop
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.STriggerAnimLengthQueryByPropsEvent:
                        structure = new Dictionary<int, StormGameEventData>(2)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBits(16)), // m_queryId
                            [1] = new StormGameEventData(bitReader.ReadBits(32)), // m_lengthMs
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.STriggerAnimOffsetEvent:
                        structure = new Dictionary<int, StormGameEventData>(1)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBits(16)), // m_animWaitQueryId
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SCatalogModifyEvent:
                        structure = new Dictionary<int, StormGameEventData>(4)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBits(8)), // m_catalog
                            [1] = new StormGameEventData(bitReader.ReadBits(16)), // m_entry
                            [2] = new StormGameEventData(bitReader.ReadBlobAsString(8)), // m_field
                            [3] = new StormGameEventData(bitReader.ReadBlobAsString(8)), // m_value
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SHeroTalentTreeSelectedEvent:
                        structure = new Dictionary<int, StormGameEventData>(1)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBits(32)), // m_index
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;
                    case StormGameEventType.SHeroTalentTreeSelectionPanelToggledEvent:
                        structure = new Dictionary<int, StormGameEventData>(1)
                        {
                            [0] = new StormGameEventData(bitReader.ReadBoolean()), // m_shown
                        };

                        gameEvent = new StormGameEvent(player, timeStamp, gameEventType, new StormGameEventData(structure));
                        break;

                    default:
                        throw new NotImplementedException();
                }

                bitReader.AlignToByte();

                if (gameEvent != null)
                    replay.GameEventsInternal.Add(gameEvent.Value);
            }
        }

        // ('_choice',[(0,2),{0:('None',86),1:('Mask',52),2:('OneIndices',100),3:('ZeroIndices',100)}]),  #101
        private static StormGameEventData MaskChoice(StormReplay replay, ref BitReader bitReader)
        {
            switch (bitReader.ReadBits(2))
            {
                case 0: // None
                    return new StormGameEventData(structureByIndex: null);
                case 1: // Mask
                    if (replay.ReplayVersion.Major < 2)
                        return new StormGameEventData(bitReader.ReadBitArray(9));
                    else
                        return new StormGameEventData(bitReader.ReadBitArray(6));
                case 2: // OneIndices
                case 3: // ZeroIndices
                    StormGameEventData[] indicesArray = new StormGameEventData[bitReader.ReadBits(6)];
                    for (int i = 0; i < indicesArray.Length; i++)
                        indicesArray[i] = new StormGameEventData(bitReader.ReadBits(5));

                    return new StormGameEventData(indicesArray);
                default:
                    throw new NotImplementedException();
            }
        }

        // ('_struct',[[('x',87,-3),('y',87,-2),('z',88,-1)]])
        private static Dictionary<int, StormGameEventData> PointXYZStucture(ref BitReader bitReader)
        {
            return new Dictionary<int, StormGameEventData>(3)
            {
                [0] = new StormGameEventData(bitReader.ReadBits(20)), // x
                [1] = new StormGameEventData(bitReader.ReadBits(20)), // y
                [2] = new StormGameEventData((int)(bitReader.ReadBits(32) - 2147483648)), // z
            };
        }
    }
}
