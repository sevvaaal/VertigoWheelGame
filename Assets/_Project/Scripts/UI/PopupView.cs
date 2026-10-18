using DG.Tweening;
using UnityEngine;

// Shared behavior of every popup: hidden at start, pops in with DOTween, hides again.
// Each popup inherits this and only adds its own content and buttons.
public abstract class PopupView : MonoBehaviour
{
    [SerializeField] private RectTransform panel;
    [SerializeField] private float popDuration = 0.3f;

    protected virtual void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);

        // Grow from nothing to full size with a little overshoot.
        panel.localScale = Vector3.zero;
        panel.DOScale(1f, popDuration).SetEase(Ease.OutBack);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
