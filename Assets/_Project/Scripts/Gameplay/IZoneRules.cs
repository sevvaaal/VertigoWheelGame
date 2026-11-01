using System.Collections.Generic;
using Vertigo.Data;

namespace Vertigo.Gameplay
{
    public interface IZoneRules
    {
        IReadOnlyList<ZoneTypeSO> SpecialZoneTypes { get; }

        ZoneTypeSO GetZoneType(int zone);

        int GetNextZone(int fromZone, ZoneTypeSO zoneType);

        int ScaleAmount(int baseAmount, int zone);
    }
}
