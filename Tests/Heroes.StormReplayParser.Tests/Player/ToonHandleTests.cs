namespace Heroes.StormReplayParser.Tests.Player;

[TestClass]
public class ToonHandleTests
{
    [TestMethod]
    [DataRow(1, 123456789, 2, 1457856945)]
    [DataRow(1, 123456789, 2, 456)]
    public void EqualsTest(int region, int programId, int realm, long id)
    {
        Assert.AreEqual(
            new ToonHandle()
            {
                Region = region,
                ProgramId = programId,
                Realm = realm,
                Id = id,
            },
            new ToonHandle()
            {
                Region = region,
                ProgramId = programId,
                Realm = realm,
                Id = id,
            });
    }

    [TestMethod]
    public void EqualsMethodTest()
    {
        Assert.IsFalse(
            new ToonHandle()
            {
                Region = 1,
                ProgramId = 123456789,
                Realm = 2,
                Id = 234234,
            }
            .Equals((int?)null));

        Assert.IsFalse(
            new ToonHandle()
            {
                Region = 1,
                ProgramId = 123456789,
                Realm = 2,
                Id = 234234,
            }
            .Equals(5));

        ToonHandle toonHandle = new ToonHandle()
        {
            Region = 1,
            ProgramId = 123456789,
            Realm = 2,
            Id = 234234,
        };
        Assert.IsTrue(toonHandle.Equals(obj: toonHandle));
    }

    [TestMethod]
    [DataRow(1, 123456789, 2, 1457856945)]
    [DataRow(1, 123456789, 2, 345345)]
    [DataRow(5, 123456789, 2, 345345)]
    [DataRow(5, 123456789, 4, 345345)]
    [DataRow(5, 123456789, 2, 345345)]
    [DataRow(5, 11111, 2, 345345)]
    [DataRow(5, 123456789, 2, 45757)]
    public void NotEqualsTest(int region, int programId, int realm, long id)
    {
        ToonHandle toonHandle = new ToonHandle()
        {
            Region = 1,
            ProgramId = 123456789,
            Realm = 2,
            Id = 234234,
        };

        Assert.AreNotEqual(toonHandle, new ToonHandle()
        {
            Region = region,
            ProgramId = programId,
            Realm = realm,
            Id = id,
        });
    }

    [TestMethod]
    public void NotSameObjectTest()
    {
        ToonHandle toonHandle = new ToonHandle()
        {
            Region = 1,
            ProgramId = 123456789,
            Realm = 2,
            Id = 234234,
        };

        Assert.AreNotEqual(new List<string>() { "asdf" }, toonHandle);
    }

    [TestMethod]
    [DataRow(1, 123456789, 2, 1457856945, 1, 123456789, 2, 1457856945)]
    public void OperatorEqualTest(int region, int programId, int realm, long id, int region2, int programId2, int realm2, long id2)
    {
        ToonHandle toonHandle = new ToonHandle()
        {
            Region = region,
            ProgramId = programId,
            Realm = realm,
            Id = id,
        };

        ToonHandle toonHandle2 = new ToonHandle()
        {
            Region = region2,
            ProgramId = programId2,
            Realm = realm2,
            Id = id2,
        };

#pragma warning disable SA1131 // Use readable conditions
        Assert.IsFalse(null! == toonHandle2);
#pragma warning restore SA1131 // Use readable conditions
        Assert.IsFalse(toonHandle2 == null!);

        Assert.IsTrue(null! == (ToonHandle)null!);
        Assert.IsTrue(toonHandle == toonHandle2);
    }

    [TestMethod]
    [DataRow(1, 123456789, 2, 1457856945, 1, 123456789, 2, 145785694588888)]
    [DataRow(1, 123456789, 2, 1457856945, 1, 123456789, 2, 2344)]
    [DataRow(1, 123456789, 2, 1457856945, 1, 234, 3, 1457856945)]
    [DataRow(1, 123456789, 2, 1457856945, 1, 123456789, 3, 1457856945)]
    [DataRow(1, 123456789, 2, 1457856945, 2, 123456789, 2, 1457856945)]

    public void OperatorNotEqualTest(int region, int programId, int realm, long id, int region2, int programId2, int realm2, long id2)
    {
        ToonHandle toonHandle = new ToonHandle()
        {
            Region = region,
            ProgramId = programId,
            Realm = realm,
            Id = id,
        };

        ToonHandle toonHandle2 = new ToonHandle()
        {
            Region = region2,
            ProgramId = programId2,
            Realm = realm2,
            Id = id2,
        };

#pragma warning disable SA1131 // Use readable conditions
        Assert.IsTrue(null! != toonHandle2);
#pragma warning restore SA1131 // Use readable conditions
        Assert.IsTrue(toonHandle2 != null!);

        Assert.IsFalse(null! != (ToonHandle)null!);
        Assert.IsTrue(toonHandle != toonHandle2);
    }

    [TestMethod]
    [DataRow(1, 123456789, 2, 1457856945, 1, 123456789, 2, 1457856945)]
    public void GetHashCodeEqualTest(int region, int programId, int realm, long id, int region2, int programId2, int realm2, long id2)
    {
        ToonHandle toonHandle = new ToonHandle()
        {
            Region = region,
            ProgramId = programId,
            Realm = realm,
            Id = id,
        };

        ToonHandle toonHandle2 = new ToonHandle()
        {
            Region = region2,
            ProgramId = programId2,
            Realm = realm2,
            Id = id2,
        };

        Assert.AreEqual(toonHandle.GetHashCode(), toonHandle2.GetHashCode());
    }

    [TestMethod]
    [DataRow(1, 123456789, 2, 1457856945, 1, 123456789, 2, 145785694588888)]
    [DataRow(1, 123456789, 2, 1457856945, 1, 123456789, 2, 2344)]
    [DataRow(1, 123456789, 2, 1457856945, 1, 234, 3, 1457856945)]
    [DataRow(1, 123456789, 2, 1457856945, 1, 123456789, 3, 1457856945)]
    [DataRow(1, 123456789, 2, 1457856945, 2, 123456789, 2, 1457856945)]
    public void GetHashCodeNotEqualTest(int region, int programId, int realm, long id, int region2, int programId2, int realm2, long id2)
    {
        ToonHandle toonHandle = new ToonHandle()
        {
            Region = region,
            ProgramId = programId,
            Realm = realm,
            Id = id,
        };

        ToonHandle toonHandle2 = new ToonHandle()
        {
            Region = region2,
            ProgramId = programId2,
            Realm = realm2,
            Id = id2,
        };

        Assert.AreNotEqual(toonHandle.GetHashCode(), toonHandle2.GetHashCode());
    }
}
