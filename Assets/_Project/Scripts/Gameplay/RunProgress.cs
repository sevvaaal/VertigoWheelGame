using System;
using System.Collections.Generic;
using Vertigo.Data;

namespace Vertigo.Gameplay
{
    // Where the player is in the run, and everything that follows from it: the wheel, the scaled rewards,
    // and the numbers the views show. Plain C#, so it is tested without a scene.
    public sealed class RunProgress
    {
        #region Constants

        private const int FIRST_ZONE = 1;

        #endregion

        #region Fields

        private readonly IZoneRules _zoneRules;
        private readonly int _visibleZoneCount;

        #endregion

        #region Constructor

        public RunProgress(IZoneRules zoneRules, int visibleZoneCount)
        {
            if (zoneRules == null)
            {
                throw new ArgumentNullException(nameof(zoneRules));
            }

            if (visibleZoneCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(visibleZoneCount), visibleZoneCount, "The zone bar needs at least one cell.");
            }

            _zoneRules = zoneRules;
            _visibleZoneCount = visibleZoneCount;
            CurrentZone = FIRST_ZONE;
        }

        #endregion

        #region Properties

        public int CurrentZone { get; private set; }

        public ZoneTypeSO CurrentZoneType
        {
            get { return _zoneRules.GetZoneType(CurrentZone); }
        }

        public WheelSO CurrentWheel
        {
            get { return CurrentZoneType.GetWheel(CurrentZone); }
        }

        #endregion

        #region Public Methods

        public void Reset()
        {
            CurrentZone = FIRST_ZONE;
        }

        public void Advance()
        {
            CurrentZone++;
        }

        // The slice stores the base amount; what the player actually wins grows with the zone.
        public Reward GetReward(SliceData slice)
        {
            if (slice.IsBomb)
            {
                throw new ArgumentException("A bomb slice has no reward.", nameof(slice));
            }

            return new Reward(slice.Reward.Type, _zoneRules.ScaleAmount(slice.Reward.Amount, CurrentZone));
        }

        // The amount shown on each slice of the current wheel; bombs show nothing, so they get 0.
        public IReadOnlyList<int> GetSliceAmounts()
        {
            IReadOnlyList<SliceData> slices = CurrentWheel.Slices;
            List<int> amounts = new List<int>(slices.Count);

            foreach (SliceData slice in slices)
            {
                amounts.Add(slice.IsBomb ? 0 : _zoneRules.ScaleAmount(slice.Reward.Amount, CurrentZone));
            }

            return amounts;
        }

        // The current zone sits in the middle cell; the cells to its left are past zones.
        public ZoneBarModel BuildZoneBarModel()
        {
            int centerIndex = _visibleZoneCount / 2;
            List<ZoneBarModel.Cell> cells = new List<ZoneBarModel.Cell>(_visibleZoneCount);

            for (int i = 0; i < _visibleZoneCount; i++)
            {
                int zone = CurrentZone + (i - centerIndex);
                ZoneTypeSO zoneType = zone >= FIRST_ZONE ? _zoneRules.GetZoneType(zone) : null;
                cells.Add(new ZoneBarModel.Cell(zone, zoneType, zone < CurrentZone));
            }

            Dictionary<ZoneTypeSO, int> nextZones = new Dictionary<ZoneTypeSO, int>();
            foreach (ZoneTypeSO zoneType in _zoneRules.SpecialZoneTypes)
            {
                nextZones[zoneType] = _zoneRules.GetNextZone(CurrentZone, zoneType);
            }

            return new ZoneBarModel(cells, nextZones);
        }

        #endregion
    }
}
