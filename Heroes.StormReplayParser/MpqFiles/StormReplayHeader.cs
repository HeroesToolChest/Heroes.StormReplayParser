using Heroes.StormReplayParser.Decoders;
using Heroes.StormReplayParser.MpqHeroesTool;
using System;

namespace Heroes.StormReplayParser.MpqFiles
{
    internal static class StormReplayHeader
    {
        public static void Parse(StormReplay replay, ReadOnlySpan<byte> source)
        {
            BitReader bitReader = new BitReader(source, EndianType.BigEndian);

            bitReader.ReadAlignedBytes(3);
            bitReader.ReadAlignedByte();
            bitReader.ReadAlignedBytes(4); // Data Max Size
            bitReader.ReadAlignedBytes(4); // Header Offset
            bitReader.ReadAlignedBytes(4); // User Data Header Size

            VersionedDecoder versionedDecoder = new VersionedDecoder(ref bitReader);

            // headerStructure.StructureByIndex[0].GetValueAsString(); // m_signature => "Heroes of the Storm replay 11

            // m_version struct
            replay.ReplayVersion.Major = (int)(versionedDecoder.Structure?[1].Structure?[1].GetValueAsUInt32()!); // m_major
            replay.ReplayVersion.Minor = (int)(versionedDecoder.Structure?[1].Structure?[2].GetValueAsUInt32()!); // m_minor
            replay.ReplayVersion.Revision = (int)(versionedDecoder.Structure?[1].Structure?[3].GetValueAsUInt32()!); // m_revision
            replay.ReplayVersion.Build = (int)(versionedDecoder.Structure?[1].Structure?[4].GetValueAsUInt32()!); // m_build
            replay.ReplayVersion.BaseBuild = (int)(versionedDecoder.Structure?[1].Structure?[5].GetValueAsUInt32()!); // m_baseBuild

            // the major version is a 0 before build 51978, it may be set as a 1
            /* if (stormReplay.ReplayBuild < 51978)
                stormReplay.ReplayVersion.Major = 1; */

            /* headerStructure.StructureByIndex[2].GetValueAsUInt32(); m_type */

            replay.ElapsedGamesLoops = (int)versionedDecoder.Structure![3].GetValueAsUInt32(); // m_elapsedGameLoops

            /* headerStructure.StructureByIndex?[4].GetValueAsInt32(); // m_useScaledTime */
            /* headerStructure.StructureByIndex?[5].GetValueAsInt32(); // m_ngdpRootKey */

            if (replay.ReplayBuild >= 39951)
                replay.ReplayVersion.BaseBuild = (int)versionedDecoder.Structure![6].GetValueAsUInt32(); // m_dataBuildNum

            /* headerStructure.StructureByIndex?[7].GetValueAsInt32(); // replayCompatibilityHash */
        }
    }
}
