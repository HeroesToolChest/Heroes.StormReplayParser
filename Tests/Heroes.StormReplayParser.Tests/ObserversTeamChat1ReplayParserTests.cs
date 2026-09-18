namespace Heroes.StormReplayParser.Tests;

[TestClass]
public class ObserversTeamChat1ReplayParserTests
{
    private readonly string _replaysFolder = "Replays";
    private readonly StormReplay _stormReplay;

    public ObserversTeamChat1ReplayParserTests()
    {
        StormReplayResult result = StormReplay.Parse(Path.Combine(_replaysFolder, "ObserversTeamChat1.StormR"));
        _stormReplay = result.Replay;
    }

    [TestMethod]
    public void ChatMessagesTest()
    {
        List<IStormMessage> chatMessages = [.. _stormReplay.ChatMessages];

        Assert.HasCount(3, chatMessages);
        Assert.AreEqual(StormTeam.Blue, chatMessages[0].MessageSender!.Team);
        Assert.AreEqual(StormMessageTarget.All, ((ChatMessage)chatMessages[0]).MessageTarget);

        Assert.AreEqual(StormTeam.Observer, chatMessages[1].MessageSender!.Team);
        Assert.AreEqual(StormMessageTarget.Observers, ((ChatMessage)chatMessages[1]).MessageTarget);

        Assert.AreEqual(StormTeam.Observer, chatMessages[2].MessageSender!.Team);
        Assert.AreEqual(StormMessageTarget.Observers, ((ChatMessage)chatMessages[2]).MessageTarget);
    }

    [TestMethod]
    public void TeamChatMessagesTest()
    {
        List<ChatMessage> chatMessages = [.. _stormReplay.TeamChatMessages];

        Assert.HasCount(3, chatMessages);
        Assert.AreEqual(StormTeam.Blue, chatMessages[0].MessageSender!.Team);
        Assert.AreEqual(StormMessageTarget.Unknown, chatMessages[0].MessageTarget);

        Assert.AreEqual(StormTeam.Blue, chatMessages[1].MessageSender!.Team);
        Assert.AreEqual(StormMessageTarget.Unknown, chatMessages[1].MessageTarget);

        Assert.AreEqual(StormTeam.Blue, chatMessages[2].MessageSender!.Team);
        Assert.AreEqual(StormMessageTarget.Unknown, chatMessages[2].MessageTarget);
    }
}
