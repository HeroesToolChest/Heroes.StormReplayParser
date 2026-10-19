namespace Heroes.StormReplayParser.Pregame;

internal class StormBattleLobbyAttribute
{
    public StormBattleLobbyAttribute(ReplayAttributeEventType replayAttributeEventType)
    {
        EventType = replayAttributeEventType;
    }

    public ReplayAttributeEventType EventType { get; }

    public List<StormBattleLobbyAttributeValue> Values { get; set; } = new();

    public ReplayAttributeEventType? EnabledEventType { get; set; }

    public List<StormBattleLobbyEnabledAttributeValue> EnabledValues { get; set; } = new();
}
