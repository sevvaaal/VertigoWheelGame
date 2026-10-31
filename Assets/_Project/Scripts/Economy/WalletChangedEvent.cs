using Vertigo.Core;
using Vertigo.Data;

namespace Vertigo.Economy
{
    public readonly struct WalletChangedEvent : IGameEvent
    {
        #region Constructor

        public WalletChangedEvent(RewardTypeSO type, int total)
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
