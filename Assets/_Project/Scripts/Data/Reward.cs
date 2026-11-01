using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Vertigo.Data
{
    [Serializable]
    public class Reward
    {
        #region Serialized Fields

        [FormerlySerializedAs("type")]
        [SerializeField] private RewardTypeSO _type;

        [FormerlySerializedAs("amount")]
        [Min(1)]
        [SerializeField] private int _amount = 1;

        #endregion

        #region Constructors

        // Unity needs this one to load rewards from wheel assets.
        public Reward()
        {
        }

        public Reward(RewardTypeSO type, int amount)
        {
            _type = type;
            _amount = amount;
        }

        #endregion

        #region Properties

        public RewardTypeSO Type
        {
            get { return _type; }
        }

        public int Amount
        {
            get { return _amount; }
        }

        #endregion
    }
}
