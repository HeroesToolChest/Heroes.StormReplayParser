using Heroes.StormReplayParser.MpqHeroesTool;
using System;

namespace Heroes.StormReplayParser.MpqFiles
{
    internal static class ReplayServerBattlelobby
    {
        private const string _exceptionHeader = "battlelobby";

        public static string FileName { get; } = "replay.server.battlelobby";

        public static void Parse(StormReplay replay, ReadOnlySpan<byte> source)
        {
            BitReader.ResetIndex();
            BitReader.EndianType = EndianType.BigEndian;

            uint dependenciesLength = source.ReadBits(6);
            for (int i = 0; i < dependenciesLength; i++)
            {
                source.ReadBlobAsString(10);
            }

            // s2ma cache handles
            uint s2maCacheHandlesLength = source.ReadBits(6);
            for (int i = 0; i < s2maCacheHandlesLength; i++)
            {
                if (source.ReadStringFromBytes(4) != "s2ma")
                    throw new StormParseException($"{_exceptionHeader}: s2ma cache");

                source.ReadAlignedBytes(36);
            }

            //source.ReadAlignedBytes(94);
            //if (source.ReadStringFromBytes(4) != "Clsd")
            //    throw new StormParseException($"{ExceptionHeader}: Clsd");

            // we're just going to skip all the way down to the s2mh

            BitReader.AlignToByte();

            //for (; ;)
            //{
            //    if (source.ReadStringFromBytes(4) == "s2mh")
            //    {
            //        BitReader.Index -= 4;
            //        break;
            //    }
            //    else
            //    {
            //        BitReader.Index -= 3;
            //    }
            //}

            /////////
            //for (; ; )
            //{
            //    if (source.ReadStringFromBytes(4) == "s2mv")
            //    {
            //        BitReader.Index -= 4;
            //        break;
            //    }
            //    else
            //    {
            //        BitReader.Index -= 3;
            //    }
            //}

            //// first hit
            //BitReader.Index -= 1;

            //uint s2mvCacheHandlesLength = source.ReadBits(8);

            //for (int i = 0; i < s2mvCacheHandlesLength; i++)
            //{
            //    if (source.ReadStringFromBytes(4) != "s2mv")
            //        throw new StormParseException($"{ExceptionHeader}: s2mv cache");

            //    source.ReadAlignedBytes(36);
            //}

            //uint localeCount = source.ReadBits(5);

            //for (int i = 0; i < localeCount; i++)
            //{
            //    source.ReadStringFromBits(32); // locale

            //    uint s2mlCacheHandlesLength = source.ReadBits(6);

            //    for (int j = 0; j < s2mlCacheHandlesLength; j++)
            //    {
            //        if (source.ReadStringFromBytes(4) != "s2ml")
            //            throw new StormParseException($"{ExceptionHeader}: s2ml cache");

            //        source.ReadAlignedBytes(36);
            //    }
            //}

            //source.ReadAlignedBytes(16);
            //uint sm2vCacheBlizzLength = source.ReadBits(8);

            //for (int i = 0; i < sm2vCacheBlizzLength; i++)
            //{

            //}

            //// second s2mv hit
            //for (; ; )
            //{
            //    if (source.ReadStringFromBytes(4) == "s2mv")
            //    {
            //        BitReader.Index -= 4;
            //        break;
            //    }
            //    else
            //    {
            //        BitReader.Index -= 3;
            //    }
            //}
            //for (int i = 0; i < 2; i++)
            //{
            //    if (source.ReadStringFromBytes(4) != "s2mv")
            //        throw new StormParseException($"{ExceptionHeader}: s2mv cache");

            //    source.ReadAlignedBytes(36);
            //}

            //source.ReadBits(1);

            //uint region = source.ReadBits(8); // m_region
            //if (source.ReadStringFromBits(32) != "Hero") // m_programId
            //    throw new StormParseException($"{ExceptionHeader}: Not Hero");
            //source.ReadBits(32); // m_realm

            //int blizzIdLength = (int)source.ReadBits(7);

            //if (region >= 90)
            //{
            //    if (source.ReadStringFromBytes(2) != "T:")
            //        throw new StormParseException($"{ExceptionHeader}: Not blizz T:");
            //    source.ReadStringFromBytes(blizzIdLength);
            //}
            //else
            //{
            //    source.ReadStringFromBytes(blizzIdLength);
            //    source.ReadStringFromBytes(2);
            //}

            //source.ReadBits(8); // m_region
            //if (source.ReadStringFromBits(32) != "Hero") // m_programId
            //    throw new StormParseException($"{ExceptionHeader}: Not Hero");
            //source.ReadBits(32); // m_realm
            //source.ReadLongBits(64); // m_id


            //int klj = (int)source.ReadBits(12);

            //int sdfad = (int)source.ReadBits(12);


            //source.ReadBits(1); //temp

            //////////////


            for (; ; )
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
            }


            // source.ReadBits(???); // this depends on previous data (not byte aligned)

            // s2mh cache handles
            // uint s2mhCacheHandlesLength = source.ReadBits(6);
            // for (int i = 0; i < s2mhCacheHandlesLength; i++)
            for (int i = 0; i < s2maCacheHandlesLength; i++) // temp
            {
                if (source.ReadStringFromBytes(4) != "s2mh")
                    throw new StormParseException($"{_exceptionHeader}: s2mh cache");

                source.ReadAlignedBytes(36);
            }

            // player collections

            uint collectionSize;

            // strings gone starting with build (ptr) 55929
            if (replay.ReplayBuild >= 48027)
                collectionSize = source.ReadBits(16);
            else
                collectionSize = source.ReadBits(32);

            for (uint i = 0; i < collectionSize; i++)
            {
                if (replay.ReplayBuild >= 55929)
                    source.ReadAlignedBytes(8); // most likey an identifier for the item; first six bytes are 0x00
                else
                    source.ReadStringFromBytes(source.ReadAlignedByte());
            }

            // use to determine if the collection item is usable by the player (owns/free to play/internet cafe)
            if (source.ReadBits(32) != collectionSize)
                throw new StormParseException($"{_exceptionHeader}: collection difference");

            for (int i = 0; i < collectionSize; i++)
            {
                for (int j = 0; j < 16; j++) // 16 is total player slots
                {
                    source.ReadAlignedByte();
                    source.ReadAlignedByte(); // more likely a boolean to get the value

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
               // return;
            }

            replay.RandomValue = source.ReadBits(32); // m_randomSeed

            source.ReadAlignedBytes(4);

            uint playerListLength = source.ReadBits(5);

            if (replay.PlayersWithObserversCount != playerListLength)
                throw new StormParseException($"{_exceptionHeader}: mismatch on player list length - {playerListLength} to {replay.PlayersWithObserversCount}");

            for (uint i = 0; i < playerListLength; i++)
            {
                source.ReadBits(32);

                source.ReadBits(5); // player index

                // toon
                source.ReadBits(8); // m_region
                if (source.ReadStringFromBits(32) != "Hero") // m_programId
                    throw new StormParseException($"{_exceptionHeader}: Not Hero");
                source.ReadBits(32); // m_realm
                source.ReadLongBits(64); // m_id

                // internal toon
                source.ReadBits(8); // m_region
                if (source.ReadStringFromBits(32) != "Hero") // m_programId
                    throw new StormParseException($"{_exceptionHeader}: Not Hero");
                source.ReadBits(32); // m_realm

                int idLength = (int)source.ReadBits(7) + 2;
                replay.ClientListByUserID[i].BattleTID = source.ReadStringFromBytes(idLength);

                source.ReadBits(6);

                if (replay.ReplayBuild <= 47479)
                {
                    // internal toon repeat
                    source.ReadBits(8); // m_region
                    if (source.ReadStringFromBits(32) != "Hero") // m_programId
                        throw new StormParseException($"{_exceptionHeader}: Not Hero");
                    source.ReadBits(32); // m_realm

                    idLength = (int)source.ReadBits(7) + 2;
                    if (replay.ClientListByUserID[i].BattleTID != source.ReadStringFromBytes(idLength))
                        throw new StormParseException($"{_exceptionHeader}: Duplicate internal id does not match");

                    source.ReadBits(6);
                }

                source.ReadBits(2);
                source.ReadUnalignedBytes(25);
                source.ReadBits(24);

                // source.ReadUnalignedBytes(8); //ai games have 8 more bytes somewhere around here

                source.ReadBits(7);

                if (!source.ReadBoolean())
                {
                    // repeat of the collection section above
                    if (replay.ReplayBuild > 51609 || replay.ReplayBuild == 47903 || replay.ReplayBuild == 47479)
                    {
                        source.ReadBitArray((int)source.ReadBits(12));
                    }
                    else if (replay.ReplayBuild > 47219)
                    {
                        // each byte has a max value of 0x7F (127)
                        source.ReadUnalignedBytes((int)source.ReadBits(15) * 2);
                    }
                    else
                    {
                        source.ReadBitArray((int)source.ReadBits(9));
                    }

                    source.ReadBoolean();
                }

                source.ReadBoolean(); // m_hasSilencePenalty

                if (replay.ReplayBuild >= 61718)
                {
                    source.ReadBoolean();
                    source.ReadBoolean(); // m_hasVoiceSilencePenalty
                }

                if (replay.ReplayBuild >= 66977)
                    source.ReadBoolean(); // m_isBlizzardStaff

                if (source.ReadBoolean()) // is player in party
                    replay.ClientListByUserID[i].PartyValue = source.ReadLongBits(64); // players in same party will have the same exact 8 bytes of data

                source.ReadBoolean();
                replay.ClientListByUserID[i].BattleTag = source.ReadBlobAsString(7);

                if (!string.IsNullOrEmpty(replay.ClientListByUserID[i].BattleTag) && (!replay.ClientListByUserID[i].BattleTag.Contains('#')))
                    throw new StormParseException($"{_exceptionHeader}: Invalid battletag");

                if (replay.ReplayBuild >= 52860 || (replay.ReplayVersion.Major == 2 && replay.ReplayBuild >= 51978))
                    replay.ClientListByUserID[i].AccountLevel = (int)source.ReadBits(32);  // in custom games, this is a 0

                if (replay.ReplayBuild >= 69947)
                    source.ReadBoolean(); // m_hasActiveBoost
            }
        }
    }
}
