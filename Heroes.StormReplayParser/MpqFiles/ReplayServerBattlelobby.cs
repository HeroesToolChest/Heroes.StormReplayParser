namespace Heroes.StormReplayParser.MpqFiles;

internal static class ReplayServerBattlelobby
{
    private const string _exceptionHeader = "battlelobby";

    public static string FileName { get; } = "replay.server.battlelobby";

    public static void Parse(StormReplay replay, ReadOnlySpan<byte> source)
    {
        BitReader bitReader = new(source, EndianType.BigEndian);

        uint dependenciesLength = bitReader.ReadBits(6);

        for (int i = 0; i < dependenciesLength; i++)
        {
            bitReader.ReadBlobAsString(10);
        }

        // s2ma cache handles
        uint s2maCacheHandlesLength = bitReader.ReadBits(6);
        for (int i = 0; i < s2maCacheHandlesLength; i++)
        {
            if (bitReader.ReadStringFromBytes(4) != "s2ma")
                throw new StormParseException($"{_exceptionHeader}: s2ma cache");

            bitReader.ReadAlignedBytes(36);
        }

        /*source.ReadAlignedBytes(94);
        if (source.ReadStringFromBytes(4) != "Clsd")
           throw new StormParseException($"{ExceptionHeader}: Clsd"); */

        //// we're just going to skip all the way down to the s2mh

        bitReader.AlignToByte();

        // skip to the first hero attribute section

        bitReader.EndianType = EndianType.LittleEndian;

        for (; ;)
        {
            List<string> items = new();
            if (bitReader.ReadStringFromBits(32) == "Abat") // tabA [BE - 1096966516] 0x41626174
            {
                bitReader.BitReversement(32);

                // back to get the first item
                bitReader.BitReversement(29);
                bitReader.BitReversement(32);

                bitReader.ReadStringFromBits(32); // first actual item is \0\0\0\0
                bitReader.BitReversement(32);

                bitReader.EndianType = EndianType.BigEndian;
                bitReader.BitReversement(12);
                uint heroAttributeSizeS1 = bitReader.ReadBits(12); // get collection size
                bitReader.EndianType = EndianType.LittleEndian;

                for (int i = 0; i < heroAttributeSizeS1; i++)
                {
                    items.Add(bitReader.ReadStringFromBits(32));
                    bitReader.ReadBits(29);
                }

                bitReader.EndianType = EndianType.BigEndian;

                break;
            }
            else
            {
                bitReader.BitReversement(31);
            }
        }

        // skip to the second hero attribute section

        bitReader.EndianType = EndianType.LittleEndian;

        for (; ;)
        {
            List<string> items = new();
            if (bitReader.ReadStringFromBits(32) == "Abat") // tabA [BE - 1096966516] 0x41626174
            {
                bitReader.BitReversement(32);

                // back to get the first item
                bitReader.BitReversement(29);
                bitReader.BitReversement(32);

                bitReader.ReadStringFromBits(32); // first actual item is \0\0\0\0
                bitReader.BitReversement(32);

                bitReader.EndianType = EndianType.BigEndian;
                bitReader.BitReversement(12);
                uint heroAttributeSizeS1 = bitReader.ReadBits(12); // get collection size
                bitReader.EndianType = EndianType.LittleEndian;

                for (int i = 0; i < heroAttributeSizeS1; i++)
                {
                    items.Add(bitReader.ReadStringFromBits(32));
                    bitReader.ReadBits(29);
                }

                bitReader.EndianType = EndianType.BigEndian;

                break;
            }
            else
            {
                bitReader.BitReversement(31);
            }
        }

        // skip to the banner attribute section

        bitReader.EndianType = EndianType.LittleEndian;

        for (; ;)
        {
            List<string> items = new();
            if (bitReader.ReadStringFromBits(32) == "Rand") // dnaR
            {
                bitReader.BitReversement(32);

                // back to get the first item
                bitReader.BitReversement(29);
                bitReader.BitReversement(32);

                bitReader.ReadStringFromBits(32); // first actual item is \0\0\0\0
                bitReader.BitReversement(32);

                bitReader.EndianType = EndianType.BigEndian;
                bitReader.BitReversement(12);
                uint bannerAttributeSizeS1 = bitReader.ReadBits(12); // get collection size
                bitReader.EndianType = EndianType.LittleEndian;

                for (int i = 0; i < 22; i++)
                {
                    items.Add(bitReader.ReadStringFromBits(32));
                    bitReader.ReadBits(29);
                }

                // hmmmmmmmmmmmmmmmm
                uint adsf = bitReader.ReadBits(4);
                items.Add(bitReader.ReadStringFromBits(32)); // hVH8  8HVh

                uint rxl = bitReader.ReadBits(2);
                for (int i = 0; i < bannerAttributeSizeS1 - 23; i++)
                {
                    items.Add(bitReader.ReadStringFromBits(32));
                    bitReader.ReadBits(29);
                }

                bitReader.EndianType = EndianType.BigEndian;

                break;
            }
            else
            {
                bitReader.BitReversement(31);
            }
        }

        // skip to the hero skin attribute section

        bitReader.EndianType = EndianType.LittleEndian;

        for (; ;)
        {
            List<string> items = new();
            if (bitReader.ReadStringFromBits(32) == "But3") // 3tuB
            {
                bitReader.BitReversement(32);

                // back to get the first item
                bitReader.BitReversement(29);
                bitReader.BitReversement(32);

                string ko = bitReader.ReadStringFromBits(32);
                bitReader.BitReversement(32);

                // back to get the first item
                bitReader.BitReversement(29);
                bitReader.BitReversement(32);

                string ko2 = bitReader.ReadStringFromBits(32);
                bitReader.BitReversement(32);

                // back to get the first item
                bitReader.BitReversement(29);
                bitReader.BitReversement(32);

                string ko3 = bitReader.ReadStringFromBits(32);

                bitReader.BitReversement(32);

                // back to get the first item
                bitReader.BitReversement(29);
                bitReader.BitReversement(32);

                string ko4 = bitReader.ReadStringFromBits(32);
                bitReader.EndianType = EndianType.BigEndian;
                bitReader.BitReversement(32);

                // back to get the first item
                bitReader.BitReversement(29);
                bitReader.BitReversement(32);

                string ko5 = bitReader.ReadStringFromBits(32);

                bitReader.BitReversement(32);

                // back to get the first item
                bitReader.BitReversement(29);
                bitReader.BitReversement(32);

                string ko6 = bitReader.ReadStringFromBits(32);

                bitReader.BitReversement(32);

                // back to get the first item
                bitReader.BitReversement(29);
                bitReader.BitReversement(32);

                bitReader.ReadStringFromBits(32); // first actual item is \0\0\0\0
                bitReader.BitReversement(32);

                bitReader.EndianType = EndianType.BigEndian;
                bitReader.BitReversement(12);
                uint heroSkinAttributeSizeS1 = bitReader.ReadBits(12); // get collection size
                bitReader.EndianType = EndianType.LittleEndian;

                for (int i = 0; i < heroSkinAttributeSizeS1; i++) // 1564
                {
                    items.Add(bitReader.ReadStringFromBits(32));
                    if (bitReader.ReadBoolean())
                        bitReader.ReadBits(28);
                    else
                        bitReader.ReadBits(5);
                }

                // finder
                //for (int i = 1; i < 999; i++)
                //{
                //    bitReader.ReadBits(i);
                //    string xvcdvcx = bitReader.ReadStringFromBits(32);
                //    bitReader.BitReversement(32);
                //    bitReader.BitReversement(i);
                //}

                bitReader.EndianType = EndianType.BigEndian;

                break;
            }
            else
            {
                bitReader.BitReversement(31);
            }
        }

        // skip to the third hero attribute section

        bitReader.EndianType = EndianType.LittleEndian;

        for (; ;)
        {
            List<string> items = new();
            if (bitReader.ReadStringFromBits(32) == "Abat") // tabA [BE - 1096966516] 0x41626174
            {
                bitReader.BitReversement(32);

                // back to get the first item
                bitReader.BitReversement(29);
                bitReader.BitReversement(32);

                bitReader.ReadStringFromBits(32); // first actual item is \0\0\0\0
                bitReader.BitReversement(32);

                bitReader.EndianType = EndianType.BigEndian;
                bitReader.BitReversement(12);
                uint heroAttributeSizeS1 = bitReader.ReadBits(12); // get collection size
                bitReader.EndianType = EndianType.LittleEndian;

                for (int i = 0; i < heroAttributeSizeS1; i++)
                {
                    items.Add(bitReader.ReadStringFromBits(32));
                    bitReader.ReadBits(29);
                }

                bitReader.EndianType = EndianType.BigEndian;

                break;
            }
            else
            {
                bitReader.BitReversement(31);
            }
        }

        // skip to the fourth hero attribute section

        bitReader.EndianType = EndianType.LittleEndian;

        for (; ;)
        {
            List<string> items = new();
            if (bitReader.ReadStringFromBits(32) == "Abat") // tabA [BE - 1096966516] 0x41626174
            {
                bitReader.BitReversement(32);

                // back to get the first item
                bitReader.BitReversement(29);
                bitReader.BitReversement(32);

                bitReader.ReadStringFromBits(32); // first actual item is \0\0\0\0
                bitReader.BitReversement(32);

                bitReader.EndianType = EndianType.BigEndian;
                bitReader.BitReversement(12);
                uint heroAttributeSizeS1 = bitReader.ReadBits(12); // get collection size
                bitReader.EndianType = EndianType.LittleEndian;

                for (int i = 0; i < heroAttributeSizeS1; i++)
                {
                    items.Add(bitReader.ReadStringFromBits(32));
                    bitReader.ReadBits(29);
                }

                bitReader.EndianType = EndianType.BigEndian;

                break;
            }
            else
            {
                bitReader.BitReversement(31);
            }
        }

        // skip to the voiceline attribute section
        for (; ;)
        {
            List<string> items = new();
            if (bitReader.ReadStringFromBits(32) == "10BA") // 10BA
            {
                bitReader.BitReversement(32);

                // back to get the first item
                bitReader.BitReversement(29);
                bitReader.BitReversement(32);

                string ko = bitReader.ReadStringFromBits(32);
                bitReader.BitReversement(32);

                // back to get the first item
                bitReader.BitReversement(29);
                bitReader.BitReversement(32);

                string ko2 = bitReader.ReadStringFromBits(32); // \0\0\0\0
                bitReader.BitReversement(32);

                bitReader.EndianType = EndianType.BigEndian;
                bitReader.BitReversement(12);
                uint voicelineAttributeSizeS1 = bitReader.ReadBits(12); // get collection size
                bitReader.EndianType = EndianType.LittleEndian;

                for (int i = 0; i < voicelineAttributeSizeS1; i++) // 1564
                {
                    items.Add(bitReader.ReadStringFromBits(32));
                    bitReader.ReadBits(29);
                }

                bitReader.EndianType = EndianType.BigEndian;

                break;
            }
            else
            {
                bitReader.BitReversement(31);
            }
        }

        // skip to the fifth hero attribute section

        bitReader.EndianType = EndianType.LittleEndian;

        for (; ;)
        {
            List<string> items = new();
            if (bitReader.ReadStringFromBits(32) == "Abat") // tabA [BE - 1096966516] 0x41626174
            {
                bitReader.BitReversement(32);

                // back to get the first item
                bitReader.BitReversement(29);
                bitReader.BitReversement(32);

                bitReader.ReadStringFromBits(32); // first actual item is \0\0\0\0
                bitReader.BitReversement(32);

                bitReader.EndianType = EndianType.BigEndian;
                bitReader.BitReversement(12);
                uint heroAttributeSizeS1 = bitReader.ReadBits(12); // get collection size
                bitReader.EndianType = EndianType.LittleEndian;

                for (int i = 0; i < heroAttributeSizeS1; i++)
                {
                    items.Add(bitReader.ReadStringFromBits(32));
                    bitReader.ReadBits(29);
                }

                bitReader.EndianType = EndianType.BigEndian;

                break;
            }
            else
            {
                bitReader.BitReversement(31);
            }
        }

        // skip to the sixth hero attribute section

        bitReader.EndianType = EndianType.LittleEndian;

        for (; ;)
        {
            List<string> items = new();
            if (bitReader.ReadStringFromBits(32) == "Abat") // tabA [BE - 1096966516] 0x41626174
            {
                bitReader.BitReversement(32);

                // back to get the first item
                bitReader.BitReversement(29);
                bitReader.BitReversement(32);

                bitReader.ReadStringFromBits(32); // first actual item is \0\0\0\0
                bitReader.BitReversement(32);

                bitReader.EndianType = EndianType.BigEndian;
                bitReader.BitReversement(12);
                uint heroAttributeSizeS1 = bitReader.ReadBits(12); // get collection size
                bitReader.EndianType = EndianType.LittleEndian;

                for (int i = 0; i < heroAttributeSizeS1; i++)
                {
                    items.Add(bitReader.ReadStringFromBits(32));
                    bitReader.ReadBits(29);
                }

                bitReader.EndianType = EndianType.BigEndian;

                break;
            }
            else
            {
                bitReader.BitReversement(31);
            }
        }

        // skip to the seventh hero attribute section

        bitReader.EndianType = EndianType.LittleEndian;

        for (; ;)
        {
            List<string> items = new();
            if (bitReader.ReadStringFromBits(32) == "Abat") // tabA [BE - 1096966516] 0x41626174
            {
                bitReader.BitReversement(32);

                // back to get the first item
                bitReader.BitReversement(29);
                bitReader.BitReversement(32);

                bitReader.ReadStringFromBits(32); // first actual item is \0\0\0\0
                bitReader.BitReversement(32);

                bitReader.EndianType = EndianType.BigEndian;
                bitReader.BitReversement(12);
                uint heroAttributeSizeS1 = bitReader.ReadBits(12); // get collection size
                bitReader.EndianType = EndianType.LittleEndian;

                for (int i = 0; i < heroAttributeSizeS1; i++)
                {
                    items.Add(bitReader.ReadStringFromBits(32));
                    bitReader.ReadBits(29);
                }

                bitReader.EndianType = EndianType.BigEndian;

                break;
            }
            else
            {
                bitReader.BitReversement(31);
            }
        }

        // skip to the spray attribute section

        bitReader.EndianType = EndianType.LittleEndian;

        for (; ;)
        {
            List<string> items = new();
            if (bitReader.ReadStringFromBits(32) == "Rand")
            {
                bitReader.BitReversement(32);

                // back to get the first item
                bitReader.BitReversement(29);
                bitReader.BitReversement(32);

                bitReader.ReadStringFromBits(32); // first actual item is \0\0\0\0
                bitReader.BitReversement(32);

                bitReader.EndianType = EndianType.BigEndian;
                bitReader.BitReversement(12);
                uint sprayAttributeSizeS1 = bitReader.ReadBits(12); // get collection size
                bitReader.EndianType = EndianType.LittleEndian;

                for (int i = 0; i < sprayAttributeSizeS1; i++)
                {
                    items.Add(bitReader.ReadStringFromBits(32));
                    bitReader.ReadBits(29);
                }

                bitReader.EndianType = EndianType.BigEndian;

                break;
            }
            else
            {
                bitReader.BitReversement(31);
            }
        }

        // skip to the mount attribute section

        bitReader.EndianType = EndianType.LittleEndian;

        for (; ;)
        {
            List<string> items = new();
            if (bitReader.ReadStringFromBits(32) == "Tilu")
            {
                bitReader.BitReversement(32);

                // back to get the first item
                bitReader.BitReversement(29);
                bitReader.BitReversement(32);

                string ko = bitReader.ReadStringFromBits(32);
                bitReader.BitReversement(32);

                // back to get the first item
                bitReader.BitReversement(29);
                bitReader.BitReversement(32);

                string ko2 = bitReader.ReadStringFromBits(32);
                bitReader.BitReversement(32);

                // back to get the first item
                bitReader.BitReversement(29);
                bitReader.BitReversement(32);

                string ko3 = bitReader.ReadStringFromBits(32);

                bitReader.BitReversement(32);

                // back to get the first item
                bitReader.BitReversement(29);
                bitReader.BitReversement(32);

                string ko4 = bitReader.ReadStringFromBits(32);
                bitReader.EndianType = EndianType.BigEndian;
                bitReader.BitReversement(32);

                // back to get the first item
                bitReader.BitReversement(29);
                bitReader.BitReversement(32);

                string ko5 = bitReader.ReadStringFromBits(32);

                bitReader.BitReversement(32);

                // back to get the first item
                bitReader.BitReversement(29);
                bitReader.BitReversement(32);

                string ko6 = bitReader.ReadStringFromBits(32);

                bitReader.BitReversement(32);

                // back to get the first item
                bitReader.BitReversement(29);
                bitReader.BitReversement(32);

                bitReader.ReadStringFromBits(32); // first actual item is \0\0\0\0
                bitReader.BitReversement(32);

                bitReader.EndianType = EndianType.BigEndian;
                bitReader.BitReversement(12);
                uint mountAttributeSizeS1 = bitReader.ReadBits(12); // get collection size
                bitReader.EndianType = EndianType.LittleEndian;

                for (int i = 0; i < mountAttributeSizeS1; i++)
                {
                    items.Add(bitReader.ReadStringFromBits(32));
                    if (bitReader.ReadBoolean())
                        bitReader.ReadBits(28);
                    else
                        bitReader.ReadBits(5);
                }

                bitReader.EndianType = EndianType.BigEndian;

                break;
            }
            else
            {
                bitReader.BitReversement(31);
            }
        }

        // skip to the announcer attribute section

        bitReader.EndianType = EndianType.LittleEndian;

        for (; ;)
        {
            List<string> items = new();
            if (bitReader.ReadStringFromBits(32) == "Rand")
            {
                bitReader.BitReversement(32);

                // back to get the first item
                bitReader.BitReversement(29);
                bitReader.BitReversement(32);

                string ko = bitReader.ReadStringFromBits(32);
                bitReader.BitReversement(32);

                // back to get the first item
                bitReader.BitReversement(29);
                bitReader.BitReversement(32);

                bitReader.ReadStringFromBits(32); // first actual item is \0\0\0\0
                bitReader.BitReversement(32);

                bitReader.EndianType = EndianType.BigEndian;
                bitReader.BitReversement(12);
                uint announcerAttributeSizeS1 = bitReader.ReadBits(12); // get collection size
                bitReader.EndianType = EndianType.LittleEndian;

                for (int i = 0; i < announcerAttributeSizeS1; i++)
                {
                    items.Add(bitReader.ReadStringFromBits(32));
                    if (bitReader.ReadBoolean())
                        bitReader.ReadBits(28);
                    else
                        bitReader.ReadBits(5);
                }

                bitReader.EndianType = EndianType.BigEndian;

                break;
            }
            else
            {
                bitReader.BitReversement(31);
            }
        }

        /*for (; ;)
        {
            if (source.ReadStringFromBytes(4) == "s2mh")
            {
                BitReader.Index -= 4;
                break;
            }
            else
            {
                BitReader.Index -= 3;
            }
        }*/

        /*
        for (; ; )
        {
            if (source.ReadStringFromBytes(4) == "s2mv")
            {
                BitReader.Index -= 4;
                break;
            }
            else
            {
                BitReader.Index -= 3;
            }
        }*/

        // first hit
        /*
        BitReader.Index -= 1;

        uint s2mvCacheHandlesLength = source.ReadBits(8);

        for (int i = 0; i < s2mvCacheHandlesLength; i++)
        {
            if (source.ReadStringFromBytes(4) != "s2mv")
                throw new StormParseException($"{ExceptionHeader}: s2mv cache");

            source.ReadAlignedBytes(36);
        }

        uint localeCount = source.ReadBits(5);

        for (int i = 0; i < localeCount; i++)
        {
            source.ReadStringFromBits(32); // locale

            uint s2mlCacheHandlesLength = source.ReadBits(6);

            for (int j = 0; j < s2mlCacheHandlesLength; j++)
            {
                if (source.ReadStringFromBytes(4) != "s2ml")
                    throw new StormParseException($"{ExceptionHeader}: s2ml cache");

                source.ReadAlignedBytes(36);
            }
        }

        source.ReadAlignedBytes(16);
        uint sm2vCacheBlizzLength = source.ReadBits(8);

        for (int i = 0; i < sm2vCacheBlizzLength; i++)
        {

        }
        */

        // second s2mv hit
        /*
        for (; ; )
        {
            if (source.ReadStringFromBytes(4) == "s2mv")
            {
                BitReader.Index -= 4;
                break;
            }
            else
            {
                BitReader.Index -= 3;
            }
        }
        for (int i = 0; i < 2; i++)
        {
            if (source.ReadStringFromBytes(4) != "s2mv")
                throw new StormParseException($"{ExceptionHeader}: s2mv cache");

            source.ReadAlignedBytes(36);
        }

        source.ReadBits(1);

        uint region = source.ReadBits(8); // m_region
        if (source.ReadStringFromBits(32) != "Hero") // m_programId
            throw new StormParseException($"{ExceptionHeader}: Not Hero");
        source.ReadBits(32); // m_realm

        int blizzIdLength = (int)source.ReadBits(7);

        if (region >= 90)
        {
            if (source.ReadStringFromBytes(2) != "T:")
                throw new StormParseException($"{ExceptionHeader}: Not blizz T:");
            source.ReadStringFromBytes(blizzIdLength);
        }
        else
        {
            source.ReadStringFromBytes(blizzIdLength);
            source.ReadStringFromBytes(2);
        }

        source.ReadBits(8); // m_region
        if (source.ReadStringFromBits(32) != "Hero") // m_programId
            throw new StormParseException($"{ExceptionHeader}: Not Hero");
        source.ReadBits(32); // m_realm
        source.ReadLongBits(64); // m_id


        int klj = (int)source.ReadBits(12);

        int sdfad = (int)source.ReadBits(12);


        source.ReadBits(1); //temp

        */

        for (; ;)
        {
            if (bitReader.ReadStringFromBytes(4) == "s2mh")
            {
                bitReader.Index -= 4;
                break;
            }
            else
            {
                bitReader.Index -= 3;
            }
        }

        // source.ReadBits(???); // this depends on previous data (not byte aligned)

        // s2mh cache handles
        // uint s2mhCacheHandlesLength = source.ReadBits(6);
        // for (int i = 0; i < s2mhCacheHandlesLength; i++)
        for (int i = 0; i < s2maCacheHandlesLength; i++)
        {
            if (bitReader.ReadStringFromBytes(4) != "s2mh")
                throw new StormParseException($"{_exceptionHeader}: s2mh cache");

            bitReader.ReadAlignedBytes(36);
        }

        // player collections

        uint collectionSize;

        // strings gone starting with build (ptr) 55929
        if (replay.ReplayBuild >= 48027)
            collectionSize = bitReader.ReadBits(16);
        else
            collectionSize = bitReader.ReadBits(32);

        for (uint i = 0; i < collectionSize; i++)
        {
            if (replay.ReplayBuild >= 55929)
                bitReader.ReadAlignedBytes(8); // most likey an identifier for the item; first six bytes are 0x00
            else
                bitReader.ReadStringFromBytes(bitReader.ReadAlignedByte());
        }

        // use to determine if the collection item is usable by the player (owns/free to play/internet cafe)
        if (bitReader.ReadBits(32) != collectionSize)
            throw new StormParseException($"{_exceptionHeader}: collection difference");

        for (int i = 0; i < collectionSize; i++)
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

        // Player info

        if (replay.ReplayBuild <= 47479 || replay.ReplayBuild == 47801 || replay.ReplayBuild == 47903)
        {
            // Builds that are not yet supported for detailed parsing
            // build 47801 is a ptr build that had new data in the battletag section, the data was changed in 47944 (patch for 47801)
            // GetBattleTags(replay, source);
            return;
        }

        if (replay.ReplayBuild >= 85027)
        {
            // m_disabledHeroList
            uint disabledHeroListLength = bitReader.ReadBits(8);

            bitReader.EndianType = EndianType.LittleEndian;

            for (int i = 0; i < disabledHeroListLength; i++)
            {
                string disabledHeroAttributeId = bitReader.ReadStringFromBits(32);

                if (replay.DisabledHeroAttributeIdList.Count == 0)
                    replay.DisabledHeroAttributeIdList.Add(disabledHeroAttributeId);
            }

            bitReader.EndianType = EndianType.BigEndian;
        }

        replay.RandomValue = bitReader.ReadBits(32); // m_randomSeed

        bitReader.ReadAlignedBytes(4);

        uint playerListLength = bitReader.ReadBits(5);

        if (replay.PlayersWithObserversCount != playerListLength)
            throw new StormParseException($"{_exceptionHeader}: mismatch on player list length - {playerListLength} to {replay.PlayersWithObserversCount}");

        for (uint i = 0; i < playerListLength; i++)
        {
            bitReader.ReadBits(32);

            uint playerIndex = bitReader.ReadBits(5); // player index

            StormPlayer player = replay.ClientListByUserID[playerIndex];

            // toon handle
            uint playerRegion = bitReader.ReadBits(8); // m_region

            bitReader.EndianType = EndianType.LittleEndian;
            if (bitReader.ReadBits(32) != 1869768008) // m_programId
                throw new StormParseException($"{_exceptionHeader}: Not Hero");
            bitReader.EndianType = EndianType.BigEndian;

            uint playerRealm = bitReader.ReadBits(32); // m_realm
            long playerId = bitReader.ReadLongBits(64); // m_id

            if (player.PlayerType == PlayerType.Human)
            {
                if (player.ToonHandle!.Region != playerRegion)
                    throw new StormParseException($"{_exceptionHeader}: Mismatch on player region");
                if (player.ToonHandle.Realm != playerRealm)
                    throw new StormParseException($"{_exceptionHeader}: Mismatch on player realm");
                if (player.ToonHandle.Id != playerId)
                    throw new StormParseException($"{_exceptionHeader}: Mismatch on player id");
            }
            else if (player.PlayerType == PlayerType.Observer || player.PlayerType == PlayerType.Unknown)
            {
                // observers don't have the information carried over to the details file and sometimes not the initdata file
                player.ToonHandle ??= new ToonHandle();

                player.ToonHandle.Region = (int)playerRegion;
                player.ToonHandle.ProgramId = 1869768008;
                player.ToonHandle.Realm = (int)playerRealm;
                player.ToonHandle.Id = (int)playerId;
                player.PlayerType = PlayerType.Observer;
                player.Team = StormTeam.Observer;
            }

            // toon handle again but with T_ shortcut
            bitReader.ReadBits(8); // m_region

            bitReader.EndianType = EndianType.LittleEndian;
            if (bitReader.ReadBits(32) != 1869768008) // m_programId (Hero)
                throw new StormParseException($"{_exceptionHeader}: Not Hero");
            bitReader.EndianType = EndianType.BigEndian;

            bitReader.ReadBits(32); // m_realm

            int idLength = (int)bitReader.ReadBits(7) + 2;

            player.ToonHandle ??= new ToonHandle();
            player.ToonHandle.ShortcutId = bitReader.ReadStringFromBytes(idLength);

            bitReader.ReadBits(6);

            if (replay.ReplayBuild <= 47479)
            {
                // toon handle repeat again with T_ shortcut
                bitReader.ReadBits(8); // m_region
                if (bitReader.ReadStringFromBits(32) != "Hero") // m_programId
                    throw new StormParseException($"{_exceptionHeader}: Not Hero");
                bitReader.ReadBits(32); // m_realm

                idLength = (int)bitReader.ReadBits(7) + 2;
                if (player.ToonHandle.ShortcutId != bitReader.ReadStringFromBytes(idLength))
                    throw new StormParseException($"{_exceptionHeader}: Duplicate shortcut id does not match");

                bitReader.ReadBits(6);
            }

            bitReader.ReadBits(2);
            bitReader.ReadUnalignedBytes(25);
            bitReader.ReadBits(24);

            // ai games have 8 more bytes somewhere around here
            if (replay.GameMode == StormGameMode.Cooperative)
                bitReader.ReadUnalignedBytes(8);

            bitReader.ReadBits(7);

            if (!bitReader.ReadBoolean())
            {
                // repeat of the collection section above
                if (replay.ReplayBuild > 51609 || replay.ReplayBuild == 47903 || replay.ReplayBuild == 47479)
                {
                    bitReader.ReadBitArray(bitReader.ReadBits(12));
                }
                else if (replay.ReplayBuild > 47219)
                {
                    // each byte has a max value of 0x7F (127)
                    bitReader.ReadUnalignedBytes((int)bitReader.ReadBits(15) * 2);
                }
                else
                {
                    bitReader.ReadBitArray(bitReader.ReadBits(9));
                }

                bitReader.ReadBoolean();
            }

            bool isSilenced = bitReader.ReadBoolean(); // m_hasSilencePenalty
            if (player.PlayerType == PlayerType.Observer)
                player.IsSilenced = isSilenced;

            if (replay.ReplayBuild >= 61718)
            {
                bitReader.ReadBoolean();
                bool isVoiceSilenced = bitReader.ReadBoolean(); // m_hasVoiceSilencePenalty
                if (player.PlayerType == PlayerType.Observer)
                    player.IsVoiceSilenced = isVoiceSilenced;
            }

            if (replay.ReplayBuild >= 66977)
            {
                bool isBlizzardStaff = bitReader.ReadBoolean(); // m_isBlizzardStaff
                if (player.PlayerType == PlayerType.Observer)
                    player.IsBlizzardStaff = isBlizzardStaff;
            }

            if (bitReader.ReadBoolean()) // is player in party
                player.PartyValue = bitReader.ReadLongBits(64); // players in same party will have the same exact 8 bytes of data

            bitReader.ReadBoolean();

            string battleTagName = bitReader.ReadBlobAsString(7);
            int poundIndex = battleTagName.IndexOf('#');

            if (!string.IsNullOrEmpty(battleTagName) && poundIndex < 0)
                throw new StormParseException($"{_exceptionHeader}: Invalid battletag");

            // check if there is no tag number
            if (poundIndex >= 0)
            {
                ReadOnlySpan<char> namePart = battleTagName.AsSpan(0, poundIndex);
                if (!namePart.SequenceEqual(player.Name))
                    throw new StormParseException($"{_exceptionHeader}: Mismatch on battletag name with player name");
            }

            player.BattleTagName = battleTagName;

            if (replay.ReplayBuild >= 52860 || (replay.ReplayVersion.Major == 2 && replay.ReplayBuild >= 51978))
                player.AccountLevel = (int)bitReader.ReadBits(32);  // in custom games, this is a 0

            if (replay.ReplayBuild >= 69947)
            {
                bool hasActiveBoost = bitReader.ReadBoolean(); // m_hasActiveBoost
                if (player.PlayerType == PlayerType.Observer)
                    player.HasActiveBoost = hasActiveBoost;
            }
        }

        replay.IsBattleLobbyPlayerInfoParsed = true;


        // skip to the optional 8th Abat attribute
        //for (; ; )
        //{
        //    if (bitReader.ReadStringFromBits(32) == "tabA") // Abat 1096966516 0x41626174
        //    {
        //        List<string> itemsss = new();
        //        for (int i = 0; i < 100; i++)
        //        {
        //            bitReader.ReadBits(29); // no
        //            itemsss.Add(bitReader.ReadStringFromBits(32));
        //        }

        //        break;
        //    }
        //    else
        //    {
        //        bitReader.BitReversement(31);
        //    }
        //}
    }
}
