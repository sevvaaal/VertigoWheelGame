using UnityEngine;

[System.Serializable]
public class Reward
{
    [SerializeField] private RewardTypeSO type;
    [SerializeField] private int amount;

    public RewardTypeSO Type => type;
    public int Amount => amount;
}