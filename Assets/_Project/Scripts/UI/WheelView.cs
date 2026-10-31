using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Vertigo.Data;
using Vertigo.Gameplay;

// Shows a wheel: its bronze, silver or golden art, its slices with the amounts for the current zone, and its effects.
public class WheelView : MonoBehaviour
{
    [SerializeField] private Image baseImage;
    [SerializeField] private Image indicatorImage;
    [SerializeField] private RectTransform sliceContainer;
    [SerializeField] private SliceView slicePrefab;
    [SerializeField] private float sliceRadius = 145f;

    // Effects live on their own child objects, never on the wheel's root (Vertigo's animator rule).
    [SerializeField] private RectTransform glow;
    [SerializeField] private Image flash;
    [SerializeField] private float glowTurnDuration = 20f;
    [SerializeField] private float effectDuration = 0.4f;

    private readonly List<SliceView> sliceViews = new List<SliceView>();

    private void Start()
    {
        // A soft light behind the wheel that turns slowly and breathes, forever (-1 loops).
        glow.DORotate(new Vector3(0f, 0f, 360f), glowTurnDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1);
        glow.DOScale(1.08f, 1.2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);

        flash.color = new Color(1f, 1f, 1f, 0f);
    }

    // Where the winning slice is on screen, so the flying reward icon can start from it.
    public Vector3 GetSlicePosition(int sliceIndex)
    {
        return sliceViews[sliceIndex].transform.position;
    }

    // A flash bursts at the pointer and the winning slice punches. Returns the tween so the caller can wait for it.
    public Tween PlayWinEffect(int sliceIndex)
    {
        flash.rectTransform.localScale = Vector3.one * 0.5f;
        flash.color = Color.white;

        Sequence effect = DOTween.Sequence();
        effect.Append(flash.rectTransform.DOScale(1.6f, effectDuration).SetEase(Ease.OutQuad));
        effect.Join(flash.DOFade(0f, effectDuration));
        effect.Join(sliceViews[sliceIndex].transform.DOPunchScale(Vector3.one * 0.4f, effectDuration, 6));
        return effect;
    }

    // The wheel shakes when it lands on the bomb. Returns the tween so the caller can wait for it.
    public Tween PlayBombShake()
    {
        return sliceContainer.DOShakeAnchorPos(0.5f, 25f, 30);
    }

    public void Show(WheelSO wheel, ZoneRules zoneRules, int zone)
    {
        baseImage.sprite = wheel.BaseSprite;
        indicatorImage.sprite = wheel.IndicatorSprite;

        ClearSlices();
        BuildSlices(wheel, zoneRules, zone);
    }

    private void BuildSlices(WheelSO wheel, ZoneRules zoneRules, int zone)
    {
        int sliceCount = wheel.Slices.Count;
        float anglePerSlice = 360f / sliceCount;

        for (int i = 0; i < sliceCount; i++)
        {
            float angle = i * anglePerSlice;
            float radians = angle * Mathf.Deg2Rad;

            // Copy the template → place it on the circle → turn it to face outward → fill it with its reward.
            SliceView sliceView = Instantiate(slicePrefab, sliceContainer);
            sliceView.transform.localPosition = new Vector3(Mathf.Sin(radians), Mathf.Cos(radians), 0f) * sliceRadius;
            sliceView.transform.localRotation = Quaternion.Euler(0f, 0f, -angle);

            SliceData slice = wheel.Slices[i];
            int amount = slice.IsBomb ? 0 : zoneRules.ScaleAmount(slice.Reward.Amount, zone);
            sliceView.Setup(slice, wheel.BombIcon, amount);

            sliceViews.Add(sliceView);
        }
    }

    private void ClearSlices()
    {
        foreach (SliceView sliceView in sliceViews)
        {
            Destroy(sliceView.gameObject);
        }

        sliceViews.Clear();
    }
}
