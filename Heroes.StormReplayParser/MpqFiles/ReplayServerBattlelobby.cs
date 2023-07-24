﻿using System.Text.Json;

namespace Heroes.StormReplayParser.MpqFiles;

internal static class ReplayServerBattlelobby
{
    private const string _exceptionHeader = "battlelobby";

    public static string FileName { get; } = "replay.server.battlelobby";

    public static void Parse(StormReplayPregame replay, ReadOnlySpan<byte> source)
    {
        for (int i = 0; i < 16; i++)
            replay.ClientListByUserID[i] = new();

        Dictionary<ReplayAttributeEventType, StormBattleLobbyAttribute> lobbyAttributesByAttributeEventType = new();
        List<StormS2mFiles> s2mFiles = new();
        List<string> battleNetCachePaths = new();

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

                if (bitReader.ReadBoolean()) // first
                {
                    if (bitReader.ReadBoolean()) // f32
                    {
                        bitReader.ReadBits(32);
                    }

                    bitReader.ReadBits(6); // NewField6
                    stormBattleLobbyAttributeValue.FirstId = bitReader.ReadInt16Unaligned(); // word - id for s2ml xml
                }

                if (bitReader.ReadBoolean()) // second
                {
                    if (bitReader.ReadBoolean()) // f32
                    {
                        bitReader.ReadBits(32);
                    }

                    bitReader.ReadBits(6); // NewField6
                    stormBattleLobbyAttributeValue.SecondId = bitReader.ReadInt16Unaligned(); // word - id for s2ml xml
                }

                if (bitReader.ReadBoolean()) // f70
                {
                    bitReader.ReadBits(6); // NewField6
                    bitReader.ReadBits(32); // NewField32
                    bitReader.ReadBits(16); // NewField16
                    bitReader.ReadBits(16); // NewField16
                }

                bitReader.ReadBitArray(1); // f1
                bitReader.ReadBitArray(1); // f1
                bitReader.ReadBitArray(1); // f1

                stormBattleLobbyAttribute.AttributeValues.Add(stormBattleLobbyAttributeValue);
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

                    stormBattleLobbyAttribute.ReplayAttributeEnabledEventType = (ReplayAttributeEventType)bitReader.ReadInt32Unaligned();

                    int enabledAttributeCount = (int)bitReader.ReadBits(12);

                    for (int j = 0; j < enabledAttributeCount; j++)
                    {
                        StormBattleLobbyEnabledAttributeValue stormBattleLobbyEnabledAttributeValue = new()
                        {
                            Value = bitReader.ReadStringFromUnalignedBytes(4), // attribute value
                        };

                        stormBattleLobbyAttribute.EnabledValueAttributes.Add(stormBattleLobbyEnabledAttributeValue);
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

            if (!lobbyAttributesByAttributeEventType.TryAdd(stormBattleLobbyAttribute.ReplayAttributeEventType, stormBattleLobbyAttribute))
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

        /* Locales section */

        uint s2mvCacheHandlesLength = bitReader.ReadBits(6);

        for (int i = 0; i < s2mvCacheHandlesLength; i++)
        {
            s2mFiles.Add(new StormS2mFiles(ref bitReader));
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

        /* Skin collection */

        uint skinCollectionLength = bitReader.ReadBits(16);

        for (int i = 0; i < skinCollectionLength; i++)
        {
            bitReader.ReadAlignedBytes(8);
        }

        uint hasSkinCollectionLength = bitReader.ReadBits(32);

        for (int i = 0; i < hasSkinCollectionLength; i++)
        {
            // 16 is total player slots
            for (int j = 0; j < 16; j++)
            {
                bitReader.ReadAlignedByte();
                bitReader.ReadAlignedByte();
            }
        }

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

        bitReader.ReadBitArray(32);

        uint playersLength = bitReader.ReadBits(5);

        for (int i = 0; i < playersLength; i++)
        {
            bitReader.ReadBitArray(32); // m_Unk1

            uint playerIndex = bitReader.ReadBits(5); // m_userId

            StormPregamePlayer player = replay.ClientListByUserID[playerIndex];
            ToonHandle playerToonHandle = player.ToonHandle ??= new();

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

            // m_Unk2
            bitReader.ReadBitArray(198);

            // m_unk3
            if (bitReader.ReadBoolean())
                bitReader.ReadBitArray(64);

            // m_unk4
            bitReader.ReadBitArray(2); // m_UnkFlags1
            bitReader.ReadBitArray(2); // m_Unk1
            replay.ReplayBuild = bitReader.ReadInt32Unaligned(); // client base build
            bitReader.ReadBitArray(1); // 1 bit left

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

            if (replay.ReplayBuild >= 52860 || replay.ReplayBuild >= 51978)
                player.AccountLevel = (int)bitReader.ReadBits(32);  // in custom games, this is a 0

            if (replay.ReplayBuild >= 69947)
                player.HasActiveBoost = bitReader.ReadBoolean(); // m_hasActiveBoost
        }

        /* _end */

        bitReader.ReadBitArray(64); // sss

        // game mode toon
        AmmIdToonChoice(replay, ref bitReader);

        bitReader.ReadBits(32);
        bitReader.ReadBitArray(87); // b87

        uint a38Length = bitReader.ReadBits(4) + 1;

        for (int i = 0; i < a38Length; i++)
        {
            bitReader.ReadBitArray(38);
        }

        bitReader.ReadBits(1); // unk

        // Neut
        uint neutLength = bitReader.ReadBits(4) + 1;

        for (int i = 0; i < neutLength; i++)
        {
            bitReader.ReadBitArray(32);
        }

        // hero
        uint heroLength2 = bitReader.ReadBits(5);

        for (int i = 0; i < heroLength2; i++)
        {
            bitReader.ReadBits(32); // b32

            uint heroIconLength = bitReader.ReadBits(11);

            for (int j = 0; j < heroIconLength; j++)
            {
                bitReader.ReadStringFromUnalignedBytes(4); // HERO
                bitReader.ReadStringFromUnalignedBytes(4); // ICON

                bitReader.ReadBits(32); // b32
            }

            bitReader.ReadBitArray(11); // b11
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
            bitReader.ReadBitArray(32); // b32
            bitReader.ReadBits(16); // b16
        }

        bitReader.ReadBitArray(23); // b23
        bitReader.ReadBitArray(32); // b32

        bitReader.ReadBitArray(10); // endFiller

        bitReader.ReadBitArray(32); // b32

        uint allHeroesLength = bitReader.ReadBits(10);

        for (int i = 0; i < allHeroesLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4);
            bitReader.ReadBitArray(5); // b5
            bitReader.ReadBitArray(128); // b128
        }

        bitReader.ReadBitArray(2);

        replay.MapLink = GetMapLink(battleNetCachePaths.Last(), s2mFiles.Last());
    }

    // ('_choice', [(0,8),{0:('GlobalValue',6256),1:('ByPlayerValue',6260)}]), #6261
    private static void PlayerSelectedAttributeChoice(ref BitReader bitReader, StormReplayPregame replay, StormBattleLobbyAttribute attribute)
    {
        switch (bitReader.ReadBits(8))
        {
            case 0: // GlobalValue
                {
                    bitReader.ReadBitArray(11); // ValueIndex
                    bitReader.ReadBitArray(1); // IsSet

                    break;
                }

            case 1: // ByPlayerValue
                {
                    uint totalPlayerSlots = bitReader.ReadBits(5);

                    for (int playerSlotIndex = 0; playerSlotIndex < totalPlayerSlots; playerSlotIndex++)
                    {
                        int attributeValueIndex = (int)bitReader.ReadBits(11); // ValueIndex
                        bitReader.ReadBitArray(1); // IsSet

                        SetAttributeEvent(replay, attribute, playerSlotIndex, attributeValueIndex);
                    }

                    break;
                }

            default:
                throw new NotImplementedException();
        }
    }

    private static void SetAttributeEvent(StormReplayPregame replay, StormBattleLobbyAttribute attribute, int playerSlotIndex, int attributeValueIndex)
    {
        switch (attribute.ReplayAttributeEventType)
        {
            case ReplayAttributeEventType.PlayerType:
                {
                    PlayerType playerType = attribute.AttributeValues[attributeValueIndex].Value switch
                    {
                        "Clsd" => PlayerType.Closed,
                        "Open" => PlayerType.Open,
                        "Humn" => PlayerType.Human,
                        "Comp" => PlayerType.Computer,

                        _ => PlayerType.Unknown,
                    };

                    replay.ClientListByUserID[playerSlotIndex].PlayerType = playerType;

                    break;
                }

            case ReplayAttributeEventType.SkinAndSkinTint:
                {
                    replay.ClientListByUserID[playerSlotIndex].PlayerLoadout.SkinAndSkinTintAttributeId = attribute.AttributeValues[attributeValueIndex].Value;

                    break;
                }

            case ReplayAttributeEventType.Banner:
                {
                    replay.ClientListByUserID[playerSlotIndex].PlayerLoadout.BannerAttributeId = attribute.AttributeValues[attributeValueIndex].Value;

                    break;
                }

            case ReplayAttributeEventType.VoiceLine:
                {
                    replay.ClientListByUserID[playerSlotIndex].PlayerLoadout.VoiceLineAttributeId = attribute.AttributeValues[attributeValueIndex].Value;

                    break;
                }

            case ReplayAttributeEventType.Spray:
                {
                    replay.ClientListByUserID[playerSlotIndex].PlayerLoadout.SprayAttributeId = attribute.AttributeValues[attributeValueIndex].Value;

                    break;
                }

            case ReplayAttributeEventType.MountAndMountTint:
                {
                    replay.ClientListByUserID[playerSlotIndex].PlayerLoadout.MountAndMountTintAttributeId = attribute.AttributeValues[attributeValueIndex].Value;

                    break;
                }

            case ReplayAttributeEventType.Announcer:
                {
                    replay.ClientListByUserID[playerSlotIndex].PlayerLoadout.AnnouncerPackAttributeId = attribute.AttributeValues[attributeValueIndex].Value;

                    break;
                }

            case ReplayAttributeEventType.HeroAttributeId:
                {
                    replay.ClientListByUserID[playerSlotIndex].PlayerHero ??= new();

                    replay.ClientListByUserID[playerSlotIndex].PlayerHero!.HeroAttributeId = attribute.AttributeValues[attributeValueIndex].Value;

                    break;
                }

            case ReplayAttributeEventType.HeroLevel:
                {
                    replay.ClientListByUserID[playerSlotIndex].PlayerHero!.HeroLevel = int.Parse(attribute.AttributeValues[attributeValueIndex].Value);

                    break;
                }

            default:
                break;
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

                    break;
                }

            default:
                throw new NotImplementedException();
        }
    }

    private static string? GetMapLink(string battleNetCachePath, StormS2mFiles mapFile)
    {
        ReadOnlySpan<char> firstTwo = mapFile.FileName.AsSpan(0, 2);
        ReadOnlySpan<char> nextTwo = mapFile.FileName.AsSpan(2, 2);

        int lastSeparatorIndex = battleNetCachePath.LastIndexOf(Path.DirectorySeparatorChar) - 6; // 6 is the two parent directories
        if (lastSeparatorIndex < 0)
            throw new ArgumentException("Invalid file path.");

        ReadOnlySpan<char> battleNetCachePathSpan = battleNetCachePath.AsSpan(0, lastSeparatorIndex + 1);

        Span<char> filePathBuffer = stackalloc char[battleNetCachePathSpan.Length + firstTwo.Length + nextTwo.Length + mapFile.FileName.Length + mapFile.FileType.Length + 3]; // 3 = directory seperators + extension .

        // base path
        battleNetCachePathSpan.CopyTo(filePathBuffer);

        // first two directory
        firstTwo.CopyTo(filePathBuffer[battleNetCachePathSpan.Length..]);
        filePathBuffer[battleNetCachePathSpan.Length + 2] = Path.DirectorySeparatorChar;

        // next two directory
        nextTwo.CopyTo(filePathBuffer[(battleNetCachePathSpan.Length + 3)..]);
        filePathBuffer[battleNetCachePathSpan.Length + 5] = Path.DirectorySeparatorChar;

        // file with extension
        mapFile.FileName.CopyTo(filePathBuffer[(battleNetCachePathSpan.Length + 6)..]);
        filePathBuffer[^5] = '.';
        mapFile.FileType.CopyTo(filePathBuffer[^4..]);

        try
        {
            using FileStream fileStream = File.Open(filePathBuffer.ToString(), FileMode.Open, FileAccess.Read, FileShare.Read);

            using JsonDocument jsonDocument = JsonDocument.Parse(fileStream);

            JsonElement root = jsonDocument.RootElement;

            if (root.TryGetProperty("MapInfo", out JsonElement mapInfoElement) &&
                mapInfoElement.TryGetProperty("Properties", out JsonElement propertiesElement) &&
                propertiesElement.TryGetProperty("Loading", out JsonElement loadingElement) &&
                loadingElement.TryGetProperty("MapLink", out JsonElement mapLinkElement))
            {
               return mapLinkElement.GetString();
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
}
