using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

// The zone strip at the top: the current zone in the middle, colored by type, sliding one step when the player advances.
// The same number cells are reused for every zone instead of creating new ones.
public class ZoneBarView : MonoBehaviour
{
    [SerializeField] private RectTransform strip;
    [SerializeField] private List<TMP_Text> zoneTexts = new List<TMP_Text>();
    [SerializeField] private TMP_Text nextSuperZoneText;
    [SerializeField] private TMP_Text nextSafeZoneText;
    [SerializeField] private float cellWidth = 100f;
    [SerializeField] private float slideDuration = 0.3f;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color safeColor = new Color32(0x3E, 0xE0, 0x7A, 0xFF);
    [SerializeField] private Color superColor = new Color32(0xFF, 0xC8, 0x3D, 0xFF);
    [SerializeField] private float pastZoneAlpha = 0.4f;

    public void Show(int currentZone, ZoneRules zoneRules, bool animate)
    {
        int centerIndex = zoneTexts.Count / 2;

        for (int i = 0; i < zoneTexts.Count; i++)
        {
            int zone = currentZone + (i - centerIndex);
            TMP_Text text = zoneTexts[i];

            // Cells before zone 1 stay empty.
            text.text = zone >= 1 ? zone.ToString() : "";

            Color color = GetColor(zoneRules.GetZoneType(Mathf.Max(1, zone)));
            if (zone < currentZone)
            {
                color.a = pastZoneAlpha;
            }

            text.color = color;
        }

        nextSuperZoneText.text = $"SUPER ZONE {zoneRules.GetNextZone(currentZone, ZoneType.Super)}";
        nextSafeZoneText.text = $"SAFE ZONE {zoneRules.GetNextZone(currentZone, ZoneType.Safe)}";

        if (animate)
        {
            // The numbers have already moved one cell to the left, so start the strip one cell to the right
            // and slide it back: on screen it looks like the whole strip slides left by one zone.
            strip.DOKill();
            strip.anchoredPosition = new Vector2(cellWidth, 0f);
            strip.DOAnchorPosX(0f, slideDuration).SetEase(Ease.OutCubic);
        }
    }

    private Color GetColor(ZoneType type)
    {
        switch (type)
        {
            case ZoneType.Super: return superColor;
            case ZoneType.Safe: return safeColor;
            default: return normalColor;
        }
    }
}
