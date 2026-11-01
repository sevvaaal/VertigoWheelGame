using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Vertigo.Core;
using Vertigo.Data;
using Vertigo.Gameplay;

namespace Vertigo.UI
{
    // Draws the current wheel (art, slices, amounts) and plays its effects. It is told what to show, it does not calculate it.
    public class WheelView : MonoBehaviour
    {
        #region Constants

        private const float FULL_TURN_DEGREES = 360f;

        #endregion

        #region Serialized Fields

        [FormerlySerializedAs("baseImage")]
        [SerializeField] private Image _baseImage;

        [FormerlySerializedAs("indicatorImage")]
        [SerializeField] private Image _indicatorImage;

        [FormerlySerializedAs("sliceContainer")]
        [SerializeField] private RectTransform _sliceContainer;

        [FormerlySerializedAs("slicePrefab")]
        [SerializeField] private SliceView _slicePrefab;

        [FormerlySerializedAs("sliceRadius")]
        [Min(0f)]
        [SerializeField] private float _sliceRadius = 145f;

        [Header("Effects (on their own child objects, never on the root)")]
        [FormerlySerializedAs("glow")]
        [SerializeField] private RectTransform _glow;

        [FormerlySerializedAs("flash")]
        [SerializeField] private Image _flash;

        [FormerlySerializedAs("glowTurnDuration")]
        [Min(0.1f)]
        [SerializeField] private float _glowTurnDuration = 20f;

        [Min(0.1f)]
        [SerializeField] private float _glowBreathDuration = 1.2f;

        [Min(1f)]
        [SerializeField] private float _glowBreathScale = 1.08f;

        [FormerlySerializedAs("effectDuration")]
        [Min(0.05f)]
        [SerializeField] private float _effectDuration = 0.4f;

        [Min(0f)]
        [SerializeField] private float _flashStartScale = 0.5f;

        [Min(0f)]
        [SerializeField] private float _flashEndScale = 1.6f;

        [Min(0f)]
        [SerializeField] private float _slicePunch = 0.4f;

        [Min(0.05f)]
        [SerializeField] private float _bombShakeDuration = 0.5f;

        [Min(0f)]
        [SerializeField] private float _bombShakeStrength = 25f;

        [Min(1)]
        [SerializeField] private int _bombShakeVibrato = 30;

        #endregion

        #region Fields

        // Slice objects are reused between zones; extra ones are hidden instead of destroyed.
        private readonly List<SliceView> _sliceViews = new List<SliceView>();
        private IEventBus _eventBus;
        private Tween _glowTurnTween;
        private Tween _glowBreathTween;
        private Tween _effectTween;

        #endregion

        #region Unity Methods

        private void OnValidate()
        {
            ReferenceValidator.Require(this, _baseImage, nameof(_baseImage));
            ReferenceValidator.Require(this, _indicatorImage, nameof(_indicatorImage));
            ReferenceValidator.Require(this, _sliceContainer, nameof(_sliceContainer));
            ReferenceValidator.Require(this, _slicePrefab, nameof(_slicePrefab));
            ReferenceValidator.Require(this, _glow, nameof(_glow));
            ReferenceValidator.Require(this, _flash, nameof(_flash));
        }

        private void Start()
        {
            _glowTurnTween = _glow.DORotate(new Vector3(0f, 0f, FULL_TURN_DEGREES), _glowTurnDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1);
            _glowBreathTween = _glow.DOScale(_glowBreathScale, _glowBreathDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);

            _flash.color = Color.clear;
        }

        private void OnDestroy()
        {
            _glowTurnTween?.Kill();
            _glowBreathTween?.Kill();
            _effectTween?.Kill();

            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<ZoneChangedEvent>(HandleZoneChanged);
            }
        }

        #endregion

        #region Public Methods

        public void Construct(IEventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<ZoneChangedEvent>(HandleZoneChanged);
        }

        // Where the winning slice is on screen, so the flying reward icon can start from it.
        public Vector3 GetSlicePosition(int sliceIndex)
        {
            return _sliceViews[sliceIndex].transform.position;
        }

        // A flash at the pointer and a punch on the winning slice. Returns the tween so the caller can wait for it.
        public Tween PlayWinEffect(int sliceIndex)
        {
            _effectTween?.Kill(true);

            _flash.rectTransform.localScale = Vector3.one * _flashStartScale;
            _flash.color = Color.white;

            Sequence effect = DOTween.Sequence();
            effect.Append(_flash.rectTransform.DOScale(_flashEndScale, _effectDuration).SetEase(Ease.OutQuad));
            effect.Join(_flash.DOFade(0f, _effectDuration));
            effect.Join(_sliceViews[sliceIndex].transform.DOPunchScale(Vector3.one * _slicePunch, _effectDuration));
            _effectTween = effect;
            return effect;
        }

        public Tween PlayBombShake()
        {
            _effectTween?.Kill(true);
            _effectTween = _sliceContainer.DOShakeAnchorPos(_bombShakeDuration, _bombShakeStrength, _bombShakeVibrato);
            return _effectTween;
        }

        #endregion

        #region Private Methods

        private void HandleZoneChanged(ZoneChangedEvent zoneChanged)
        {
            WheelSO wheel = zoneChanged.Wheel;
            _baseImage.sprite = wheel.BaseSprite;
            _indicatorImage.sprite = wheel.IndicatorSprite;

            IReadOnlyList<SliceData> slices = wheel.Slices;
            float anglePerSlice = FULL_TURN_DEGREES / slices.Count;

            for (int i = 0; i < slices.Count; i++)
            {
                SliceView sliceView = GetOrCreateSliceView(i);
                PlaceOnCircle(sliceView.transform, i * anglePerSlice);
                sliceView.Setup(slices[i], wheel.BombIcon, zoneChanged.SliceAmounts[i]);
            }

            for (int i = slices.Count; i < _sliceViews.Count; i++)
            {
                _sliceViews[i].gameObject.SetActive(false);
            }
        }

        private SliceView GetOrCreateSliceView(int index)
        {
            if (index == _sliceViews.Count)
            {
                _sliceViews.Add(Instantiate(_slicePrefab, _sliceContainer));
            }

            SliceView sliceView = _sliceViews[index];
            sliceView.gameObject.SetActive(true);
            return sliceView;
        }

        // On the circle at this angle (0 = top), turned so the slice faces outward.
        private void PlaceOnCircle(Transform slice, float angle)
        {
            float radians = angle * Mathf.Deg2Rad;
            slice.localPosition = new Vector3(Mathf.Sin(radians), Mathf.Cos(radians), 0f) * _sliceRadius;
            slice.localRotation = Quaternion.Euler(0f, 0f, -angle);
        }

        #endregion
    }
}
