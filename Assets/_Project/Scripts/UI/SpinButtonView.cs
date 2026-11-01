using Vertigo.Core;
using Vertigo.Gameplay;

namespace Vertigo.UI
{
    public class SpinButtonView : ButtonView
    {
        #region Overrides

        protected override void PublishClick(IEventBus eventBus)
        {
            eventBus.Publish(new SpinRequestedEvent());
        }

        protected override bool IsAvailable(GameStateChangedEvent stateChanged)
        {
            return stateChanged.CanSpin;
        }

        #endregion
    }
}
