using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Vertigo.Core;
using Vertigo.Gameplay;

namespace Vertigo.UI
{
    // Shared behaviour of every popup: it shows and hides itself from the game state and pops in with DOTween.
    public abstract class PopupView : MonoBehaviour
    {
        #region Serialized Fields

        [FormerlySerializedAs("panel")]
        [SerializeField] private RectTransform _panel;

        [FormerlySerializedAs("popDuration")]
        [Min(0f)]
        [SerializeField] private float _popDuration = 0.3f;

        #endregion

        #region Fields

        private Tween _popTween;

        #endregion

        #region Properties

        protected IEventBus EventBus { get; private set; }

        #endregion

        #region Unity Methods

        protected virtual void OnValidate()
        {
            ReferenceValidator.Require(this, _panel, nameof(_panel));
        }

        // Subscriptions are tied to Construct and OnDestroy, not OnEnable/OnDisable:
        // a hidden popup is inactive, but it still has to hear the event that shows it.
        protected virtual void OnDestroy()
        {
            _popTween?.Kill();

            if (EventBus != null)
            {
                Unsubscribe();
            }
        }

        #endregion

        #region Public Methods

        // Set up and hidden here, not in Awake: Awake only runs once the object is active, so a popup saved
        // inactive in the scene would otherwise miss its setup and hide itself again the first time it is shown.
        public void Construct(IEventBus eventBus)
        {
            EventBus = eventBus;
            Initialize();
            Subscribe();
            Hide();
        }

        #endregion

        #region Protected Methods

        protected abstract bool ShouldShow(GameState state);

        // Runs in Construct, before the popup can receive any event.
        protected virtual void Initialize()
        {
        }

        protected virtual void Subscribe()
        {
            EventBus.Subscribe<GameStateChangedEvent>(HandleStateChanged);
        }

        protected virtual void Unsubscribe()
        {
            EventBus.Unsubscribe<GameStateChangedEvent>(HandleStateChanged);
        }

        #endregion

        #region Private Methods

        private void HandleStateChanged(GameStateChangedEvent stateChanged)
        {
            bool shouldShow = ShouldShow(stateChanged.Current);
            if (shouldShow && !gameObject.activeSelf)
            {
                Show();
            }
            else if (!shouldShow && gameObject.activeSelf)
            {
                Hide();
            }
        }

        private void Show()
        {
            gameObject.SetActive(true);

            _popTween?.Kill();
            _panel.localScale = Vector3.zero;
            _popTween = _panel.DOScale(1f, _popDuration).SetEase(Ease.OutBack);
        }

        private void Hide()
        {
            _popTween?.Kill();
            gameObject.SetActive(false);
        }

        #endregion
    }
}
