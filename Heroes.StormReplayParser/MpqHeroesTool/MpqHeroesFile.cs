namespace Heroes.StormReplayParser.MpqHeroesTool;

internal static class MpqHeroesFile
{
    public static MpqHeroesArchive Open(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("Argument cannot be null or empty", nameof(fileName));
        }

        FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, 0x1000, false);

        try
        {
            return new MpqHeroesArchive(fileStream);
        }
        catch
        {
            fileStream.Dispose();
            throw;
        }
    }

    public static MpqHeroesArchive Open(Stream stream)
    {
        if (stream is null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        return new MpqHeroesArchive(stream);
    }
}
