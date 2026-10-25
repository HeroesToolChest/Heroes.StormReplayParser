namespace Heroes.StormReplayParser.Pregame;

// image in an s2mv file
internal readonly struct LobbyImage
{
    public LobbyImage(ref BitReader bitReader)
    {
        Index = bitReader.ReadBits(6); // s2mv index
        Point = new Point(bitReader.ReadBits(16), bitReader.ReadBits(16)); // x, y
        Height = bitReader.ReadBits(16); // height
        Width = bitReader.ReadBits(16); // width
    }

    public uint Index { get; }

    public Point Point { get; }

    public uint Height { get; }

    public uint Width { get; }
}
