namespace Heroes.StormReplayParser.Tests;

[TestClass]
public class GameEventsNotKeptTests
{
    private readonly string _replaysFolder = "Replays";

    [TestMethod]
    [DataRow("TowersofDoom1_39445.StormR")]
    [DataRow("VolskayaFoundry1_77548.StormR")]
    [DataRow("WarheadJunctionDisconnectingPlayer1_91418.StormR")]
    [DataRow("SilverCity2_98348.StormR")]
    public void GameEventDataIsTheSameTest(string replayFile)
    {
        StormReplayResult expectedResult = StormReplay.Parse(Path.Combine(_replaysFolder, replayFile));
        StormReplayResult result = StormReplay.Parse(Path.Combine(_replaysFolder, replayFile), new ParseOptions()
        {
            ShouldKeepGameEvents = false,
        });

        StormReplay expected = expectedResult.Replay;
        StormReplay replay = result.Replay;

        Assert.AreEqual(expectedResult.Status, result.Status);
        Assert.IsNotEmpty(expected.GameEvents);
        Assert.IsEmpty(replay.GameEvents);

        // the data that comes from the game events is the same
        Assert.AreEqual(expected.Owner?.ToString(), replay.Owner?.ToString());
        Assert.AreSequenceEqual(
            expected.TeamChatMessages.Select(x => $"{x.Timestamp} {x.MessageSender?.Name} {x.Text}").ToList(),
            replay.TeamChatMessages.Select(x => $"{x.Timestamp} {x.MessageSender?.Name} {x.Text}").ToList());

        List<StormPlayer> expectedPlayers = [.. expected.StormPlayersWithObservers];
        List<StormPlayer> players = [.. replay.StormPlayersWithObservers];

        Assert.HasCount(expectedPlayers.Count, players);

        for (int i = 0; i < players.Count; i++)
        {
            Assert.AreEqual(expectedPlayers[i].IsPlatformMac, players[i].IsPlatformMac);
            Assert.AreEqual(expectedPlayers[i].IsReplayOwner, players[i].IsReplayOwner);
            Assert.AreEqual(expectedPlayers[i].LastCameraUpdateEvent, players[i].LastCameraUpdateEvent);
            Assert.AreSequenceEqual(
                expectedPlayers[i].Talents.Select(x => $"{x.TalentSlotId} {x.TalentNameId} {x.Timestamp}").ToList(),
                players[i].Talents.Select(x => $"{x.TalentSlotId} {x.TalentNameId} {x.Timestamp}").ToList());
            Assert.AreSequenceEqual(
                expectedPlayers[i].PlayerDisconnects.Select(x => $"{x.From} {x.To} {x.LeaveReason}").ToList(),
                players[i].PlayerDisconnects.Select(x => $"{x.From} {x.To} {x.LeaveReason}").ToList());
        }
    }
}
