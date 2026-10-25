namespace Heroes.StormReplayParser.Pregame;

[DebuggerDisplay("\\{{Value,nq}\\}")]
internal readonly struct LobbyEnabledAttributeValue
{
    public LobbyEnabledAttributeValue(ref BitReader bitReader)
    {
        Value = bitReader.ReadStringFromUnalignedBytes(4); // attribute value
    }

    public string Value { get; }
}
