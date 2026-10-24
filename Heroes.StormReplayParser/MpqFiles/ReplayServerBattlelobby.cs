using System.Runtime.InteropServices;
using System.Text.Json;

namespace Heroes.StormReplayParser.MpqFiles;

internal static class ReplayServerBattlelobby
{
    private const string _exceptionHeader = "battlelobby";

    public static string FileName { get; } = "replay.server.battlelobby";

    public static void Parse(StormReplayPregame replay, ReadOnlySpan<byte> source, bool pregameMode = false, bool shouldParseLicenseCatalog = false)
    {
        // just return if too old
        if (replay.ReplayBuild <= 61718)
            return;

        for (int i = 0; i < 16; i++)
            replay.ClientListByWorkingSetSlotID[i] = new();

        Dictionary<ReplayAttributeEventType, StormBattleLobbyAttribute> lobbyAttributesByAttributeEventType = [];
        List<StormS2mFiles> s2mvFiles = [];
        List<string> battleNetCachePaths = [];

        BitReader bitReader = new(source, EndianType.BigEndian);

        uint dependenciesLength = bitReader.ReadBits(6);

        for (int i = 0; i < dependenciesLength; i++)
        {
            battleNetCachePaths.Add(bitReader.ReadBlobAsString(10));
        }

        // repeat s2ma cache handles for the above
        uint s2maCacheHandleLength = bitReader.ReadBits(6);

        for (int i = 0; i < s2maCacheHandleLength; i++)
        {
            _ = new StormS2mFiles(ref bitReader);
        }

        /* Attribute section */

        uint count = bitReader.ReadBits(9); // total number of attributes

        for (int i = 0; i < count; i++)
        {
            bitReader.ReadInt32Unaligned(); // namespace

            StormBattleLobbyAttribute stormBattleLobbyAttribute = new((ReplayAttributeEventType)bitReader.ReadInt32Unaligned());

            int attributeCount = (int)bitReader.ReadBits(12);

            for (int j = 0; j < attributeCount; j++)
            {
                StormBattleLobbyAttributeValue stormBattleLobbyAttributeValue = new()
                {
                    Value = bitReader.ReadStringFromUnalignedBytes(4), // attribute value
                };

                // first
                if (bitReader.ReadBoolean())
                {
                    // f32
                    if (bitReader.ReadBoolean())
                    {
                        bitReader.ReadBits(32);
                    }

                    bitReader.ReadBits(6); // NewField6
                    stormBattleLobbyAttributeValue.FirstId = bitReader.ReadInt16Unaligned(); // word - id for s2ml xml
                }

                // second
                if (bitReader.ReadBoolean())
                {
                    // f32
                    if (bitReader.ReadBoolean())
                    {
                        bitReader.ReadBits(32);
                    }

                    bitReader.ReadBits(6); // NewField6
                    stormBattleLobbyAttributeValue.SecondId = bitReader.ReadInt16Unaligned(); // word - id for s2ml xml
                }

                // f70
                if (bitReader.ReadBoolean())
                {
                    bitReader.ReadBits(6); // NewField6
                    bitReader.ReadBits(32); // NewField32
                    bitReader.ReadBits(16); // NewField16
                    bitReader.ReadBits(16); // NewField16
                }

                bitReader.ReadBitArray(1); // f1
                bitReader.ReadBitArray(1); // f1
                bitReader.ReadBitArray(1); // f1

                stormBattleLobbyAttribute.Values.Add(stormBattleLobbyAttributeValue);
            }

            // EnabledWithAttribs
            if (bitReader.ReadBoolean())
            {
                bitReader.ReadBitArray(25); // NewField25

                uint typeCount = bitReader.ReadBits(5);

                for (int x = 0; x < typeCount; x++)
                {
                    bitReader.ReadBitArray(5);

                    bitReader.ReadInt32Unaligned(); // namespace

                    stormBattleLobbyAttribute.EnabledEventType = (ReplayAttributeEventType)bitReader.ReadInt32Unaligned();

                    int enabledAttributeCount = (int)bitReader.ReadBits(12);

                    for (int j = 0; j < enabledAttributeCount; j++)
                    {
                        StormBattleLobbyEnabledAttributeValue stormBattleLobbyEnabledAttributeValue = new()
                        {
                            Value = bitReader.ReadStringFromUnalignedBytes(4), // attribute value
                        };

                        stormBattleLobbyAttribute.EnabledValues.Add(stormBattleLobbyEnabledAttributeValue);
                    }
                }
            }

            // _end
            bitReader.ReadBitArray(5); // NewField5
            bitReader.ReadBitArray(5); // NewField5
            bitReader.ReadBitArray(5); // NewField5

            bitReader.ReadBits(32); // newfield

            uint a170Length = bitReader.ReadBits(8);
            for (int y = 0; y < a170Length; y++)
            {
                bitReader.ReadBitArray(5); // b5
                bitReader.ReadBitArray(165); // b165
                bitReader.ReadBitArray(15); // b15
            }

            // NewField
            bitReader.ReadBitArray(11); // b11
            bitReader.ReadBits(1); // f1
            bitReader.ReadBits(9); // b9

            if (!lobbyAttributesByAttributeEventType.TryAdd(stormBattleLobbyAttribute.EventType, stormBattleLobbyAttribute))
                throw new StormParseException($"{_exceptionHeader}: duplicate attribute");
        }

        /* Player attribute selections */

        bitReader.ReadBitArray(30); // NewField
        bitReader.ReadBits(16); // NewField
        bitReader.ReadBitArray(81); // NewField

        count = bitReader.ReadBits(9); // attribute count should be the same as previous

        for (int i = 0; i < count; i++)
        {
            bitReader.ReadInt32Unaligned(); // namespace

            ReplayAttributeEventType attributeEventType = (ReplayAttributeEventType)bitReader.ReadInt32Unaligned();

            PlayerSelectedAttributeChoice(ref bitReader, replay, lobbyAttributesByAttributeEventType[attributeEventType]);
        }

        // the selections are not in attribute id order, so this can only be done once they have all been read
        SetPlayerTeams(replay);

        /* Local/locales section */

        uint s2mvCacheHandlesLength = bitReader.ReadBits(6);

        for (int i = 0; i < s2mvCacheHandlesLength; i++)
        {
            s2mvFiles.Add(new StormS2mFiles(ref bitReader));
        }

        uint localesLength = bitReader.ReadBits(5); // number of locales

        for (int i = 0; i < localesLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4);

            uint s2mlSize = bitReader.ReadBits(6); // number of s2ml in locale

            for (int j = 0; j < s2mlSize; j++)
            {
                _ = new StormS2mFiles(ref bitReader);
            }
        }

        bitReader.ReadBitArray(128); // fil

        uint usedModsLength = bitReader.ReadBits(5);

        for (int i = 0; i < usedModsLength; i++)
        {
            // ModId
            bitReader.ReadBits(32); // id
            bitReader.ReadBits(32); // ofs

            // Priority
            bitReader.ReadBits(4);
        }

        uint mods2Length = bitReader.ReadBits(7);

        for (int i = 0; i < mods2Length; i++)
        {
            // ModId
            bitReader.ReadBits(32); // id
            bitReader.ReadBits(32); // ofs

            // n16
            bitReader.ReadBits(16); // n16
            bitReader.ReadBits(16); // n16
            bitReader.ReadBits(24); // n24
            bitReader.ReadBits(7); // n7
            bitReader.ReadBits(15); // n15

            bitReader.ReadBitArray(2); // f2
            bitReader.ReadBitArray(74); // fil

            uint s2mvCacheHandlesLength2 = bitReader.ReadBits(6);

            for (int j = 0; j < s2mvCacheHandlesLength2; j++)
            {
                _ = new StormS2mFiles(ref bitReader);
            }

            bitReader.ReadBitArray(1); // flag

            // string toon
            bitReader.ReadBits(8); // region
            bitReader.ReadStringFromUnalignedBytes(4); // programId
            bitReader.ReadBits(32); // realm
            bitReader.ReadBlobAsString(7, 2);  // m_globalName

            // toon
            bitReader.ReadBits(8); // region
            bitReader.ReadStringFromUnalignedBytes(4); // programId
            bitReader.ReadBits(32); // realm
            bitReader.ReadLongBits(64); // id

            // trttt

            // b32
            if (bitReader.ReadBoolean())
            {
                bitReader.ReadBitArray(32);
            }

            // b23
            if (bitReader.ReadBoolean())
            {
                bitReader.ReadBitArray(23);
            }

            bitReader.ReadBitArray(1); // b1

            // b32
            if (bitReader.ReadBoolean())
            {
                bitReader.ReadBitArray(32);
            }

            bitReader.ReadBitArray(3); // b3

            bitReader.ReadBits(30); // b30

            bitReader.ReadBitArray(3); // b3

            // filler
            bitReader.ReadStringFromUnalignedBytes(2); // word
            bitReader.ReadBitArray(8); // b8
            bitReader.ReadBitArray(8); // b8
            bitReader.ReadBitArray(34); // b34

            if (bitReader.ReadBoolean())
            {
                bitReader.ReadBitArray(54); // b54
            }

            bitReader.ReadBitArray(1); // b1
            bitReader.ReadBitArray(5); // b5
            bitReader.ReadBitArray(5); // b5
            bitReader.ReadBitArray(5); // b5

            // bbbb
            // ModId
            bitReader.ReadBits(32); // id
            bitReader.ReadBits(32); // ofs

            bitReader.ReadBits(32); // n32
            bitReader.ReadBitArray(1); // f
            bitReader.ReadBits(21); // 21

            bitReader.ReadBits(32); // CAFEBABE

            // fil28
            bitReader.ReadBits(28);

            // fil
            bitReader.ReadBitArray(36);

            // hero
            if (bitReader.ReadBoolean())
            {
                bitReader.ReadBitArray(11); // b11

                uint size = bitReader.ReadBits(4);

                for (int j = 0; j < size; j++)
                {
                    bitReader.ReadBits(6); // num
                    bitReader.ReadBits(32); // b32
                    bitReader.ReadBits(16); // b16
                    bitReader.ReadBits(16); // b16
                    bitReader.ReadBits(23); // b23
                }

                bitReader.ReadBitArray(4); // b4

                ChoiceSection2(ref bitReader);

                uint size2 = bitReader.ReadBits(7);

                for (int x = 0; x < size2; x++)
                {
                    bitReader.ReadBitArray(23);
                }

                bitReader.ReadBitArray(11); // b11

                if (bitReader.ReadBoolean())
                {
                    bitReader.ReadBitArray(38); // b38
                    bitReader.ReadBitArray(16); // b16
                    bitReader.ReadBitArray(16); // b16
                }

                bitReader.ReadBitArray(1); // b1

                uint heroLength = bitReader.ReadBits(5);

                for (int x = 0; x < heroLength; x++)
                {
                    bitReader.ReadStringFromUnalignedBytes(4); // Hero
                }

                bitReader.ReadBitArray(23); // b23
            }

            // ModDependencies
            uint modDependenciesLength = bitReader.ReadBits(6);

            for (int j = 0; j < modDependenciesLength; j++)
            {
                bitReader.ReadBits(32); // id
                bitReader.ReadBits(32); // ofs
            }

            // fil
            bitReader.ReadBitArray(32); // b32
        }

        // s2mh
        uint s2mhCacheHandlesLength = bitReader.ReadBits(6);

        for (int i = 0; i < s2mhCacheHandlesLength; i++)
        {
            _ = new StormS2mFiles(ref bitReader);
        }

        /* Collection section */

        // all the collection items (heroes, skins, mounts, portraits, sprays, etc.) as their License values from the game data, 8 bytes each
        int collectionLength = (int)bitReader.ReadBits(16);

        ReadOnlySpan<byte> collectionLicenses = bitReader.ReadAlignedBytes(collectionLength * 8);

        int hasCollectionLength = (int)bitReader.ReadBits(32);

        if (collectionLength != hasCollectionLength)
            throw new StormParseException($"{_exceptionHeader}: collection license lengths do not match: {collectionLength} != {hasCollectionLength}");

        // for each item, 2 bytes for each of the 16 player slots (m_workingSetSlotId), non-zero if the player owns the item
        ReadOnlySpan<byte> collectionOwnedFlags = bitReader.ReadAlignedBytes(hasCollectionLength * 16 * 2);

        SetLicenseIds(replay, collectionLicenses, collectionLength, collectionOwnedFlags, shouldParseLicenseCatalog);

        /* init data section */
        if (replay.ReplayBuild >= 85027)
        {
            // m_disabledHeroList
            uint disabledHeroListLength = bitReader.ReadBits(8);

            for (int i = 0; i < disabledHeroListLength; i++)
            {
                replay.DisabledHeroAttributeIdList.Add(bitReader.ReadStringFromBits(32));
            }
        }

        replay.RandomValue = bitReader.ReadBits(32); // m_randomSeed

        uint replayOwnerLobbyId = bitReader.ReadBits(32); // the lobby id of the player whose game client created this file (owner of the replay file)
        uint playersLength = bitReader.ReadBits(5);

        Dictionary<uint, uint> workingSetSlotIdByPlayerLobbyId = [];

        for (int i = 0; i < playersLength; i++)
        {
            uint playerLobbyId = bitReader.ReadBits(32); // sequential for each player in the lobby
            uint playerIndex = bitReader.ReadBits(5); // m_workingSetSlotId

            workingSetSlotIdByPlayerLobbyId[playerLobbyId] = playerIndex;

            PregameStormPlayer player = replay.ClientListByWorkingSetSlotID[playerIndex];
            ToonHandle playerToonHandle = player.ToonHandle ??= new();

            if (replayOwnerLobbyId == playerLobbyId)
            {
                replay.Owner = player;
                player.IsReplayOwner = true;
            }

            // toon handle
            playerToonHandle.Region = (int)bitReader.ReadBits(8); // m_region
            playerToonHandle.ProgramId = bitReader.ReadInt32Unaligned(); // m_programId
            playerToonHandle.Realm = (int)bitReader.ReadBits(32); // m_realm
            playerToonHandle.Id = bitReader.ReadInt64Unaligned(); // m_id

            // string toon T:xxxxxxxx
            bitReader.ReadBits(8); // region
            if (bitReader.ReadStringFromUnalignedBytes(4) != "Hero") throw new StormParseException($"{_exceptionHeader}: Not Hero"); // programId
            bitReader.ReadBits(32); // realm

            playerToonHandle.ShortcutId = bitReader.ReadBlobAsString(7, 2); // region global id

            bitReader.ReadBitArray(1); // m_flags
            bitReader.ReadBitArray(2); // m_Unk4

            if (bitReader.ReadBoolean())
            {
                bitReader.ReadBitArray(2); // m_Unk1

                // repeat string toon T:xxxxxxxx
                bitReader.ReadBits(8); // region
                if (bitReader.ReadStringFromUnalignedBytes(4) != "Hero") throw new StormParseException($"{_exceptionHeader}: Not Hero"); // programId
                bitReader.ReadBits(32); // realm
                bitReader.ReadBlobAsString(7, 2); // region global id

                bitReader.ReadBitArray(1); // m_flags
            }

            // m_Unk2 - per lobby value that increments with each player, followed by a per player value, the rest is all 0s
            bitReader.ReadBitArray(198);

            // m_unk3
            if (bitReader.ReadBoolean())
                bitReader.ReadBitArray(64);

            // m_unk4
            bitReader.ReadBitArray(4);

            // client base build, which is 0 for some players, so it must not replace a known build
            int clientBaseBuild = bitReader.ReadInt32Unaligned();

            if (replay.ReplayBuild >= 69228 && clientBaseBuild > 0)
                replay.ReplayBuild = clientBaseBuild;

            PlayerCollectionChoice(ref bitReader);

            player.IsSilenced = bitReader.ReadBoolean(); // m_hasSilencePenalty

            if (replay.ReplayBuild >= 61718)
            {
                bitReader.ReadBoolean();
                player.IsVoiceSilenced = bitReader.ReadBoolean(); // m_hasVoiceSilencePenalty
            }

            if (replay.ReplayBuild >= 66977)
                player.IsBlizzardStaff = bitReader.ReadBoolean(); // m_isBlizzardStaff

            // m_PartyInfo
            if (bitReader.ReadBoolean()) // is player in party
                player.PartyValue = bitReader.ReadLongBits(64); // players in same party will have the same exact 8 bytes of data

            if (bitReader.ReadBoolean())
            {
                player.BattleTagName = bitReader.ReadBlobAsString(7);

                int poundIndex = player.BattleTagName.IndexOf('#');

                if (!string.IsNullOrEmpty(player.BattleTagName) && poundIndex < 0)
                    throw new StormParseException($"{_exceptionHeader}: Invalid battletag");
            }

            if (replay.ReplayBuild >= 51978)
                player.AccountLevel = (int)bitReader.ReadBits(32);  // in custom games, this is a 0

            if (replay.ReplayBuild >= 69947)
                player.HasActiveBoost = bitReader.ReadBoolean(); // m_hasActiveBoost
        }

        /* _end */

        bitReader.ReadBitArray(64); // sss

        // game mode toon
        AmmIdToonChoice(replay, ref bitReader);

        // older, just end here
        if (replay.ReplayBuild < 89754)
        {
            replay.IsBattleLobbyPlayerInfoParsed = true;
            return;
        }

        bitReader.ReadBitArray(87); // b87

        uint a38Length = bitReader.ReadBits(4) + 1;

        for (int i = 0; i < a38Length; i++)
        {
            bitReader.ReadBits(5); // m_workingSetSlotId
            bitReader.ReadBitArray(33); // so far, always 0 (except in older builds)
        }

        bitReader.ReadBits(1); // unk

        // always "Neut" (neutral race, likely a SC2 remnant)
        uint neutLength = bitReader.ReadBits(4) + 1;

        for (int i = 0; i < neutLength; i++)
        {
            bitReader.ReadBitArray(32);
        }

        // for each player, such as "GEM (none)", "GOLD (HeroGold)", "XP (HeroXP)", "XHC (HeroGem)", "LOOT (HeroLootChest)", "Lcns (HeroLicense)", "PORT (HeroPortrait)", "POVT (HeroPortraitOverlay)", and "ICON (HeroIcon)"
        // these refer to the gamedata CReward type elements for Catatogry Tag
        // the values are 0 or hashed licenses, "PORT" is the hashed license of the player's portrait
        uint playerRewardTagsLength = bitReader.ReadBits(5); // num of players

        for (int i = 0; i < playerRewardTagsLength; i++)
        {
            uint playerLobbyId = bitReader.ReadBits(32);
            uint rewardsTagsLength = bitReader.ReadBits(11);

            if (!workingSetSlotIdByPlayerLobbyId.TryGetValue(playerLobbyId, out uint workingSetSlotId))
                throw new StormParseException($"{_exceptionHeader}: player lobby id not found");

            PregameStormPlayer player = replay.ClientListByWorkingSetSlotID[workingSetSlotId];

            uint? portraitHash = null;

            for (int j = 0; j < rewardsTagsLength; j++)
            {
                bitReader.ReadStringFromUnalignedBytes(4); // namespace - "Hero"
                string tag = bitReader.ReadStringFromUnalignedBytes(4); // tag - "GEM, GOLD, XP, etc.
                uint value = bitReader.ReadBits(32);

                if (tag == "PORT")
                    portraitHash = value;
            }

            bitReader.ReadBitArray(11);  // b11; always 0, probably another count for a loop

            player.PortraitHash = portraitHash;
            player.RewardTagsCount = (int)rewardsTagsLength;
        }

        // CSTM
        uint cstmLength = bitReader.ReadBits(6);

        for (int i = 0; i < cstmLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4);
        }

        uint a48Length = bitReader.ReadBits(5);

        for (int i = 0; i < a48Length; i++)
        {
            bitReader.ReadBits(32); // playerLobbyId

            if (replay.ReplayBuild >= 77406)
                bitReader.ReadBits(16); // b16
            else
                bitReader.ReadBits(9);
        }

        bitReader.ReadBitArray(23); // b23
        bitReader.ReadBitArray(32); // b32

        bitReader.ReadBitArray(10); // endFiller

        bitReader.ReadBitArray(32); // b32

        // same as m_heroMasteryTiers in the replay.initData, but for all heroes (including those with 0 tiers) and all players
        uint heroMasteryTiersLength = bitReader.ReadBits(10);

        for (int i = 0; i < heroMasteryTiersLength; i++)
        {
            string heroAttributeName = bitReader.ReadStringFromUnalignedBytes(4);

            uint slotsLength = bitReader.ReadBits(5); // total player slots, indexed by m_workingSetSlotId

            for (int j = 0; j < slotsLength; j++)
            {
                int tier = (int)bitReader.ReadBits(8); // m_tier

                // only for tiers greater than 0
                if (tier > 0 && j < replay.ClientListByWorkingSetSlotID.Length)
                {
                    replay.ClientListByWorkingSetSlotID[j].HeroMasteryTiersByHeroAttributeIdInternal[heroAttributeName] = new HeroMasteryTier()
                    {
                        HeroAttributeId = heroAttributeName,
                        TierLevel = tier,
                    };
                }
            }
        }

        if (replay.ReplayBuild >= 77406) // 68509
            bitReader.ReadBitArray(2);

        if (pregameMode)
        {
            (string? MapLink, string? MapTitle)? mapInfo = GetMapInfo(battleNetCachePaths.Last(), s2mvFiles.Last());
            replay.MapLink = mapInfo?.MapLink;
            replay.MapTitle = mapInfo?.MapTitle;

            replay.MapId = GetMapId(battleNetCachePaths.Last());
        }

        replay.IsBattleLobbyPlayerInfoParsed = true;
    }

    // ('_choice', [(0,8),{0:('GlobalValue',6256),1:('ByPlayerValue',6260)}]), #6261
    private static void PlayerSelectedAttributeChoice(ref BitReader bitReader, StormReplayPregame replay, StormBattleLobbyAttribute attribute)
    {
        switch (bitReader.ReadBits(8))
        {
            case 0: // GlobalValue
                {
                    int attributeValueIndex = (int)bitReader.ReadBits(11); // ValueIndex
                    bitReader.ReadBitArray(1); // IsSet

                    SetGlobalAttributeEvent(replay, attribute, attributeValueIndex);

                    break;
                }

            case 1: // ByPlayerValue
                {
                    uint totalPlayerSlots = bitReader.ReadBits(5);

                    for (int playerSlotIndex = 0; playerSlotIndex < totalPlayerSlots; playerSlotIndex++)
                    {
                        int attributeValueIndex = (int)bitReader.ReadBits(11); // ValueIndex
                        bitReader.ReadBitArray(1); // IsSet

                        SetPlayerAttributeEvent(replay, attribute, playerSlotIndex, attributeValueIndex);
                    }

                    break;
                }

            default:
                throw new NotImplementedException();
        }
    }

    private static void SetGlobalAttributeEvent(StormReplayPregame replay, StormBattleLobbyAttribute attribute, int attributeValueIndex)
    {
        string value = attribute.Values[attributeValueIndex].Value;

        switch (attribute.EventType)
        {
            case ReplayAttributeEventType.GameSpeed:
                {
                    replay.GameSpeed = ReplayAttributeEvents.GetGameSpeed(value);

                    break;
                }

            case ReplayAttributeEventType.DraftTeam1Ban1:
            case ReplayAttributeEventType.DraftTeam1Ban2:
            case ReplayAttributeEventType.DraftTeam1Ban3:
            case ReplayAttributeEventType.DraftTeam2Ban1:
            case ReplayAttributeEventType.DraftTeam2Ban2:
            case ReplayAttributeEventType.DraftTeam2Ban3:
                switch (attribute.EventType)
                {
                    case ReplayAttributeEventType.DraftTeam1Ban1:
                        replay.TeamHeroAttributeIdBans[0][0] = value;
                        break;
                    case ReplayAttributeEventType.DraftTeam1Ban2:
                        replay.TeamHeroAttributeIdBans[0][1] = value;
                        break;
                    case ReplayAttributeEventType.DraftTeam1Ban3:
                        replay.TeamHeroAttributeIdBans[0][2] = value;
                        break;
                    case ReplayAttributeEventType.DraftTeam2Ban1:
                        replay.TeamHeroAttributeIdBans[1][0] = value;
                        break;
                    case ReplayAttributeEventType.DraftTeam2Ban2:
                        replay.TeamHeroAttributeIdBans[1][1] = value;
                        break;
                    case ReplayAttributeEventType.DraftTeam2Ban3:
                        replay.TeamHeroAttributeIdBans[1][2] = value;
                        break;
                }

                break;

            case ReplayAttributeEventType.DraftTeam1BanChooserSlot:
                {
                    replay.TeamBanChoosers[0] = SetBanChooser(replay, value);

                    break;
                }

            case ReplayAttributeEventType.DraftTeam2BanChooserSlot:
                {
                    replay.TeamBanChoosers[1] = SetBanChooser(replay, value);

                    break;
                }

            case ReplayAttributeEventType.PrivacyOption:
                {
                    replay.GamePrivacy = ReplayAttributeEvents.GetPrivacyOption(value);

                    break;
                }

            case ReplayAttributeEventType.ReadyMode:
                {
                    replay.ReadyMode = ReplayAttributeEvents.GetReadyMode(value);

                    break;
                }

            case ReplayAttributeEventType.LobbyMode:
                {
                    replay.LobbyMode = ReplayAttributeEvents.GetLobbyMode(value);

                    break;
                }

            case ReplayAttributeEventType.DraftBanMode:
                {
                    replay.BanMode = ReplayAttributeEvents.GetDraftBanMode(value);

                    break;
                }

            case ReplayAttributeEventType.FirstReadyingTeam:
                {
                    replay.FirstDraftTeam = ReplayAttributeEvents.GetFirstReadyingTeam(value);

                    break;
                }

            case ReplayAttributeEventType.GameMode:
                {
                    if (value == "Priv")
                        replay.GameMode = StormGameMode.Custom;

                    break;
                }

            case ReplayAttributeEventType.TeamSize:
                {
                    replay.TeamSize = value;

                    break;
                }

            default:
                break;
        }
    }

    private static StormBanChooserType SetBanChooser(StormReplayPregame replay, string value)
    {
        StormBanChooserType banChooser = ReplayAttributeEvents.GetBanChooser(value, out int? playerSlot);

        if (playerSlot.HasValue)
            replay.ClientListByWorkingSetSlotID[playerSlot.Value].IsBanChooser = true;

        return banChooser;
    }

    private static void SetPlayerAttributeEvent(StormReplayPregame replay, StormBattleLobbyAttribute attribute, int playerSlotIndex, int attributeValueIndex)
    {
        string value = attribute.Values[attributeValueIndex].Value;
        PregameStormPlayer player = replay.ClientListByWorkingSetSlotID[playerSlotIndex];

        player.PlayerHero ??= new();

        switch (attribute.EventType)
        {
            case ReplayAttributeEventType.PlayerType:
                {
                    player.PlayerSlotType = ReplayAttributeEvents.GetPlayerType(value);

                    break;
                }

            case ReplayAttributeEventType.PlayerTeam1v1:
            case ReplayAttributeEventType.PlayerTeam2v2:
            case ReplayAttributeEventType.PlayerTeam3v3:
            case ReplayAttributeEventType.PlayerTeam4v4:
            case ReplayAttributeEventType.PlayerTeam5v5:
            case ReplayAttributeEventType.PlayerTeam6v6:
                {
                    player.PlayerTeamValueByAttributeEventType.TryAdd(attribute.EventType, value);

                    break;
                }

            case ReplayAttributeEventType.DifficultyLevel:
                {
                    player.ComputerDifficulty = ReplayAttributeEvents.GetComputerDifficultyLevel(value);

                    break;
                }

            case ReplayAttributeEventType.ParticipantRole:
                {
                    player.PlayerType = ReplayAttributeEvents.GetParticipantRole(value);

                    break;
                }

            case ReplayAttributeEventType.HeroAttributeId:
                {
                    player.PlayerHero.HeroAttributeId = value;

                    break;
                }

            case ReplayAttributeEventType.SkinAndSkinTint:
                {
                    player.PlayerLoadout.SkinAndSkinTintAttributeId = value;

                    break;
                }

            case ReplayAttributeEventType.MountAndMountTint:
                {
                    player.PlayerLoadout.MountAndMountTintAttributeId = value;

                    break;
                }

            case ReplayAttributeEventType.HeroLevel:
                {
                    player.PlayerHero.HeroLevel = int.Parse(value);

                    break;
                }

            case ReplayAttributeEventType.Banner:
                {
                    player.PlayerLoadout.BannerAttributeId = value;

                    break;
                }

            case ReplayAttributeEventType.Spray:
                {
                    player.PlayerLoadout.SprayAttributeId = value;

                    break;
                }

            case ReplayAttributeEventType.VoiceLine:
                {
                    player.PlayerLoadout.VoiceLineAttributeId = value;

                    break;
                }

            case ReplayAttributeEventType.Announcer:
                {
                    player.PlayerLoadout.AnnouncerPackAttributeId = value;

                    break;
                }

            case ReplayAttributeEventType.HeroMasteryRingTier:
                {
                    if (value == string.Empty)
                    {
                        break;
                    }
                    else
                    {
                        if (int.TryParse(value[^1..], out int tierLevel))
                            player.PlayerHero.HeroMasteryTier = tierLevel;
                    }

                    break;
                }

            default:
                break;
        }
    }

    private static void SetPlayerTeams(StormReplayPregame replay)
    {
        ReplayAttributeEventType? playerTeamAttributeEventType = replay.TeamSize switch
        {
            "1v1" => ReplayAttributeEventType.PlayerTeam1v1,
            "2v2" => ReplayAttributeEventType.PlayerTeam2v2,
            "3v3" => ReplayAttributeEventType.PlayerTeam3v3,
            "4v4" => ReplayAttributeEventType.PlayerTeam4v4,
            "5v5" => ReplayAttributeEventType.PlayerTeam5v5,
            "6v6" => ReplayAttributeEventType.PlayerTeam6v6,
            _ => null,
        };

        if (playerTeamAttributeEventType is null)
            return;

        for (int i = 0; i < replay.ClientListByWorkingSetSlotID.Length; i++)
        {
            PregameStormPlayer player = replay.ClientListByWorkingSetSlotID[i];

            if (player.PlayerSlotType is not (PlayerSlotType.Human or PlayerSlotType.Computer))
                continue;

            // the team attribute is only enabled for participants
            if (player.PlayerType == PlayerType.Observer)
            {
                player.Team = StormTeam.Observer;
                continue;
            }

            // if not found, leave as unknown
            if (!player.PlayerTeamValueByAttributeEventType.TryGetValue(playerTeamAttributeEventType.Value, out string? teamValue))
                continue;

            player.Team = teamValue switch
            {
                "T1" => StormTeam.Blue,
                "T2" => StormTeam.Red,
                _ => StormTeam.Unknown,
            };
        }
    }

    // ('_choice', [(0,4),{0:('None',6353),1:('None',6354),2:('None',6355),3:('None',6356),4:('None',6357),5:('Data',6363)}]), #6364
    private static void ChoiceSection2(ref BitReader bitReader)
    {
        switch (bitReader.ReadBits(4))
        {
            case 0: // None
                {
                    break;
                }

            case 1: // None
                {
                    break;
                }

            case 2: // None
                {
                    break;
                }

            case 3: // None
                {
                    break;
                }

            case 4: // None
                {
                    break;
                }

            case 5: // Data
                {
                    bitReader.ReadBitArray(40); // b40
                    bitReader.ReadBitArray(16); // b16
                    bitReader.ReadBitArray(79); // b79
                    bitReader.ReadBitArray(160); // b160
                    bitReader.ReadBitArray(105); // b105

                    break;
                }

            default:
                throw new NotImplementedException();
        }
    }

    // the license id catalog is the same for all replays of the same build, so a provided catalog is used as is and never modified
    private static void SetLicenseIds(StormReplayPregame replay, ReadOnlySpan<byte> licenseIds, int licenseIdsLength, ReadOnlySpan<byte> ownedFlags, bool shouldParseLicenseCatalog)
    {
        IReadOnlySet<long>? providedCatalog = replay.LicenseIdCatalog;

        if (providedCatalog is null && !shouldParseLicenseCatalog)
            return;

        // only build a new catalog when one was not provided
        HashSet<long>? newCatalog = providedCatalog is null ? new HashSet<long>(licenseIdsLength) : null;
        bool isSameCatalog = providedCatalog?.Count == licenseIdsLength;

        for (int item = 0; item < licenseIdsLength; item++)
        {
            long licenseId = BinaryPrimitives.ReadInt64BigEndian(licenseIds.Slice(item * 8, 8));

            newCatalog?.Add(licenseId);

            if (isSameCatalog && !providedCatalog!.Contains(licenseId))
                isSameCatalog = false;

            for (int i = 0; i < replay.ClientListByWorkingSetSlotID.Length; i++)
            {
                int flagIndex = ((item * 16) + i) * 2;

                if ((ownedFlags[flagIndex] | ownedFlags[flagIndex + 1]) != 0)
                    replay.ClientListByWorkingSetSlotID[i].OwnedLicenseIdsInternal.Add(licenseId);
            }
        }

        if (newCatalog is not null)
            replay.LicenseIdCatalog = newCatalog;
        else if (!isSameCatalog)
            replay.LicenseIdCatalog = CreateLicenseIdCatalog(licenseIds, licenseIdsLength); // provided catalog is from a different build
    }

    private static HashSet<long> CreateLicenseIdCatalog(ReadOnlySpan<byte> licenseIds, int licenseIdsLength)
    {
        HashSet<long> catalog = new(licenseIdsLength);

        for (int item = 0; item < licenseIdsLength; item++)
            catalog.Add(BinaryPrimitives.ReadInt64BigEndian(licenseIds.Slice(item * 8, 8)));

        return catalog;
    }

    private static void PlayerCollectionChoice(ref BitReader bitReader)
    {
        switch (bitReader.ReadBits(1))
        {
            case 0:
                {
                    // older builds, a bit for each item in the collection section
                    bitReader.ReadBitArray(bitReader.ReadBits(12));
                    bitReader.ReadBoolean();
                    break;
                }

            case 1:
                {
                    break;
                }

            default:
                throw new NotImplementedException();
        }
    }

    // ('_choice', [(0,1),{0:('filler',6437),1:('m_toon',6442)}]), #6443
    private static void AmmIdToonChoice(StormReplayPregame replay, ref BitReader bitReader)
    {
        switch (bitReader.ReadBits(1))
        {
            case 0: // filler
                {
                    bitReader.ReadBitArray(16);

                    break;
                }

            case 1:
                {
                    // toon handle
                    bitReader.ReadBits(8); // region
                    bitReader.ReadStringFromUnalignedBytes(4); // programId
                    bitReader.ReadBits(32); // realm

                    replay.GameMode = ReplayInitData.GetGameMode(ref bitReader); // m_ammId

                    bitReader.ReadBits(32);

                    break;
                }

            default:
                throw new NotImplementedException();
        }
    }

    private static (string? MapLink, string? MapTitle)? GetMapInfo(string battleNetCachePath, StormS2mFiles mapFile)
    {
        ReadOnlySpan<char> firstTwo = mapFile.FileName.AsSpan(0, 2);
        ReadOnlySpan<char> nextTwo = mapFile.FileName.AsSpan(2, 2);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            battleNetCachePath = battleNetCachePath.Replace('\\', Path.DirectorySeparatorChar);

        string? directory = Path.GetDirectoryName(battleNetCachePath);
        if (directory is null)
            return null;

        string finalPath = Path.Join(directory.AsSpan(0, directory.Length - 6), firstTwo, nextTwo, Path.ChangeExtension(mapFile.FileName, mapFile.FileType));

        try
        {
            if (!File.Exists(finalPath))
                return null;

            using FileStream fileStream = File.Open(finalPath, FileMode.Open, FileAccess.Read, FileShare.Read);

            using JsonDocument jsonDocument = JsonDocument.Parse(fileStream);

            JsonElement rootElement = jsonDocument.RootElement;

            if (rootElement.TryGetProperty("MapInfo", out JsonElement mapInfoElement) &&
                mapInfoElement.TryGetProperty("Properties", out JsonElement propertiesElement) &&
                propertiesElement.TryGetProperty("Loading", out JsonElement loadingElement) &&
                loadingElement.TryGetProperty("MapLink", out JsonElement mapLinkElement) &&
                rootElement.TryGetProperty("UsedStrings", out JsonElement usedStringsElement) &&
                usedStringsElement.TryGetProperty("DocInfo/Name", out JsonElement docInfoNameElement))
            {
                return (mapLinkElement.GetString(), docInfoNameElement.GetProperty("enUS").GetString());
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    private static string? GetMapId(string battleNetCachePath)
    {
        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            battleNetCachePath = battleNetCachePath.Replace('\\', Path.DirectorySeparatorChar);

        try
        {
            if (!File.Exists(battleNetCachePath))
                return null;

            using MpqHeroesArchive archive = MpqHeroesFile.Open(battleNetCachePath);

            if (!archive.TryGetEntry("MapScript.galaxy", out MpqHeroesArchiveEntry? entry))
                return null;

            using StreamReader streamReader = new(archive.DecompressEntry(entry.Value));

            while (!streamReader.EndOfStream)
            {
                string? line = streamReader.ReadLine();

                if (string.IsNullOrWhiteSpace(line) || !line.Contains("mAPMapStringID", StringComparison.OrdinalIgnoreCase))
                    continue;

                int equalsIndex = line.IndexOf('=');
                if (equalsIndex < 1)
                    continue;

                return line.AsSpan(equalsIndex + 1).Trim().Trim(['"', ';']).ToString();
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
}
