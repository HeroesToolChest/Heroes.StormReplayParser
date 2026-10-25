namespace Heroes.StormReplayParser.Pregame;

internal class LobbyAttribute
{
    public LobbyAttribute(ReplayAttributeEventType replayAttributeEventType)
    {
        EventType = replayAttributeEventType;
    }

    public ReplayAttributeEventType EventType { get; }

    public List<LobbyAttributeValue> Values { get; set; } = [];

    public ReplayAttributeEventType? EnabledEventType { get; set; }

    public List<LobbyEnabledAttributeValue> EnabledValues { get; set; } = [];

    // the string table (s2ml) string id of the attribute's event type, essentially the "group name" of the attribute values.
    public uint? EventTypeStringId { get; set; }

    // the position of the mod that defines the attribute in the battlelobby's mod lists (used mods, a locale's s2ml list, and the s2mh handles)
    public uint? ModIndex { get; set; }

    // the header (s2mh) file of the mod that defines the attribute
    public StormS2mFiles? S2mhFile { get; set; }

    // the string table (s2ml) file of the mod that defines the attribute, for each locale in the battlelobby (either only "enUS" or all of them)
    // shared by all the attributes of the same mod
    public IReadOnlyDictionary<string, StormS2mFiles>? S2mlFileByLocale { get; set; }
}
