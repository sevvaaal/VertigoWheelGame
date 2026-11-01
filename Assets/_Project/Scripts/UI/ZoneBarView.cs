using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Vertigo.Core;
using Vertigo.Data;
using Vertigo.Gameplay;

namespace Vertigo.UI
{
    // The zone strip at the top: draws the ZoneBarModel it is given and slides one step per zone.
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

        private IEventBus _eventBus;
        private Tween _slideTween;

        #endregion

        #region Properties

        // How many zones the bar shows at once; RunProgress builds exactly this many cells.
        public int CellCount
        {
            get { return _zoneTexts.Count; }
        }

        #endregion

        #region Unity Methods

        private void OnValidate()
        {
            ReferenceValidator.Require(this, _strip, nameof(_strip));
            for (int i = 0; i < _zoneTexts.Count; i++)
            {
                ReferenceValidator.Require(this, _zoneTexts[i], $"{nameof(_zoneTexts)}[{i}]");
            }
        }

        private void OnDestroy()
        {
            _slideTween?.Kill();

            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<ZoneChangedEvent>(HandleZoneChanged);
            }
        }

        #endregion

        #region Public Methods

        public void Construct(IEventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<ZoneChangedEvent>(HandleZoneChanged);
        }

        #endregion

        #region Private Methods

        private void HandleZoneChanged(ZoneChangedEvent zoneChanged)
        {
            ZoneBarModel model = zoneChanged.ZoneBar;

            for (int i = 0; i < _zoneTexts.Count; i++)
            {
                ZoneBarModel.Cell cell = model.Cells[i];
                TMP_Text text = _zoneTexts[i];

                if (cell.IsEmpty)
                {
                    text.text = string.Empty;
                    continue;
                }

                text.text = cell.Zone.ToString();
                Color color = cell.ZoneType.Color;
                if (cell.IsPast)
                {
                    color.a = _pastZoneAlpha;
                }

                text.color = color;
            }

            foreach (UpcomingZoneLabel label in _upcomingZoneLabels)
            {
                if (model.NextZones.TryGetValue(label.ZoneType, out int nextZone))
                {
                    label.Text.text = $"{label.ZoneType.DisplayName} {nextZone}";
                }
            }

            if (zoneChanged.Animate)
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
