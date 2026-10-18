using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

// A copy of the won reward's icon flies from the wheel into the collected rewards panel.
public class RewardFlyEffect : MonoBehaviour
{
    [SerializeField] private Image flyingIconPrefab;
    [SerializeField] private float flyDuration = 0.5f;
    [SerializeField] private float endScale = 0.5f;

    // Returns the tween, so the caller can wait for the icon to arrive.
    public Tween Fly(Sprite icon, Vector3 fromWorldPosition, Transform target)
    {
        Image flyingIcon = Instantiate(flyingIconPrefab, transform);
        flyingIcon.sprite = icon;
        flyingIcon.transform.position = fromWorldPosition;

        Sequence flight = DOTween.Sequence();
        flight.Append(flyingIcon.transform.DOMove(target.position, flyDuration).SetEase(Ease.InQuad));
        flight.Join(flyingIcon.transform.DOScale(endScale, flyDuration));
        flight.OnComplete(() => Destroy(flyingIcon.gameObject));
        return flight;
    }
}
