using TMPro;
using UnityEngine;
using UnityEngine.UI;

// One slice on the wheel: the reward's icon and its amount for the current zone, or the bomb.
public class SliceView : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text amountText;

    public void Setup(SliceData slice, Sprite bombIcon, int displayedAmount)
    {
        if (slice.IsBomb)
        {
            iconImage.sprite = bombIcon;
            amountText.text = "";
        }
        else
        {
            iconImage.sprite = slice.Reward.Type.Icon;
            amountText.text = $"x{displayedAmount}";
        }
    }
}
