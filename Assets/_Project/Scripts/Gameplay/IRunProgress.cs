using System.Collections.Generic;
using Vertigo.Data;

namespace Vertigo.Gameplay
{
    public interface IRunProgress
    {
        int CurrentZone { get; }

        WheelSO CurrentWheel { get; }

        void Reset();

        void Advance();

        Reward GetReward(SliceData slice);

        IReadOnlyList<int> GetSliceAmounts();

        ZoneBarModel BuildZoneBarModel();
    }
}
