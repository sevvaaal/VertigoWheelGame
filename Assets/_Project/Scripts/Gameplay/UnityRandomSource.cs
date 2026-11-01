namespace Vertigo.Gameplay
{
    public sealed class UnityRandomSource : IRandomSource
    {
        #region Public Methods

        public int Range(int minInclusive, int maxExclusive)
        {
            return UnityEngine.Random.Range(minInclusive, maxExclusive);
        }

        #endregion
    }
}
