namespace Heroes.StormReplayParser.GameEvent.Tests;

[TestClass]
public class StormGameEventDataTests
{
    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void BooleanTest(bool value)
    {
        StormGameEventData data = new(value);

        Assert.AreEqual(StormGameEventDataType.Bool, data.DataType);
        Assert.AreEqual(value, data.Boolean);
        AssertOtherValuesAreNull(data);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(-5)]
    [DataRow(int.MinValue)]
    [DataRow(int.MaxValue)]
    public void Integer32Test(int value)
    {
        StormGameEventData data = new(value);

        Assert.AreEqual(StormGameEventDataType.Integer32, data.DataType);
        Assert.AreEqual(value, data.Integer32);
        AssertOtherValuesAreNull(data);
    }

    [TestMethod]
    [DataRow(0U)]
    [DataRow(uint.MaxValue)]
    public void UnsignedInteger32Test(uint value)
    {
        StormGameEventData data = new(value);

        Assert.AreEqual(StormGameEventDataType.UnsignedInteger32, data.DataType);
        Assert.AreEqual(value, data.UnsignedInteger32);
        AssertOtherValuesAreNull(data);
    }

    [TestMethod]
    [DataRow(0UL)]
    [DataRow(ulong.MaxValue)]
    public void UnsignedInteger64Test(ulong value)
    {
        StormGameEventData data = new(value);

        Assert.AreEqual(StormGameEventDataType.UnsignedInteger64, data.DataType);
        Assert.AreEqual(value, data.UnsignedInteger64);
        AssertOtherValuesAreNull(data);
    }

    [TestMethod]
    public void BlobTest()
    {
        StormGameEventData data = new("Hanamura");

        Assert.AreEqual(StormGameEventDataType.Blob, data.DataType);
        Assert.AreEqual("Hanamura", data.Blob);
        AssertOtherValuesAreNull(data);
    }

    [TestMethod]
    public void NullBlobTest()
    {
        StormGameEventData data = new(value: (string?)null);

        Assert.AreEqual(StormGameEventDataType.Blob, data.DataType);
        Assert.IsNull(data.Blob);
        AssertOtherValuesAreNull(data);
    }

    [TestMethod]
    public void StructureTest()
    {
        StormDataStructure<StormGameEventData> structure = new(2)
        {
            [0] = new StormGameEventData(5U),
            [1] = new StormGameEventData(true),
        };

        StormGameEventData data = new(structure);

        Assert.AreEqual(StormGameEventDataType.Structure, data.DataType);
        Assert.AreSame(structure, data.Structure);
        Assert.AreEqual("{5, True}", data.ToString());
        AssertOtherValuesAreNull(data);
    }

    [TestMethod]
    public void NullStructureTest()
    {
        StormGameEventData data = new(structure: null);

        Assert.AreEqual(StormGameEventDataType.Structure, data.DataType);
        Assert.IsNull(data.Structure);
        AssertOtherValuesAreNull(data);
    }

    [TestMethod]
    public void ArrayTest()
    {
        StormGameEventData[] array = [new StormGameEventData(-1), new StormGameEventData("a")];

        StormGameEventData data = new(array);

        Assert.AreEqual(StormGameEventDataType.Array, data.DataType);
        Assert.AreSame(array, data.Array);
        Assert.AreEqual("[-1, \"a\"]", data.ToString());
        AssertOtherValuesAreNull(data);
    }

    [TestMethod]
    public void BitArrayTest()
    {
        bool[] bitArray = [true, false, true];

        StormGameEventData data = new(bitArray);

        Assert.AreEqual(StormGameEventDataType.BitArray, data.DataType);
        Assert.AreSame(bitArray, data.BitArray);
        Assert.AreEqual("[True,False,True]", data.ToJson());
        AssertOtherValuesAreNull(data);
    }

    // only the property of the data type has a value
    private static void AssertOtherValuesAreNull(StormGameEventData data)
    {
        if (data.DataType != StormGameEventDataType.Bool)
            Assert.IsNull(data.Boolean);
        if (data.DataType != StormGameEventDataType.Integer32)
            Assert.IsNull(data.Integer32);
        if (data.DataType != StormGameEventDataType.UnsignedInteger32)
            Assert.IsNull(data.UnsignedInteger32);
        if (data.DataType != StormGameEventDataType.UnsignedInteger64)
            Assert.IsNull(data.UnsignedInteger64);
        if (data.DataType != StormGameEventDataType.Blob)
            Assert.IsNull(data.Blob);
        if (data.DataType != StormGameEventDataType.Structure)
            Assert.IsNull(data.Structure);
        if (data.DataType != StormGameEventDataType.Array)
            Assert.IsNull(data.Array);
        if (data.DataType != StormGameEventDataType.BitArray)
            Assert.IsNull(data.BitArray);
    }
}
