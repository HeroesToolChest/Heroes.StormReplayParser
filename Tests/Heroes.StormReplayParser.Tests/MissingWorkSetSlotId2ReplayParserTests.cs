namespace Heroes.StormReplayParser.Tests;

[TestClass]
public class MissingWorkSetSlotId2ReplayParserTests
{
    private readonly string _replaysFolder = "Replays";
    private readonly string _replayFile = "MissingWorkSetSlotId2_92264.StormR";
    private readonly StormReplay _stormReplay;
    private readonly StormReplayParseStatus _result;

    public MissingWorkSetSlotId2ReplayParserTests()
    {
        StormReplayResult result = StormReplay.Parse(Path.Combine(_replaysFolder, _replayFile));
        _stormReplay = result.Replay;
        _result = result.Status;
    }

    [TestMethod]
    public void ParseResultTest()
    {
        Assert.AreEqual(StormReplayParseStatus.Incomplete, _result); // because of no winner
    }

    [TestMethod]
    public void PlayerCountTest()
    {
        Assert.AreEqual(10, _stormReplay.PlayersWithObserversCount);
        Assert.AreEqual(10, _stormReplay.PlayersCount);
    }

    [TestMethod]
    public void StormReplayDetailsTest()
    {
        List<StormPlayer> players = [.. _stormReplay.StormPlayers];
        StormPlayer player0 = players[0];
        StormPlayer player9 = players[9];

        Assert.StartsWith("Moro", player0.Name);
        Assert.StartsWith("teamplayer", player9.Name);

        Assert.IsTrue(_stormReplay.NoDetailsWorkingSetSlotID);
    }

    [TestMethod]
    public void StormReplayInitDataTest()
    {
        List<StormPlayer> players = [.. _stormReplay.StormPlayers];
        StormPlayer player0 = players[0];

        Assert.AreEqual("Random", player0.PlayerLoadout.SkinAndSkinTint);
        Assert.AreEqual("Random", player0.PlayerLoadout.MountAndMountTint);

        StormPlayer player8 = players[8];

        Assert.AreEqual("Random", player8.PlayerLoadout.SkinAndSkinTint);
        Assert.AreEqual("Random", player8.PlayerLoadout.MountAndMountTint);
    }
}
