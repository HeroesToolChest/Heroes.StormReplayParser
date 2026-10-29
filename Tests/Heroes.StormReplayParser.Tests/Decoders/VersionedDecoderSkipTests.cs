namespace Heroes.StormReplayParser.Decoders.Tests;

[TestClass]
public class VersionedDecoderSkipTests
{
    private readonly string _replaysFolder = "Replays";
    private readonly string _s2mFilesFolder = "S2MFiles";
    private readonly string _s2mhFile = "82cf19ac9ceb903631db0994023efba040a1ccfbb0e920033a742d62cb65a77b.s2mh";

    [TestMethod]
    [DataRow("TowersofDoom1_39445.StormR", "replay.tracker.events")]
    [DataRow("VolskayaFoundry1_77548.StormR", "replay.tracker.events")]
    [DataRow("SilverCity2_98348.StormR", "replay.tracker.events")]
    [DataRow("VolskayaFoundry1_77548.StormR", "replay.details")]
    public void SkipReplayEntryTest(string replayFile, string entryName)
    {
        using MpqHeroesArchive archive = MpqHeroesFile.Open(Path.Combine(_replaysFolder, replayFile));
        MpqHeroesArchiveEntry entry = archive.GetEntry(entryName);
        byte[] bytes = new byte[(int)entry.FileSize];
        archive.DecompressEntry(entry, bytes);

        AssertSkipEndsWhereDecodeEnds(bytes);
    }

    [TestMethod]
    public void SkipS2mhFileTest()
    {
        byte[] bytes = File.ReadAllBytes(Path.Combine(_s2mFilesFolder, _s2mhFile));

        AssertSkipEndsWhereDecodeEnds(bytes);
    }

    // the bytes are a sequence of versioned values, and each skip has to end where its decode ends
    private static void AssertSkipEndsWhereDecodeEnds(byte[] bytes)
    {
        BitReader decodeReader = new(bytes, EndianType.BigEndian);
        BitReader skipReader = new(bytes, EndianType.BigEndian);
        int valueCount = 0;

        while (decodeReader.Index < bytes.Length)
        {
            _ = new VersionedDecoder(ref decodeReader);
            VersionedDecoder.Skip(ref skipReader);
            valueCount++;

            Assert.AreEqual(decodeReader.Index, skipReader.Index, $"value {valueCount}");
        }

        Assert.AreEqual(bytes.Length, skipReader.Index);
    }
}
