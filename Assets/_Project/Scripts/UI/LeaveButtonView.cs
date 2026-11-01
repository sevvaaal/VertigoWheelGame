using Vertigo.Core;
using Vertigo.Gameplay;

namespace Vertigo.UI
{
    public class LeaveButtonView : ButtonView
    {
        #region Overrides

        protected override void PublishClick(IEventBus eventBus)
        {
            eventBus.Publish(new LeaveRequestedEvent());
        }

        protected override bool IsAvailable(GameStateChangedEvent stateChanged)
        {
            return stateChanged.CanLeave;
        }

        #endregion
    }
}
