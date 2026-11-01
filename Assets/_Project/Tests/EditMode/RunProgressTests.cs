using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vertigo.Data;
using Vertigo.Gameplay;

namespace Vertigo.Tests.EditMode
{
    public class RunProgressTests
    {
        #region Constants

        private const int VISIBLE_ZONES = 9;
        private const int REWARDS_PER_WHEEL = 7;
        private const float REWARD_GROWTH = 0.1f;

        #endregion

        #region Fields

        private TestAssets _assets;
        private WheelSO _normalWheel;
        private WheelSO _safeWheel;
        private ZoneTypeSO _safe;
        private ZoneTypeSO _super;
        private RunProgress _progress;

        #endregion

        #region Setup

        [SetUp]
        public void CreateProgress()
        {
            _assets = new TestAssets();
            _normalWheel = _assets.CreateWheel(REWARDS_PER_WHEEL, 0);
            _safeWheel = _assets.CreateWheel(REWARDS_PER_WHEEL + 1);
            ZoneTypeSO normal = _assets.CreateZoneType(1, 1, (1, _normalWheel));
            _safe = _assets.CreateZoneType(5, 0, (1, _safeWheel));
            _super = _assets.CreateZoneType(30, 0, (1, _safeWheel));

            ZoneRules rules = new ZoneRules(new[] { _super, _safe }, normal, REWARD_GROWTH);
            _progress = new RunProgress(rules, VISIBLE_ZONES);
        }

        [TearDown]
        public void DestroyAssets()
        {
            _assets.DestroyAll();
        }

        #endregion

        #region Tests

        [Test]
        public void StartsAtZoneOne_WithNormalWheel()
        {
            Assert.AreEqual(1, _progress.CurrentZone);
            Assert.AreEqual(_normalWheel, _progress.CurrentWheel);
        }

        [Test]
        public void AdvanceToFifthZone_UsesSafeWheel()
        {
            AdvanceTo(5);

            Assert.AreEqual(_safe, _progress.CurrentZoneType);
            Assert.AreEqual(_safeWheel, _progress.CurrentWheel);
        }

        [Test]
        public void Reset_ReturnsToZoneOne()
        {
            AdvanceTo(7);

            _progress.Reset();

            Assert.AreEqual(1, _progress.CurrentZone);
        }

        [Test]
        public void GetReward_ScalesWithZone()
        {
            // Zone 11: 1 + 10 * 0.1 = x2 on the base amount of 1.
            AdvanceTo(11);
            SliceData rewardSlice = _progress.CurrentWheel.Slices[1];

            Assert.AreEqual(2, _progress.GetReward(rewardSlice).Amount);
        }

        [Test]
        public void GetReward_ForBomb_Throws()
        {
            Assert.Throws<ArgumentException>(GetRewardForBomb);
        }

        [Test]
        public void GetSliceAmounts_BombShowsZero()
        {
            IReadOnlyList<int> amounts = _progress.GetSliceAmounts();

            Assert.AreEqual(REWARDS_PER_WHEEL + 1, amounts.Count);
            Assert.AreEqual(0, amounts[0]);
            Assert.AreEqual(1, amounts[1]);
        }

        [Test]
        public void BuildZoneBarModel_CentersCurrentZone()
        {
            ZoneBarModel model = _progress.BuildZoneBarModel();

            Assert.AreEqual(VISIBLE_ZONES, model.Cells.Count);
            Assert.IsTrue(model.Cells[0].IsEmpty);
            Assert.AreEqual(1, model.Cells[4].Zone);
            Assert.AreEqual(2, model.Cells[5].Zone);
            Assert.AreEqual(_safe, model.Cells[8].ZoneType);
        }

        [Test]
        public void BuildZoneBarModel_HasNextZoneOfEverySpecialType()
        {
            AdvanceTo(6);

            ZoneBarModel model = _progress.BuildZoneBarModel();

            Assert.AreEqual(10, model.NextZones[_safe]);
            Assert.AreEqual(30, model.NextZones[_super]);
            Assert.IsTrue(model.Cells[0].IsPast);
        }

        #endregion

        #region Helpers

        private void AdvanceTo(int zone)
        {
            while (_progress.CurrentZone < zone)
            {
                _progress.Advance();
            }
        }

        private void GetRewardForBomb()
        {
            _progress.GetReward(_progress.CurrentWheel.Slices[0]);
        }

        #endregion
    }
}
