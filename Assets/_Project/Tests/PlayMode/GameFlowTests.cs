#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Vertigo.Core;
using Vertigo.Data;
using Vertigo.Economy;
using Vertigo.Gameplay;

namespace Vertigo.Tests.PlayMode
{
    // The game flow through the real GameController and the real ZoneConfig, wired the way GameBootstrapper wires them.
    // Only the spin pick, the wheel and the effects are faked, so a test chooses the slice and never waits for animations.
    // The config is loaded through the AssetDatabase, so these run in the editor's Play mode only.
    public class GameFlowTests
    {
        #region Fakes

        private sealed class FakeSpinResolver : ISpinResolver
        {
            public int NextSliceIndex { get; set; }

            public int PickSliceIndex(int sliceCount)
            {
                return NextSliceIndex;
            }
        }

        // Stops at once, or never, so a test can send input while the wheel is still turning.
        private sealed class FakeWheelSpinner : IWheelSpinner
        {
            private readonly IEventBus _eventBus;

            public FakeWheelSpinner(IEventBus eventBus)
            {
                _eventBus = eventBus;
            }

            public bool StopsInstantly { get; set; } = true;

            public int LastSliceIndex { get; private set; } = -1;

            public void SpinTo(int sliceIndex, int sliceCount)
            {
                LastSliceIndex = sliceIndex;
                if (StopsInstantly)
                {
                    _eventBus.Publish(new SpinFinishedEvent());
                }
            }
        }

        private sealed class InstantResultPresenter : ISpinResultPresenter
        {
            public IEnumerator PlayRewardCoroutine(int sliceIndex, RewardTypeSO rewardType)
            {
                yield break;
            }

            public IEnumerator PlayBombCoroutine()
            {
                yield break;
            }
        }

        #endregion

        #region Constants

        private const string ZONE_CONFIG_PATH = "Assets/_Project/Data/ZoneConfig.asset";
        private const int VISIBLE_ZONE_COUNT = 9;

        // The fakes finish within a frame or two; the cap only keeps a broken flow from hanging the test run.
        private const int MAX_WAIT_FRAMES = 30;

        #endregion

        #region Fields

        private EventBus _eventBus;
        private GameStateMachine _stateMachine;
        private FakeSpinResolver _spinResolver;
        private FakeWheelSpinner _wheelSpinner;
        private RunProgress _runProgress;
        private RunInventory _runInventory;
        private Wallet _wallet;
        private GameController _gameController;

        #endregion

        #region Setup

        [UnitySetUp]
        public IEnumerator CreateGameCoroutine()
        {
            ZoneConfigSO zoneConfig = AssetDatabase.LoadAssetAtPath<ZoneConfigSO>(ZONE_CONFIG_PATH);
            Assert.IsNotNull(zoneConfig, $"No zone config at {ZONE_CONFIG_PATH}.");

            _eventBus = new EventBus();
            _stateMachine = new GameStateMachine(_eventBus);
            _spinResolver = new FakeSpinResolver();
            _wheelSpinner = new FakeWheelSpinner(_eventBus);
            ZoneRules zoneRules = new ZoneRules(zoneConfig.SpecialZoneTypes, zoneConfig.DefaultZoneType, zoneConfig.RewardGrowthPerZone);
            _runProgress = new RunProgress(zoneRules, VISIBLE_ZONE_COUNT);
            _runInventory = new RunInventory(_eventBus);
            _wallet = new Wallet(_eventBus);

            _gameController = new GameObject("GameController").AddComponent<GameController>();
            _gameController.Construct(_eventBus, _stateMachine, _spinResolver, _wheelSpinner, new InstantResultPresenter(),
                _runProgress, _runInventory, _wallet);

            // Lets the controller's Start run before the first spin, as it does in the scene.
            yield return null;
        }

        [TearDown]
        public void DestroyGame()
        {
            if (_gameController)
            {
                Object.Destroy(_gameController.gameObject);
            }
        }

        #endregion

        #region Tests

        [UnityTest]
        public IEnumerator RewardSpin_AddsTheChosenSlicesReward_AndMovesToNextZone()
        {
            int rewardIndex = FindRewardSlice();
            Reward expected = _runProgress.GetReward(_runProgress.CurrentWheel.Slices[rewardIndex]);

            yield return SpinCoroutine(rewardIndex);

            Assert.AreEqual(GameState.Idle, _stateMachine.Current);
            Assert.AreEqual(rewardIndex, _wheelSpinner.LastSliceIndex);
            Assert.AreEqual(expected.Amount, _runInventory.Amounts[expected.Type]);
            Assert.AreEqual(2, _runProgress.CurrentZone);
        }

        [UnityTest]
        public IEnumerator Bomb_LosesRunRewards_AndBlocksSpinAndLeave()
        {
            yield return SpinCoroutine(FindRewardSlice());
            CollectionAssert.IsNotEmpty(_runInventory.Amounts);

            yield return SpinCoroutine(FindBombSlice());

            Assert.AreEqual(GameState.Busted, _stateMachine.Current);
            CollectionAssert.IsEmpty(_runInventory.Amounts);

            _eventBus.Publish(new SpinRequestedEvent());
            _eventBus.Publish(new LeaveRequestedEvent());

            Assert.AreEqual(GameState.Busted, _stateMachine.Current);
            AssertWalletIsEmpty();
        }

        [UnityTest]
        public IEnumerator Restart_AfterBomb_StartsAgainAtZoneOne()
        {
            yield return SpinCoroutine(FindRewardSlice());
            yield return SpinCoroutine(FindBombSlice());

            _eventBus.Publish(new RestartRequestedEvent());

            Assert.AreEqual(GameState.Idle, _stateMachine.Current);
            Assert.AreEqual(1, _runProgress.CurrentZone);
            CollectionAssert.IsEmpty(_runInventory.Amounts);
        }

        [UnityTest]
        public IEnumerator Leave_OnNormalZone_MovesRunRewardsIntoWallet()
        {
            int rewardIndex = FindRewardSlice();
            Reward won = _runProgress.GetReward(_runProgress.CurrentWheel.Slices[rewardIndex]);
            yield return SpinCoroutine(rewardIndex);

            // Zone 2 is a normal zone: leaving is allowed on any zone while the wheel is still (see README).
            _eventBus.Publish(new LeaveRequestedEvent());

            Assert.AreEqual(GameState.Collected, _stateMachine.Current);
            Assert.AreEqual(won.Amount, _wallet.GetAmount(won.Type));
        }

        [UnityTest]
        public IEnumerator PlayAgain_AfterLeave_KeepsWalletAndEmptiesRun()
        {
            int rewardIndex = FindRewardSlice();
            Reward won = _runProgress.GetReward(_runProgress.CurrentWheel.Slices[rewardIndex]);
            yield return SpinCoroutine(rewardIndex);
            _eventBus.Publish(new LeaveRequestedEvent());

            _eventBus.Publish(new RestartRequestedEvent());

            Assert.AreEqual(GameState.Idle, _stateMachine.Current);
            CollectionAssert.IsEmpty(_runInventory.Amounts);
            Assert.AreEqual(won.Amount, _wallet.GetAmount(won.Type));
        }

        [UnityTest]
        public IEnumerator Leave_WhileSpinning_IsIgnored()
        {
            yield return SpinCoroutine(FindRewardSlice());
            _wheelSpinner.StopsInstantly = false;
            yield return SpinCoroutine(FindRewardSlice());

            _eventBus.Publish(new LeaveRequestedEvent());

            Assert.AreEqual(GameState.Spinning, _stateMachine.Current);
            AssertWalletIsEmpty();
        }

        #endregion

        #region Helpers

        // Requests a spin that lands on sliceIndex and waits until its result has finished playing.
        private IEnumerator SpinCoroutine(int sliceIndex)
        {
            _spinResolver.NextSliceIndex = sliceIndex;
            _eventBus.Publish(new SpinRequestedEvent());

            for (int frame = 0; frame < MAX_WAIT_FRAMES && _stateMachine.Current == GameState.ShowingResult; frame++)
            {
                yield return null;
            }
        }

        private int FindRewardSlice()
        {
            return FindSlice(false);
        }

        private int FindBombSlice()
        {
            return FindSlice(true);
        }

        private int FindSlice(bool isBomb)
        {
            IReadOnlyList<SliceData> slices = _runProgress.CurrentWheel.Slices;
            for (int i = 0; i < slices.Count; i++)
            {
                if (slices[i].IsBomb == isBomb)
                {
                    return i;
                }
            }

            Assert.Fail($"Zone {_runProgress.CurrentZone}'s wheel has no {(isBomb ? "bomb" : "reward")} slice.");
            return -1;
        }

        private void AssertWalletIsEmpty()
        {
            CollectionAssert.IsEmpty(_wallet.Currencies);
            CollectionAssert.IsEmpty(_wallet.Items);
        }

        #endregion
    }
}
#endif
