using System.Text;

namespace Heroes.StormReplayParser.Tests;

[TestClass]
public class BattleLobbyNotBattlelobbyTests
{
    private readonly string _replaysFolder = "Replays";
    private readonly string _replayFile = "VolskayaFoundry1_77548.StormR";

    [TestMethod]
    public void ParseReplayFileTest()
    {
        StormReplayPregameResult result = StormReplayPregame.Parse(Path.Combine(_replaysFolder, _replayFile));

        AssertNotBattlelobby(result, "the s2ma cache handle count does not match the dependency count");
    }

    [TestMethod]
    [DataRow("replay.details")]
    [DataRow("replay.initData")]
    [DataRow("replay.tracker.events")]
    public void ParseReplayEntryTest(string entryName)
    {
        using MpqHeroesArchive archive = MpqHeroesFile.Open(Path.Combine(_replaysFolder, _replayFile));
        using Stream stream = archive.DecompressEntry(archive.GetEntry(entryName));

        StormReplayPregameResult result = StormReplayPregame.Parse(stream);

        AssertNotBattlelobby(result, "the s2ma cache handle count does not match the dependency count");
    }

    [TestMethod]
    public void ParseAttributeEventsTest()
    {
        using MpqHeroesArchive archive = MpqHeroesFile.Open(Path.Combine(_replaysFolder, _replayFile));
        using Stream stream = archive.DecompressEntry(archive.GetEntry("replay.attributes.events"));

        StormReplayPregameResult result = StormReplayPregame.Parse(stream);

        AssertNotBattlelobby(result, "there are no s2ma dependencies");
    }

    [TestMethod]
    public void ParseCacheHandleCountMismatchTest()
    {
        using MemoryStream stream = new(CreateLobbyStart(cacheHandleCount: 2));

        StormReplayPregameResult result = StormReplayPregame.Parse(stream);

        AssertNotBattlelobby(result, "the s2ma cache handle count does not match the dependency count");
    }

    private static void AssertNotBattlelobby(StormReplayPregameResult result, string reason)
    {
        Assert.AreEqual(StormReplayPregameParseStatus.Exception, result.Status);
        Assert.IsInstanceOfType<StormParseException>(result.Exception?.InnerException);
        Assert.Contains("not a battlelobby file", result.Exception.InnerException.Message);
        Assert.Contains(reason, result.Exception.InnerException.Message);
    }

    // the start of a battlelobby with one s2ma dependency, followed by its cache handles
    private static byte[] CreateLobbyStart(int cacheHandleCount)
    {
        byte[] path = Encoding.UTF8.GetBytes("Cache/1f/1b/1f1b.s2ma");

        List<byte> bytes =
        [
            1, // the dependency count in the low 6 bits, then the 10 bit path length
            (byte)path.Length,
            .. path,
            (byte)cacheHandleCount,
        ];

        for (int i = 0; i < cacheHandleCount; i++)
        {
            bytes.AddRange(Encoding.ASCII.GetBytes("s2ma"));
            bytes.AddRange(Encoding.ASCII.GetBytes("\0\0US"));
            bytes.AddRange(new byte[32]); // the hash
        }

        return [.. bytes];
    }
}
