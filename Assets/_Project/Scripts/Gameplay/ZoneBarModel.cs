using System.Collections.Generic;
using Vertigo.Data;

namespace Vertigo.Gameplay
{
    // Everything the zone bar draws, already worked out, so the view does no rule lookups of its own.
    public sealed class ZoneBarModel
    {
        #region Types

        public readonly struct Cell
        {
            public Cell(int zone, ZoneTypeSO zoneType, bool isPast)
            {
                Zone = zone;
                ZoneType = zoneType;
                IsPast = isPast;
            }

            // Cells before zone 1 have no zone and stay empty.
            public bool IsEmpty
            {
                get { return Zone < 1; }
            }

            public int Zone { get; }

            public ZoneTypeSO ZoneType { get; }

            public bool IsPast { get; }
        }

        #endregion

        #region Constructor

        public ZoneBarModel(IReadOnlyList<Cell> cells, IReadOnlyDictionary<ZoneTypeSO, int> nextZones)
        {
            Cells = cells;
            NextZones = nextZones;
        }

        #endregion

        #region Properties

        public IReadOnlyList<Cell> Cells { get; }

        // The next zone of every special type, e.g. Super -> 30, Safe -> 10.
        public IReadOnlyDictionary<ZoneTypeSO, int> NextZones { get; }

        #endregion
    }
}
