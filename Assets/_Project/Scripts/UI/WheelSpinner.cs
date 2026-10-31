using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Vertigo.Core;
using Vertigo.Gameplay;

namespace Vertigo.UI
{
    // Only rotates the wheel to the slice it is given. Which slice wins is decided by SpinResolver.
    public class WheelSpinner : MonoBehaviour, IWheelSpinner
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

        private IEventBus _eventBus;
        private Tween _spinTween;

        #endregion

        #region Unity Methods

        private void OnDestroy()
        {
            _spinTween?.Kill();
        }

        #endregion

        #region Public Methods

        public void Construct(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void SpinTo(int sliceIndex, int sliceCount)
        {
            if (sliceCount <= 0 || sliceIndex < 0 || sliceIndex >= sliceCount)
            {
                throw new ArgumentOutOfRangeException(nameof(sliceIndex), sliceIndex, $"No slice {sliceIndex} on a wheel with {sliceCount} slices.");
            }

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
            _eventBus.Publish(new SpinFinishedEvent());
        }

        #endregion
    }
}
