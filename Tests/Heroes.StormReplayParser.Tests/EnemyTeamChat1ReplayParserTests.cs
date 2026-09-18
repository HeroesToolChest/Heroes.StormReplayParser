namespace Heroes.StormReplayParser.Tests;

[TestClass]
public class EnemyTeamChat1ReplayParserTests
{
    private readonly string _replaysFolder = "Replays";
    private readonly StormReplay _stormReplay;

    public EnemyTeamChat1ReplayParserTests()
    {
        StormReplayResult result = StormReplay.Parse(Path.Combine(_replaysFolder, "EnemyTeamChat1.StormR"));
        _stormReplay = result.Replay;
    }

    [TestMethod]
    public void ChatMessagesTest()
    {
        List<IStormMessage> chatMessages = [.. _stormReplay.ChatMessages];

        Assert.HasCount(2, chatMessages);
        Assert.AreEqual(StormTeam.Red, chatMessages[0].MessageSender!.Team);
        Assert.AreEqual(StormMessageTarget.Allies, ((ChatMessage)chatMessages[0]).MessageTarget);

        Assert.AreEqual(StormTeam.Red, chatMessages[1].MessageSender!.Team);
        Assert.AreEqual(StormMessageTarget.Allies, ((ChatMessage)chatMessages[1]).MessageTarget);
    }

    [TestMethod]
    public void TeamChatMessagesTest()
    {
        List<IStormMessage> chatMessages = [.. _stormReplay.TeamChatMessages];

        Assert.HasCount(4, chatMessages);
        Assert.AreEqual(StormTeam.Red, chatMessages[0].MessageSender!.Team);
        Assert.AreEqual(StormMessageTarget.Unknown, ((ChatMessage)chatMessages[0]).MessageTarget);

        Assert.AreEqual(StormTeam.Blue, chatMessages[1].MessageSender!.Team);
        Assert.AreEqual(StormMessageTarget.Unknown, ((ChatMessage)chatMessages[1]).MessageTarget);

        Assert.AreEqual(StormTeam.Blue, chatMessages[2].MessageSender!.Team);
        Assert.AreEqual(StormMessageTarget.Unknown, ((ChatMessage)chatMessages[2]).MessageTarget);

        Assert.AreEqual(StormTeam.Red, chatMessages[3].MessageSender!.Team);
        Assert.AreEqual(StormMessageTarget.Unknown, ((ChatMessage)chatMessages[3]).MessageTarget);
    }
}
