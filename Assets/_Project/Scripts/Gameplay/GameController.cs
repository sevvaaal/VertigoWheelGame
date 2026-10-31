using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Vertigo.Core;
using Vertigo.Data;
using Vertigo.UI;

namespace Vertigo.Gameplay
{
    // Runs the game flow: reacts to requests, asks the state machine what is allowed, and plays the result sequences.
    public class GameController : MonoBehaviour
    {
        #region Constants

        private const int FIRST_ZONE = 1;

        #endregion

        #region Serialized Fields

        [FormerlySerializedAs("wheelSpinner")]
        [SerializeField] private WheelSpinner _wheelSpinner;

        [FormerlySerializedAs("wheelView")]
        [SerializeField] private WheelView _wheelView;

        [FormerlySerializedAs("rewardList")]
        [SerializeField] private RewardListView _rewardList;

        [FormerlySerializedAs("zoneBar")]
        [SerializeField] private ZoneBarView _zoneBar;

        [FormerlySerializedAs("rewardFly")]
        [SerializeField] private RewardFlyEffect _rewardFly;

        [FormerlySerializedAs("zoneConfig")]
        [SerializeField] private ZoneConfigSO _zoneConfig;

        #endregion

        #region Fields

        private IEventBus _eventBus;
        private GameStateMachine _stateMachine;
        private SpinResolver _spinResolver;
        private Inventory _inventory;
        private ZoneRules _zoneRules;
        private WheelSO _currentWheel;
        private int _currentZone;
        private int _winningSliceIndex;
        private Coroutine _resultCoroutine;

        #endregion

        #region Unity Methods

        // Showing the first zone talks to other objects, so it waits for Start, when every Awake has run.
        private void Start()
        {
            _currentZone = FIRST_ZONE;
            ShowZone(false);
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

        public void Construct(IEventBus eventBus, GameStateMachine stateMachine, SpinResolver spinResolver)
        {
            _eventBus = eventBus;
            _stateMachine = stateMachine;
            _spinResolver = spinResolver;

            _inventory = new Inventory();
            _zoneRules = new ZoneRules(_zoneConfig.SpecialZoneTypes, _zoneConfig.DefaultZoneType, _zoneConfig.RewardGrowthPerZone);
            _rewardList.Bind(_inventory);

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
            int sliceCount = _currentWheel.Slices.Count;
            _winningSliceIndex = _spinResolver.PickSliceIndex(sliceCount);
            _wheelSpinner.SpinTo(_winningSliceIndex, sliceCount);
        }

        private void HandleSpinFinished(SpinFinishedEvent spinFinished)
        {
            _stateMachine.ShowResult();

            SliceData slice = _currentWheel.Slices[_winningSliceIndex];
            _resultCoroutine = StartCoroutine(slice.IsBomb ? PlayBombSequenceCoroutine() : PlayRewardSequenceCoroutine(slice));
        }

        private void HandleLeaveRequested(LeaveRequestedEvent leaveRequested)
        {
            if (!_stateMachine.CanLeave)
            {
                return;
            }

            // The rewards go out first, so the collect popup has its rows ready when the state shows it.
            _eventBus.Publish(new RewardsCollectedEvent(_inventory.Amounts));
            _stateMachine.TryLeave();
        }

        private void HandleRestartRequested(RestartRequestedEvent restartRequested)
        {
            if (!_stateMachine.TryRestart())
            {
                return;
            }

            StopResultCoroutine();
            _inventory.Clear();
            _currentZone = FIRST_ZONE;
            ShowZone(false);
        }

        #endregion

        #region Coroutines

        private IEnumerator PlayBombSequenceCoroutine()
        {
            yield return _wheelView.PlayBombShake().WaitForCompletion();

            _inventory.Clear();
            _resultCoroutine = null;
            _stateMachine.Bust();
        }

        // The reward is only added once its icon has landed in the panel.
        private IEnumerator PlayRewardSequenceCoroutine(SliceData slice)
        {
            Vector3 iconStart = _wheelView.GetSlicePosition(_winningSliceIndex);

            yield return _wheelView.PlayWinEffect(_winningSliceIndex).WaitForCompletion();

            Transform flyTarget = _rewardList.GetFlyTarget(slice.Reward.Type);
            yield return _rewardFly.Fly(slice.Reward.Type.Icon, iconStart, flyTarget).WaitForCompletion();

            int amount = _zoneRules.ScaleAmount(slice.Reward.Amount, _currentZone);
            _inventory.Add(new Reward(slice.Reward.Type, amount));

            _currentZone++;
            ShowZone(true);

            _resultCoroutine = null;
            _stateMachine.FinishResult();
        }

        #endregion

        #region Private Methods

        // animate: slide the zone bar when advancing; jump straight there at the start and on restart.
        private void ShowZone(bool animate)
        {
            ZoneTypeSO zoneType = _zoneRules.GetZoneType(_currentZone);
            _currentWheel = zoneType.GetWheel(_currentZone);

            _wheelView.Show(_currentWheel, _zoneRules, _currentZone);
            _zoneBar.Show(_currentZone, _zoneRules, animate);
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
