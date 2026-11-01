using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Vertigo.Core;
using Vertigo.Data;

namespace Vertigo.UI
{
    // One slice on the wheel: the reward's icon and the amount it pays in this zone, or the bomb.
    public class SliceView : MonoBehaviour
    {
        #region Serialized Fields

        [FormerlySerializedAs("iconImage")]
        [SerializeField] private Image _iconImage;

        [FormerlySerializedAs("amountText")]
        [SerializeField] private TMP_Text _amountText;

        #endregion

        #region Unity Methods

        private void OnValidate()
        {
            ReferenceValidator.Require(this, _iconImage, nameof(_iconImage));
            ReferenceValidator.Require(this, _amountText, nameof(_amountText));
        }

        #endregion

        #region Public Methods

        public void Setup(SliceData slice, Sprite bombIcon, int amount)
        {
            if (slice.IsBomb)
            {
                _iconImage.sprite = bombIcon;
                _amountText.text = string.Empty;
            }
            else
            {
                _iconImage.sprite = slice.Reward.Type.Icon;
                _amountText.text = NumberFormatter.FormatMultiplier(amount);
            }
        }

        #endregion
    }
}
