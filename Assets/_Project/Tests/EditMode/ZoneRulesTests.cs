using System;
using NUnit.Framework;
using Vertigo.Data;
using Vertigo.Gameplay;

namespace Vertigo.Tests.EditMode
{
    // The zone rules from the case, built from zone type data like the real ZoneConfig.
    public class ZoneRulesTests
    {
        #region Constants

        private const int SAFE_INTERVAL = 5;
        private const int SUPER_INTERVAL = 30;
        private const float REWARD_GROWTH = 0.1f;

        #endregion

        #region Fields

        private TestAssets _assets;
        private ZoneTypeSO _normal;
        private ZoneTypeSO _safe;
        private ZoneTypeSO _super;
        private ZoneRules _rules;

        #endregion

        #region Setup

        [SetUp]
        public void CreateRules()
        {
            _assets = new TestAssets();
            _normal = _assets.CreateZoneType(1, 1);
            _safe = _assets.CreateZoneType(SAFE_INTERVAL, 0);
            _super = _assets.CreateZoneType(SUPER_INTERVAL, 0);

            // Super above Safe, like the real ZoneConfig list.
            _rules = new ZoneRules(new[] { _super, _safe }, _normal, REWARD_GROWTH);
        }

        [TearDown]
        public void DestroyAssets()
        {
            _assets.DestroyAll();
        }

        #endregion

        #region Tests

        [TestCase(1)]
        [TestCase(4)]
        [TestCase(29)]
        public void GetZoneType_OrdinaryZone_IsDefaultType(int zone)
        {
            Assert.AreEqual(_normal, _rules.GetZoneType(zone));
        }

        [TestCase(5)]
        [TestCase(25)]
        public void GetZoneType_EveryFifthZone_IsSafe(int zone)
        {
            Assert.AreEqual(_safe, _rules.GetZoneType(zone));
        }

        [TestCase(30)]
        [TestCase(60)]
        public void GetZoneType_EveryThirtiethZone_IsSuper(int zone)
        {
            Assert.AreEqual(_super, _rules.GetZoneType(zone));
        }

        [Test]
        public void GetZoneType_ListOrderDecidesPriority()
        {
            // The same types in the other order: now 30 counts as safe, proving priority comes from data, not code.
            ZoneRules safeFirst = new ZoneRules(new[] { _safe, _super }, _normal, REWARD_GROWTH);

            Assert.AreEqual(_safe, safeFirst.GetZoneType(30));
        }

        [Test]
        public void Constructor_ZeroInterval_Throws()
        {
            Assert.Throws<ArgumentException>(CreateRulesWithZeroInterval);
        }

        [Test]
        public void ScaleAmount_FirstZonePaysBaseAmount()
        {
            Assert.AreEqual(100, _rules.ScaleAmount(100, 1));
        }

        [Test]
        public void ScaleAmount_GrowsTenPercentPerZone()
        {
            // Zone 11 is 10 zones after zone 1: 1 + 10 * 0.1 = x2.
            Assert.AreEqual(200, _rules.ScaleAmount(100, 11));
        }

        [Test]
        public void ScaleAmount_NeverDropsBelowOne()
        {
            Assert.AreEqual(1, _rules.ScaleAmount(0, 1));
        }

        [Test]
        public void GetNextZone_FindsNextSafeAndSuperZones()
        {
            Assert.AreEqual(30, _rules.GetNextZone(28, _super));

            // 30 is a super zone, not a safe one, so the next safe zone after 28 is 35.
            Assert.AreEqual(35, _rules.GetNextZone(28, _safe));
        }

        #endregion

        #region Helpers

        private void CreateRulesWithZeroInterval()
        {
            ZoneTypeSO broken = _assets.CreateZoneType(0, 0);
            new ZoneRules(new[] { broken }, _normal, REWARD_GROWTH);
        }

        #endregion
    }
}
