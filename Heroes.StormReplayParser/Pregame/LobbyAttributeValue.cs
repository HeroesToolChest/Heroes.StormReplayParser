namespace Heroes.StormReplayParser.Pregame;

[DebuggerDisplay("\\{{Value,nq}\\}")]
internal readonly struct LobbyAttributeValue
{
    public LobbyAttributeValue(ref BitReader bitReader, string? value = null)
    {
        Value = value ?? string.Empty;

        if (bitReader.ReadBoolean())
            NameStringReference = new LobbyStringReference(ref bitReader);

        if (bitReader.ReadBoolean())
            DescriptionStringReference = new LobbyStringReference(ref bitReader);

        if (bitReader.ReadBoolean())
            Image = new LobbyImage(ref bitReader);
    }

    public string Value { get; }

    public LobbyStringReference? NameStringReference { get; }

    public LobbyStringReference? DescriptionStringReference { get; }

    public LobbyImage? Image { get; }
}
