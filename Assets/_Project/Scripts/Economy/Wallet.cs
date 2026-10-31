using System;
using System.Collections.Generic;
using Vertigo.Core;
using Vertigo.Data;

namespace Vertigo.Economy
{
    // Everything the player has taken home, across runs. Currencies and items are kept apart:
    // currencies are balances that can be spent, items are only owned.
    public sealed class Wallet : IWallet
    {
        #region Fields

        private readonly IEventBus _eventBus;
        private readonly Dictionary<RewardTypeSO, int> _currencies = new Dictionary<RewardTypeSO, int>();
        private readonly Dictionary<RewardTypeSO, int> _items = new Dictionary<RewardTypeSO, int>();

        #endregion

        #region Constructor

        public Wallet(IEventBus eventBus)
        {
            if (eventBus == null)
            {
                throw new ArgumentNullException(nameof(eventBus));
            }

            _eventBus = eventBus;
        }

        #endregion

        #region Properties

        public IReadOnlyDictionary<RewardTypeSO, int> Currencies
        {
            get { return _currencies; }
        }

        public IReadOnlyDictionary<RewardTypeSO, int> Items
        {
            get { return _items; }
        }

        #endregion

        #region Public Methods

        public int GetAmount(RewardTypeSO type)
        {
            GetStore(type).TryGetValue(type, out int amount);
            return amount;
        }

        public void Deposit(IReadOnlyDictionary<RewardTypeSO, int> rewards)
        {
            foreach (KeyValuePair<RewardTypeSO, int> reward in rewards)
            {
                if (reward.Value <= 0)
                {
                    continue;
                }

                Dictionary<RewardTypeSO, int> store = GetStore(reward.Key);
                store.TryGetValue(reward.Key, out int current);
                store[reward.Key] = current + reward.Value;

                _eventBus.Publish(new WalletChangedEvent(reward.Key, store[reward.Key]));
            }
        }

        // Ready for spending currency, e.g. the optional "continue after a bomb" bonus.
        public bool TrySpend(RewardTypeSO currency, int amount)
        {
            if (!currency || currency.Kind != RewardKind.Currency)
            {
                throw new ArgumentException("Only currencies can be spent.", nameof(currency));
            }

            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "The amount to spend must be positive.");
            }

            _currencies.TryGetValue(currency, out int balance);
            if (balance < amount)
            {
                return false;
            }

            _currencies[currency] = balance - amount;
            _eventBus.Publish(new WalletChangedEvent(currency, _currencies[currency]));
            return true;
        }

        #endregion

        #region Private Methods

        private Dictionary<RewardTypeSO, int> GetStore(RewardTypeSO type)
        {
            return type.Kind == RewardKind.Currency ? _currencies : _items;
        }

        #endregion
    }
}
