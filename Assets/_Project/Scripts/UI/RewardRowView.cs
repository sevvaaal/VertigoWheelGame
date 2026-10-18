using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// One row of the collected rewards list: the reward's icon and how many have been collected.
public class RewardRowView : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private float countDuration = 0.4f;

    private int displayedAmount;

    public void Setup(Sprite icon)
    {
        iconImage.sprite = icon;
    }

    // Rolls the number up from what is shown now to the new total, and gives the row a small pop.
    public void SetAmount(int amount)
    {
        DOTween.Kill(this);
        DOVirtual.Int(displayedAmount, amount, countDuration, ShowAmount).SetTarget(this);

        iconImage.rectTransform.DOKill(true);
        iconImage.rectTransform.DOPunchScale(Vector3.one * 0.3f, countDuration, 6);
    }

    private void ShowAmount(int amount)
    {
        displayedAmount = amount;

        // "N0" adds thousands separators, like 1,634 in the original game.
        amountText.text = amount.ToString("N0");
    }
}
