using UnityEngine;

[System.Serializable]
public class SliceData
{
    [SerializeField] private bool isBomb;
    [SerializeField] private Reward reward;

    public bool IsBomb => isBomb;
    public Reward Reward => reward;
}