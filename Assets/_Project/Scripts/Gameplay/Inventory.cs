using System;
using System.Collections.Generic;

// The rewards collected in this run. Plain C# with no UI, so it can be tested on its own.
public class Inventory
{
    private readonly Dictionary<RewardTypeSO, int> amounts = new Dictionary<RewardTypeSO, int>();

    public event Action<RewardTypeSO, int> AmountChanged;
    public event Action Cleared;

    public IReadOnlyDictionary<RewardTypeSO, int> Amounts => amounts;

    public void Add(Reward reward)
    {
        amounts.TryGetValue(reward.Type, out int current);
        amounts[reward.Type] = current + reward.Amount;

        AmountChanged?.Invoke(reward.Type, amounts[reward.Type]);
    }

    // The bomb: everything collected so far is lost.
    public void Clear()
    {
        amounts.Clear();
        Cleared?.Invoke();
    }
}
