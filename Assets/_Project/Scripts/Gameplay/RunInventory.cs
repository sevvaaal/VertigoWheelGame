using System;
using System.Collections.Generic;
using Vertigo.Core;
using Vertigo.Data;

namespace Vertigo.Gameplay
{
    // What the player has won in the current run. Lost on a bomb; moved into the wallet on Leave.
    public sealed class RunInventory
    {
        #region Fields

        private readonly IEventBus _eventBus;
        private readonly Dictionary<RewardTypeSO, int> _amounts = new Dictionary<RewardTypeSO, int>();

        #endregion

        #region Constructor

        public RunInventory(IEventBus eventBus)
        {
            if (eventBus == null)
            {
                throw new ArgumentNullException(nameof(eventBus));
            }

            _eventBus = eventBus;
        }

        #endregion

        #region Properties

        public IReadOnlyDictionary<RewardTypeSO, int> Amounts
        {
            get { return _amounts; }
        }

        #endregion

        #region Public Methods

        public void Add(Reward reward)
        {
            if (reward == null || !reward.Type)
            {
                throw new ArgumentException("A reward needs a reward type.", nameof(reward));
            }

            _amounts.TryGetValue(reward.Type, out int current);
            int total = current + reward.Amount;
            _amounts[reward.Type] = total;

            _eventBus.Publish(new RunRewardChangedEvent(reward.Type, total));
        }

        public void Clear()
        {
            _amounts.Clear();
            _eventBus.Publish(new RunRewardsClearedEvent());
        }

        #endregion
    }
}
