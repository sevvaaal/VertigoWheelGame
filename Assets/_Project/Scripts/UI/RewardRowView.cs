using TMPro;
using UnityEngine;
using UnityEngine.UI;

// One row of the collected rewards list: the reward's icon and how many have been collected.
public class RewardRowView : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text amountText;

    public void Setup(Sprite icon)
    {
        iconImage.sprite = icon;
    }

    public void SetAmount(int amount)
    {
        // "N0" adds thousands separators, like 1,634 in the original game.
        amountText.text = amount.ToString("N0");
    }
}
