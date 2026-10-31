using Vertigo.Core;

namespace Vertigo.Gameplay
{
    // Carries what the player may do in the new state, so views never work out the rules themselves.
    public readonly struct GameStateChangedEvent : IGameEvent
    {
        #region Constructor

        public GameStateChangedEvent(GameState previous, GameState current, bool canSpin, bool canLeave)
        {
            Previous = previous;
            Current = current;
            CanSpin = canSpin;
            CanLeave = canLeave;
        }

        #endregion

        #region Properties

        public GameState Previous { get; }

        public GameState Current { get; }

        public bool CanSpin { get; }

        public bool CanLeave { get; }

        #endregion
    }
}
