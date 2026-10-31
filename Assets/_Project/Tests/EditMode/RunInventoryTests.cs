using NUnit.Framework;
using Vertigo.Core;
using Vertigo.Data;
using Vertigo.Gameplay;

namespace Vertigo.Tests.EditMode
{
    public class RunInventoryTests
    {
        #region Fields

        private TestAssets _assets;
        private EventBus _eventBus;
        private RunInventory _inventory;
        private RewardTypeSO _gold;
        private RewardTypeSO _cash;
        private int _announcedTotal;
        private bool _wasCleared;

        #endregion

        #region Setup

        [SetUp]
        public void CreateInventory()
        {
            _assets = new TestAssets();
            _eventBus = new EventBus();
            _eventBus.Subscribe<RunRewardChangedEvent>(HandleRewardChanged);
            _eventBus.Subscribe<RunRewardsClearedEvent>(HandleRewardsCleared);
            _inventory = new RunInventory(_eventBus);
            _gold = _assets.CreateRewardType(RewardKind.Currency);
            _cash = _assets.CreateRewardType(RewardKind.Currency);
            _announcedTotal = 0;
            _wasCleared = false;
        }

        [TearDown]
        public void Cleanup()
        {
            _eventBus.Unsubscribe<RunRewardChangedEvent>(HandleRewardChanged);
            _eventBus.Unsubscribe<RunRewardsClearedEvent>(HandleRewardsCleared);
            _assets.DestroyAll();
        }

        #endregion

        #region Tests

        [Test]
        public void Add_SameTypeTwice_AddsUp()
        {
            _inventory.Add(new Reward(_gold, 100));
            _inventory.Add(new Reward(_gold, 50));

            Assert.AreEqual(150, _inventory.Amounts[_gold]);
        }

        [Test]
        public void Add_DifferentTypes_StaySeparate()
        {
            _inventory.Add(new Reward(_gold, 100));
            _inventory.Add(new Reward(_cash, 300));

            Assert.AreEqual(100, _inventory.Amounts[_gold]);
            Assert.AreEqual(300, _inventory.Amounts[_cash]);
        }

        [Test]
        public void Add_PublishesNewTotal()
        {
            _inventory.Add(new Reward(_gold, 100));
            _inventory.Add(new Reward(_gold, 50));

            Assert.AreEqual(150, _announcedTotal);
        }

        [Test]
        public void Clear_EmptiesEverythingAndPublishes()
        {
            _inventory.Add(new Reward(_gold, 100));

            _inventory.Clear();

            Assert.AreEqual(0, _inventory.Amounts.Count);
            Assert.IsTrue(_wasCleared);
        }

        #endregion

        #region Helpers

        private void HandleRewardChanged(RunRewardChangedEvent rewardChanged)
        {
            _announcedTotal = rewardChanged.Total;
        }

        private void HandleRewardsCleared(RunRewardsClearedEvent rewardsCleared)
        {
            _wasCleared = true;
        }

        #endregion
    }
}
