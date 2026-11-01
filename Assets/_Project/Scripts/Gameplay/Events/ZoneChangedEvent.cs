using System.Collections.Generic;
using Vertigo.Core;
using Vertigo.Data;

namespace Vertigo.Gameplay
{
    // A new zone is on screen: which wheel, what each slice shows, and the zone bar's contents.
    public readonly struct ZoneChangedEvent : IGameEvent
    {
        #region Constructor

        public ZoneChangedEvent(WheelSO wheel, IReadOnlyList<int> sliceAmounts, ZoneBarModel zoneBar, bool animate)
        {
            Wheel = wheel;
            SliceAmounts = sliceAmounts;
            ZoneBar = zoneBar;
            Animate = animate;
        }

        #endregion

        #region Properties

        public WheelSO Wheel { get; }

        public IReadOnlyList<int> SliceAmounts { get; }

        public ZoneBarModel ZoneBar { get; }

        // Slide the zone bar when advancing; jump straight there at the start and on restart.
        public bool Animate { get; }

        #endregion
    }
}
