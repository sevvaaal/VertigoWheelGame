using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vertigo.Data
{
    // A kind of zone (normal, safe, super...). Adding a new kind is a new asset in the ZoneConfig list, not a code change.
    [CreateAssetMenu(fileName = "ZoneType", menuName = "Vertigo/Zone Type")]
    public class ZoneTypeSO : ScriptableObject
    {
        #region Serialized Fields

        [Tooltip("Shown before the zone number in the upcoming zone labels, e.g. \"SAFE ZONE\".")]
        [SerializeField] private string _displayName;

        [Tooltip("This type appears on every zone divisible by this number.")]
        [Min(1)]
        [SerializeField] private int _interval = 1;

        [Tooltip("How many bombs every wheel of this zone type must have.")]
        [Min(0)]
        [SerializeField] private int _requiredBombCount;

        [SerializeField] private Color _color = Color.white;

        [Tooltip("Sorted by From Zone. The last tier whose From Zone has been reached is used.")]
        [SerializeField] private List<WheelTier> _wheelTiers = new List<WheelTier>();

        #endregion

        #region Properties

        public string DisplayName
        {
            get { return _displayName; }
        }

        public int Interval
        {
            get { return _interval; }
        }

        public Color Color
        {
            get { return _color; }
        }

        #endregion

        #region Unity Methods

        private void OnValidate()
        {
            foreach (string error in GetValidationErrors())
            {
                Debug.LogError($"{name}: {error}", this);
            }
        }

        #endregion

        #region Public Methods

        public WheelSO GetWheel(int zone)
        {
            WheelSO wheel = null;
            foreach (WheelTier tier in _wheelTiers)
            {
                if (tier.FromZone <= zone)
                {
                    wheel = tier.Wheel;
                }
            }

            if (!wheel)
            {
                throw new InvalidOperationException($"{name} has no wheel for zone {zone}.");
            }

            return wheel;
        }

        public List<string> GetValidationErrors()
        {
            List<string> errors = new List<string>();

            if (_wheelTiers.Count == 0)
            {
                errors.Add("no wheel tiers.");
                return errors;
            }

            if (_wheelTiers[0].FromZone != 1)
            {
                errors.Add("the first wheel tier must start at zone 1.");
            }

            for (int i = 0; i < _wheelTiers.Count; i++)
            {
                WheelTier tier = _wheelTiers[i];

                if (i > 0 && tier.FromZone <= _wheelTiers[i - 1].FromZone)
                {
                    errors.Add($"wheel tier {i} must start after tier {i - 1}.");
                }

                if (!tier.Wheel)
                {
                    errors.Add($"wheel tier {i} has no wheel.");
                    continue;
                }

                if (tier.Wheel.BombCount != _requiredBombCount)
                {
                    errors.Add($"{tier.Wheel.name} has {tier.Wheel.BombCount} bomb(s), but {name} zones need exactly {_requiredBombCount}.");
                }

                foreach (string wheelError in tier.Wheel.GetValidationErrors())
                {
                    errors.Add($"{tier.Wheel.name}: {wheelError}");
                }
            }

            return errors;
        }

        #endregion
    }
}
