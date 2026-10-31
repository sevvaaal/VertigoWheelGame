using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vertigo.Core;
using Vertigo.Data;
using Vertigo.Economy;

namespace Vertigo.Tests.EditMode
{
    public class WalletTests
    {
        #region Fields

        private TestAssets _assets;
        private EventBus _eventBus;
        private Wallet _wallet;
        private RewardTypeSO _cash;
        private RewardTypeSO _gold;
        private RewardTypeSO _rifle;
        private WalletChangedEvent _lastChange;

        #endregion

        #region Setup

        [SetUp]
        public void CreateWallet()
        {
            _assets = new TestAssets();
            _eventBus = new EventBus();
            _eventBus.Subscribe<WalletChangedEvent>(HandleWalletChanged);
            _wallet = new Wallet(_eventBus);
            _cash = _assets.CreateRewardType(RewardKind.Currency);
            _gold = _assets.CreateRewardType(RewardKind.Currency);
            _rifle = _assets.CreateRewardType(RewardKind.Item);
        }

        [TearDown]
        public void Cleanup()
        {
            _eventBus.Unsubscribe<WalletChangedEvent>(HandleWalletChanged);
            _assets.DestroyAll();
        }

        #endregion

        #region Tests

        [Test]
        public void Deposit_KeepsCurrenciesAndItemsApart()
        {
            _wallet.Deposit(Run((_cash, 300), (_rifle, 1)));

            Assert.AreEqual(300, _wallet.Currencies[_cash]);
            Assert.AreEqual(1, _wallet.Items[_rifle]);
            Assert.IsFalse(_wallet.Currencies.ContainsKey(_rifle));
            Assert.IsFalse(_wallet.Items.ContainsKey(_cash));
        }

        [Test]
        public void Deposit_HoldsSeveralCurrencies()
        {
            _wallet.Deposit(Run((_cash, 300), (_gold, 20)));

            Assert.AreEqual(300, _wallet.GetAmount(_cash));
            Assert.AreEqual(20, _wallet.GetAmount(_gold));
        }

        [Test]
        public void Deposit_AddsUpAcrossRuns()
        {
            _wallet.Deposit(Run((_cash, 300)));
            _wallet.Deposit(Run((_cash, 200)));

            Assert.AreEqual(500, _wallet.GetAmount(_cash));
        }

        [Test]
        public void Deposit_PublishesNewTotal()
        {
            _wallet.Deposit(Run((_cash, 300)));
            _wallet.Deposit(Run((_cash, 200)));

            Assert.AreEqual(_cash, _lastChange.Type);
            Assert.AreEqual(500, _lastChange.Total);
        }

        [Test]
        public void TrySpend_EnoughBalance_Subtracts()
        {
            _wallet.Deposit(Run((_cash, 300)));

            Assert.IsTrue(_wallet.TrySpend(_cash, 100));
            Assert.AreEqual(200, _wallet.GetAmount(_cash));
        }

        [Test]
        public void TrySpend_NotEnoughBalance_KeepsBalance()
        {
            _wallet.Deposit(Run((_cash, 50)));

            Assert.IsFalse(_wallet.TrySpend(_cash, 100));
            Assert.AreEqual(50, _wallet.GetAmount(_cash));
        }

        [Test]
        public void TrySpend_Item_Throws()
        {
            Assert.Throws<ArgumentException>(SpendRifle);
        }

        #endregion

        #region Helpers

        private static Dictionary<RewardTypeSO, int> Run(params (RewardTypeSO Type, int Amount)[] rewards)
        {
            Dictionary<RewardTypeSO, int> run = new Dictionary<RewardTypeSO, int>();
            foreach ((RewardTypeSO type, int amount) in rewards)
            {
                run[type] = amount;
            }

            return run;
        }

        private void HandleWalletChanged(WalletChangedEvent walletChanged)
        {
            _lastChange = walletChanged;
        }

        private void SpendRifle()
        {
            _wallet.TrySpend(_rifle, 1);
        }

        #endregion
    }
}
