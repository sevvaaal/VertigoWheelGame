using System;

// The zone rules as pure logic: no Unity objects, so they can be unit tested.
// The numbers come from the ZoneConfig asset, so designers can change them without touching code.
public class ZoneRules
{
    private readonly int safeZoneInterval;
    private readonly int superZoneInterval;
    private readonly float rewardGrowthPerZone;

    public ZoneRules(int safeZoneInterval, int superZoneInterval, float rewardGrowthPerZone)
    {
        this.safeZoneInterval = safeZoneInterval;
        this.superZoneInterval = superZoneInterval;
        this.rewardGrowthPerZone = rewardGrowthPerZone;
    }

    public ZoneType GetZoneType(int zone)
    {
        // Super is checked first: zone 30 is also divisible by 5, but it must be a super zone.
        if (zone % superZoneInterval == 0)
        {
            return ZoneType.Super;
        }

        if (zone % safeZoneInterval == 0)
        {
            return ZoneType.Safe;
        }

        return ZoneType.Normal;
    }

    // The first zone of this type at or after fromZone. Used for the "next safe / next super zone" labels.
    public int GetNextZone(int fromZone, ZoneType type)
    {
        int zone = fromZone;
        while (GetZoneType(zone) != type)
        {
            zone++;
        }

        return zone;
    }

    // Rewards get better every zone: zone 1 pays the base amount, each later zone a bit more.
    public float GetRewardMultiplier(int zone)
    {
        return 1f + (zone - 1) * rewardGrowthPerZone;
    }

    public int ScaleAmount(int baseAmount, int zone)
    {
        return Math.Max(1, (int)Math.Round(baseAmount * GetRewardMultiplier(zone)));
    }
}
