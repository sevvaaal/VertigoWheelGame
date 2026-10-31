using System.Collections;
using DG.Tweening;
using UnityEngine;
using Vertigo.Data;
using Vertigo.Gameplay;

namespace Vertigo.UI
{
    // Plays what a spin result looks like, so the GameController only decides what happens and waits for it.
    public class SpinResultPresenter : MonoBehaviour, ISpinResultPresenter
    {
        #region Serialized Fields

        [SerializeField] private WheelView _wheelView;
        [SerializeField] private RewardListView _rewardList;
        [SerializeField] private RewardFlyEffect _rewardFly;

        #endregion

        #region Public Methods

        // The winning slice flashes, then its icon flies into its row in the rewards panel.
        public IEnumerator PlayRewardCoroutine(int sliceIndex, RewardTypeSO rewardType)
        {
            Vector3 iconStart = _wheelView.GetSlicePosition(sliceIndex);
            yield return _wheelView.PlayWinEffect(sliceIndex).WaitForCompletion();

            Transform flyTarget = _rewardList.GetFlyTarget(rewardType);
            yield return _rewardFly.Fly(rewardType.Icon, iconStart, flyTarget).WaitForCompletion();
        }

        public IEnumerator PlayBombCoroutine()
        {
            yield return _wheelView.PlayBombShake().WaitForCompletion();
        }

        #endregion
    }
}
