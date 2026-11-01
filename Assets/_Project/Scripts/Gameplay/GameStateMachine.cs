using System;
using System.Collections.Generic;
using Vertigo.Core;

namespace Vertigo.Gameplay
{
    // The single source of truth for what the player can do right now. Buttons and popups follow its events.
    public sealed class GameStateMachine
    {
        #region Types

        private enum Trigger
        {
            StartSpin,
            ShowResult,
            FinishResult,
            Bust,
            Leave,
            Restart
        }

        #endregion

        #region Constants

        // Every move the game may make. A trigger that is missing here for the current state is not allowed.
        private static readonly Dictionary<(GameState, Trigger), GameState> TRANSITIONS = new Dictionary<(GameState, Trigger), GameState>
        {
            { (GameState.Idle, Trigger.StartSpin), GameState.Spinning },
            { (GameState.Idle, Trigger.Leave), GameState.Collected },
            { (GameState.Spinning, Trigger.ShowResult), GameState.ShowingResult },
            { (GameState.ShowingResult, Trigger.FinishResult), GameState.Idle },
            { (GameState.ShowingResult, Trigger.Bust), GameState.Busted },
            { (GameState.Busted, Trigger.Restart), GameState.Idle },
            { (GameState.Collected, Trigger.Restart), GameState.Idle }
        };

        #endregion

        #region Fields

        private readonly IEventBus _eventBus;

        #endregion

        #region Constructor

        public GameStateMachine(IEventBus eventBus)
        {
            if (eventBus == null)
            {
                throw new ArgumentNullException(nameof(eventBus));
            }

            _eventBus = eventBus;
            Current = GameState.Idle;
        }

        #endregion

        #region Properties

        public GameState Current { get; private set; }

        public bool CanSpin
        {
            get { return CanFire(Trigger.StartSpin); }
        }

        // Leaving is allowed on any zone while nothing is happening (the case's "before spinning" rule).
        public bool CanLeave
        {
            get { return CanFire(Trigger.Leave); }
        }

        public bool CanRestart
        {
            get { return CanFire(Trigger.Restart); }
        }

        #endregion

        #region Public Methods

        // Player input can arrive at any time, so a click in the wrong state is ignored instead of treated as a bug.
        public bool TryStartSpin()
        {
            return TryFire(Trigger.StartSpin);
        }

        public bool TryLeave()
        {
            return TryFire(Trigger.Leave);
        }

        public bool TryRestart()
        {
            return TryFire(Trigger.Restart);
        }

        // Called by our own flow code, so the wrong state here means a bug and throws.
        public void ShowResult()
        {
            Fire(Trigger.ShowResult);
        }

        public void FinishResult()
        {
            Fire(Trigger.FinishResult);
        }

        public void Bust()
        {
            Fire(Trigger.Bust);
        }

        // Re-sends the current state, so views can set themselves up at the start of the game.
        public void PublishCurrent()
        {
            _eventBus.Publish(new GameStateChangedEvent(Current, Current, CanSpin, CanLeave));
        }

        #endregion

        #region Private Methods

        private bool CanFire(Trigger trigger)
        {
            return TRANSITIONS.ContainsKey((Current, trigger));
        }

        private bool TryFire(Trigger trigger)
        {
            if (!CanFire(trigger))
            {
                return false;
            }

            Fire(trigger);
            return true;
        }

        private void Fire(Trigger trigger)
        {
            if (!TRANSITIONS.TryGetValue((Current, trigger), out GameState next))
            {
                throw new InvalidOperationException($"{trigger} is not allowed in state {Current}.");
            }

            GameState previous = Current;
            Current = next;
            _eventBus.Publish(new GameStateChangedEvent(previous, Current, CanSpin, CanLeave));
        }

        #endregion
    }
}
