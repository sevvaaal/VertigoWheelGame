using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Vertigo.UI
{
    // Only rotates the wheel to the slice it is given. Which slice wins is decided by SpinResolver.
    public class WheelSpinner : MonoBehaviour
    {
        #region Constants

        private const float FULL_TURN_DEGREES = 360f;

        #endregion

        #region Serialized Fields

        [FormerlySerializedAs("wheel")]
        [SerializeField] private RectTransform _wheel;

        [FormerlySerializedAs("spinDuration")]
        [Min(0.1f)]
        [SerializeField] private float _spinDuration = 3f;

        [FormerlySerializedAs("fullTurns")]
        [Min(1)]
        [SerializeField] private int _fullTurns = 5;

        [FormerlySerializedAs("spinEase")]
        [SerializeField] private Ease _spinEase = Ease.OutCubic;

        #endregion

        #region Fields

        private Tween _spinTween;

        #endregion

        #region Events

        public event Action SpinStarted;

        public event Action SpinCompleted;

        #endregion

        #region Properties

        public bool IsSpinning { get; private set; }

        #endregion

        #region Unity Methods

        private void OnDestroy()
        {
            _spinTween?.Kill();
        }

        #endregion

        #region Public Methods

        public void SpinTo(int sliceIndex, int sliceCount)
        {
            if (sliceCount <= 0 || sliceIndex < 0 || sliceIndex >= sliceCount)
            {
                throw new ArgumentOutOfRangeException(nameof(sliceIndex), sliceIndex, $"No slice {sliceIndex} on a wheel with {sliceCount} slices.");
            }

            IsSpinning = true;
            SpinStarted?.Invoke();

            // Extra full turns backwards make it look like a real spin; the end angle puts the slice under the pointer.
            float anglePerSlice = FULL_TURN_DEGREES / sliceCount;
            float targetAngle = sliceIndex * anglePerSlice - _fullTurns * FULL_TURN_DEGREES;

            _spinTween = _wheel.DORotate(new Vector3(0f, 0f, targetAngle), _spinDuration, RotateMode.FastBeyond360)
                .SetEase(_spinEase)
                .OnComplete(HandleSpinFinished);
        }

        #endregion

        #region Private Methods

        private void HandleSpinFinished()
        {
            _spinTween = null;
            IsSpinning = false;
            SpinCompleted?.Invoke();
        }

        #endregion
    }
}
