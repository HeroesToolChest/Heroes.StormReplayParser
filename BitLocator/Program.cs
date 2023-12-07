using Heroes.MpqTool;
using Heroes.StormReplayParser.MpqFiles;

if (args is not null && args.Length == 1 && File.Exists(args[0]))
{
    MpqHeroesArchive archive = MpqHeroesFile.Open(args[0]);
    MpqHeroesArchiveEntry entry = archive.GetEntry(ReplayServerBattlelobby.FileName);

    Span<byte> buffer = stackalloc byte[(int)entry.FileSize];

    archive.DecompressEntry(entry, buffer);

    Console.WriteLine("File found.");

    do
    {
        string? inputValue;
        string? inputBits;
        string? inputIsBigEndian;

        long valueToFind = 0;
        int bitsToRead = 0;
        bool isBigEndianFormat = true;

        do
        {
            Console.Write("Numerical value to find: ");
            inputValue = Console.ReadLine();
        }
        while (!long.TryParse(inputValue, out valueToFind));

        do
        {
            Console.Write("Number of bits value has: ");
            inputBits = Console.ReadLine();
        }
        while (!int.TryParse(inputBits, out bitsToRead));

        do
        {
            Console.Write("Big Endian format? (T/F): ");
            inputIsBigEndian = Console.ReadLine();
        }
        while (!ParseBooleanInput(inputIsBigEndian, out isBigEndianFormat));

        BitReader bitReader = new(buffer, isBigEndianFormat ? EndianType.BigEndian : EndianType.LittleEndian);

        while (bitReader.Index < buffer.Length && (bitReader.Index + (bitsToRead / 8)) < buffer.Length)
        {
            long value;

            if (bitsToRead < 33)
                value = bitReader.ReadBits(bitsToRead);
            else
                value = bitReader.ReadLongBits(bitsToRead);

            if (value == valueToFind)
                Console.WriteLine($"End Index: {bitReader.Index}");

            bitReader.BitReversement(bitsToRead - 1);
        }

        Console.WriteLine("Reached end of file.");
        Console.WriteLine();
    }
    while (true);
}
else
{
    Console.WriteLine("No file found.");
}

static bool ParseBooleanInput(string? value, out bool result)
{
    result = false;

    if (value is null)
        return false;

    value = value.ToUpperInvariant();

    if (bool.TryParse(value, out result))
        return true;

    if (value == "T")
    {
        result = true;
        return true;
    }
    else if (value == "F")
    {
        result = false;
        return true;
    }

    return false;
}