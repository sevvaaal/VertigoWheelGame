using System.Collections;
using Vertigo.Data;

namespace Vertigo.Gameplay
{
    // Plays the effects of a spin result. The flow waits on these, so they are coroutines.
    public interface ISpinResultPresenter
    {
        IEnumerator PlayRewardCoroutine(int sliceIndex, RewardTypeSO rewardType);

        IEnumerator PlayBombCoroutine();
    }
}
