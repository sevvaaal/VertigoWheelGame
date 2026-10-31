using NUnit.Framework;
using Vertigo.Data;

namespace Vertigo.Tests.EditMode
{
    // The bomb rule and wheel tiers, checked through the same validation the editor and the bootstrapper use.
    public class ZoneDataValidationTests
    {
        #region Constants

        private const int REWARDS_PER_WHEEL = 7;

        #endregion

        #region Fields

        private TestAssets _assets;

        #endregion

        #region Setup

        [SetUp]
        public void CreateAssets()
        {
            _assets = new TestAssets();
        }

        [TearDown]
        public void DestroyAssets()
        {
            _assets.DestroyAll();
        }

        #endregion

        #region Tests

        [Test]
        public void NormalZone_WheelWithOneBomb_IsValid()
        {
            WheelSO wheel = _assets.CreateWheel(REWARDS_PER_WHEEL, 0);
            ZoneTypeSO normal = _assets.CreateZoneType(1, 1, (1, wheel));

            Assert.IsEmpty(normal.GetValidationErrors());
        }

        [Test]
        public void NormalZone_WheelWithoutBomb_IsInvalid()
        {
            WheelSO wheel = _assets.CreateWheel(REWARDS_PER_WHEEL + 1);
            ZoneTypeSO normal = _assets.CreateZoneType(1, 1, (1, wheel));

            Assert.IsNotEmpty(normal.GetValidationErrors());
        }

        [Test]
        public void SafeZone_WheelWithBomb_IsInvalid()
        {
            WheelSO wheel = _assets.CreateWheel(REWARDS_PER_WHEEL, 0);
            ZoneTypeSO safe = _assets.CreateZoneType(5, 0, (1, wheel));

            Assert.IsNotEmpty(safe.GetValidationErrors());
        }

        [TestCase(8)]
        [TestCase(-1)]
        public void Wheel_BombSlotOutsideWheel_IsInvalid(int bombSlot)
        {
            WheelSO wheel = _assets.CreateWheel(REWARDS_PER_WHEEL, bombSlot);

            Assert.IsNotEmpty(wheel.GetValidationErrors());
        }

        [Test]
        public void Wheel_SameBombSlotTwice_IsInvalid()
        {
            WheelSO wheel = _assets.CreateWheel(REWARDS_PER_WHEEL - 1, 0, 0);

            Assert.IsNotEmpty(wheel.GetValidationErrors());
        }

        [Test]
        public void Wheel_BombSlice_HasNoReward()
        {
            WheelSO wheel = _assets.CreateWheel(REWARDS_PER_WHEEL, 3);

            Assert.AreEqual(REWARDS_PER_WHEEL + 1, wheel.Slices.Count);
            Assert.IsTrue(wheel.Slices[3].IsBomb);
            Assert.IsNull(wheel.Slices[3].Reward);
            Assert.IsFalse(wheel.Slices[0].IsBomb);
            Assert.IsNotNull(wheel.Slices[0].Reward);
        }

        [TestCase(1, 0)]
        [TestCase(9, 0)]
        [TestCase(10, 1)]
        [TestCase(25, 1)]
        public void GetWheel_UsesLastReachedTier(int zone, int expectedTier)
        {
            WheelSO early = _assets.CreateWheel(REWARDS_PER_WHEEL, 0);
            WheelSO later = _assets.CreateWheel(REWARDS_PER_WHEEL, 0);
            ZoneTypeSO normal = _assets.CreateZoneType(1, 1, (1, early), (10, later));

            WheelSO expected = expectedTier == 0 ? early : later;
            Assert.AreEqual(expected, normal.GetWheel(zone));
        }

        [Test]
        public void WheelTiers_NotStartingAtZoneOne_IsInvalid()
        {
            WheelSO wheel = _assets.CreateWheel(REWARDS_PER_WHEEL, 0);
            ZoneTypeSO normal = _assets.CreateZoneType(1, 1, (5, wheel));

            Assert.IsNotEmpty(normal.GetValidationErrors());
        }

        #endregion
    }
}
