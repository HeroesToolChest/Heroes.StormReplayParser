using Heroes.StormReplayParser.MpqHeroesTool;
using Heroes.StormReplayParser.Player;
using Heroes.StormReplayParser.Replay;
using System;

namespace Heroes.StormReplayParser.MpqFiles
{
    internal static class ReplayInitData
    {
        public static string FileName { get; } = "replay.initData";

        public static void Parse(StormReplay replay, ReadOnlySpan<byte> source)
        {
            /* m_userInitialData section */

            BitReader.ResetIndex();
            BitReader.EndianType = EndianType.BigEndian;

            // all slots (16)
            uint playerListLength = source.ReadBits(5);

            for (int i = 0; i < playerListLength; i++)
            {
                string name = source.ReadBlobAsString(8); // m_name

                if (!string.IsNullOrEmpty(name))
                    replay.ClientListByUserID[i] = new StormPlayer { Name = name };

                if (source.ReadBoolean())
                    source.ReadBlobAsString(8); // m_clanTag

                if (source.ReadBoolean())
                    source.ReadBlobAsString(40); // m_clanLogo

                if (source.ReadBoolean())
                    source.ReadBits(8); // m_highestLeague

                if (source.ReadBoolean())
                    source.ReadBits(32); // m_combinedRaceLevels

                source.ReadUInt32Aligned(); // m_randomSeed (So far, always 0 in Heroes)

                if (source.ReadBoolean())
                    source.ReadBits(8); // m_racePreference

                if (source.ReadBoolean())
                    source.ReadBits(8); // m_teamPreference

                source.ReadBoolean(); // m_testMap
                source.ReadBoolean(); // m_testAuto
                source.ReadBoolean(); // m_examine
                source.ReadBoolean(); // m_customInterface

                source.ReadUInt32Aligned(); // m_testType

                source.ReadBits(2); // m_observe

                source.ReadBlobAsString(9); // m_hero - Currently Empty String
                source.ReadBlobAsString(9); // m_skin - Currently Empty String
                source.ReadBlobAsString(9); // m_mount - Currently Empty String

                if (replay.ReplayVersion.Major >= 2)
                {
                    source.ReadBlobAsString(9); // m_banner - Currently Empty String
                    source.ReadBlobAsString(9); // m_spray - Currently Empty String
                }

                source.ReadBlobAsString(7); // m_toonHandle - Currently Empty String
            }

            /* m_gameDescription section */

            replay.RandomValue = source.ReadBits(32); // m_randomValue

            source.ReadBlobAsString(10); // m_gameCacheName - "Dflt"

            // m_gameOptions
            source.ReadBoolean(); // m_lockTeams
            source.ReadBoolean(); // m_teamsTogether
            source.ReadBoolean(); // m_advancedSharedControl
            source.ReadBoolean(); // m_randomRaces
            source.ReadBoolean(); // m_battleNet
            source.ReadBoolean(); // m_amm
            source.ReadBoolean(); // m_competitive
            source.ReadBoolean(); // m_practice
            source.ReadBoolean(); // m_cooperative
            source.ReadBoolean(); // m_noVictoryOrDefeat
            source.ReadBoolean(); // m_heroDuplicatesAllowed
            source.ReadBits(2); // m_fog
            source.ReadBits(2); // m_observers
            source.ReadBits(2); // m_userDifficulty
            source.ReadUInt64Unaligned(); // m_clientDebugFlags

            if (replay.ReplayBuild >= 43905 && source.ReadBoolean())
            {
                replay.GameMode = source.ReadInt32Unaligned() switch // m_ammId
                {
                    50001 => GameMode.QuickMatch,
                    50021 => GameMode.Cooperative,
                    50031 => GameMode.Brawl,
                    50041 => GameMode.Practice,
                    50051 => GameMode.UnrankedDraft,
                    50061 => GameMode.HeroLeague,
                    50071 => GameMode.TeamLeague,
                    50091 => GameMode.StormLeague,

                    _ => GameMode.Unknown,
                };
            }

            source.ReadBits(3); // m_gameSpeed
            source.ReadBits(3); // m_gameType

            if (source.ReadBits(5) < 10 && replay.GameMode != GameMode.Brawl) // m_maxUsers
                replay.GameMode = GameMode.TryMe; // or it could be a custom

            source.ReadBits(5); // m_maxObservers
            source.ReadBits(5); // m_maxPlayers
            source.ReadBits(4); // m_maxTeams
            source.ReadBits(6); // m_maxColors
            source.ReadBits(8); // m_maxRaces

            // Max Controls
            if (replay.ReplayBuild < 59279) // m_maxControls
                source.ReadBits(8);
            else
                source.ReadBits(4);

            // m_mapSizeX and m_mapSizeY
            replay.MapInfo.MapSize = new Point(source.ReadBits(8), source.ReadBits(8));

            source.ReadBits(32); // m_mapFileSyncChecksum
            source.ReadBlobAsString(11); // m_mapFileName
            source.ReadBlobAsString(8); // m_mapAuthorName
            source.ReadBits(32); // m_modFileSyncChecksum

            // m_slotDescriptions
            uint slotDescriptionLength = source.ReadBits(5);
            for (int i = 0; i < slotDescriptionLength; i++)
            {
                source.ReadBitArray((int)source.ReadBits(6)); // m_allowedColors
                source.ReadBitArray((int)source.ReadBits(8)); // m_allowedRaces
                source.ReadBitArray((int)source.ReadBits(6)); // m_allowedDifficulty

                // m_allowedControls
                if (replay.ReplayBuild < 59279)
                    source.ReadBitArray((int)source.ReadBits(8));
                else
                    source.ReadBitArray((int)source.ReadBits(4));

                source.ReadBitArray((int)source.ReadBits(2)); // m_allowedObserveTypes
                source.ReadBitArray((int)source.ReadBits(7)); // m_allowedAIBuilds
            }

            source.ReadBits(6); // m_defaultDifficulty
            source.ReadBits(7); // m_defaultAIBuild

            // m_cacheHandles
            uint cacheHandlesLength = source.ReadBits(6);
            for (int i = 0; i < cacheHandlesLength; i++)
                source.ReadAlignedBytes(40);

            source.ReadBoolean(); // m_hasExtensionMod
            source.ReadBoolean(); // m_isBlizzardMap
            source.ReadBoolean(); // m_isPremadeFFA
            source.ReadBoolean(); // m_isCoopMode

            /* m_lobbyState section */

            source.ReadBits(3); // m_phase
            source.ReadBits(5); // m_maxUsers
            source.ReadBits(5); // m_maxObservers

            // m_slots
            uint slotsLength = source.ReadBits(5);

            for (int i = 0; i < slotsLength; i++)
            {
                int? userId = null;
                int? workingSetSlotID = null;

                source.ReadBits(8); // m_control

                if (source.ReadBoolean())
                    userId = (int)source.ReadBits(4); // m_userId

                source.ReadBits(4); // m_teamId

                if (source.ReadBoolean())
                    source.ReadBits(5); // m_colorPref
                if (source.ReadBoolean())
                    source.ReadBits(8); // m_racePref

                source.ReadBits(6); // m_difficulty
                source.ReadBits(7); // m_aiBuild
                source.ReadBits(7); // m_handicap

                uint observerStatus = source.ReadBits(2); // m_observe

                source.ReadBits(32); // m_logoIndex

                string heroId = source.ReadBlobAsString(9); // m_hero (heroId)

                string skinAndSkinTint = source.ReadBlobAsString(9); // m_skin
                string mountAndMountTint = source.ReadBlobAsString(9); // m_mount

                // m_artifacts
                if (replay.ReplayBuild < 65579 || replay.ReplayBuild == 65617 || replay.ReplayBuild == 65654)
                {
                    uint artifactsLength = source.ReadBits(4);
                    for (uint j = 0; j < artifactsLength; j++)
                        source.ReadBlobAsString(9);
                }

                if (source.ReadBoolean())
                    workingSetSlotID = (int)source.ReadBits(8); // m_workingSetSlotId

                if (userId.HasValue && workingSetSlotID.HasValue)
                {
                    if (replay.ClientListByWorkingSetSlotID[workingSetSlotID.Value] != null)
                        replay.ClientListByUserID[userId.Value] = replay.ClientListByWorkingSetSlotID[workingSetSlotID.Value];

                    if (observerStatus == 1)
                        replay.ClientListByUserID[userId.Value].PlayerType = PlayerType.Observer;

                    replay.ClientListByUserID[userId.Value].PlayerHero.HeroId = heroId;
                    replay.ClientListByUserID[userId.Value].PlayerLoadout.SkinAndSkinTint = skinAndSkinTint;
                    replay.ClientListByUserID[userId.Value].PlayerLoadout.MountAndMountTint = mountAndMountTint;
                }

                // m_rewards
                uint rewardsLength = source.ReadBits(17);
                for (uint j = 0; j < rewardsLength; j++)
                    source.ReadBits(32);

                source.ReadBlobAsString(7); // m_toonHandle

                // m_licenses
                if (replay.ReplayBuild < 49582 || replay.ReplayBuild == 49838)
                {
                    uint licensesLength = source.ReadBits(9);
                    for (uint j = 0; j < licensesLength; j++)
                        source.ReadBits(32);
                }

                if (source.ReadBoolean())
                    source.ReadBits(4); // m_tandemLeaderUserId

                if (replay.ReplayBuild <= 41504)
                {
                    source.ReadBlobAsString(9); // m_commander - Empty string

                    source.ReadBits(32); // m_commanderLevel - So far, always 0
                }

                if (source.ReadBoolean() && userId.HasValue) // m_hasSilencePenalty
                    replay.ClientListByUserID[userId.Value].IsSilenced = true;

                if (replay.ReplayBuild >= 61718 && source.ReadBoolean() && userId.HasValue) // m_hasVoiceSilencePenalty
                    replay.ClientListByUserID[userId.Value].IsVoiceSilenced = true;

                if (replay.ReplayBuild >= 66977 && source.ReadBoolean() && userId.HasValue) // m_isBlizzardStaff
                    replay.ClientListByUserID[userId.Value].IsBlizzardStaff = true;

                if (replay.ReplayBuild >= 69947 && source.ReadBoolean() && userId.HasValue) // m_hasActiveBoost
                    replay.ClientListByUserID[userId.Value].HasActiveBoost = true;

                if (replay.ReplayVersion.Major >= 2)
                {
                    string banner = source.ReadBlobAsString(9); // m_banner
                    if (userId.HasValue)
                        replay.ClientListByUserID[userId.Value].PlayerLoadout.Banner = banner;

                    string spray = source.ReadBlobAsString(9); // m_spray
                    if (userId.HasValue)
                        replay.ClientListByUserID[userId.Value].PlayerLoadout.Spray = spray;

                    string announcer = source.ReadBlobAsString(9); // m_announcerPack
                    if (userId.HasValue)
                        replay.ClientListByUserID[userId.Value].PlayerLoadout.AnnouncerPack = announcer;

                    string voiceLine = source.ReadBlobAsString(9); // m_voiceLine
                    if (userId.HasValue)
                        replay.ClientListByUserID[userId.Value].PlayerLoadout.VoiceLine = voiceLine;

                    // m_heroMasteryTiers
                    if (replay.ReplayBuild >= 52561)
                    {
                        uint heroMasteryTiersLength = source.ReadBits(10);
                        for (int j = 0; j < heroMasteryTiersLength; j++)
                        {
                            BitReader.EndianType = EndianType.LittleEndian;
                            string heroAttributeName = source.ReadStringFromBits(32);
                            BitReader.EndianType = EndianType.BigEndian;

                            int tier = (int)source.ReadBits(8); // m_tier

                            if (userId.HasValue)
                            {
                                replay.ClientListByUserID[userId.Value].HeroMasteryTiersInternal.Add(new HeroMasteryTier()
                                {
                                    HeroAttributeId = heroAttributeName,
                                    TierLevel = tier,
                                });
                            }
                        }
                    }
                }
            }

            if (source.ReadBits(32) != replay.RandomValue) // m_randomSeed
                throw new StormParseException("Random seed values in replayInitData did not match.");

            if (source.ReadBoolean())
                source.ReadBits(4); // m_hostUserId

            source.ReadBoolean(); // m_isSinglePlayer
            source.ReadBits(8); // m_pickedMapTag - So far, always 0
            source.ReadBits(32); // m_gameDuration - So far, always 0
            source.ReadBits(6); // m_defaultDifficulty
            source.ReadBits(7); // m_defaultAIBuild
        }
    }
}