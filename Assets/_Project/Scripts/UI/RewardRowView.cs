using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Vertigo.Core;

namespace Vertigo.UI
{
    // One row in a rewards list: the reward's icon and its amount, counting up when it changes.
    public class RewardRowView : MonoBehaviour
    {
        #region Serialized Fields

        [FormerlySerializedAs("iconImage")]
        [SerializeField] private Image _iconImage;

        [FormerlySerializedAs("amountText")]
        [SerializeField] private TMP_Text _amountText;

        [FormerlySerializedAs("countDuration")]
        [Min(0f)]
        [SerializeField] private float _countDuration = 0.4f;

        [Min(0f)]
        [SerializeField] private float _iconPunch = 0.3f;

        #endregion

        #region Fields

        private int _displayedAmount;
        private Tween _countTween;
        private Tween _punchTween;

        #endregion

        #region Unity Methods

        private void OnDestroy()
        {
            _countTween?.Kill();
            _punchTween?.Kill();
        }

        #endregion

        #region Public Methods

        public void Setup(Sprite icon)
        {
            _iconImage.sprite = icon;
        }

        // Rolls the number up from what is shown now to the new total, and gives the icon a small pop.
        public void SetAmount(int amount)
        {
            _countTween?.Kill();
            _countTween = DOVirtual.Int(_displayedAmount, amount, _countDuration, ShowAmount);

            _punchTween?.Kill(true);
            _punchTween = _iconImage.rectTransform.DOPunchScale(Vector3.one * _iconPunch, _countDuration);
        }

        #endregion

        #region Private Methods

        private void ShowAmount(int amount)
        {
            _displayedAmount = amount;
            _amountText.text = NumberFormatter.FormatAmount(amount);
        }

        #endregion
    }
}
