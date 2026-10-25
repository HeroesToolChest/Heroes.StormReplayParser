using System.Text;

namespace Heroes.StormReplayParser.Decoders.Tests;

[TestClass]
public class VersionedDecoderS2mhTests
{
    private readonly string _s2mFilesFolder = "S2MFiles";
    private readonly string _s2mhFile = "82cf19ac9ceb903631db0994023efba040a1ccfbb0e920033a742d62cb65a77b.s2mh"; // the Teams10.StormMod header (.s2mh) from the Battle.net cache
    private readonly int _fileLength;
    private readonly int _bytesRead;
    private readonly VersionedDecoder _versionedDecoder;

    public VersionedDecoderS2mhTests()
    {
        byte[] bytes = File.ReadAllBytes(Path.Combine(_s2mFilesFolder, _s2mhFile));

        BitReader bitReader = new(bytes, EndianType.BigEndian);

        _versionedDecoder = new VersionedDecoder(ref bitReader);
        _fileLength = bytes.Length;
        _bytesRead = bitReader.Index;
    }

    private VersionedDecoder Header => _versionedDecoder.Structure![0];

    private VersionedDecoder[] Attributes => Header.Structure![5].ArrayData!;

    [TestMethod]
    public void DecodesEntireFileTest()
    {
        Assert.AreEqual(_fileLength, _bytesRead);
        Assert.AreEqual(0x05, _versionedDecoder.DataType);
        Assert.HasCount(2, _versionedDecoder.Structure!);
    }

    [TestMethod]
    public void HeaderTest()
    {
        Assert.AreEqual(243563U, Header.Structure![0].Structure![0].GetValueAsUInt32());
        Assert.AreEqual(65561U, Header.Structure![0].Structure![1].GetValueAsUInt32());
        Assert.AreEqual("Teams10.StormMod", Header.Structure![1].GetValueAsString());
        Assert.AreEqual(289930U, Header.Structure![3].GetValueAsUInt32());

        byte[] s2maHandle = Header.Structure![2].Value!;

        Assert.AreEqual(0x02, Header.Structure![2].DataType);
        Assert.HasCount(40, s2maHandle);
        Assert.AreEqual("s2ma", Encoding.UTF8.GetString(s2maHandle, 0, 4));
        Assert.AreEqual("US", Encoding.UTF8.GetString(s2maHandle, 6, 2));
    }

    [TestMethod]
    public void MissingFieldTagsTest()
    {
        List<VersionedDecoder> header = Header.Structure!;

        Assert.HasCount(25, header);

        // these field tags are not in the structure
        Assert.IsNull(header[6]);
        Assert.IsNull(header[11]);
        Assert.IsNull(header[20]);
        Assert.IsNull(header[21]);
        Assert.IsNull(header[23]);

        // the fields after the missing tags are still at the index of their tag
        Assert.AreEqual(0x00, header[7].DataType);
        Assert.AreEqual(243551U, header[14].ArrayData![0].Structure![0].GetValueAsUInt32());
        Assert.AreEqual(1U, header[15].GetValueAsUInt32());
        Assert.AreEqual(0U, header[22].GetValueAsUInt32());
        Assert.AreEqual(0x00, header[24].DataType);
    }

    [TestMethod]
    public void MapInfoTest()
    {
        List<VersionedDecoder> mapInfo = Header.Structure![4].Structure!;

        Assert.IsNull(mapInfo[0].Structure![0].OptionalData);
        Assert.AreEqual(0U, mapInfo[0].Structure![1].GetValueAsUInt32());
        Assert.AreEqual(1U, mapInfo[0].Structure![2].GetValueAsUInt32());
        Assert.AreEqual(2U, mapInfo[1].Structure![2].GetValueAsUInt32());

        Assert.AreEqual(0x04, mapInfo[3].DataType);
        Assert.AreEqual(24U, mapInfo[3].OptionalData!.Structure![3].GetValueAsUInt32());
        Assert.AreEqual(24U, mapInfo[3].OptionalData!.Structure![4].GetValueAsUInt32());

        Assert.AreEqual(16U, mapInfo[4].GetValueAsUInt32()); // max players
        Assert.AreEqual(22U, mapInfo[5].GetValueAsUInt32());
    }

    [TestMethod]
    public void AttributesTest()
    {
        Assert.HasCount(15, Attributes);

        VersionedDecoder colorAttribute = Attributes[0];
        VersionedDecoder teamAttribute = Attributes[1];

        Assert.AreEqual(3002U, colorAttribute.Structure![0].Structure![1].GetValueAsUInt32());
        Assert.HasCount(16, colorAttribute.Structure![1].ArrayData!);
        Assert.AreEqual(1U, colorAttribute.Structure![4].GetValueAsUInt32()); // restriction
        Assert.AreEqual(3U, colorAttribute.Structure![5].GetValueAsUInt32()); // visibility
        Assert.AreEqual(1U, colorAttribute.Structure![6].GetValueAsUInt32()); // access
        Assert.AreEqual(11U, colorAttribute.Structure![9].GetValueAsUInt32()); // sort order

        Assert.AreEqual(2006U, teamAttribute.Structure![0].Structure![1].GetValueAsUInt32());
        Assert.HasCount(10, teamAttribute.Structure![1].ArrayData!);
    }

    [TestMethod]
    public void FourCCTest()
    {
        VersionedDecoder colorValue = Attributes[0].Structure![1].ArrayData![0].Structure![0];
        VersionedDecoder teamValue = Attributes[1].Structure![1].ArrayData![0].Structure![0];

        Assert.AreEqual(0x07, colorValue.DataType);
        Assert.AreEqual(1952657456U, colorValue.GetValueAsUInt32());
        Assert.AreEqual("{\"text\": \"tc00\",\"value\": 1952657456}", colorValue.ToJson());

        // zero padded
        Assert.AreEqual(0x07, teamValue.DataType);
        Assert.AreEqual(21553U, teamValue.GetValueAsUInt32());
        Assert.AreEqual("{\"text\": \"T1\",\"value\": 21553}", teamValue.ToJson());
    }

    [TestMethod]
    public void LocalesTest()
    {
        VersionedDecoder[] locales = Header.Structure![8].ArrayData!;

        Assert.HasCount(12, locales);
        Assert.AreEqual(1701729619U, locales[0].Structure![0].GetValueAsUInt32());
        Assert.AreEqual("{\"text\": \"enUS\",\"value\": 1701729619}", locales[0].Structure![0].ToJson());
        Assert.AreEqual("{\"text\": \"koKR\",\"value\": 1802455890}", locales[11].Structure![0].ToJson());
        Assert.HasCount(1, locales[0].Structure![1].ArrayData!); // s2ml cache handle
    }

    [TestMethod]
    public void ChoiceTest()
    {
        // a global default value
        VersionedDecoder globalDefault = Attributes[2].Structure![8];

        Assert.AreEqual(0x03, globalDefault.DataType);
        Assert.AreEqual(0U, globalDefault.GetValueAsUInt32());
        Assert.AreEqual(0x05, globalDefault.ChoiceData!.DataType);
        Assert.AreEqual("{\"choice\": 0,\"value\": {\"0\": 0,\"1\": 0}}", globalDefault.ToJson());

        // a default value for each of the 16 player slots
        VersionedDecoder slotsDefault = Attributes[0].Structure![8];

        Assert.AreEqual(1U, slotsDefault.GetValueAsUInt32());
        Assert.HasCount(16, slotsDefault.ChoiceData!.ArrayData!);
        Assert.AreEqual(5U, slotsDefault.ChoiceData!.ArrayData![5].Structure![0].GetValueAsUInt32());
        Assert.StartsWith("{\"choice\": 1,\"value\": [{\"0\": {\"0\": 0,\"1\": 0},\"1\": {\"0\": 1,\"1\": 0},", slotsDefault.ToJson());
    }

    [TestMethod]
    public void OptionalTest()
    {
        VersionedDecoder optional = Header.Structure![9];

        Assert.AreEqual(0x04, optional.DataType);
        Assert.IsNull(optional.OptionalData);
        Assert.AreEqual("null", optional.ToJson());
    }

    [TestMethod]
    public void UInt8Test()
    {
        VersionedDecoder u8 = Header.Structure![15];

        Assert.AreEqual(0x06, u8.DataType);
        Assert.AreEqual(1U, u8.GetValueAsUInt32());
        Assert.AreEqual("1", u8.ToJson());
        Assert.AreEqual("{\"0\": 0}", _versionedDecoder.Structure![1].ToJson());
    }

    [TestMethod]
    public void InvalidValueCallTest()
    {
        Assert.ThrowsExactly<InvalidOperationException>(() => Header.GetValueAsUInt32());
        Assert.ThrowsExactly<InvalidOperationException>(() => Attributes[0].Structure![1].GetValueAsInt64());
    }

    [TestMethod]
    public void ToJsonTest()
    {
        using JsonDocument jsonDocument = JsonDocument.Parse(_versionedDecoder.ToJson());

        JsonElement header = jsonDocument.RootElement.GetProperty("0");

        Assert.AreEqual("Teams10.StormMod", header.GetProperty("1").GetString());
        Assert.AreEqual(289930, header.GetProperty("3").GetInt32());
        Assert.AreEqual(JsonValueKind.Null, header.GetProperty("9").ValueKind);

        // binary blobs are written as hex
        Assert.StartsWith("0x73326d6100005553", header.GetProperty("2").GetString()); // "s2ma", "\0\0", "US"

        // the missing field tags are not written
        Assert.IsFalse(header.TryGetProperty("6", out _));
        Assert.IsFalse(header.TryGetProperty("11", out _));
        Assert.IsFalse(header.TryGetProperty("23", out _));
        Assert.IsTrue(header.TryGetProperty("24", out _));

        // tags
        JsonElement tags = header.GetProperty("18")[0];

        Assert.AreEqual("PRGN", tags.GetProperty("0").GetProperty("text").GetString());
        Assert.AreEqual(1347569486U, tags.GetProperty("0").GetProperty("value").GetUInt32());
        Assert.AreEqual("BLIZ", tags.GetProperty("1").GetProperty("text").GetString());
        Assert.AreEqual(1112295770U, tags.GetProperty("1").GetProperty("value").GetUInt32());

        // attribute default value
        JsonElement defaultValue = header.GetProperty("5")[0].GetProperty("2").GetProperty("8");

        Assert.AreEqual(0, defaultValue.GetProperty("choice").GetInt32());
        Assert.AreEqual(JsonValueKind.Object, defaultValue.GetProperty("value").ValueKind);
    }

    [TestMethod]
    public void ToStringTest()
    {
        string? text = _versionedDecoder.ToString();

        Assert.IsNotNull(text);
        Assert.Contains("\"Teams10.StormMod\"", text);
    }
}
