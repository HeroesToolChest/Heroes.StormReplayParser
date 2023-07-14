using Heroes.StormReplayParser.MpqHeroesTool;

namespace Heroes.StormReplayParser.Tests.MpqHeroesTool;

[TestClass]
public class BitReaderTests
{
    public BitReaderTests()
    {
    }

    [TestMethod]
    public void ReadBitsTest()
    {
        Span<byte> buffer = stackalloc byte[4] { 8, 130, 67, 58 };

        BitReader bitReader = new(buffer, EndianType.BigEndian);

        uint result = bitReader.ReadBits(6);

        Assert.AreEqual(8U, result);
    }

    [TestMethod]
    public void BitReversementTest()
    {
        Span<byte> buffer = stackalloc byte[18] { 8, 130, 67, 58, 92, 80, 114, 102, 0, 1, 12, 13, 115, 23, 28, 56, 47, 35 };

        for (int bitsToRead = 1; bitsToRead < 33; bitsToRead++)
        {
            BitReader bitReader = new(buffer, EndianType.BigEndian);

            uint resultFirst = bitReader.ReadBits(bitsToRead);
            bitReader.BitReversement(bitsToRead);

            uint resultAfterReversement = bitReader.ReadBits(bitsToRead);

            Assert.AreEqual(resultAfterReversement, resultFirst);
        }
    }

    [TestMethod]
    public void BitReversementWithStartingOffsetTest()
    {
        Span<byte> buffer = stackalloc byte[18] { 8, 130, 67, 58, 92, 80, 114, 102, 0, 1, 12, 13, 115, 23, 28, 56, 47, 35 };

        for (int offset = 1; offset < 32; offset++)
        {
            for (int bitsToRead = 1; bitsToRead < 18; bitsToRead++)
            {
                BitReader bitReader = new(buffer, EndianType.BigEndian);
                bitReader.ReadBits(offset);

                uint resultFirst = bitReader.ReadBits(bitsToRead);
                bitReader.BitReversement(bitsToRead);

                uint resultAfterReversement = bitReader.ReadBits(bitsToRead);

                Assert.AreEqual(resultAfterReversement, resultFirst);
            }
        }
    }

    [TestMethod]
    public void ReadStringFromBitsAsBigEndian()
    {
        Span<byte> buffer = stackalloc byte[4] { 115, 50, 109, 118 };

        BitReader bitReader = new(buffer, EndianType.BigEndian);

        string value = bitReader.ReadStringFromBits(32);

        Assert.AreEqual("s2mv", value);
    }

    [TestMethod]
    public void ReadStringFromBitsAsLittleEndian()
    {
        Span<byte> buffer = stackalloc byte[4] { 115, 50, 109, 118 };

        BitReader bitReader = new(buffer, EndianType.LittleEndian);

        string value = bitReader.ReadStringFromBits(32);

        Assert.AreEqual("vm2s", value);
    }

    [TestMethod]
    public void ReadStringFromBytesAsBigEndian()
    {
        Span<byte> buffer = stackalloc byte[4] { 115, 50, 109, 118 };

        BitReader bitReader = new(buffer, EndianType.BigEndian);

        string value = bitReader.ReadStringFromAlignedBytes(4);

        Assert.AreEqual("s2mv", value);
    }

    [TestMethod]
    public void ReadStringFromBytesAsLittleEndian()
    {
        Span<byte> buffer = stackalloc byte[4] { 115, 50, 109, 118 };

        BitReader bitReader = new(buffer, EndianType.LittleEndian);

        string value = bitReader.ReadStringFromAlignedBytes(4);

        Assert.AreEqual("vm2s", value);
    }

    [TestMethod]
    public void ReadBlobAsString()
    {
        Span<byte> buffer = stackalloc byte[5] { 4, 115, 50, 109, 118 };

        BitReader bitReader = new(buffer, EndianType.BigEndian);

        string value = bitReader.ReadBlobAsString(8);

        Assert.AreEqual("s2mv", value);
    }
}
