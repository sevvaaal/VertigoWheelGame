using System.Collections.Generic;
using Vertigo.Core;
using Vertigo.Data;

namespace Vertigo.Gameplay
{
    public readonly struct RewardsCollectedEvent : IGameEvent
    {
        #region Constructor

        public RewardsCollectedEvent(IReadOnlyDictionary<RewardTypeSO, int> rewards)
        {
            Rewards = rewards;
        }

        #endregion

        #region Properties

        public IReadOnlyDictionary<RewardTypeSO, int> Rewards { get; }

        #endregion
    }
}
