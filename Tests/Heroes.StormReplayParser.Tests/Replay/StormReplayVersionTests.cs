namespace Heroes.StormReplayParser.Replay.Tests;

[TestClass]
public class StormReplayVersionTests
{
    [TestMethod]
    [DataRow(1, 1, 1, 11111, 11111)]
    [DataRow(1, 1, 1, 11111, 11111)]
    public void EqualsTest(int major, int minor, int revision, int build, int baseBuild)
    {
        Assert.AreEqual(
            new StormReplayVersion()
            {
                Major = major,
                Minor = minor,
                Revision = revision,
                Build = build,
                BaseBuild = baseBuild,
            },
            new StormReplayVersion()
            {
                Major = major,
                Minor = minor,
                Revision = revision,
                Build = build,
                BaseBuild = baseBuild,
            });
    }

    [TestMethod]
    public void EqualsMethodTest()
    {
        Assert.IsFalse(
            new StormReplayVersion()
            {
                Major = 2,
                Minor = 3,
                Revision = 45,
                Build = 454545,
                BaseBuild = 454545,
            }
            .Equals((int?)null));

        Assert.IsFalse(
            new StormReplayVersion()
            {
                Major = 2,
                Minor = 3,
                Revision = 45,
                Build = 454545,
                BaseBuild = 454545,
            }
            .Equals(5));

        StormReplayVersion version = new StormReplayVersion()
        {
            Major = 2,
            Minor = 3,
            Revision = 45,
            Build = 454545,
            BaseBuild = 454545,
        };
        Assert.IsTrue(version.Equals(obj: version));
    }

    [TestMethod]
    public void CompareToMethodTest()
    {
        StormReplayVersion version = new StormReplayVersion()
        {
            Major = 2,
            Minor = 3,
            Revision = 45,
            Build = 454545,
            BaseBuild = 454545,
        };

        Assert.AreEqual(1, version.CompareTo((int?)null));
        Assert.ThrowsException<ArgumentException>(() =>
        {
            version.CompareTo(5);
        });

        Assert.AreEqual(0, version.CompareTo(obj: version));
    }

    [TestMethod]
    [DataRow(2, 2, 2, 11111, 11112)]
    [DataRow(-1, 1, 1, 11111, 11111)]
    [DataRow(-1, 1, 1, 20, 11111)]
    [DataRow(-1, 2, 1, 11111, 11111)]
    [DataRow(1, 1, -1, 11111, 11111)]
    [DataRow(1, 1, 1, 11111, 11111)]
    public void NotEqualsTest(int major, int minor, int revision, int build, int baseBuild)
    {
        StormReplayVersion version = new StormReplayVersion()
        {
            Major = 2,
            Minor = 3,
            Revision = 45,
            Build = 454545,
            BaseBuild = 454545,
        };

        Assert.AreNotEqual(version, new StormReplayVersion()
        {
            Major = major,
            Minor = minor,
            Revision = revision,
            Build = build,
            BaseBuild = baseBuild,
        });
    }

    [TestMethod]
    public void NotSameObjectTest()
    {
        StormReplayVersion version = new StormReplayVersion()
        {
            Major = 2,
            Minor = 3,
            Revision = 45,
            Build = 454545,
            BaseBuild = 454545,
        };

        Assert.AreNotEqual(new List<string>() { "asdf" }, version);
    }

    [TestMethod]
    [DataRow(2, 2, 2, 11111, 11111, 2, 2, 2, 11111, 11111)]
    public void OperatorEqualTest(int major, int minor, int revision, int build, int baseBuild, int major2, int minor2, int revision2, int build2, int baseBuild2)
    {
        StormReplayVersion version = new StormReplayVersion()
        {
            Major = major,
            Minor = minor,
            Revision = revision,
            Build = build,
            BaseBuild = baseBuild,
        };

        StormReplayVersion version2 = new StormReplayVersion()
        {
            Major = major2,
            Minor = minor2,
            Revision = revision2,
            Build = build2,
            BaseBuild = baseBuild2,
        };

#pragma warning disable SA1131 // Use readable conditions
        Assert.IsFalse(null! == version2);
#pragma warning restore SA1131 // Use readable conditions
        Assert.IsFalse(version2 == null!);

        Assert.IsTrue(null! == (StormReplayVersion)null!);
        Assert.IsTrue(version == version2);
    }

    [TestMethod]
    [DataRow(2, 2, 2, 11111, 11111, 3, 2, 2, 11111, 11111)]
    [DataRow(2, 2, 2, 11111, 11111, 2, 3, 2, 11111, 11111)]
    [DataRow(2, 2, 2, 11111, 11111, 2, 2, 8, 11111, 11111)]
    [DataRow(2, 2, 2, 11111, 11111, 2, 2, 2, 12112, 12112)]
    [DataRow(2, 2, 2, 11111, 11111, 2, 2, 2, 11111, 77777)]
    [DataRow(2, 2, 2, 11111, 11111, 2, 4, 78, 45781, 45781)]
    public void OperatorNotEqualTest(int major, int minor, int revision, int build, int baseBuild, int major2, int minor2, int revision2, int build2, int baseBuild2)
    {
        StormReplayVersion version = new StormReplayVersion()
        {
            Major = major,
            Minor = minor,
            Revision = revision,
            Build = build,
            BaseBuild = baseBuild,
        };

        StormReplayVersion version2 = new StormReplayVersion()
        {
            Major = major2,
            Minor = minor2,
            Revision = revision2,
            Build = build2,
            BaseBuild = baseBuild2,
        };

#pragma warning disable SA1131 // Use readable conditions
        Assert.IsTrue(null! != version2);
#pragma warning restore SA1131 // Use readable conditions
        Assert.IsTrue(version2 != null!);

        Assert.IsFalse(null! != (StormReplayVersion)null!);
        Assert.IsTrue(version != version2);
    }

    [TestMethod]
    [DataRow(1, 2, 2, 11111, 11111, 2, 2, 2, 11111, 11111)]
    [DataRow(2, 1, 2, 11111, 11111, 2, 2, 2, 11111, 11111)]
    [DataRow(2, 2, 0, 11111, 11111, 2, 2, 2, 11111, 11111)]
    [DataRow(2, 2, 2, 10000, 10000, 2, 2, 2, 11111, 11111)]
    [DataRow(2, 2, 2, 10000, 10000, 2, 2, 2, 11111, 11111)]
    [DataRow(2, 2, 2, 10000, 10000, 2, 2, 2, 10000, 77777)]
    public void OperatorLessThanTest(int major, int minor, int revision, int build, int baseBuild, int major2, int minor2, int revision2, int build2, int baseBuild2)
    {
        StormReplayVersion version = new StormReplayVersion()
        {
            Major = major,
            Minor = minor,
            Revision = revision,
            Build = build,
            BaseBuild = baseBuild,
        };

        StormReplayVersion version2 = new StormReplayVersion()
        {
            Major = major2,
            Minor = minor2,
            Revision = revision2,
            Build = build2,
            BaseBuild = baseBuild2,
        };

#pragma warning disable SA1131 // Use readable conditions
        Assert.IsTrue(null! < version2);
#pragma warning restore SA1131 // Use readable conditions
        Assert.IsFalse(version2 < null!);

        Assert.IsFalse(null! < (StormReplayVersion)null!);
#pragma warning disable CS1718 // Comparison made to same variable
        Assert.IsFalse(version < version);
#pragma warning restore CS1718 // Comparison made to same variable
        Assert.IsFalse(version2 < version);
        Assert.IsTrue(version < version2);
    }

    [TestMethod]
    [DataRow(1, 2, 2, 11111, 11111, 2, 2, 2, 11111, 11111)]
    [DataRow(2, 1, 2, 11111, 11111, 2, 2, 2, 11111, 11111)]
    [DataRow(2, 2, 0, 11111, 11111, 2, 2, 2, 11111, 11111)]
    [DataRow(2, 2, 2, 10000, 10000, 2, 2, 2, 11111, 11111)]
    [DataRow(2, 2, 2, 10000, 10000, 2, 2, 2, 11111, 11111)]
    [DataRow(2, 2, 2, 10000, 10000, 2, 2, 2, 10000, 10000)]
    public void OperatorLessThanOrEqualTest(int major, int minor, int revision, int build, int baseBuild, int major2, int minor2, int revision2, int build2, int baseBuild2)
    {
        StormReplayVersion version = new StormReplayVersion()
        {
            Major = major,
            Minor = minor,
            Revision = revision,
            Build = build,
            BaseBuild = baseBuild,
        };

        StormReplayVersion version2 = new StormReplayVersion()
        {
            Major = major2,
            Minor = minor2,
            Revision = revision2,
            Build = build2,
            BaseBuild = baseBuild2,
        };

#pragma warning disable SA1131 // Use readable conditions
        Assert.IsTrue(null! <= version2);
#pragma warning restore SA1131 // Use readable conditions
        Assert.IsFalse(version2 <= null!);

        Assert.IsTrue(null! <= (StormReplayVersion)null!);
#pragma warning disable CS1718 // Comparison made to same variable
        Assert.IsTrue(version <= version);
#pragma warning restore CS1718 // Comparison made to same variable
        Assert.IsTrue(version <= version2);
    }

    [TestMethod]
    [DataRow(2, 2, 2, 11111, 11111, 1, 2, 2, 11111, 11111)]
    [DataRow(2, 2, 2, 11111, 11111, 2, 1, 2, 11111, 11111)]
    [DataRow(2, 2, 2, 11111, 11111, 2, 2, 0, 11111, 11111)]
    [DataRow(2, 2, 2, 11111, 11111, 2, 2, 2, 10000, 10000)]
    [DataRow(2, 2, 2, 11111, 77777, 2, 2, 2, 11111, 11111)]
    [DataRow(2, 2, 2, 88888, 11111, 2, 2, 2, 11111, 11111)]
    public void OperatorGreaterThanTest(int major, int minor, int revision, int build, int baseBuild, int major2, int minor2, int revision2, int build2, int baseBuild2)
    {
        StormReplayVersion version = new StormReplayVersion()
        {
            Major = major,
            Minor = minor,
            Revision = revision,
            Build = build,
            BaseBuild = baseBuild,
        };

        StormReplayVersion version2 = new StormReplayVersion()
        {
            Major = major2,
            Minor = minor2,
            Revision = revision2,
            Build = build2,
            BaseBuild = baseBuild2,
        };

#pragma warning disable SA1131 // Use readable conditions
        Assert.IsFalse(null! > version2);
#pragma warning restore SA1131 // Use readable conditions
        Assert.IsTrue(version2 > null!);

        Assert.IsFalse(null! > (StormReplayVersion)null!);
#pragma warning disable CS1718 // Comparison made to same variable
        Assert.IsFalse(version > version);
#pragma warning restore CS1718 // Comparison made to same variable
        Assert.IsFalse(version2 > version);
        Assert.IsTrue(version > version2);
    }

    [TestMethod]
    [DataRow(2, 2, 2, 11111, 11111, 1, 2, 2, 11111, 11111)]
    [DataRow(2, 2, 2, 11111, 11111, 2, 1, 2, 11111, 11111)]
    [DataRow(2, 2, 2, 11111, 11111, 2, 2, 0, 11111, 11111)]
    [DataRow(2, 2, 2, 11111, 11111, 2, 2, 2, 10000, 10000)]
    [DataRow(2, 2, 2, 11111, 11111, 2, 2, 2, 10000, 10000)]
    [DataRow(2, 2, 2, 10000, 10000, 2, 2, 2, 10000, 10000)]
    public void OperatorGreaterThanOrEqualTest(int major, int minor, int revision, int build, int baseBuild, int major2, int minor2, int revision2, int build2, int baseBuild2)
    {
        StormReplayVersion version = new StormReplayVersion()
        {
            Major = major,
            Minor = minor,
            Revision = revision,
            Build = build,
            BaseBuild = baseBuild,
        };

        StormReplayVersion version2 = new StormReplayVersion()
        {
            Major = major2,
            Minor = minor2,
            Revision = revision2,
            Build = build2,
            BaseBuild = baseBuild2,
        };

#pragma warning disable SA1131 // Use readable conditions
        Assert.IsFalse(null! >= version2);
#pragma warning restore SA1131 // Use readable conditions
        Assert.IsTrue(version2 >= null!);

        Assert.IsTrue(null! >= (StormReplayVersion)null!);
#pragma warning disable CS1718 // Comparison made to same variable
        Assert.IsTrue(version >= version);
#pragma warning restore CS1718 // Comparison made to same variable
        Assert.IsTrue(version >= version2);
    }

    [TestMethod]
    [DataRow(2, 2, 2, 11111, 11111, 2, 2, 2, 11111, 11111)]
    public void GetHashCodeEqualTest(int major, int minor, int revision, int build, int baseBuild, int major2, int minor2, int revision2, int build2, int baseBuild2)
    {
        StormReplayVersion version = new StormReplayVersion()
        {
            Major = major,
            Minor = minor,
            Revision = revision,
            Build = build,
            BaseBuild = baseBuild,
        };

        StormReplayVersion version2 = new StormReplayVersion()
        {
            Major = major2,
            Minor = minor2,
            Revision = revision2,
            Build = build2,
            BaseBuild = baseBuild2,
        };

        Assert.AreEqual(version.GetHashCode(), version2.GetHashCode());
    }

    [TestMethod]
    [DataRow(2, 2, 2, 11111, 11111, 0, 2, 2, 11111, 11111)]
    [DataRow(2, 2, 2, 11111, 11111, 2, 74, 2, 11111, 11111)]
    [DataRow(2, 2, 2, 11111, 11111, 2, 2, 247, 11111, 11111)]
    [DataRow(2, 2, 2, 11111, 11111, 2, 2, 2, 12457, 11111)]
    [DataRow(2, 2, 2, 11111, 11111, 2, 2, 2, 11112, 11111)]
    [DataRow(2, 2, 2, 11111, 11111, 2, 2, 2, 11111, 88888)]
    [DataRow(2, 2, 2, 11111, 88888, 2, 2, 2, 11111, 11111)]
    public void GetHashCodeNotEqualTest(int major, int minor, int revision, int build, int baseBuild, int major2, int minor2, int revision2, int build2, int baseBuild2)
    {
        StormReplayVersion version = new StormReplayVersion()
        {
            Major = major,
            Minor = minor,
            Revision = revision,
            Build = build,
            BaseBuild = baseBuild,
        };

        StormReplayVersion version2 = new StormReplayVersion()
        {
            Major = major2,
            Minor = minor2,
            Revision = revision2,
            Build = build2,
            BaseBuild = baseBuild2,
        };

        Assert.AreNotEqual(version.GetHashCode(), version2.GetHashCode());
    }
}
