using Vertigo.Core;
using Vertigo.Data;

namespace Vertigo.Gameplay
{
    public readonly struct RunRewardChangedEvent : IGameEvent
    {
        #region Constructor

        public RunRewardChangedEvent(RewardTypeSO type, int total)
        {
            Type = type;
            Total = total;
        }

        #endregion

        #region Properties

        public RewardTypeSO Type { get; }

        public int Total { get; }

        #endregion
    }
}
