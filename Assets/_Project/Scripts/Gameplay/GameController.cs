using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vertigo.Core;
using Vertigo.Data;
using Vertigo.Economy;

namespace Vertigo.Gameplay
{
    // Runs the game flow and nothing else: reacts to requests, asks the state machine what is allowed,
    // and moves the run forward. Every dependency is handed in by the GameBootstrapper.
    public class GameController : MonoBehaviour
    {
        #region Fields

        private IEventBus _eventBus;
        private GameStateMachine _stateMachine;
        private ISpinResolver _spinResolver;
        private IWheelSpinner _wheelSpinner;
        private ISpinResultPresenter _resultPresenter;
        private RunProgress _runProgress;
        private IRunInventory _runInventory;
        private IWallet _wallet;
        private int _winningSliceIndex;
        private Coroutine _resultCoroutine;

        #endregion

        #region Unity Methods

        private void Start()
        {
            PublishZone(false);
            _stateMachine.PublishCurrent();
        }

        private void OnDestroy()
        {
            StopResultCoroutine();

            if (_eventBus == null)
            {
                return;
            }

            _eventBus.Unsubscribe<SpinRequestedEvent>(HandleSpinRequested);
            _eventBus.Unsubscribe<SpinFinishedEvent>(HandleSpinFinished);
            _eventBus.Unsubscribe<LeaveRequestedEvent>(HandleLeaveRequested);
            _eventBus.Unsubscribe<RestartRequestedEvent>(HandleRestartRequested);
        }

        #endregion

        #region Public Methods

        public void Construct(IEventBus eventBus, GameStateMachine stateMachine, ISpinResolver spinResolver,
            IWheelSpinner wheelSpinner, ISpinResultPresenter resultPresenter, RunProgress runProgress,
            IRunInventory runInventory, IWallet wallet)
        {
            _eventBus = eventBus;
            _stateMachine = stateMachine;
            _spinResolver = spinResolver;
            _wheelSpinner = wheelSpinner;
            _resultPresenter = resultPresenter;
            _runProgress = runProgress;
            _runInventory = runInventory;
            _wallet = wallet;

            _eventBus.Subscribe<SpinRequestedEvent>(HandleSpinRequested);
            _eventBus.Subscribe<SpinFinishedEvent>(HandleSpinFinished);
            _eventBus.Subscribe<LeaveRequestedEvent>(HandleLeaveRequested);
            _eventBus.Subscribe<RestartRequestedEvent>(HandleRestartRequested);
        }

        #endregion

        #region Event Handlers

        private void HandleSpinRequested(SpinRequestedEvent spinRequested)
        {
            if (!_stateMachine.TryStartSpin())
            {
                return;
            }

            // Decide first, then let the wheel rotate to the slice that was chosen.
            int sliceCount = _runProgress.CurrentWheel.Slices.Count;
            _winningSliceIndex = _spinResolver.PickSliceIndex(sliceCount);
            _wheelSpinner.SpinTo(_winningSliceIndex, sliceCount);
        }

        private void HandleSpinFinished(SpinFinishedEvent spinFinished)
        {
            _stateMachine.ShowResult();

            SliceData slice = _runProgress.CurrentWheel.Slices[_winningSliceIndex];
            _resultCoroutine = StartCoroutine(slice.IsBomb ? PlayBombSequenceCoroutine() : PlayRewardSequenceCoroutine(slice));
        }

        private void HandleLeaveRequested(LeaveRequestedEvent leaveRequested)
        {
            if (!_stateMachine.CanLeave)
            {
                return;
            }

            // A copy, because the run inventory is cleared on Play Again while the popup and wallet still use it.
            // The rewards go out before the state changes, so the collect popup has its rows ready when it appears.
            Dictionary<RewardTypeSO, int> collected = new Dictionary<RewardTypeSO, int>(_runInventory.Amounts);
            _eventBus.Publish(new RewardsCollectedEvent(collected));
            _wallet.Deposit(collected);
            _stateMachine.TryLeave();
        }

        private void HandleRestartRequested(RestartRequestedEvent restartRequested)
        {
            if (!_stateMachine.TryRestart())
            {
                return;
            }

            StopResultCoroutine();
            _runInventory.Clear();
            _runProgress.Reset();
            PublishZone(false);
        }

        #endregion

        #region Coroutines

        private IEnumerator PlayBombSequenceCoroutine()
        {
            yield return _resultPresenter.PlayBombCoroutine();

            _runInventory.Clear();
            _resultCoroutine = null;
            _stateMachine.Bust();
        }

        // The reward is only added once its icon has landed in the panel.
        private IEnumerator PlayRewardSequenceCoroutine(SliceData slice)
        {
            yield return _resultPresenter.PlayRewardCoroutine(_winningSliceIndex, slice.Reward.Type);

            _runInventory.Add(_runProgress.GetReward(slice));
            _runProgress.Advance();
            PublishZone(true);

            _resultCoroutine = null;
            _stateMachine.FinishResult();
        }

        #endregion

        #region Private Methods

        private void PublishZone(bool animate)
        {
            _eventBus.Publish(new ZoneChangedEvent(
                _runProgress.CurrentWheel,
                _runProgress.GetSliceAmounts(),
                _runProgress.BuildZoneBarModel(),
                animate));
        }

        private void StopResultCoroutine()
        {
            if (_resultCoroutine != null)
            {
                StopCoroutine(_resultCoroutine);
                _resultCoroutine = null;
            }
        }

        #endregion
    }
}
