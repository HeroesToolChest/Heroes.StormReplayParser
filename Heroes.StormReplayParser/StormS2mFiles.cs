namespace Heroes.StormReplayParser;

internal readonly struct StormS2mFiles
{
    public StormS2mFiles(ref BitReader bitReader)
    {
        FileType = bitReader.ReadStringFromAlignedBytes(4);

        if (Enum.TryParse(bitReader.ReadStringFromAlignedBytes(4), out StormRegion stormRegion))
            Region = stormRegion;
        else
            Region = StormRegion.Unknown;

        ReadOnlySpan<byte> hashOfFile = bitReader.ReadAlignedBytes(32);

        StringBuilder hexString = new(hashOfFile.Length * 2);
        foreach (byte b in hashOfFile)
        {
            hexString.AppendFormat("{0:x2}", b);
        }

        FileName = hexString.ToString();
    }

    public string FileType { get; }

    public StormRegion Region { get; }

    // this is a sha256 hash of the file itself
    public string FileName { get; } = string.Empty;
}
