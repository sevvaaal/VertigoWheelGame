using System.Collections.Generic;
using Vertigo.Data;

namespace Vertigo.Economy
{
    public interface IWallet
    {
        IReadOnlyDictionary<RewardTypeSO, int> Currencies { get; }

        IReadOnlyDictionary<RewardTypeSO, int> Items { get; }

        int GetAmount(RewardTypeSO type);

        void Deposit(IReadOnlyDictionary<RewardTypeSO, int> rewards);

        bool TrySpend(RewardTypeSO currency, int amount);
    }
}
