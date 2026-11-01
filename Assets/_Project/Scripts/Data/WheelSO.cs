using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Vertigo.Data
{
    // A wheel's art and contents. Rewards and bomb positions are stored separately and combined into slices at runtime.
    [CreateAssetMenu(fileName = "Wheel", menuName = "Vertigo/Wheel")]
    public class WheelSO : ScriptableObject
    {
        #region Serialized Fields

        [FormerlySerializedAs("baseSprite")]
        [SerializeField] private Sprite _baseSprite;

        [FormerlySerializedAs("indicatorSprite")]
        [SerializeField] private Sprite _indicatorSprite;

        [FormerlySerializedAs("bombIcon")]
        [SerializeField] private Sprite _bombIcon;

        [Tooltip("The reward slices, in order around the wheel. Bomb slots are inserted between them.")]
        [SerializeField] private List<Reward> _rewards = new List<Reward>();

        [Tooltip("Positions on the wheel (0 = under the pointer at rest) that hold a bomb.")]
        [SerializeField] private List<int> _bombSlots = new List<int>();

        #endregion

        #region Fields

        [System.NonSerialized] private List<SliceData> _slices;

        #endregion

        #region Properties

        public Sprite BaseSprite
        {
            get { return _baseSprite; }
        }

        public Sprite IndicatorSprite
        {
            get { return _indicatorSprite; }
        }

        public Sprite BombIcon
        {
            get { return _bombIcon; }
        }

        public int BombCount
        {
            get { return _bombSlots.Count; }
        }

        public IReadOnlyList<SliceData> Slices
        {
            get
            {
                if (_slices == null)
                {
                    _slices = BuildSlices();
                }

                return _slices;
            }
        }

        #endregion

        #region Unity Methods

        private void OnValidate()
        {
            _slices = null;

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
            int sliceCount = _rewards.Count + _bombSlots.Count;

            if (sliceCount == 0)
            {
                errors.Add("the wheel has no slices.");
            }

            HashSet<int> usedSlots = new HashSet<int>();
            foreach (int slot in _bombSlots)
            {
                if (slot < 0 || slot >= sliceCount)
                {
                    errors.Add($"bomb slot {slot} is outside the wheel (0 to {sliceCount - 1}).");
                }
                else if (!usedSlots.Add(slot))
                {
                    errors.Add($"bomb slot {slot} is listed twice.");
                }
            }

            for (int i = 0; i < _rewards.Count; i++)
            {
                if (_rewards[i] == null || !_rewards[i].Type)
                {
                    errors.Add($"reward {i} has no reward type.");
                }
                else if (_rewards[i].Amount < 1)
                {
                    errors.Add($"reward {i} ({_rewards[i].Type.name}) has an amount below 1.");
                }
            }

            return errors;
        }

        #endregion

        #region Private Methods

        private List<SliceData> BuildSlices()
        {
            int sliceCount = _rewards.Count + _bombSlots.Count;
            List<SliceData> slices = new List<SliceData>(sliceCount);
            int rewardIndex = 0;

            for (int slot = 0; slot < sliceCount; slot++)
            {
                if (_bombSlots.Contains(slot))
                {
                    slices.Add(SliceData.CreateBomb());
                }
                else
                {
                    slices.Add(SliceData.CreateReward(_rewards[rewardIndex]));
                    rewardIndex++;
                }
            }

            return slices;
        }

        #endregion
    }
}
