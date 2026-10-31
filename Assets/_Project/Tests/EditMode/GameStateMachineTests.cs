using System;
using NUnit.Framework;
using Vertigo.Core;
using Vertigo.Gameplay;

namespace Vertigo.Tests.EditMode
{
    public class GameStateMachineTests
    {
        #region Fields

        private EventBus _eventBus;
        private GameStateMachine _stateMachine;
        private GameStateChangedEvent _lastEvent;
        private int _eventCount;

        #endregion

        #region Setup

        [SetUp]
        public void CreateStateMachine()
        {
            _eventBus = new EventBus();
            _eventBus.Subscribe<GameStateChangedEvent>(HandleStateChanged);
            _stateMachine = new GameStateMachine(_eventBus);
            _eventCount = 0;
        }

        [TearDown]
        public void UnsubscribeFromBus()
        {
            _eventBus.Unsubscribe<GameStateChangedEvent>(HandleStateChanged);
        }

        #endregion

        #region Tests

        [Test]
        public void StartsIdle_CanSpinAndLeave()
        {
            Assert.AreEqual(GameState.Idle, _stateMachine.Current);
            Assert.IsTrue(_stateMachine.CanSpin);
            Assert.IsTrue(_stateMachine.CanLeave);
        }

        [Test]
        public void WhileSpinning_CannotSpinOrLeave()
        {
            _stateMachine.TryStartSpin();

            Assert.IsFalse(_stateMachine.CanSpin);
            Assert.IsFalse(_stateMachine.CanLeave);
            Assert.IsFalse(_stateMachine.TryLeave());
        }

        [Test]
        public void WhileShowingResult_CannotSpinOrLeave()
        {
            _stateMachine.TryStartSpin();
            _stateMachine.ShowResult();

            Assert.IsFalse(_stateMachine.CanSpin);
            Assert.IsFalse(_stateMachine.CanLeave);
        }

        [Test]
        public void RewardFlow_ReturnsToIdle()
        {
            _stateMachine.TryStartSpin();
            _stateMachine.ShowResult();
            _stateMachine.FinishResult();

            Assert.AreEqual(GameState.Idle, _stateMachine.Current);
            Assert.IsTrue(_stateMachine.CanSpin);
            Assert.IsTrue(_stateMachine.CanLeave);
        }

        [Test]
        public void BombFlow_BlocksSpinAndLeaveUntilRestart()
        {
            _stateMachine.TryStartSpin();
            _stateMachine.ShowResult();
            _stateMachine.Bust();

            Assert.AreEqual(GameState.Busted, _stateMachine.Current);
            Assert.IsFalse(_stateMachine.TryStartSpin());
            Assert.IsFalse(_stateMachine.TryLeave());

            Assert.IsTrue(_stateMachine.TryRestart());

            Assert.AreEqual(GameState.Idle, _stateMachine.Current);
            Assert.IsTrue(_stateMachine.CanSpin);
        }

        [Test]
        public void AfterLeave_SpinIsBlockedUntilRestart()
        {
            Assert.IsTrue(_stateMachine.TryLeave());

            Assert.AreEqual(GameState.Collected, _stateMachine.Current);
            Assert.IsFalse(_stateMachine.CanSpin);
            Assert.IsFalse(_stateMachine.TryStartSpin());

            Assert.IsTrue(_stateMachine.TryRestart());

            Assert.AreEqual(GameState.Idle, _stateMachine.Current);
        }

        [Test]
        public void TryRestart_WhileIdle_ReturnsFalse()
        {
            Assert.IsFalse(_stateMachine.TryRestart());
            Assert.AreEqual(GameState.Idle, _stateMachine.Current);
        }

        [Test]
        public void TryStartSpin_WhileSpinning_ReturnsFalse()
        {
            Assert.IsTrue(_stateMachine.TryStartSpin());
            Assert.IsFalse(_stateMachine.TryStartSpin());
        }

        [Test]
        public void Bust_WhileIdle_Throws()
        {
            Assert.Throws<InvalidOperationException>(BustWhileIdle);
        }

        [Test]
        public void StateChange_PublishesPreviousAndCurrent()
        {
            _stateMachine.TryStartSpin();

            Assert.AreEqual(1, _eventCount);
            Assert.AreEqual(GameState.Idle, _lastEvent.Previous);
            Assert.AreEqual(GameState.Spinning, _lastEvent.Current);
            Assert.IsFalse(_lastEvent.CanSpin);
            Assert.IsFalse(_lastEvent.CanLeave);
        }

        #endregion

        #region Helpers

        private void HandleStateChanged(GameStateChangedEvent gameEvent)
        {
            _eventCount++;
            _lastEvent = gameEvent;
        }

        private void BustWhileIdle()
        {
            _stateMachine.Bust();
        }

        #endregion
    }
}
