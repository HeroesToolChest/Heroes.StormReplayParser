using Heroes.StormReplayParser.MpqHeroesTool;

namespace Heroes.StormReplayParser.Tests.MpqHeroesTool;

[TestClass]
public class BitReaderTests
{
    private byte[] _byteArray = new byte[7] { 8, 130, 67, 58, 92, 80, 114 };

    public BitReaderTests()
    {
    }

    [TestMethod]
    public void ReadBitsTest()
    {
        Span<byte> buffer = stackalloc byte[7];
        _byteArray.CopyTo(buffer);

        BitReader bitReader = new(buffer, EndianType.BigEndian);

        uint result = bitReader.ReadBits(6);

        Assert.AreEqual(8U, result);
    }

    [TestMethod]
    public void BitReversementTest()
    {
        Span<byte> buffer = stackalloc byte[7];
        _byteArray.CopyTo(buffer);

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
        Span<byte> buffer = stackalloc byte[7];
        _byteArray.CopyTo(buffer);

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
    public void BitReversementWithStartingOffsetTest22222()
    {
        Span<byte> buffer = stackalloc byte[7];
        _byteArray.CopyTo(buffer);

        BitReader bitReader = new(buffer, EndianType.BigEndian);

        bitReader.ReadBytes(3);
        int asd = bitReader.ReadInt32Aligned();

        bitReader = new(buffer, EndianType.BigEndian);

        int bitsToRead = 32;
        for (int i = 0; i < 32; i++)
        {

            uint valueHere = bitReader.ReadBits(bitsToRead);
            if (valueHere == asd)
            {
                break;
            }
            bitReader.BitReversement(bitsToRead - 1);
        }

        int sdf = bitReader.ReadInt32Unaligned();

        //for (int offset = 1; offset < 32; offset++)
        //{
        //    for (int bitsToRead = 1; bitsToRead < 18; bitsToRead++)
        //    {
        //        BitReader bitReader = new(buffer, EndianType.BigEndian);
        //        bitReader.ReadBits(offset);

        //        uint resultFirst = bitReader.ReadBits(bitsToRead);
        //        bitReader.BitReversement(bitsToRead);

        //        uint resultAfterReversement = bitReader.ReadBits(bitsToRead);

        //        Assert.AreEqual(resultAfterReversement, resultFirst);
        //    }
        //}
    }
}
