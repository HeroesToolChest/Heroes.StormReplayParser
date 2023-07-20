using System;

namespace Heroes.StormReplayParser.MpqFiles;

internal static class ReplayServerBattlelobby
{
    private const string _exceptionHeader = "battlelobby";

    public static string FileName { get; } = "replay.server.battlelobby";

    public static void Parse(StormReplay replay, ReadOnlySpan<byte> source)
    {
        List<StormBattleLobbyAttribute> stormBattleLobbyAttributes = new();

        BitReader bitReader = new(source, EndianType.BigEndian);

        uint dependenciesLength = bitReader.ReadBits(6);

        for (int i = 0; i < dependenciesLength; i++)
        {
            bitReader.ReadBlobAsString(10);
        }

        // repeat s2ma cache handles for the above
        uint s2maCacheHandleLength = bitReader.ReadBits(6);
        for (int i = 0; i < s2maCacheHandleLength; i++)
        {
            if (bitReader.ReadStringFromAlignedBytes(4) != "s2ma")
                throw new StormParseException($"{_exceptionHeader}: s2ma cache");

            bitReader.ReadAlignedBytes(36);
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
                    stormBattleLobbyAttributeValue.Id = bitReader.ReadInt16Unaligned(); // word - id for s2ml xml
                }

                if (bitReader.ReadBoolean()) // second
                {
                    if (bitReader.ReadBoolean()) // f32
                    {
                        bitReader.ReadBits(32);
                    }

                    bitReader.ReadBits(6); // NewField6
                    bitReader.ReadBits(16); // word - id for s2ml xml
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

                    _ = (ReplayAttributeEventType)bitReader.ReadInt32Unaligned(); // should be still the same

                    attributeCount = (int)bitReader.ReadBits(12);

                    for (int j = 0; j < attributeCount; j++)
                    {
                        StormBattleLobbyEnabledAttributeValue stormBattleLobbyEnabledAttributeValue = new();

                        stormBattleLobbyEnabledAttributeValue.Value = bitReader.ReadStringFromUnalignedBytes(4);  // attribute value

                        stormBattleLobbyAttribute.EnabledValueAttributes.Add(stormBattleLobbyEnabledAttributeValue);
                    }
                }
            }

            // _end
            bitReader.ReadBitArray(5); // NewField5
            bitReader.ReadBitArray(5); // NewField5
            bitReader.ReadBitArray(5); // NewField5

            bitReader.ReadBits(32); // newfield

            uint a170 = bitReader.ReadBits(8);
            for (int y = 0; y < a170; y++)
            {
                bitReader.ReadBitArray(5); // b5
                bitReader.ReadBitArray(165); // b165
                bitReader.ReadBitArray(15); // b15
            }

            // NewField
            bitReader.ReadBitArray(11); // b11
            bitReader.ReadBits(1); // f1
            bitReader.ReadBits(9); // b9

            stormBattleLobbyAttributes.Add(stormBattleLobbyAttribute);
        }

        /* Player attribute selections */

        bitReader.ReadBitArray(30); // NewField
        bitReader.ReadBits(16); // NewField
        bitReader.ReadBitArray(81); // NewField

        count = bitReader.ReadBits(9); // attribute count should be the same as previous

        for (int i = 0; i < count; i++)
        {
            bitReader.ReadInt32Unaligned(); // namespace

            _ = (ReplayAttributeEventType)bitReader.ReadInt32Unaligned();

            PlayerAttributeChoice(ref bitReader);
        }

        /* Locales section */

        uint s2mvCacheHandlesLength = bitReader.ReadBits(6);

        for (int i = 0; i < s2mvCacheHandlesLength; i++)
        {
            if (bitReader.ReadStringFromAlignedBytes(4) != "s2mv")
                throw new StormParseException($"{_exceptionHeader}: s2mv");

            bitReader.ReadAlignedBytes(36);
        }

        uint localesLength = bitReader.ReadBits(5); // number of locales

        for (int i = 0; i < localesLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4);

            uint s2mlSize = bitReader.ReadBits(6); // number of s2ml in locale

            for (int j = 0; j < s2mlSize; j++)
            {
                if (bitReader.ReadStringFromAlignedBytes(4) != "s2ml")
                    throw new StormParseException($"{_exceptionHeader}: s2ml");

                bitReader.ReadAlignedBytes(36);
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
                if (bitReader.ReadStringFromAlignedBytes(4) != "s2mv")
                    throw new StormParseException($"{_exceptionHeader}: s2mv");

                bitReader.ReadAlignedBytes(36);
            }

            bitReader.ReadBitArray(1); // flag

            // string toon
            bitReader.ReadBits(8); // region
            bitReader.ReadStringFromUnalignedBytes(4); // programId
            bitReader.ReadBits(32); // realm

            int idLength = (int)bitReader.ReadBits(7) + 2;
            bitReader.ReadStringFromAlignedBytes(idLength); // m_globalName

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
                    bitReader.ReadBitArray(32); // b32
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
            if (bitReader.ReadStringFromAlignedBytes(4) != "s2mh")
                throw new StormParseException($"{_exceptionHeader}: s2mh");

            bitReader.ReadAlignedBytes(36);
        }


        /* Skin collection */
        uint skinCollectionLength = bitReader.ReadBits(16);

        for (int i = 0; i < skinCollectionLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(8);
        }

        uint hasSkinCollectionLength = bitReader.ReadBits(32);

        for (int i = 0; i < hasSkinCollectionLength; i++)
        {
            // 16 is total player slots
            for (int j = 0; j < 16; j++)
            {
                bitReader.ReadAlignedByte();
                bitReader.ReadAlignedByte(); // more likely a boolean to get the value

                if (replay.ReplayBuild < 55929)
                {
                    // when the identifier is a string, set the value to the appropriate array index
                }
            }
        }


        /* filler */
        //if (replay.ReplayBuild >= 85027)
        //{
        //    // m_disabledHeroList
        //    uint disabledHeroListLength = bitReader.ReadBits(8);

        //    bitReader.EndianType = EndianType.LittleEndian;

        //    for (int i = 0; i < disabledHeroListLength; i++)
        //    {
        //        string disabledHeroAttributeId = bitReader.ReadStringFromBits(32);

        //        if (replay.DisabledHeroAttributeIdList.Count == 0)
        //            replay.DisabledHeroAttributeIdList.Add(disabledHeroAttributeId);
        //    }

        //    bitReader.EndianType = EndianType.BigEndian;
        //}

        //replay.RandomValue = bitReader.ReadBits(32); // m_randomSeed

        bitReader.ReadBitArray(72);
        //bitReader.ReadBitArray(32);


        uint playersLength = bitReader.ReadBits(5);

        for (int i = 0; i < playersLength; i++)
        {
            // m_Unk1
            bitReader.ReadBitArray(32); // m_Unk1
            uint slotId = bitReader.ReadBits(5);

            // toon handle
            bitReader.ReadBits(8); // region
            bitReader.ReadStringFromUnalignedBytes(4); // programId
            bitReader.ReadBits(32); // realm
            bitReader.ReadLongBits(64); // id

            // string toon T:xxxxxxxx
            bitReader.ReadBits(8); // region
            bitReader.ReadStringFromUnalignedBytes(4); // programId
            bitReader.ReadBits(32); // realm

            int idLength = (int)bitReader.ReadBits(7) + 2;
            bitReader.ReadStringFromAlignedBytes(idLength); // m_globalName

            bitReader.ReadBitArray(1); // m_flags
            bitReader.ReadBitArray(2); // m_Unk4

            if (bitReader.ReadBoolean())
            {
                bitReader.ReadBitArray(2); // m_Unk1

                // string toon T:xxxxxxxx
                bitReader.ReadBits(8); // region
                bitReader.ReadStringFromUnalignedBytes(4); // programId
                bitReader.ReadBits(32); // realm

                int idLength2 = (int)bitReader.ReadBits(7) + 2;
                bitReader.ReadStringFromAlignedBytes(idLength2); // m_globalName

                bitReader.ReadBitArray(1); // m_flags
            }

            // m_Unk2
            bitReader.ReadBitArray(198);

            // m_Unk3
            if (bitReader.ReadBoolean())
            {
                bitReader.ReadBitArray(64);
            }

            // m_Unk4
            bitReader.ReadBitArray(2); // m_UnkFlags1
            bitReader.ReadBitArray(35); // m_Unk1

            bitReader.ReadBitArray(4); // m_Skins2

            // m_PartyInfo
            if (bitReader.ReadBoolean())
            {
                bitReader.ReadBitArray(64);
            }

            // m_TagInfo
            if (bitReader.ReadBoolean())
            {
                string battleTagName = bitReader.ReadBlobAsString(7);
            }

            int playerLevel = (int)bitReader.ReadBits(32);  // in custom games, this is a 0

            // m_UnkFlags
            bitReader.ReadBitArray(1);
        }

        /* _end */

        bitReader.ReadBitArray(62); // sss

        // m_toon
        ChoiceSection3(ref bitReader);

        bitReader.ReadBitArray(87); // b87

        uint length = bitReader.ReadBits(5) + 1;

        for (int i = 0; i < length; i++)
        {
            bitReader.ReadBitArray(38);
        }

        // Neut
        length = bitReader.ReadBits(4) + 1;

        for (int i = 0; i < length; i++)
        {
            bitReader.ReadBitArray(32);
        }

        uint heroLength2 = bitReader.ReadBits(5);

        for (int i = 0; i < heroLength2; i++)
        {
            bitReader.ReadBits(32);

            uint heroIconLength = bitReader.ReadBits(11);

            for (int j = 0; j < heroIconLength; j++)
            {
                bitReader.ReadStringFromUnalignedBytes(4); // HERO
                bitReader.ReadStringFromUnalignedBytes(4); // ICON

                bitReader.ReadBits(32);
            }

            bitReader.ReadBitArray(11); // b11
        }

        // CSTM


        return;

        #region
        //    uint collectionSize;

        //    // strings gone starting with build (ptr) 55929
        //    if (replay.ReplayBuild >= 48027)
        //        collectionSize = bitReader.ReadBits(16);
        //    else
        //        collectionSize = bitReader.ReadBits(32);

        //    for (uint i = 0; i < collectionSize; i++)
        //    {
        //        if (replay.ReplayBuild >= 55929)
        //            bitReader.ReadAlignedBytes(8); // most likey an identifier for the item; first six bytes are 0x00
        //        else
        //            bitReader.ReadStringFromAlignedBytes(bitReader.ReadAlignedByte());
        //    }

        //    // use to determine if the collection item is usable by the player (owns/free to play/internet cafe)
        //    if (bitReader.ReadBits(32) != collectionSize)
        //        throw new StormParseException($"{_exceptionHeader}: collection difference");

        //    for (int i = 0; i < collectionSize; i++)
        //    {
        //        // 16 is total player slots
        //        for (int j = 0; j < 16; j++)
        //        {
        //            bitReader.ReadAlignedByte();
        //            bitReader.ReadAlignedByte(); // more likely a boolean to get the value

        //            if (replay.ReplayBuild < 55929)
        //            {
        //                // when the identifier is a string, set the value to the appropriate array index
        //            }
        //        }
        //    }

        //    // Player info

        //    if (replay.ReplayBuild <= 47479 || replay.ReplayBuild == 47801 || replay.ReplayBuild == 47903)
        //    {
        //        // Builds that are not yet supported for detailed parsing
        //        // build 47801 is a ptr build that had new data in the battletag section, the data was changed in 47944 (patch for 47801)
        //        // GetBattleTags(replay, source);
        //        return;
        //    }

        //    if (replay.ReplayBuild >= 85027)
        //    {
        //        // m_disabledHeroList
        //        uint disabledHeroListLength = bitReader.ReadBits(8);

        //        bitReader.EndianType = EndianType.LittleEndian;

        //        for (int i = 0; i < disabledHeroListLength; i++)
        //        {
        //            string disabledHeroAttributeId = bitReader.ReadStringFromBits(32);

        //            if (replay.DisabledHeroAttributeIdList.Count == 0)
        //                replay.DisabledHeroAttributeIdList.Add(disabledHeroAttributeId);
        //        }

        //        bitReader.EndianType = EndianType.BigEndian;
        //    }

        //    replay.RandomValue = bitReader.ReadBits(32); // m_randomSeed

        //    bitReader.ReadAlignedBytes(4);

        //    uint playerListLength = bitReader.ReadBits(5);

        //    if (replay.PlayersWithObserversCount != playerListLength)
        //        throw new StormParseException($"{_exceptionHeader}: mismatch on player list length - {playerListLength} to {replay.PlayersWithObserversCount}");
        #endregion
        //    for (uint i = 0; i < playerListLength; i++)
        //    {
        //        bitReader.ReadBits(32);

        //        uint playerIndex = bitReader.ReadBits(5); // player index

        //        StormPlayer player = replay.ClientListByUserID[playerIndex];

        //        // toon handle
        //        uint playerRegion = bitReader.ReadBits(8); // m_region

        //        bitReader.EndianType = EndianType.LittleEndian;
        //        if (bitReader.ReadBits(32) != 1869768008) // m_programId
        //            throw new StormParseException($"{_exceptionHeader}: Not Hero");
        //        bitReader.EndianType = EndianType.BigEndian;

        //        uint playerRealm = bitReader.ReadBits(32); // m_realm
        //        long playerId = bitReader.ReadLongBits(64); // m_id

        //        if (player.PlayerType == PlayerType.Human)
        //        {
        //            if (player.ToonHandle!.Region != playerRegion)
        //                throw new StormParseException($"{_exceptionHeader}: Mismatch on player region");
        //            if (player.ToonHandle.Realm != playerRealm)
        //                throw new StormParseException($"{_exceptionHeader}: Mismatch on player realm");
        //            if (player.ToonHandle.Id != playerId)
        //                throw new StormParseException($"{_exceptionHeader}: Mismatch on player id");
        //        }
        //        else if (player.PlayerType == PlayerType.Observer || player.PlayerType == PlayerType.Unknown)
        //        {
        //            // observers don't have the information carried over to the details file and sometimes not the initdata file
        //            player.ToonHandle ??= new ToonHandle();

        //            player.ToonHandle.Region = (int)playerRegion;
        //            player.ToonHandle.ProgramId = 1869768008;
        //            player.ToonHandle.Realm = (int)playerRealm;
        //            player.ToonHandle.Id = (int)playerId;
        //            player.PlayerType = PlayerType.Observer;
        //            player.Team = StormTeam.Observer;
        //        }

        //        // toon handle again but with T_ shortcut
        //        bitReader.ReadBits(8); // m_region

        //        bitReader.EndianType = EndianType.LittleEndian;
        //        if (bitReader.ReadBits(32) != 1869768008) // m_programId (Hero)
        //            throw new StormParseException($"{_exceptionHeader}: Not Hero");
        //        bitReader.EndianType = EndianType.BigEndian;

        //        bitReader.ReadBits(32); // m_realm

        //        int idLength = (int)bitReader.ReadBits(7) + 2;

        //        player.ToonHandle ??= new ToonHandle();
        //        player.ToonHandle.ShortcutId = bitReader.ReadStringFromAlignedBytes(idLength);

        //        bitReader.ReadBits(6);

        //        if (replay.ReplayBuild <= 47479)
        //        {
        //            // toon handle repeat again with T_ shortcut
        //            bitReader.ReadBits(8); // m_region
        //            if (bitReader.ReadStringFromBits(32) != "Hero") // m_programId
        //                throw new StormParseException($"{_exceptionHeader}: Not Hero");
        //            bitReader.ReadBits(32); // m_realm

        //            idLength = (int)bitReader.ReadBits(7) + 2;
        //            if (player.ToonHandle.ShortcutId != bitReader.ReadStringFromAlignedBytes(idLength))
        //                throw new StormParseException($"{_exceptionHeader}: Duplicate shortcut id does not match");

        //            bitReader.ReadBits(6);
        //        }

        //        bitReader.ReadBits(2);
        //        bitReader.ReadUnalignedBytes(25);
        //        bitReader.ReadBits(24);

        //        // ai games have 8 more bytes somewhere around here
        //        if (replay.GameMode == StormGameMode.Cooperative)
        //            bitReader.ReadUnalignedBytes(8);

        //        bitReader.ReadBits(7);

        //        if (!bitReader.ReadBoolean())
        //        {
        //            // repeat of the collection section above
        //            if (replay.ReplayBuild > 51609 || replay.ReplayBuild == 47903 || replay.ReplayBuild == 47479)
        //            {
        //                bitReader.ReadBitArray(bitReader.ReadBits(12));
        //            }
        //            else if (replay.ReplayBuild > 47219)
        //            {
        //                // each byte has a max value of 0x7F (127)
        //                bitReader.ReadUnalignedBytes((int)bitReader.ReadBits(15) * 2);
        //            }
        //            else
        //            {
        //                bitReader.ReadBitArray(bitReader.ReadBits(9));
        //            }

        //            bitReader.ReadBoolean();
        //        }

        //        bool isSilenced = bitReader.ReadBoolean(); // m_hasSilencePenalty
        //        if (player.PlayerType == PlayerType.Observer)
        //            player.IsSilenced = isSilenced;

        //        if (replay.ReplayBuild >= 61718)
        //        {
        //            bitReader.ReadBoolean();
        //            bool isVoiceSilenced = bitReader.ReadBoolean(); // m_hasVoiceSilencePenalty
        //            if (player.PlayerType == PlayerType.Observer)
        //                player.IsVoiceSilenced = isVoiceSilenced;
        //        }

        //        if (replay.ReplayBuild >= 66977)
        //        {
        //            bool isBlizzardStaff = bitReader.ReadBoolean(); // m_isBlizzardStaff
        //            if (player.PlayerType == PlayerType.Observer)
        //                player.IsBlizzardStaff = isBlizzardStaff;
        //        }

        //        if (bitReader.ReadBoolean()) // is player in party
        //            player.PartyValue = bitReader.ReadLongBits(64); // players in same party will have the same exact 8 bytes of data

        //        bitReader.ReadBoolean();

        //        string battleTagName = bitReader.ReadBlobAsString(7);
        //        int poundIndex = battleTagName.IndexOf('#');

        //        if (!string.IsNullOrEmpty(battleTagName) && poundIndex < 0)
        //            throw new StormParseException($"{_exceptionHeader}: Invalid battletag");

        //        // check if there is no tag number
        //        if (poundIndex >= 0)
        //        {
        //            ReadOnlySpan<char> namePart = battleTagName.AsSpan(0, poundIndex);
        //            if (!namePart.SequenceEqual(player.Name))
        //                throw new StormParseException($"{_exceptionHeader}: Mismatch on battletag name with player name");
        //        }

        //        player.BattleTagName = battleTagName;

        //        if (replay.ReplayBuild >= 52860 || (replay.ReplayVersion.Major == 2 && replay.ReplayBuild >= 51978))
        //            player.AccountLevel = (int)bitReader.ReadBits(32);  // in custom games, this is a 0

        //        if (replay.ReplayBuild >= 69947)
        //        {
        //            bool hasActiveBoost = bitReader.ReadBoolean(); // m_hasActiveBoost
        //            if (player.PlayerType == PlayerType.Observer)
        //                player.HasActiveBoost = hasActiveBoost;
        //        }
        //    }

        //    replay.IsBattleLobbyPlayerInfoParsed = true;


        //    // skip to the optional 8th Abat attribute
        //    //for (; ; )
        //    //{
        //    //    if (bitReader.ReadStringFromBits(32) == "tabA") // Abat 1096966516 0x41626174
        //    //    {
        //    //        List<string> itemsss = new();
        //    //        for (int i = 0; i < 100; i++)
        //    //        {
        //    //            bitReader.ReadBits(29); // no
        //    //            itemsss.Add(bitReader.ReadStringFromBits(32));
        //    //        }

        //    //        break;
        //    //    }
        //    //    else
        //    //    {
        //    //        bitReader.BitReversement(31);
        //    //    }
        //    //}
    }

    // ('_choice', [(0,8),{0:('GlobalValue',6256),1:('ByPlayerValue',6260)}]), #6261
    private static void PlayerAttributeChoice(ref BitReader bitReader)
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
                    uint count = bitReader.ReadBits(5);

                    for (int i = 0; i < count; i++)
                    {
                        bitReader.ReadBitArray(11); // ValueIndex
                        bitReader.ReadBitArray(1); // IsSet
                    }

                    break;
                }

            default:
                throw new NotImplementedException();
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
    private static void ChoiceSection3(ref BitReader bitReader)
    {
        switch (bitReader.ReadBits(1))
        {
            case 0: // filler
                {
                    bitReader.ReadBitArray(16);

                    // toon handle
                    bitReader.ReadBits(8); // region
                    bitReader.ReadStringFromUnalignedBytes(4); // programId
                    bitReader.ReadBits(32); // realm
                    bitReader.ReadLongBits(64); // id

                    break;
                }

            case 1: // None
                {
                    break;
                }

            default:
                throw new NotImplementedException();
        }
    }
}
