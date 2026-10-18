using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Shows a wheel: its bronze, silver or golden art and its slices with the amounts for the current zone.
public class WheelView : MonoBehaviour
{
    [SerializeField] private Image baseImage;
    [SerializeField] private Image indicatorImage;
    [SerializeField] private RectTransform sliceContainer;
    [SerializeField] private SliceView slicePrefab;
    [SerializeField] private float sliceRadius = 145f;

    private readonly List<SliceView> sliceViews = new List<SliceView>();

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
