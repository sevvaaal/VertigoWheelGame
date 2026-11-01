using System;

namespace Vertigo.Gameplay
{
    // Decides which slice wins. Kept out of the wheel view so the rule can be tested without a scene.
    public sealed class SpinResolver : ISpinResolver
    {
        #region Fields

        private readonly IRandomSource _random;

        #endregion

        #region Constructor

        public SpinResolver(IRandomSource random)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            _random = random;
        }

        #endregion

        #region Public Methods

        public int PickSliceIndex(int sliceCount)
        {
            if (sliceCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sliceCount), sliceCount, "A wheel needs at least one slice.");
            }

            return _random.Range(0, sliceCount);
        }

        #endregion
    }
}
