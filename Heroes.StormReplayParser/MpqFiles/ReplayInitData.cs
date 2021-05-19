using Heroes.StormReplayParser.MpqHeroesTool;
using Heroes.StormReplayParser.Player;
using Heroes.StormReplayParser.Replay;
using System;
using System.Linq;

namespace Heroes.StormReplayParser.MpqFiles
{
    internal static class ReplayInitData
    {
        public static string FileName { get; } = "replay.initData";

        public static void Parse(StormReplay replay, ReadOnlySpan<byte> source)
        {
            BitReader bitReader = new BitReader(source, EndianType.BigEndian);

            /* m_userInitialData section */

            // all slots (16)
            uint playerListLength = bitReader.ReadBits(5);

            for (int i = 0; i < playerListLength; i++)
            {
                string name = bitReader.ReadBlobAsString(8); // m_name

                if (!string.IsNullOrEmpty(name))
                    replay.ClientListByUserID[i] = new StormPlayer { Name = name };

                if (bitReader.ReadBoolean())
                    bitReader.ReadBlobAsString(8); // m_clanTag

                if (bitReader.ReadBoolean())
                    bitReader.ReadBlobAsString(40); // m_clanLogo

                if (bitReader.ReadBoolean())
                    bitReader.ReadBits(8); // m_highestLeague

                if (bitReader.ReadBoolean())
                    bitReader.ReadBits(32); // m_combinedRaceLevels

                bitReader.ReadUInt32Aligned(); // m_randomSeed (So far, always 0 in Heroes)

                if (bitReader.ReadBoolean())
                    bitReader.ReadBits(8); // m_racePreference

                if (bitReader.ReadBoolean())
                    bitReader.ReadBits(8); // m_teamPreference

                bitReader.ReadBoolean(); // m_testMap
                bitReader.ReadBoolean(); // m_testAuto
                bitReader.ReadBoolean(); // m_examine
                bitReader.ReadBoolean(); // m_customInterface

                bitReader.ReadUInt32Aligned(); // m_testType

                bitReader.ReadBits(2); // m_observe

                bitReader.ReadBlobAsString(9); // m_hero - Currently Empty String
                bitReader.ReadBlobAsString(9); // m_skin - Currently Empty String
                bitReader.ReadBlobAsString(9); // m_mount - Currently Empty String

                if (replay.ReplayVersion.Major >= 2)
                {
                    bitReader.ReadBlobAsString(9); // m_banner - Currently Empty String
                    bitReader.ReadBlobAsString(9); // m_spray - Currently Empty String
                }

                bitReader.ReadBlobAsString(7); // m_toonHandle - Currently Empty String
            }

            /* m_gameDescription section */

            replay.RandomValue = bitReader.ReadBits(32); // m_randomValue

            bitReader.ReadBlobAsString(10); // m_gameCacheName - "Dflt"

            // m_gameOptions
            bitReader.ReadBoolean(); // m_lockTeams
            bitReader.ReadBoolean(); // m_teamsTogether
            bitReader.ReadBoolean(); // m_advancedSharedControl
            bitReader.ReadBoolean(); // m_randomRaces
            bitReader.ReadBoolean(); // m_battleNet
            bitReader.ReadBoolean(); // m_amm
            bitReader.ReadBoolean(); // m_competitive
            bitReader.ReadBoolean(); // m_practice
            bitReader.ReadBoolean(); // m_cooperative
            bitReader.ReadBoolean(); // m_noVictoryOrDefeat
            bitReader.ReadBoolean(); // m_heroDuplicatesAllowed
            bitReader.ReadBits(2); // m_fog
            bitReader.ReadBits(2); // m_observers
            bitReader.ReadBits(2); // m_userDifficulty
            bitReader.ReadUInt64Unaligned(); // m_clientDebugFlags

            if (replay.ReplayBuild >= 43905 && bitReader.ReadBoolean())
            {
                replay.GameMode = bitReader.ReadInt32Unaligned() switch // m_ammId
                {
                    50001 => StormGameMode.QuickMatch,
                    50021 => StormGameMode.Cooperative,
                    50031 => StormGameMode.Brawl,
                    50041 => StormGameMode.Practice,
                    50051 => StormGameMode.UnrankedDraft,
                    50061 => StormGameMode.HeroLeague,
                    50071 => StormGameMode.TeamLeague,
                    50091 => StormGameMode.StormLeague,
                    50101 => StormGameMode.ARAM,

                    _ => StormGameMode.Unknown,
                };
            }

            bitReader.ReadBits(3); // m_gameSpeed
            bitReader.ReadBits(3); // m_gameType

            if (bitReader.ReadBits(5) < 10 && replay.GameMode != StormGameMode.Brawl) // m_maxUsers
                replay.GameMode = StormGameMode.TryMe; // or it could be a custom

            bitReader.ReadBits(5); // m_maxObservers
            bitReader.ReadBits(5); // m_maxPlayers
            bitReader.ReadBits(4); // m_maxTeams
            bitReader.ReadBits(6); // m_maxColors
            bitReader.ReadBits(8); // m_maxRaces

            // Max Controls
            if (replay.ReplayBuild < 59279) // m_maxControls
                bitReader.ReadBits(8);
            else
                bitReader.ReadBits(4);

            // m_mapSizeX and m_mapSizeY
            replay.MapInfo.MapSize = new Point(bitReader.ReadBits(8), bitReader.ReadBits(8));

            bitReader.ReadBits(32); // m_mapFileSyncChecksum
            bitReader.ReadBlobAsString(11); // m_mapFileName
            bitReader.ReadBlobAsString(8); // m_mapAuthorName
            bitReader.ReadBits(32); // m_modFileSyncChecksum

            // m_slotDescriptions
            uint slotDescriptionLength = bitReader.ReadBits(5);
            for (int i = 0; i < slotDescriptionLength; i++)
            {
                bitReader.ReadBitArray(bitReader.ReadBits(6)); // m_allowedColors
                bitReader.ReadBitArray(bitReader.ReadBits(8)); // m_allowedRaces
                bitReader.ReadBitArray(bitReader.ReadBits(6)); // m_allowedDifficulty

                // m_allowedControls
                if (replay.ReplayBuild < 59279)
                    bitReader.ReadBitArray(bitReader.ReadBits(8));
                else
                    bitReader.ReadBitArray(bitReader.ReadBits(4));

                bitReader.ReadBitArray(bitReader.ReadBits(2)); // m_allowedObserveTypes
                bitReader.ReadBitArray(bitReader.ReadBits(7)); // m_allowedAIBuilds
            }

            bitReader.ReadBits(6); // m_defaultDifficulty
            bitReader.ReadBits(7); // m_defaultAIBuild

            // m_cacheHandles
            uint cacheHandlesLength = bitReader.ReadBits(6);
            for (int i = 0; i < cacheHandlesLength; i++)
                bitReader.ReadAlignedBytes(40);

            bitReader.ReadBoolean(); // m_hasExtensionMod
            bitReader.ReadBoolean(); // m_isBlizzardMap
            bitReader.ReadBoolean(); // m_isPremadeFFA
            bitReader.ReadBoolean(); // m_isCoopMode

            if (replay.ReplayBuild >= 85027)
            {
                bitReader.ReadBoolean(); // m_isRandomTestValue

                // m_disabledHeroList
                uint disabledHeroListLength = bitReader.ReadBits(10);

                bitReader.EndianType = EndianType.LittleEndian;

                for (int i = 0; i < disabledHeroListLength; i++)
                {
                    string disabledHero = bitReader.ReadStringFromBits(32);
                }

                bitReader.EndianType = EndianType.BigEndian;
            }

            /* m_lobbyState section */

            bitReader.ReadBits(3); // m_phase
            bitReader.ReadBits(5); // m_maxUsers
            bitReader.ReadBits(5); // m_maxObservers

            // m_slots
            uint slotsLength = bitReader.ReadBits(5);

            for (int i = 0; i < slotsLength; i++)
            {
                uint? userId = null;
                uint? workingSetSlotID = null;

                uint control = bitReader.ReadBits(8); // m_control

                if (bitReader.ReadBoolean())
                    userId = bitReader.ReadBits(4); // m_userId

                bitReader.ReadBits(4); // m_teamId

                if (bitReader.ReadBoolean())
                    bitReader.ReadBits(5); // m_colorPref
                if (bitReader.ReadBoolean())
                    bitReader.ReadBits(8); // m_racePref

                bitReader.ReadBits(6); // m_difficulty
                bitReader.ReadBits(7); // m_aiBuild
                bitReader.ReadBits(7); // m_handicap

                uint observerStatus = bitReader.ReadBits(2); // m_observe

                bitReader.ReadBits(32); // m_logoIndex

                string heroId = bitReader.ReadBlobAsString(9); // m_hero (heroId)

                string skinAndSkinTint = bitReader.ReadBlobAsString(9); // m_skin
                string mountAndMountTint = bitReader.ReadBlobAsString(9); // m_mount

                // m_artifacts
                if (replay.ReplayBuild < 65579 || replay.ReplayBuild == 65617 || replay.ReplayBuild == 65654)
                {
                    uint artifactsLength = bitReader.ReadBits(4);
                    for (uint j = 0; j < artifactsLength; j++)
                        bitReader.ReadBlobAsString(9);
                }

                if (bitReader.ReadBoolean())
                    workingSetSlotID = bitReader.ReadBits(8); // m_workingSetSlotId

                // m_rewards
                uint rewardsLength = bitReader.ReadBits(17);
                for (uint j = 0; j < rewardsLength; j++)
                    bitReader.ReadBits(32);

                string toonHandle = bitReader.ReadBlobAsString(7); // m_toonHandle

                if (userId.HasValue && workingSetSlotID.HasValue)
                {
                    if (replay.ClientListByWorkingSetSlotID[workingSetSlotID.Value] != null)
                    {
                        replay.ClientListByUserID[userId.Value] = replay.ClientListByWorkingSetSlotID[workingSetSlotID.Value];
                    }
                    else if (!string.IsNullOrEmpty(toonHandle) && observerStatus == 0)
                    {
                        replay.ClientListByUserID[userId.Value] = replay.Players.FirstOrDefault(x => x.ToonHandle?.ToString() == toonHandle)
                            ?? throw new StormParseException($"Unable to set {nameof(replay.ClientListByUserID)}, could not find player with a toonhandle of {toonHandle}");
                    }
                    else if (observerStatus == 0)
                    {
                        throw new StormParseException($"Unable to set {nameof(replay.ClientListByUserID)}, no toonhandle");
                    }

                    if (observerStatus > 0)
                        replay.ClientListByUserID[userId.Value].PlayerType = PlayerType.Observer;

                    if (replay.ClientListByUserID[userId.Value].PlayerType != PlayerType.Observer)
                    {
                        replay.ClientListByUserID[userId.Value].PlayerHero ??= new PlayerHero();
                        replay.ClientListByUserID[userId.Value].PlayerHero!.HeroId = heroId;
                        replay.ClientListByUserID[userId.Value].PlayerLoadout.SkinAndSkinTint = skinAndSkinTint;
                        replay.ClientListByUserID[userId.Value].PlayerLoadout.MountAndMountTint = mountAndMountTint;
                    }
                }

                // m_licenses
                if (replay.ReplayBuild < 49582 || replay.ReplayBuild == 49838)
                {
                    uint licensesLength = bitReader.ReadBits(9);
                    for (uint j = 0; j < licensesLength; j++)
                        bitReader.ReadBits(32);
                }

                if (bitReader.ReadBoolean())
                    bitReader.ReadBits(4); // m_tandemLeaderUserId

                if (replay.ReplayBuild <= 41504)
                {
                    bitReader.ReadBlobAsString(9); // m_commander - Empty string

                    bitReader.ReadBits(32); // m_commanderLevel - So far, always 0
                }

                if (bitReader.ReadBoolean() && userId.HasValue) // m_hasSilencePenalty
                    replay.ClientListByUserID[userId.Value].IsSilenced = true;

                if (replay.ReplayBuild >= 61718 && bitReader.ReadBoolean() && userId.HasValue) // m_hasVoiceSilencePenalty
                    replay.ClientListByUserID[userId.Value].IsVoiceSilenced = true;

                if (replay.ReplayBuild >= 66977 && bitReader.ReadBoolean() && userId.HasValue) // m_isBlizzardStaff
                    replay.ClientListByUserID[userId.Value].IsBlizzardStaff = true;

                if (replay.ReplayBuild >= 69947 && bitReader.ReadBoolean() && userId.HasValue) // m_hasActiveBoost
                    replay.ClientListByUserID[userId.Value].HasActiveBoost = true;

                if (replay.ReplayVersion.Major >= 2)
                {
                    string banner = bitReader.ReadBlobAsString(9); // m_banner
                    if (userId.HasValue)
                        replay.ClientListByUserID[userId.Value].PlayerLoadout.Banner = banner;

                    string spray = bitReader.ReadBlobAsString(9); // m_spray
                    if (userId.HasValue)
                        replay.ClientListByUserID[userId.Value].PlayerLoadout.Spray = spray;

                    string announcer = bitReader.ReadBlobAsString(9); // m_announcerPack
                    if (userId.HasValue)
                        replay.ClientListByUserID[userId.Value].PlayerLoadout.AnnouncerPack = announcer;

                    string voiceLine = bitReader.ReadBlobAsString(9); // m_voiceLine
                    if (userId.HasValue)
                        replay.ClientListByUserID[userId.Value].PlayerLoadout.VoiceLine = voiceLine;

                    // m_heroMasteryTiers
                    if (replay.ReplayBuild >= 52561)
                    {
                        uint heroMasteryTiersLength = bitReader.ReadBits(10);
                        for (int j = 0; j < heroMasteryTiersLength; j++)
                        {
                            bitReader.EndianType = EndianType.LittleEndian;
                            string heroAttributeName = bitReader.ReadStringFromBits(32);
                            bitReader.EndianType = EndianType.BigEndian;

                            int tier = (int)bitReader.ReadBits(8); // m_tier

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

            if (bitReader.ReadBits(32) != replay.RandomValue) // m_randomSeed
                throw new StormParseException("Random seed values in replayInitData did not match.");

            if (bitReader.ReadBoolean())
                bitReader.ReadBits(4); // m_hostUserId

            bitReader.ReadBoolean(); // m_isSinglePlayer
            bitReader.ReadBits(8); // m_pickedMapTag - So far, always 0
            bitReader.ReadBits(32); // m_gameDuration - So far, always 0
            bitReader.ReadBits(6); // m_defaultDifficulty
            bitReader.ReadBits(7); // m_defaultAIBuild
        }
    }
}