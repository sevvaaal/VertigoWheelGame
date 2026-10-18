using NUnit.Framework;

// The zone rules from the case, checked without any scene or UI.
public class ZoneRulesTests
{
    private ZoneRules rules;

    [SetUp]
    public void CreateRules()
    {
        // The same numbers as the ZoneConfig asset: safe every 5th, super every 30th, rewards +10% per zone.
        rules = new ZoneRules(5, 30, 0.1f);
    }

    [TestCase(1, ZoneType.Normal)]
    [TestCase(4, ZoneType.Normal)]
    [TestCase(5, ZoneType.Safe)]
    [TestCase(25, ZoneType.Safe)]
    [TestCase(30, ZoneType.Super)]
    [TestCase(60, ZoneType.Super)]
    public void GetZoneType_ReturnsTypeForZone(int zone, ZoneType expected)
    {
        Assert.AreEqual(expected, rules.GetZoneType(zone));
    }

    [Test]
    public void Zone30_IsSuperEvenThoughDivisibleBy5()
    {
        Assert.AreEqual(ZoneType.Super, rules.GetZoneType(30));
    }

    [TestCase(false, true)]
    [TestCase(true, false)]
    public void CanLeave_OnlyWhileWheelIsStill(bool isSpinning, bool expected)
    {
        Assert.AreEqual(expected, rules.CanLeave(isSpinning));
    }

    [Test]
    public void ScaleAmount_FirstZonePaysBaseAmount()
    {
        Assert.AreEqual(100, rules.ScaleAmount(100, 1));
    }

    [Test]
    public void ScaleAmount_GrowsTenPercentPerZone()
    {
        // Zone 11 is 10 zones after zone 1: 1 + 10 * 0.1 = x2.
        Assert.AreEqual(200, rules.ScaleAmount(100, 11));
    }

    [Test]
    public void ScaleAmount_NeverDropsBelowOne()
    {
        Assert.AreEqual(1, rules.ScaleAmount(0, 1));
    }

    [Test]
    public void GetNextZone_FindsNextSafeAndSuperZones()
    {
        Assert.AreEqual(30, rules.GetNextZone(28, ZoneType.Super));

        // 30 is a super zone, not a safe one, so the next safe zone after 28 is 35.
        Assert.AreEqual(35, rules.GetNextZone(28, ZoneType.Safe));
    }
}
