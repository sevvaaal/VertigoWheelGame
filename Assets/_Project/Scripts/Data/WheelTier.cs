using System;
using UnityEngine;

namespace Vertigo.Data
{
    // "From this zone on, use this wheel." A zone type lists several, so rewards get better as the run goes on.
    [Serializable]
    public class WheelTier
    {
        #region Serialized Fields

        [Min(1)]
        [SerializeField] private int _fromZone = 1;

        [SerializeField] private WheelSO _wheel;

        #endregion

        #region Properties

        public int FromZone
        {
            get { return _fromZone; }
        }

        public WheelSO Wheel
        {
            get { return _wheel; }
        }

        #endregion
    }
}
