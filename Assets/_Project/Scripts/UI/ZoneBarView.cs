using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Vertigo.Data;
using Vertigo.Gameplay;

namespace Vertigo.UI
{
    // The zone strip at the top: the current zone in the middle, colored by its type, sliding one step per zone.
    // The same number cells are reused for every zone instead of creating new ones.
    public class ZoneBarView : MonoBehaviour
    {
        #region Types

        // One "next ... zone" label, e.g. Super -> "SUPER ZONE 30". A new zone type only needs a new entry.
        [Serializable]
        private class UpcomingZoneLabel
        {
            [SerializeField] private ZoneTypeSO _zoneType;
            [SerializeField] private TMP_Text _text;

            public ZoneTypeSO ZoneType
            {
                get { return _zoneType; }
            }

            public TMP_Text Text
            {
                get { return _text; }
            }
        }

        #endregion

        #region Serialized Fields

        [FormerlySerializedAs("strip")]
        [SerializeField] private RectTransform _strip;

        [FormerlySerializedAs("zoneTexts")]
        [SerializeField] private List<TMP_Text> _zoneTexts = new List<TMP_Text>();

        [SerializeField] private List<UpcomingZoneLabel> _upcomingZoneLabels = new List<UpcomingZoneLabel>();

        [FormerlySerializedAs("cellWidth")]
        [Min(0f)]
        [SerializeField] private float _cellWidth = 100f;

        [FormerlySerializedAs("slideDuration")]
        [Min(0f)]
        [SerializeField] private float _slideDuration = 0.3f;

        [FormerlySerializedAs("pastZoneAlpha")]
        [Range(0f, 1f)]
        [SerializeField] private float _pastZoneAlpha = 0.4f;

        #endregion

        #region Fields

        private Tween _slideTween;

        #endregion

        #region Unity Methods

        private void OnDestroy()
        {
            _slideTween?.Kill();
        }

        #endregion

        #region Public Methods

        public void Show(int currentZone, ZoneRules zoneRules, bool animate)
        {
            int centerIndex = _zoneTexts.Count / 2;

            for (int i = 0; i < _zoneTexts.Count; i++)
            {
                int zone = currentZone + (i - centerIndex);
                TMP_Text text = _zoneTexts[i];

                // Cells before zone 1 stay empty.
                text.text = zone >= 1 ? zone.ToString() : string.Empty;

                Color color = zoneRules.GetZoneType(Mathf.Max(1, zone)).Color;
                if (zone < currentZone)
                {
                    color.a = _pastZoneAlpha;
                }

                text.color = color;
            }

            foreach (UpcomingZoneLabel label in _upcomingZoneLabels)
            {
                label.Text.text = $"{label.ZoneType.DisplayName} {zoneRules.GetNextZone(currentZone, label.ZoneType)}";
            }

            if (animate)
            {
                // The numbers have already moved one cell left, so start the strip one cell right and slide it back:
                // on screen the whole strip appears to slide left by one zone.
                _slideTween?.Kill();
                _strip.anchoredPosition = new Vector2(_cellWidth, 0f);
                _slideTween = _strip.DOAnchorPosX(0f, _slideDuration).SetEase(Ease.OutCubic);
            }
        }

        #endregion
    }
}
