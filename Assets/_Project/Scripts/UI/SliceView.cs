using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliceView : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text amountText;

    public void Setup(SliceData slice, Sprite bombIcon)
    {
        if (slice.IsBomb)
        {
            iconImage.sprite = bombIcon;
            amountText.text = "";
        }
        else
        {
            iconImage.sprite = slice.Reward.Type.Icon;
            amountText.text = $"x{slice.Reward.Amount}";
        }
    }
}