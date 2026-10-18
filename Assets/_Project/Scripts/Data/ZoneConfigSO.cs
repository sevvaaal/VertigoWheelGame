using UnityEngine;

// Which wheel each zone type uses and how zones are spaced. Editable in the Inspector, no code changes needed.
[CreateAssetMenu(fileName = "ZoneConfig", menuName = "Vertigo/Zone Config")]
public class ZoneConfigSO : ScriptableObject
{
    [SerializeField] private WheelSO bronzeWheel;
    [SerializeField] private WheelSO silverWheel;
    [SerializeField] private WheelSO goldenWheel;
    [SerializeField] private int safeZoneInterval = 5;
    [SerializeField] private int superZoneInterval = 30;
    [SerializeField] private float rewardGrowthPerZone = 0.1f;

    public int SafeZoneInterval => safeZoneInterval;
    public int SuperZoneInterval => superZoneInterval;
    public float RewardGrowthPerZone => rewardGrowthPerZone;

    public WheelSO GetWheel(ZoneType type)
    {
        switch (type)
        {
            case ZoneType.Super: return goldenWheel;
            case ZoneType.Safe: return silverWheel;
            default: return bronzeWheel;
        }
    }
}
