using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Vertigo.Core;
using Vertigo.Gameplay;

namespace Vertigo.UI
{
    // Shared behaviour of the game's buttons. Each child only says what it publishes and when it is available.
    [RequireComponent(typeof(Button), typeof(CanvasGroup))]
    public abstract class ButtonView : MonoBehaviour
    {
        #region Serialized Fields

        [FormerlySerializedAs("button")]
        [SerializeField] private Button _button;

        // Fades the button and its label together, so "not available right now" is obvious.
        [SerializeField] private CanvasGroup _canvasGroup;

        [FormerlySerializedAs("unavailableAlpha")]
        [Range(0f, 1f)]
        [SerializeField] private float _unavailableAlpha = 0.4f;

        #endregion

        #region Fields

        private IEventBus _eventBus;

        #endregion

        #region Unity Methods

        private void OnValidate()
        {
            _button = GetComponent<Button>();
            _canvasGroup = GetComponent<CanvasGroup>();
            ReferenceValidator.Require(this, _canvasGroup, nameof(_canvasGroup));
        }

        private void Awake()
        {
            _button.onClick.AddListener(HandleClicked);
        }

        private void OnDestroy()
        {
            if (_button)
            {
                _button.onClick.RemoveListener(HandleClicked);
            }

            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<GameStateChangedEvent>(HandleStateChanged);
            }
        }

        #endregion

        #region Public Methods

        public void Construct(IEventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<GameStateChangedEvent>(HandleStateChanged);
        }

        #endregion

        #region Abstract Methods

        protected abstract void PublishClick(IEventBus eventBus);

        protected abstract bool IsAvailable(GameStateChangedEvent stateChanged);

        #endregion

        #region Private Methods

        private void HandleClicked()
        {
            PublishClick(_eventBus);
        }

        private void HandleStateChanged(GameStateChangedEvent stateChanged)
        {
            bool isAvailable = IsAvailable(stateChanged);
            _button.interactable = isAvailable;
            _canvasGroup.alpha = isAvailable ? 1f : _unavailableAlpha;
        }

        #endregion
    }
}
