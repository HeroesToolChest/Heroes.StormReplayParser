namespace Heroes.StormReplayParser.Pregame;

internal class StormBattleLobbyAttribute
{
    public StormBattleLobbyAttribute(ReplayAttributeEventType replayAttributeEventType)
    {
        ReplayAttributeEventType = replayAttributeEventType;
    }

    public ReplayAttributeEventType ReplayAttributeEventType { get; }

    public List<StormBattleLobbyAttributeValue> AttributeValues { get; set; } = new();

    public ReplayAttributeEventType? ReplayAttributeEnabledEventType { get; set; }

    public List<StormBattleLobbyEnabledAttributeValue> EnabledValueAttributes { get; set; } = new();
}
