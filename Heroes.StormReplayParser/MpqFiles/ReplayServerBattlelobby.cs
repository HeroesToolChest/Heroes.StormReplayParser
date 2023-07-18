namespace Heroes.StormReplayParser.MpqFiles;

internal static class ReplayServerBattlelobby
{
    private const string _exceptionHeader = "battlelobby";
    private const string _emptyAttributeId = "\0\0\0\0";
    private const string _randomAttributeId = "Rand";

    public static string FileName { get; } = "replay.server.battlelobby";

    public static void Parse(StormReplay replay, ReadOnlySpan<byte> source)
    {
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

        //bitReader.ReadUnalignedBytes(9);

        bitReader.ReadBits(9);
        //bitReader.ReadUnalignedBytes(1);
        //bitReader.ReadBits(1);

        int sl = bitReader.ReadInt32Unaligned();
       // bitReader.ReadBits(1);

        bitReader.ReadBits(32);
        uint attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // Part, Watch - Participant, Watcher
            bitReader.ReadBits(29);
        }

        bitReader.ReadUnalignedBytes(12);
        bitReader.ReadBits(4);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // Open, Humn, Comp
        }

        bitReader.ReadUnalignedBytes(40);
        bitReader.ReadBits(5);

        // player typer attributes - closed, human, etc...
        List<string> playerTypeAttributes = new();

        uint playerTypeAttributeLength = bitReader.ReadBits(12);
        for (int i = 0; i < playerTypeAttributeLength; i++)
        {
            playerTypeAttributes.Add(bitReader.ReadStringFromUnalignedBytes(4));
            bitReader.ReadBits(29);
        }

        bitReader.ReadUnalignedBytes(44);
        bitReader.ReadBits(4);

        // hero roles - tank, healer, ranged assassin, etc...
        List<string> heroRoleAttributes = new();

        uint heroRoleAttributeLength = bitReader.ReadBits(12);
        for (int i = 0; i < heroRoleAttributeLength; i++)
        {
            heroRoleAttributes.Add(bitReader.ReadStringFromUnalignedBytes(4));
            bitReader.ReadBits(29);
        }

        bitReader.ReadUnalignedBytes(12);
        bitReader.ReadBits(4);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // Humn
        }

        bitReader.ReadUnalignedBytes(40);
        bitReader.ReadBits(5);

        // team attributes section 1 - Team 1, Team 2, Team 3, repeat, etc...
        List<string> teamAttributesS1 = new();

        uint teamAttributesS1Length = bitReader.ReadBits(12);
        for (int i = 0; i < teamAttributesS1Length; i++)
        {
            teamAttributesS1.Add(bitReader.ReadStringFromUnalignedBytes(4));
            bitReader.ReadBits(29);
        }

        bitReader.ReadUnalignedBytes(26);
        bitReader.ReadBits(5);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // Part - Participant
        }

        bitReader.ReadUnalignedBytes(40);
        bitReader.ReadBits(5);

        // 0 -> 255 attributes
        List<string> zeroTo255Attributes = new();

        uint zeroTo255AttributeLength = bitReader.ReadBits(12);
        for (int i = 0; i < zeroTo255AttributeLength; i++)
        {
            zeroTo255Attributes.Add(bitReader.ReadStringFromUnalignedBytes(4));
            bitReader.ReadBits(29);
        }

        bitReader.ReadUnalignedBytes(12);
        bitReader.ReadBits(4);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // Humn, Comp
        }

        bitReader.ReadUnalignedBytes(40);
        bitReader.ReadBits(5);

        // hero attribute section 1
        List<string> heroAttributesS1 = new();

        uint heroAttributeS1Length = bitReader.ReadBits(12);
        for (int i = 0; i < heroAttributeS1Length; i++)
        {
            heroAttributesS1.Add(bitReader.ReadStringFromBits(32));
            bitReader.ReadBits(29);
        }

        bitReader.ReadUnalignedBytes(12);
        bitReader.ReadBits(4);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // drft - Draft
        }

        bitReader.ReadUnalignedBytes(17);
        bitReader.ReadBits(4);

        // hero attribute section 2
        List<string> heroAttributesS2 = new();

        uint heroAttributeS2Length = bitReader.ReadBits(12);
        for (int i = 0; i < heroAttributeS2Length; i++)
        {
            heroAttributesS2.Add(bitReader.ReadStringFromBits(32));
            bitReader.ReadBits(29);
        }

        bitReader.ReadUnalignedBytes(12);
        bitReader.ReadBits(4);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // drft - Draft
        }

        bitReader.ReadUnalignedBytes(17);
        bitReader.ReadBits(4);

        // team attributes section 2 - Team 1 -> Team 10, repeat, etc...
        List<string> teamAttributesS2 = new();

        uint teamAttributesS2Length = bitReader.ReadBits(12);
        for (int i = 0; i < teamAttributesS2Length; i++)
        {
            teamAttributesS2.Add(bitReader.ReadStringFromUnalignedBytes(4));
            bitReader.ReadBits(29);
        }

        bitReader.ReadUnalignedBytes(12);
        bitReader.ReadBits(4);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // CuTa - Custom Teams Archon ???
        }

        bitReader.ReadUnalignedBytes(8);
        bitReader.ReadBits(5);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // Part - Participant
        }

        bitReader.ReadUnalignedBytes(17);
        bitReader.ReadUnalignedBytes(22);
        bitReader.ReadBits(13);

        // hero skin attributes
        List<string> heroSkinAttributes = new();

        uint heroSkinAttributeLength = bitReader.ReadBits(12);
        for (int i = 0; i < heroSkinAttributeLength; i++)
        {
            heroSkinAttributes.Add(bitReader.ReadStringFromUnalignedBytes(4));
            if (bitReader.ReadBoolean())
                bitReader.ReadBits(28);
            else
                bitReader.ReadBits(5);
        }

        bitReader.ReadUnalignedBytes(12);
        bitReader.ReadBits(4);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // Dflt - Default
        }

        bitReader.ReadUnalignedBytes(8);
        bitReader.ReadBits(5);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // Humn, Comp
        }

        bitReader.ReadUnalignedBytes(8);
        bitReader.ReadBits(5);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // Part - Participant
        }

        bitReader.ReadUnalignedBytes(101);
        bitReader.ReadBits(7);

        // banner attributes
        List<string> bannerAttributes = new();

        uint bannerAttributeLength = bitReader.ReadBits(12);
        for (int i = 0; i < bannerAttributeLength; i++)
        {
            string bannerAttribute = bitReader.ReadStringFromUnalignedBytes(4);

            bannerAttributes.Add(bannerAttribute);

            if (bannerAttribute == "TST2")
            {
                bitReader.ReadBits(6);
            }
            else
            {
                if (bitReader.ReadBoolean())
                    bitReader.ReadBits(28);
                else
                    bitReader.ReadBits(5);
            }
        }

        bitReader.ReadUnalignedBytes(12);
        bitReader.ReadBits(4);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // Dflt - Default
        }

        bitReader.ReadUnalignedBytes(8);
        bitReader.ReadBits(5);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // Humn, Comp
        }

        bitReader.ReadUnalignedBytes(8);
        bitReader.ReadBits(5);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // Part - Participant
        }

        bitReader.ReadUnalignedBytes(40);
        bitReader.ReadBits(5);

        // team attributes section 3 - Team 1 -> Team 9, repeat, etc...
        List<string> teamAttributesS3 = new();

        uint teamAttributesS3Length = bitReader.ReadBits(12);
        for (int i = 0; i < teamAttributesS3Length; i++)
        {
            teamAttributesS3.Add(bitReader.ReadStringFromUnalignedBytes(4));
            bitReader.ReadBits(29);
        }

        bitReader.ReadUnalignedBytes(26);
        bitReader.ReadBits(5);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // Part - Participant
        }

        bitReader.ReadUnalignedBytes(40);
        bitReader.ReadBits(5);

        // hero attribute section 3
        List<string> heroAttributesS3 = new();

        uint heroAttributeS3Length = bitReader.ReadBits(12);
        for (int i = 0; i < heroAttributeS3Length; i++)
        {
            heroAttributesS3.Add(bitReader.ReadStringFromBits(32));
            bitReader.ReadBits(29);
        }

        bitReader.ReadUnalignedBytes(12);
        bitReader.ReadBits(4);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // drft - Draft
        }

        bitReader.ReadUnalignedBytes(17);
        bitReader.ReadBits(4);

        // hero attribute section 4
        List<string> heroAttributesS4 = new();

        uint heroAttributeS4Length = bitReader.ReadBits(12);
        for (int i = 0; i < heroAttributeS4Length; i++)
        {
            heroAttributesS4.Add(bitReader.ReadStringFromBits(32));
            bitReader.ReadBits(29);
        }

        bitReader.ReadUnalignedBytes(12);
        bitReader.ReadBits(4);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // drft - Draft
        }

        bitReader.ReadUnalignedBytes(19);
        bitReader.BitReversement(12);

        // 22, 0 -> 15 attributes
        List<string> zeroTo15Attributes = new();

        uint zeroTo15AttributeLength = bitReader.ReadBits(12);
        for (int i = 0; i < zeroTo15AttributeLength; i++)
        {
            zeroTo15Attributes.Add(bitReader.ReadStringFromUnalignedBytes(4));
            bitReader.ReadBits(29);
        }

        bitReader.ReadUnalignedBytes(12);
        bitReader.ReadBits(4);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // Open, Clsd, Humn, Comp
        }

        bitReader.ReadUnalignedBytes(46);
        bitReader.ReadBits(30);

        // Hmmr, 0 -> 15 attributes
        // same length as the one above?
        List<string> zeroTo15HmmrAttritbute = new();

        for (int i = 0; i < zeroTo15AttributeLength; i++)
        {
            zeroTo15HmmrAttritbute.Add(bitReader.ReadStringFromUnalignedBytes(4));
            bitReader.ReadBits(29);
        }

        bitReader.ReadUnalignedBytes(12);
        bitReader.ReadBits(4);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // drft, tour - Draft, Tournamnet Draft
        }

        bitReader.ReadUnalignedBytes(17);
        bitReader.ReadBits(4);

        // team attributes section 4 - Team 1 -> Team 8, repeat, etc...
        List<string> teamAttributesS4 = new();

        uint teamAttributesS4Length = bitReader.ReadBits(12);
        for (int i = 0; i < teamAttributesS4Length; i++)
        {
            teamAttributesS4.Add(bitReader.ReadStringFromUnalignedBytes(4));
            bitReader.ReadBits(29);
        }

        bitReader.ReadUnalignedBytes(26);
        bitReader.ReadBits(5);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // Part - Participant
        }

        bitReader.ReadUnalignedBytes(40);
        bitReader.ReadBits(5);

        // met / unmet attributes
        List<string> unmetAttributes = new();

        uint unmetAttributesLength = bitReader.ReadBits(12);
        for (int i = 0; i < unmetAttributesLength; i++)
        {
            unmetAttributes.Add(bitReader.ReadStringFromUnalignedBytes(4));
            bitReader.ReadBits(29);
        }

        bitReader.ReadUnalignedBytes(44);
        bitReader.ReadBits(4);

        // hero general roles - warrior, assassin, etc...
        List<string> heroGeneralRoleAttributes = new();

        uint heroGeneralRoleAttributeLength = bitReader.ReadBits(12);
        for (int i = 0; i < heroGeneralRoleAttributeLength; i++)
        {
            heroGeneralRoleAttributes.Add(bitReader.ReadStringFromUnalignedBytes(4));
            bitReader.ReadBits(29);
        }

        bitReader.ReadUnalignedBytes(12);
        bitReader.ReadBits(4);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // Humn
        }

        bitReader.ReadUnalignedBytes(48);
        bitReader.ReadBits(14);

        bitReader.BitReversement(29);
        bitReader.BitReversement(32);

        bitReader.BitReversement(12);

        // HMT attributes
        List<string> hmtAttributes = new();

        uint hmtAttributesLength = bitReader.ReadBits(12);
        for (int i = 0; i < hmtAttributesLength; i++)
        {
            hmtAttributes.Add(bitReader.ReadStringFromUnalignedBytes(4));
            bitReader.ReadBits(29);
        }

        bitReader.ReadUnalignedBytes(12);
        bitReader.ReadBits(4);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // Humn, Comp
        }

        bitReader.ReadUnalignedBytes(40);
        bitReader.ReadBits(5);

        // tcXX attributes
        List<string> tcAttributes = new();

        uint tcAttributesLength = bitReader.ReadBits(12);
        for (int i = 0; i < tcAttributesLength; i++)
        {
            tcAttributes.Add(bitReader.ReadStringFromUnalignedBytes(4));
            bitReader.ReadBits(29);
        }

        bitReader.ReadUnalignedBytes(12);
        bitReader.ReadBits(4);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // Dflt - Default
        }

        bitReader.ReadUnalignedBytes(8);
        bitReader.ReadBits(5);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // Humn, Comp
        }

        bitReader.ReadUnalignedBytes(8);
        bitReader.ReadBits(5);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // Part - Participant
        }

        bitReader.ReadUnalignedBytes(8);
        bitReader.ReadBits(5);

        // 22, 0 -> 15 attributes
        List<string> zeroTo15Attributes2 = new();

        uint zeroTo15AttributeLength2 = bitReader.ReadBits(12);
        for (int i = 0; i < zeroTo15AttributeLength2; i++)
        {
            zeroTo15Attributes2.Add(bitReader.ReadStringFromUnalignedBytes(4));
            bitReader.ReadBits(29);
        }

        bitReader.ReadUnalignedBytes(37);

        // team attributes section 5 - Team 1 -> Team 2, repeat, etc...
        List<string> teamAttributesS5 = new();

        uint teamAttributeS5Length = bitReader.ReadBits(12);
        for (int i = 0; i < teamAttributeS5Length; i++)
        {
            teamAttributesS5.Add(bitReader.ReadStringFromUnalignedBytes(4));
            bitReader.ReadBits(29);
        }

        bitReader.ReadUnalignedBytes(12);
        bitReader.ReadBits(4);

        attributeLength = bitReader.ReadBits(12);
        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // \06v6
        }

        bitReader.ReadUnalignedBytes(8);
        bitReader.ReadBits(5);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // Part - Participant
        }

        bitReader.ReadUnalignedBytes(40);
        bitReader.ReadBits(5);

        // voiceline attributes
        List<string> voicelineAttributes = new();

        uint voicelineAttributeLength = bitReader.ReadBits(12);
        for (int i = 0; i < voicelineAttributeLength; i++)
        {
            voicelineAttributes.Add(bitReader.ReadStringFromUnalignedBytes(4));

            if (bitReader.ReadBoolean())
                bitReader.ReadBits(28);
            else
                bitReader.ReadBits(5);
        }

        bitReader.ReadUnalignedBytes(12);
        bitReader.ReadBits(4);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // Dflt - Default
        }

        bitReader.ReadUnalignedBytes(8);
        bitReader.ReadBits(5);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // Humn, Comp
        }

        bitReader.ReadUnalignedBytes(8);
        bitReader.ReadBits(5);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // Part - Participant
        }

        bitReader.ReadUnalignedBytes(40);
        bitReader.ReadBits(5);

        // team attributes section 6 - Team 1 -> Team 2
        List<string> teamAttributesS6 = new();

        uint teamAttributeS6Length = bitReader.ReadBits(12);
        for (int i = 0; i < teamAttributeS6Length; i++)
        {
            teamAttributesS6.Add(bitReader.ReadStringFromUnalignedBytes(4));
            bitReader.ReadBits(29);
        }

        bitReader.ReadUnalignedBytes(21);
        bitReader.ReadBits(3);

        // team attributes section 7 - Team 1 -> Team 2, repeat, etc...
        List<string> teamAttributesS7 = new();

        uint teamAttributeS7Length = bitReader.ReadBits(12);
        for (int i = 0; i < teamAttributeS7Length; i++)
        {
            teamAttributesS7.Add(bitReader.ReadStringFromUnalignedBytes(4));
            bitReader.ReadBits(29);
        }

        bitReader.ReadUnalignedBytes(12);
        bitReader.ReadBits(4);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // \03v3
        }

        bitReader.ReadUnalignedBytes(8);
        bitReader.ReadBits(5);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // Part - Participant
        }

        bitReader.ReadUnalignedBytes(40);
        bitReader.ReadBits(5);

        // evry, team - everyone, team
        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4);
            bitReader.ReadBits(29);
        }

        bitReader.ReadUnalignedBytes(21);
        bitReader.ReadBits(3);

        // team attributes section 8 - Team 1 -> Team 7, repeat, etc...
        List<string> teamAttributesS8 = new();

        uint teamAttributeS8Length = bitReader.ReadBits(12);
        for (int i = 0; i < teamAttributeS8Length; i++)
        {
            teamAttributesS8.Add(bitReader.ReadStringFromUnalignedBytes(4));
            bitReader.ReadBits(29);
        }

        bitReader.ReadUnalignedBytes(26);
        bitReader.ReadBits(5);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // Part - Participant
        }

        bitReader.ReadUnalignedBytes(40);
        bitReader.ReadBits(5);

        // team attributes section 9 - Team 1 -> Team 2, repeat, etc...
        List<string> teamAttributesS9 = new();

        uint teamAttributeS9Length = bitReader.ReadBits(12);
        for (int i = 0; i < teamAttributeS9Length; i++)
        {
            teamAttributesS9.Add(bitReader.ReadStringFromUnalignedBytes(4));
            bitReader.ReadBits(29);
        }

        bitReader.ReadUnalignedBytes(12);
        bitReader.ReadBits(4);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // \02v2
        }

        bitReader.ReadUnalignedBytes(8);
        bitReader.ReadBits(5);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // Part - Participant
        }

        bitReader.ReadUnalignedBytes(40);
        bitReader.ReadBits(5);

        // draft ban mode attributes
        List<string> draftBanModeAttributes = new();

        uint draftBanModeAttributesLength = bitReader.ReadBits(12);
        for (int i = 0; i < draftBanModeAttributesLength; i++)
        {
            draftBanModeAttributes.Add(bitReader.ReadStringFromUnalignedBytes(4));
            bitReader.ReadBits(29);
        }

        bitReader.ReadUnalignedBytes(12);
        bitReader.ReadBits(4);

        attributeLength = bitReader.ReadBits(12);

        for (int i = 0; i < attributeLength; i++)
        {
            bitReader.ReadStringFromUnalignedBytes(4); // drft, tour - Draft, Tournamnet Draft
        }

        bitReader.ReadUnalignedBytes(17);
        bitReader.ReadBits(4);

        // met / unmet attributes
        List<string> unmetAttributes2 = new();

        uint unmetAttributesLength2 = bitReader.ReadBits(12);
        for (int i = 0; i < unmetAttributesLength2; i++)
        {
            unmetAttributes2.Add(bitReader.ReadStringFromUnalignedBytes(4));
            bitReader.ReadBits(29);
        }

        bitReader.ReadUnalignedBytes(46);
        //attributeLength = bitReader.ReadBits(12);

        //for (int i = 0; i < attributeLength; i++)
        //{
        //    bitReader.ReadStringFromUnalignedBytes(4);
        //    bitReader.ReadBits(29);
        //}

        //bitReader.BitReversement(29);
        //bitReader.BitReversement(32);

        //bitReader.ReadStringFromUnalignedBytes(4);

        //bitReader.ReadBits(29);
        //bitReader.ReadStringFromUnalignedBytes(4); // assn



        //  bitReader.ReadStringFromUnalignedBytes(4);
        // bitReader.ReadBits(29);
        // bitReader.ReadStringFromUnalignedBytes(4);
        List<string> items333 = new();
        for (int i = 0; i < 9999; i++)
        {
            items333.Add(bitReader.ReadStringFromUnalignedBytes(4));
            bitReader.BitReversement(31);
        }

        // skip to the fifth hero attribute section
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

                bitReader.BitReversement(12);
                uint heroAttributeSizeS1 = bitReader.ReadBits(12); // get collection size

                for (int i = 0; i < heroAttributeSizeS1; i++)
                {
                    items.Add(bitReader.ReadStringFromBits(32));
                    bitReader.ReadBits(29);
                }

                break;
            }
            else
            {
                bitReader.BitReversement(31);
            }
        }

        // skip to the sixth hero attribute section
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

                bitReader.BitReversement(12);
                uint heroAttributeSizeS1 = bitReader.ReadBits(12); // get collection size

                for (int i = 0; i < heroAttributeSizeS1; i++)
                {
                    items.Add(bitReader.ReadStringFromBits(32));
                    bitReader.ReadBits(29);
                }

                break;
            }
            else
            {
                bitReader.BitReversement(31);
            }
        }

        // skip to the seventh hero attribute section
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

                bitReader.BitReversement(12);
                uint heroAttributeSizeS1 = bitReader.ReadBits(12); // get collection size

                for (int i = 0; i < heroAttributeSizeS1; i++)
                {
                    items.Add(bitReader.ReadStringFromBits(32));
                    bitReader.ReadBits(29);
                }

                break;
            }
            else
            {
                bitReader.BitReversement(31);
            }
        }

        // skip to the spray attribute section
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

                bitReader.BitReversement(12);
                uint sprayAttributeSizeS1 = bitReader.ReadBits(12); // get collection size

                for (int i = 0; i < sprayAttributeSizeS1; i++)
                {
                    items.Add(bitReader.ReadStringFromBits(32));
                    bitReader.ReadBits(29);
                }

                break;
            }
            else
            {
                bitReader.BitReversement(31);
            }
        }

        // skip to the mount attribute section
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

                bitReader.BitReversement(12);
                uint mountAttributeSizeS1 = bitReader.ReadBits(12); // get collection size

                for (int i = 0; i < mountAttributeSizeS1; i++)
                {
                    items.Add(bitReader.ReadStringFromBits(32));
                    if (bitReader.ReadBoolean())
                        bitReader.ReadBits(28);
                    else
                        bitReader.ReadBits(5);
                }

                break;
            }
            else
            {
                bitReader.BitReversement(31);
            }
        }

        // skip to the announcer attribute section
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

                bitReader.BitReversement(12);
                uint announcerAttributeSizeS1 = bitReader.ReadBits(12); // get collection size

                for (int i = 0; i < announcerAttributeSizeS1; i++)
                {
                    items.Add(bitReader.ReadStringFromBits(32));
                    if (bitReader.ReadBoolean())
                        bitReader.ReadBits(28);
                    else
                        bitReader.ReadBits(5);
                }

                break;
            }
            else
            {
                bitReader.BitReversement(31);
            }
        }

        ////////////////////
        //// Player selections here
        ////////////////////

        // skip down to s2mv /w locales section
        for (; ;)
        {
            List<string> items = new();
            if (bitReader.ReadStringFromUnalignedBytes(4) == "s2mv")
            {
                bitReader.BitReversement(32);

                bitReader.BitReversement(6);

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
                    items.Add(bitReader.ReadStringFromUnalignedBytes(4));

                    uint s2mlSize = bitReader.ReadBits(6); // number of s2ml in locale

                    bitReader.AlignToByte();

                    for (int j = 0; j < s2mlSize; j++)
                    {
                        if (bitReader.ReadStringFromAlignedBytes(4) != "s2ml")
                            throw new StormParseException($"{_exceptionHeader}: s2ml");

                        bitReader.ReadAlignedBytes(36);
                    }
                }

                break;
            }
            else
            {
                bitReader.BitReversement(31);
            }
        }

        ////////////////////
        //// s2mv w/ blizzmaps players section
        ////////////////////

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

        // skip down to s2mh section
        for (; ;)
        {
            if (bitReader.ReadStringFromUnalignedBytes(4) == "s2mh")
            {
                bitReader.BitReversement(32);

                bitReader.BitReversement(7);

                uint s2mhCacheHandlesLength = bitReader.ReadBits(7);
                for (int i = 0; i < s2mhCacheHandlesLength; i++)
                {
                    if (bitReader.ReadStringFromAlignedBytes(4) != "s2mh")
                        throw new StormParseException($"{_exceptionHeader}: s2mh");

                    bitReader.ReadAlignedBytes(36);
                }

                break;
            }
            else
            {
                bitReader.BitReversement(31);
            }
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
                bitReader.ReadStringFromAlignedBytes(bitReader.ReadAlignedByte());
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
            player.ToonHandle.ShortcutId = bitReader.ReadStringFromAlignedBytes(idLength);

            bitReader.ReadBits(6);

            if (replay.ReplayBuild <= 47479)
            {
                // toon handle repeat again with T_ shortcut
                bitReader.ReadBits(8); // m_region
                if (bitReader.ReadStringFromBits(32) != "Hero") // m_programId
                    throw new StormParseException($"{_exceptionHeader}: Not Hero");
                bitReader.ReadBits(32); // m_realm

                idLength = (int)bitReader.ReadBits(7) + 2;
                if (player.ToonHandle.ShortcutId != bitReader.ReadStringFromAlignedBytes(idLength))
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
