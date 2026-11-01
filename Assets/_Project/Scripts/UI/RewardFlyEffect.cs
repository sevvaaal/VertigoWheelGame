using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Vertigo.Core;

namespace Vertigo.UI
{
    // A copy of the won reward's icon flies from the wheel into the rewards panel. The icon object is reused.
    public class RewardFlyEffect : MonoBehaviour
    {
        #region Serialized Fields

        [FormerlySerializedAs("flyingIconPrefab")]
        [SerializeField] private Image _flyingIconPrefab;

        [FormerlySerializedAs("flyDuration")]
        [Min(0.05f)]
        [SerializeField] private float _flyDuration = 0.5f;

        [FormerlySerializedAs("endScale")]
        [Min(0f)]
        [SerializeField] private float _endScale = 0.5f;

        #endregion

        #region Fields

        private ObjectPool<Image> _iconPool;
        private Image _flyingIcon;
        private Sequence _flight;

        #endregion

        #region Unity Methods

        private void OnValidate()
        {
            ReferenceValidator.Require(this, _flyingIconPrefab, nameof(_flyingIconPrefab));
        }

        private void Awake()
        {
            _iconPool = new ObjectPool<Image>(CreateIcon, HandleGetIcon, HandleReleaseIcon, HandleDestroyIcon);
        }

        private void OnDestroy()
        {
            _flight?.Kill();
            _iconPool?.Dispose();
        }

        #endregion

        #region Public Methods

        // Returns the tween, so the caller can wait for the icon to arrive.
        public Tween Fly(Sprite icon, Vector3 fromWorldPosition, Transform target)
        {
            // Only one result plays at a time; finishing an old flight returns its icon to the pool first.
            _flight?.Complete(true);

            _flyingIcon = _iconPool.Get();
            _flyingIcon.sprite = icon;
            _flyingIcon.transform.position = fromWorldPosition;
            _flyingIcon.transform.localScale = Vector3.one;

            _flight = DOTween.Sequence();
            _flight.Append(_flyingIcon.transform.DOMove(target.position, _flyDuration).SetEase(Ease.InQuad));
            _flight.Join(_flyingIcon.transform.DOScale(_endScale, _flyDuration));
            _flight.OnComplete(HandleFlightComplete);
            return _flight;
        }

        #endregion

        #region Private Methods

        private void HandleFlightComplete()
        {
            _iconPool.Release(_flyingIcon);
            _flyingIcon = null;
            _flight = null;
        }

        private Image CreateIcon()
        {
            return Instantiate(_flyingIconPrefab, transform);
        }

        private void HandleGetIcon(Image icon)
        {
            icon.gameObject.SetActive(true);
        }

        private void HandleReleaseIcon(Image icon)
        {
            icon.gameObject.SetActive(false);
        }

        private void HandleDestroyIcon(Image icon)
        {
            if (icon)
            {
                Destroy(icon.gameObject);
            }
        }

        #endregion
    }
}
