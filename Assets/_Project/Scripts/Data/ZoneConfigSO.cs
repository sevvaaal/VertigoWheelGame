using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Vertigo.Data
{
    // Which zone types exist and in what priority. Editable in the Inspector, no code changes needed.
    [CreateAssetMenu(fileName = "ZoneConfig", menuName = "Vertigo/Zone Config")]
    public class ZoneConfigSO : ScriptableObject
    {
        #region Serialized Fields

        [Tooltip("Checked from top to bottom: the first type whose interval divides the zone wins. " +
                 "Super is above Safe, so zone 30 is a super zone even though it is also divisible by 5.")]
        [SerializeField] private List<ZoneTypeSO> _specialZoneTypes = new List<ZoneTypeSO>();

        [Tooltip("Used for every zone no special type claims.")]
        [SerializeField] private ZoneTypeSO _defaultZoneType;

        [FormerlySerializedAs("rewardGrowthPerZone")]
        [Min(0f)]
        [SerializeField] private float _rewardGrowthPerZone = 0.1f;

        #endregion

        #region Properties

        public IReadOnlyList<ZoneTypeSO> SpecialZoneTypes
        {
            get { return _specialZoneTypes; }
        }

        public ZoneTypeSO DefaultZoneType
        {
            get { return _defaultZoneType; }
        }

        public float RewardGrowthPerZone
        {
            get { return _rewardGrowthPerZone; }
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

        public List<string> GetValidationErrors()
        {
            List<string> errors = new List<string>();

            if (!_defaultZoneType)
            {
                errors.Add("no default zone type.");
            }
            else
            {
                foreach (string error in _defaultZoneType.GetValidationErrors())
                {
                    errors.Add($"{_defaultZoneType.name}: {error}");
                }
            }

            for (int i = 0; i < _specialZoneTypes.Count; i++)
            {
                if (!_specialZoneTypes[i])
                {
                    errors.Add($"special zone type {i} is empty.");
                    continue;
                }

                foreach (string error in _specialZoneTypes[i].GetValidationErrors())
                {
                    errors.Add($"{_specialZoneTypes[i].name}: {error}");
                }
            }

            return errors;
        }

        #endregion
    }
}
