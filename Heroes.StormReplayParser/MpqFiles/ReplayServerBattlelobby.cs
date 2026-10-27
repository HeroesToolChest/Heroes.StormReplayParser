using System.Runtime.InteropServices;

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

        // The section up to the collection section is a merged copy of the headers of the map and its mods, which are in the Battle.net cache as .s2mh (versioned format) and .s2mi (publish info) files.

        for (int i = 0; i < 16; i++)
            replay.ClientListByWorkingSetSlotID[i] = new();

        Dictionary<ReplayAttributeEventType, LobbyAttribute> lobbyAttributesByAttributeEventType = [];

        List<string> battleNetCachePaths = [];
        List<StormS2mFiles> s2mvFiles = [];

        BitReader bitReader = new(source, EndianType.BigEndian);

        // the Battle.net cache paths of the map and mod archives (s2ma), in dependency order with the map last
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

        /* Attribute section, the attribute definitions of all the mods (s2mh) */

        uint attributeLength = bitReader.ReadBits(9); // total number of attributes

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadInt32Unaligned(); // namespace

            LobbyAttribute lobbyAttribute = new((ReplayAttributeEventType)bitReader.ReadInt32Unaligned());

            uint attributeValueLength = bitReader.ReadBits(12);
            uint? valueModexIndex = null;

            for (int j = 0; j < attributeValueLength; j++)
            {
                LobbyAttributeValue lobbyAttributeValue = new(ref bitReader, bitReader.ReadStringFromUnalignedBytes(4));

                valueModexIndex ??= lobbyAttributeValue.NameStringReference?.ModIndex;

                bitReader.ReadBits(3); // unknown, always 0, likely the length of an empty list

                lobbyAttribute.Values.Add(lobbyAttributeValue);
            }

            // name of the lobby attribute (for the group of attributes from the above loop)
            LobbyAttributeValue? lobbyAttributeName = new(ref bitReader);
            lobbyAttribute.EventTypeStringId = lobbyAttributeName?.NameStringReference?.StringId;
            lobbyAttribute.ModIndex = lobbyAttributeName?.NameStringReference?.ModIndex ?? valueModexIndex; // the string table index is the position of the mod that defines the attribute

            // requirements, the conditions for the attribute to be enabled
            uint requirementsLength = bitReader.ReadBits(5);

            for (int j = 0; j < requirementsLength; j++)
            {
                bitReader.ReadBits(5); // operation, 20 = a global attribute has one of the values, 21 = the same slot's attribute has one of the values
                bitReader.ReadInt32Unaligned(); // namespace

                lobbyAttribute.EnabledEventType = (ReplayAttributeEventType)bitReader.ReadInt32Unaligned();

                uint enabledAttributeValueLength = bitReader.ReadBits(12);

                for (int k = 0; k < enabledAttributeValueLength; k++)
                {
                    lobbyAttribute.EnabledValues.Add(new LobbyEnabledAttributeValue(ref bitReader));
                }
            }

            bitReader.ReadBits(5); // restriction, 1 = a value can only be used by one slot (fixed size teams and colors)
            bitReader.ReadBits(5); // visibility, 3 = everyone, 1 = only the player ("Using Custom Observer UI")
            bitReader.ReadBits(5); // access, 0 = the game, 1 = the player, 2 = the host, 3 = ban hero choices and player logo
            bitReader.ReadBits(32); // option flags, 0x1 = team attribute, 0x8 = slot attribute (controller, team, participant role, tandem leader), 0x4 = unknown

            // set default values
            AttributeValueChoice(ref bitReader, replay, lobbyAttribute);

            bitReader.ReadBits(9); // sort order

            if (!lobbyAttributesByAttributeEventType.TryAdd(lobbyAttribute.EventType, lobbyAttribute))
                throw new StormParseException($"{_exceptionHeader}: duplicate lobby attribute");
        }

        /* Game header, the same as the map info in the map's s2mh, but using the merged string tables and s2mv list */

        ReadLobbyStringReference(ref bitReader); // map name
        ReadLobbyStringReference(ref bitReader); // map description
        bitReader.ReadBoolean(); // unknown optional, never set

        if (bitReader.ReadBoolean())
            ReadImage(ref bitReader); // preview image, its s2mv index is the map's first s2mv in the s2mv list below

        bitReader.ReadBits(4); // max players - 1
        bitReader.ReadBits(5); // unknown, 22 for non-custom games, 0 (sometimes 1 or 5) for custom games

        /* Player attribute selections */

        attributeLength = bitReader.ReadBits(9); // attribute count should be the same as previous

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadInt32Unaligned(); // namespace

            ReplayAttributeEventType attributeEventType = (ReplayAttributeEventType)bitReader.ReadInt32Unaligned();

            AttributeValueChoice(ref bitReader, replay, lobbyAttributesByAttributeEventType[attributeEventType]);
        }

        // set player teams, the selections are not in attribute id order, so this can only be done once they have all been read
        SetPlayerTeams(replay);

        /* s2mv and locale section */

        // the s2mv files of all the mods, without duplicates (images and JSON data)
        uint s2mvCacheHandlesLength = bitReader.ReadBits(6);

        for (int i = 0; i < s2mvCacheHandlesLength; i++)
        {
            s2mvFiles.Add(new StormS2mFiles(ref bitReader));
        }

        int localesLength = (int)bitReader.ReadBits(5); // number of locales, either only "enUS" or all of them
        Dictionary<string, StormS2mFiles[]> s2mlFilesByLocale = new(localesLength, StringComparer.Ordinal);

        for (int i = 0; i < localesLength; i++)
        {
            string locale = bitReader.ReadStringFromUnalignedBytes(4); // locale, such as "enUS"

            // the string tables (s2ml), one per mod in the used mods order, the string table index of a string reference is its position in this list
            // sometimes there is an extra handle at the end that is all zeros
            uint s2mlSize = bitReader.ReadBits(6);
            StormS2mFiles[] s2mlFiles = new StormS2mFiles[s2mlSize];

            for (int j = 0; j < s2mlSize; j++)
            {
                s2mlFiles[j] = new StormS2mFiles(ref bitReader);
            }

            s2mlFilesByLocale[locale] = s2mlFiles;
        }

        // an attribute's mod index is the position of its mod's s2ml file in each locale, the attributes of the same mod share the same lookup
        Dictionary<uint, Dictionary<string, StormS2mFiles>> s2mlFileByLocaleByModIndex = [];

        foreach (LobbyAttribute attribute in lobbyAttributesByAttributeEventType.Values)
        {
            if (attribute.ModIndex is not uint modIndex)
                continue;

            if (!s2mlFileByLocaleByModIndex.TryGetValue(modIndex, out Dictionary<string, StormS2mFiles>? s2mlFileByLocale))
            {
                s2mlFileByLocale = new(s2mlFilesByLocale.Count, StringComparer.Ordinal);

                foreach ((string locale, StormS2mFiles[] s2mlFiles) in s2mlFilesByLocale)
                {
                    if (modIndex < s2mlFiles.Length)
                        s2mlFileByLocale[locale] = s2mlFiles[modIndex];
                }

                s2mlFileByLocaleByModIndex[modIndex] = s2mlFileByLocale;
            }

            attribute.S2mlFileByLocale = s2mlFileByLocale;
        }

        // unknown, the same value in all builds
        bitReader.ReadULongBits(64);
        bitReader.ReadULongBits(64);

        // the map and its mods, in dependency order with the map last
        uint usedModsLength = bitReader.ReadBits(5);

        for (int i = 0; i < usedModsLength; i++)
        {
            bitReader.ReadBits(32); // document id
            bitReader.ReadBits(32); // version
            bitReader.ReadBits(4); // priority, 2 for the mods, 0 for the map
        }

        /*
         * Document headers, one for each of the used mods, from their s2mh (header) and s2mi (publish info).
         * Unlike the rest of this section, the string table and s2mv indexes in here are local to the document.
         */
        uint documentsLength = bitReader.ReadBits(7);

        for (int i = 0; i < documentsLength; i++)
        {
            bitReader.ReadBits(32); // document id
            bitReader.ReadBits(32); // version
            bitReader.ReadBits(16); // major version
            bitReader.ReadBits(16); // minor version

            ReadLobbyStringReference(ref bitReader); // name
            ReadLobbyStringReference(ref bitReader); // description
            bitReader.ReadBoolean(); // unknown optional, never set

            if (bitReader.ReadBoolean())
                ReadImage(ref bitReader); // preview image

            bitReader.ReadBits(4); // max players - 1, 15 for mods and 9 for maps

            // the document's own s2mv files
            uint documentS2mvLength = bitReader.ReadBits(6);

            for (int j = 0; j < documentS2mvLength; j++)
            {
                _ = new StormS2mFiles(ref bitReader);
            }

            bitReader.ReadBoolean(); // unknown, always false

            // author name, such as "blizzmaps#1"
            bitReader.ReadBits(8); // region
            bitReader.ReadStringFromUnalignedBytes(4); // programId
            bitReader.ReadBits(32); // realm
            bitReader.ReadBlobAsString(7, 2); // m_globalName

            // author toon handle
            bitReader.ReadBits(8); // region
            bitReader.ReadStringFromUnalignedBytes(4); // programId
            bitReader.ReadBits(32); // realm
            bitReader.ReadLongBits(64); // id

            // unknown, only on maps (such as 248 and 99)
            if (bitReader.ReadBoolean())
            {
                bitReader.ReadBits(16);
                bitReader.ReadBits(16);
            }

            // unknown string, only on some maps
            if (bitReader.ReadBoolean())
                ReadLobbyStringReference(ref bitReader);

            // tags, such as "BLIZ" and "PRGN"
            uint tagsLength = bitReader.ReadBits(5);

            for (int j = 0; j < tagsLength; j++)
            {
                bitReader.ReadStringFromUnalignedBytes(4);
            }

            bitReader.ReadBoolean(); // unknown, always false

            // game variant summary, only partially decoded, the optional part is only on maps
            bitReader.ReadBits(16);
            bitReader.ReadBits(8);
            bitReader.ReadBits(8);
            bitReader.ReadULongBits(34);

            if (bitReader.ReadBoolean())
                bitReader.ReadULongBits(54);

            bitReader.ReadBoolean();
            bitReader.ReadBits(5);
            bitReader.ReadBits(5);
            bitReader.ReadBits(5);

            // publish info (s2mi)
            bitReader.ReadBits(32); // document id
            bitReader.ReadBits(32); // version
            bitReader.ReadBits(32); // unknown, always 0
            bitReader.ReadBoolean(); // unknown, always true
            bitReader.ReadBits(21); // publish flags
            bitReader.ReadBits(32); // 0xCAFEBABE
            bitReader.ReadULongBits(64); // revision id

            if (bitReader.ReadBoolean())
                GameInfo(ref bitReader);

            // the document ids this document depends on
            uint documentDependenciesLength = bitReader.ReadBits(6);

            for (int j = 0; j < documentDependenciesLength; j++)
            {
                bitReader.ReadBits(32); // document id
                bitReader.ReadBits(32); // version, always 0
            }

            bitReader.ReadBits(32); // unknown, always 0
        }

        // the s2mh (header) files of the used mods, in the same order
        uint s2mhCacheHandlesLength = bitReader.ReadBits(6);
        StormS2mFiles[] s2mhFiles = new StormS2mFiles[s2mhCacheHandlesLength];

        for (int i = 0; i < s2mhCacheHandlesLength; i++)
        {
            s2mhFiles[i] = new StormS2mFiles(ref bitReader);
        }

        // an attribute's mod index is the position of its mod's s2mh file
        foreach (LobbyAttribute attribute in lobbyAttributesByAttributeEventType.Values)
        {
            if (attribute.ModIndex is uint modIndex && modIndex < s2mhFiles.Length)
                attribute.S2mhFile = s2mhFiles[modIndex];
        }

        /* Collection section */

        // all the collection items (heroes, skins, mounts, portraits, sprays, etc.) as their License values from the game data, 8 bytes each
        int collectionLength = (int)bitReader.ReadBits(16);

        ReadOnlySpan<byte> collectionLicenses = bitReader.ReadAlignedBytes(collectionLength * 8);

        int hasCollectionLength = (int)bitReader.ReadBits(32);

        if (collectionLength != hasCollectionLength)
            throw new StormParseException($"{_exceptionHeader}: collection license lengths do not match: {collectionLength} != {hasCollectionLength}");

        // for each item, 2 bytes for each of the 16 player slots (m_workingSetSlotId), non-zero if the player has a license for the item (owned or free to play)
        ReadOnlySpan<byte> collectionLicenseFlags = bitReader.ReadAlignedBytes(hasCollectionLength * 16 * 2);

        SetLicenseIds(replay, collectionLicenses, collectionLength, collectionLicenseFlags, shouldParseLicenseCatalog);

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
        // the values are 0 or Battle.net unlockable ids, which are in the unlockable files (.unlk) of the Battle.net cache, one file per tag
        uint playerRewardTagsLength = bitReader.ReadBits(5); // num of players

        for (int i = 0; i < playerRewardTagsLength; i++)
        {
            uint playerLobbyId = bitReader.ReadBits(32);
            uint rewardsTagsLength = bitReader.ReadBits(11);

            if (!workingSetSlotIdByPlayerLobbyId.TryGetValue(playerLobbyId, out uint workingSetSlotId))
                throw new StormParseException($"{_exceptionHeader}: player lobby id not found");

            PregameStormPlayer player = replay.ClientListByWorkingSetSlotID[workingSetSlotId];

            uint? portraitUnlockableId = null;
            uint? portraitBorderUnlockableId = null;

            for (int j = 0; j < rewardsTagsLength; j++)
            {
                bitReader.ReadStringFromUnalignedBytes(4); // namespace - "Hero"
                string tag = bitReader.ReadStringFromUnalignedBytes(4); // tag - "GEM, GOLD, XP, etc.
                uint value = bitReader.ReadBits(32);

                // "PORT" is the  and "POVT" is the portrait's border
                if (tag == "PORT")
                    portraitUnlockableId = value;
                else if (tag == "POVT")
                    portraitBorderUnlockableId = value;
            }

            bitReader.ReadBits(11); // b11; always 0, probably another count for a loop

            player.PortraitUnlockableId = portraitUnlockableId;
            player.PortraitBorderUnlockableId = portraitBorderUnlockableId;
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

    private static void ReadLobbyStringReference(ref BitReader bitReader) => _ = new LobbyStringReference(ref bitReader);

    private static void ReadImage(ref BitReader bitReader) => _ = new LobbyImage(ref bitReader);

    // the same layout as the player attribute selections, but for the default value of an attribute
    private static void AttributeValueChoice(ref BitReader bitReader, StormReplayPregame replay, LobbyAttribute attribute)
    {
        switch (bitReader.ReadBits(8))
        {
            case 0: // GlobalValue
                {
                    int attributeValueIndex = (int)bitReader.ReadBits(11); // ValueIndex
                    bitReader.ReadBoolean(); // IsLocked, set in non-custom games

                    SetGlobalAttributeEvent(replay, attribute, attributeValueIndex);

                    break;
                }

            case 1: // ByPlayerValue
                {
                    uint totalPlayerSlots = bitReader.ReadBits(5);

                    for (int playerSlotIndex = 0; playerSlotIndex < totalPlayerSlots; playerSlotIndex++)
                    {
                        int attributeValueIndex = (int)bitReader.ReadBits(11); // ValueIndex
                        bitReader.ReadBoolean(); // IsLocked, set in non-custom games

                        SetPlayerAttributeEvent(replay, attribute, playerSlotIndex, attributeValueIndex);
                    }

                    break;
                }

            default:
                throw new StormParseException($"{_exceptionHeader}: unknown attribute default value type");
        }
    }

    private static void SetGlobalAttributeEvent(StormReplayPregame replay, LobbyAttribute attribute, int attributeValueIndex)
    {
        string value = attribute.Values[attributeValueIndex].Value;

        switch (attribute.EventType)
        {
            case ReplayAttributeEventType.GameSpeed:
                {
                    replay.GameSpeed = ReplayAttributeEvents.GetGameSpeed(value);

                    break;
                }

            case ReplayAttributeEventType.Team1Ban1HeroChoice:
            case ReplayAttributeEventType.Team1Ban2HeroChoice:
            case ReplayAttributeEventType.Team1Ban3HeroChoice:
            case ReplayAttributeEventType.Team2Ban1HeroChoice:
            case ReplayAttributeEventType.Team2Ba2HeroChoice:
            case ReplayAttributeEventType.Team2Ban3HeroChoice:
                switch (attribute.EventType)
                {
                    case ReplayAttributeEventType.Team1Ban1HeroChoice:
                        replay.TeamHeroAttributeIdBans[0][0] = value;
                        break;
                    case ReplayAttributeEventType.Team1Ban2HeroChoice:
                        replay.TeamHeroAttributeIdBans[0][1] = value;
                        break;
                    case ReplayAttributeEventType.Team1Ban3HeroChoice:
                        replay.TeamHeroAttributeIdBans[0][2] = value;
                        break;
                    case ReplayAttributeEventType.Team2Ban1HeroChoice:
                        replay.TeamHeroAttributeIdBans[1][0] = value;
                        break;
                    case ReplayAttributeEventType.Team2Ba2HeroChoice:
                        replay.TeamHeroAttributeIdBans[1][1] = value;
                        break;
                    case ReplayAttributeEventType.Team2Ban3HeroChoice:
                        replay.TeamHeroAttributeIdBans[1][2] = value;
                        break;
                }

                break;

            case ReplayAttributeEventType.Team1BanChooser:
                {
                    replay.TeamBanChoosers[0] = SetBanChooser(replay, value);

                    break;
                }

            case ReplayAttributeEventType.Team2BanChooser:
                {
                    replay.TeamBanChoosers[1] = SetBanChooser(replay, value);

                    break;
                }

            case ReplayAttributeEventType.GamePrivacy:
                {
                    replay.GamePrivacy = ReplayAttributeEvents.GetGamePrivacy(value);

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

            case ReplayAttributeEventType.BanMode:
                {
                    replay.BanMode = ReplayAttributeEvents.GetBanMode(value);

                    break;
                }

            case ReplayAttributeEventType.FirstDraftTeam:
                {
                    replay.FirstDraftTeam = ReplayAttributeEvents.GetFirstDraftTeam(value);

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

    private static void SetPlayerAttributeEvent(StormReplayPregame replay, LobbyAttribute attribute, int playerSlotIndex, int attributeValueIndex)
    {
        string value = attribute.Values[attributeValueIndex].Value;
        PregameStormPlayer player = replay.ClientListByWorkingSetSlotID[playerSlotIndex];

        player.PlayerHero ??= new();

        switch (attribute.EventType)
        {
            case ReplayAttributeEventType.Controller:
                {
                    player.PlayerSlotType = ReplayAttributeEvents.GetPlayerType(value);

                    break;
                }

            case ReplayAttributeEventType.Team1v1:
            case ReplayAttributeEventType.Team2v2:
            case ReplayAttributeEventType.Team3v3:
            case ReplayAttributeEventType.Team4v4:
            case ReplayAttributeEventType.Team5v5:
            case ReplayAttributeEventType.Team6v6:
                {
                    player.PlayerTeamValueByAttributeEventType[attribute.EventType] = value;

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

            case ReplayAttributeEventType.Skin:
                {
                    player.PlayerLoadout.SkinAndSkinTintAttributeId = value;

                    break;
                }

            case ReplayAttributeEventType.Mount:
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

            case ReplayAttributeEventType.AnnouncerPack:
                {
                    player.PlayerLoadout.AnnouncerPackAttributeId = value;

                    break;
                }

            case ReplayAttributeEventType.HeroMasteryRingTier:
                {
                    player.PlayerHero.HeroMasteryRingTier = ReplayAttributeEvents.GetHeroMasteryRingTier(value);

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
            "1v1" => ReplayAttributeEventType.Team1v1,
            "2v2" => ReplayAttributeEventType.Team2v2,
            "3v3" => ReplayAttributeEventType.Team3v3,
            "4v4" => ReplayAttributeEventType.Team4v4,
            "5v5" => ReplayAttributeEventType.Team5v5,
            "6v6" => ReplayAttributeEventType.Team6v6,
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

    // game info from the document's s2mh: screenshots, how to play, and matchmaker tags
    private static void GameInfo(ref BitReader bitReader)
    {
        bitReader.ReadBits(11); // unknown, always 0, likely the lengths of two empty lists

        uint screenshotsLength = bitReader.ReadBits(4);

        for (int i = 0; i < screenshotsLength; i++)
        {
            ReadImage(ref bitReader);
            ReadLobbyStringReference(ref bitReader); // caption
        }

        bitReader.ReadBits(4); // unknown, always 0, likely the length of an empty list

        // the how to play headings, such as "Destroy the Enemy Core" and "Single Lane"
        uint howToPlayLength = bitReader.ReadBits(4);

        for (int i = 0; i < howToPlayLength; i++)
        {
            ReadLobbyStringReference(ref bitReader); // heading
            bitReader.ReadBits(32); // index of the heading's first line in the how to play lines
            bitReader.ReadBits(2); // list type
            ReadLobbyStringReference(ref bitReader); // unknown, always 0
        }

        // the how to play lines that are under the headings
        uint howToPlayLinesLength = bitReader.ReadBits(7);

        for (int i = 0; i < howToPlayLinesLength; i++)
        {
            ReadLobbyStringReference(ref bitReader);
        }

        bitReader.ReadBits(11); // unknown, always 0, likely the lengths of two empty lists

        if (bitReader.ReadBoolean())
            ReadImage(ref bitReader); // 1920x1080 image, only on maps

        bitReader.ReadBoolean(); // unknown optional, never set

        // matchmaker tags, such as "Hero"
        uint matchmakerTagsLength = bitReader.ReadBits(5);

        for (int i = 0; i < matchmakerTagsLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4);
        }

        ReadLobbyStringReference(ref bitReader); // unknown, always 0
    }

    // the license id catalog is the same for all replays of the same build, so a provided catalog is used as is and never modified
    private static void SetLicenseIds(StormReplayPregame replay, ReadOnlySpan<byte> licenseIds, int licenseIdsLength, ReadOnlySpan<byte> licenseFlags, bool shouldParseLicenseCatalog)
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

                if ((licenseFlags[flagIndex] | licenseFlags[flagIndex + 1]) != 0)
                    replay.ClientListByWorkingSetSlotID[i].LicenseIdsInternal.Add(licenseId);
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
