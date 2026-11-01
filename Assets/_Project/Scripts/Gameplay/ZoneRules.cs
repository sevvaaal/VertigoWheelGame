using System;
using System.Collections.Generic;
using Vertigo.Data;

namespace Vertigo.Gameplay
{
    // The zone rules as plain logic: which type a zone is, when the next one of a type comes, how rewards grow.
    public sealed class ZoneRules : IZoneRules
    {
        #region Constants

        // A zone type that can never appear (always claimed by a higher-priority type) must not hang the search.
        private const int MAX_ZONE_SEARCH = 10000;

        #endregion

        #region Fields

        private readonly IReadOnlyList<ZoneTypeSO> _specialZoneTypes;
        private readonly ZoneTypeSO _defaultZoneType;
        private readonly float _rewardGrowthPerZone;

        #endregion

        #region Constructor

        public ZoneRules(IReadOnlyList<ZoneTypeSO> specialZoneTypes, ZoneTypeSO defaultZoneType, float rewardGrowthPerZone)
        {
            if (specialZoneTypes == null)
            {
                throw new ArgumentNullException(nameof(specialZoneTypes));
            }

            if (!defaultZoneType)
            {
                throw new ArgumentNullException(nameof(defaultZoneType));
            }

            foreach (ZoneTypeSO zoneType in specialZoneTypes)
            {
                if (!zoneType || zoneType.Interval < 1)
                {
                    throw new ArgumentException("Every special zone type needs an interval of at least 1.", nameof(specialZoneTypes));
                }
            }

            _specialZoneTypes = specialZoneTypes;
            _defaultZoneType = defaultZoneType;
            _rewardGrowthPerZone = rewardGrowthPerZone;
        }

        #endregion

        #region Properties

        public IReadOnlyList<ZoneTypeSO> SpecialZoneTypes
        {
            get { return _specialZoneTypes; }
        }

        #endregion

        #region Public Methods

        public ZoneTypeSO GetZoneType(int zone)
        {
            foreach (ZoneTypeSO zoneType in _specialZoneTypes)
            {
                if (zone % zoneType.Interval == 0)
                {
                    return zoneType;
                }
            }

            return _defaultZoneType;
        }

        // The first zone of this type at or after fromZone. Used for the "next safe / next super zone" labels.
        public int GetNextZone(int fromZone, ZoneTypeSO zoneType)
        {
            for (int zone = fromZone; zone < fromZone + MAX_ZONE_SEARCH; zone++)
            {
                if (GetZoneType(zone) == zoneType)
                {
                    return zone;
                }
            }

            throw new InvalidOperationException($"{zoneType.name} never appears after zone {fromZone}.");
        }

        // Zone 1 pays the base amount; each later zone pays a little more.
        public float GetRewardMultiplier(int zone)
        {
            return 1f + (zone - 1) * _rewardGrowthPerZone;
        }

        public int ScaleAmount(int baseAmount, int zone)
        {
            return Math.Max(1, (int)Math.Round(baseAmount * GetRewardMultiplier(zone)));
        }

        #endregion
    }
}
