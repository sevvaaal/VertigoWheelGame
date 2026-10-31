using System.Collections.Generic;
using UnityEngine;
using Vertigo.Core;
using Vertigo.Data;
using Vertigo.Gameplay;
using Vertigo.UI;

namespace Vertigo.Bootstrap
{
    // The one place where shared objects are created and handed to the scene, so every dependency is visible here.
    // Runs before every other script, so everything is connected before anyone's Start.
    [DefaultExecutionOrder(-1000)]
    public class GameBootstrapper : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private ZoneConfigSO _zoneConfig;
        [SerializeField] private GameController _gameController;
        [SerializeField] private WheelSpinner _wheelSpinner;
        [SerializeField] private SpinButtonView _spinButton;
        [SerializeField] private LeaveButtonView _leaveButton;
        [SerializeField] private BombPopup _bombPopup;
        [SerializeField] private CollectPopup _collectPopup;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            // A broken config (wrong bomb count, missing wheel...) must never reach the player: stop before anything starts.
            if (!IsConfigValid())
            {
                _gameController.gameObject.SetActive(false);
                _spinButton.gameObject.SetActive(false);
                _leaveButton.gameObject.SetActive(false);
                return;
            }

            EventBus eventBus = new EventBus();
            GameStateMachine stateMachine = new GameStateMachine(eventBus);
            SpinResolver spinResolver = new SpinResolver(new UnityRandomSource());

            _wheelSpinner.Construct(eventBus);
            _spinButton.Construct(eventBus);
            _leaveButton.Construct(eventBus);
            _bombPopup.Construct(eventBus);
            _collectPopup.Construct(eventBus);
            _gameController.Construct(eventBus, stateMachine, spinResolver);
        }

        #endregion

        #region Private Methods

        private bool IsConfigValid()
        {
            if (!_zoneConfig)
            {
                Debug.LogError($"{name}: no zone config assigned.", this);
                return false;
            }

            List<string> errors = _zoneConfig.GetValidationErrors();
            foreach (string error in errors)
            {
                Debug.LogError($"Game not started, {_zoneConfig.name}: {error}", _zoneConfig);
            }

            return errors.Count == 0;
        }

        #endregion
    }
}
