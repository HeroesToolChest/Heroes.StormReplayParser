namespace Heroes.StormReplayParser;

internal class StormBattleLobbyAttribute
{
    public StormBattleLobbyAttribute(ReplayAttributeEventType replayAttributeEventType)
    {
        ReplayAttributeEventType = replayAttributeEventType;
    }

    public ReplayAttributeEventType ReplayAttributeEventType { get; }

    public List<StormBattleLobbyAttributeValue> AttributeValues { get; set; } = new();

    public List<StormBattleLobbyEnabledAttributeValue> EnabledValueAttributes { get; set; } = new();
}
