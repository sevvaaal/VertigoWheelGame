using UnityEngine;

[System.Serializable]
public class Reward
{
    [SerializeField] private RewardTypeSO type;
    [SerializeField] private int amount;

    public RewardTypeSO Type => type;
    public int Amount => amount;

    // Used by Unity when it loads rewards from wheel assets.
    public Reward()
    {
    }

    // Used by the game to create a reward with a zone-scaled amount.
    public Reward(RewardTypeSO type, int amount)
    {
        this.type = type;
        this.amount = amount;
    }
}
