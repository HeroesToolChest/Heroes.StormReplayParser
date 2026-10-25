namespace Heroes.StormReplayParser.Pregame;

/// <summary>
/// A reference to a string in the string tables (s2ml).
/// </summary>
internal readonly struct LobbyStringReference
{
    public LobbyStringReference(ref BitReader bitReader)
    {
        if (bitReader.ReadBoolean())
            bitReader.ReadBits(32); // color as RGB, such as the values of the Color attribute

        ModIndex = bitReader.ReadBits(6); // the position of the mod in the battlelobby's mod lists (used mods, a locale's s2ml list, and the s2mh handles)
        StringId = bitReader.ReadBits(16); // the id of the <e> element in the string table
    }

    /// <summary>
    /// Gets the index (in terms of a list) of the "loaded" mod.
    /// </summary>
    public uint ModIndex { get; }

    /// <summary>
    /// Gets the id of the <c>&lt;e&gt;</c> element in the string table.
    /// </summary>
    public uint StringId { get; }
}
