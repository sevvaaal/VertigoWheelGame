using UnityEngine;

[CreateAssetMenu(fileName = "RewardType", menuName = "Vertigo/Reward Type")]
public class RewardTypeSO : ScriptableObject
{
    [SerializeField] private string displayName;
    [SerializeField] private Sprite icon;

    public string DisplayName => displayName;
    public Sprite Icon => icon;
}