using System.Collections.Generic;
using Vertigo.Data;

namespace Vertigo.Gameplay
{
    public interface IRunInventory
    {
        IReadOnlyDictionary<RewardTypeSO, int> Amounts { get; }

        void Add(Reward reward);

        void Clear();
    }
}
