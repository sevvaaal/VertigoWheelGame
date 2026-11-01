using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Vertigo.Core;
using Vertigo.Gameplay;

namespace Vertigo.UI
{
    public class BombPopup : PopupView
    {
        #region Constants

        private const string RESTART_BUTTON_NAME = "ui_button_restart";

        #endregion

        #region Serialized Fields

        [FormerlySerializedAs("restartButton")]
        [SerializeField] private Button _restartButton;

        #endregion

        #region Unity Methods

        protected override void OnValidate()
        {
            base.OnValidate();
            _restartButton = UIChildFinder.Find<Button>(transform, RESTART_BUTTON_NAME);
        }

        protected override void Awake()
        {
            if (_restartButton)
            {
                _restartButton.onClick.AddListener(HandleRestartClicked);
            }

            base.Awake();
        }

        protected override void OnDestroy()
        {
            if (_restartButton)
            {
                _restartButton.onClick.RemoveListener(HandleRestartClicked);
            }

            base.OnDestroy();
        }

        #endregion

        #region Overrides

        protected override bool ShouldShow(GameState state)
        {
            return state == GameState.Busted;
        }

        #endregion

        #region Private Methods

        private void HandleRestartClicked()
        {
            EventBus.Publish(new RestartRequestedEvent());
        }

        #endregion
    }
}
