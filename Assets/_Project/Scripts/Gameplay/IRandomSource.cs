namespace Vertigo.Gameplay
{
    // Randomness behind an interface, so tests can replace it with a predictable fake.
    public interface IRandomSource
    {
        int Range(int minInclusive, int maxExclusive);
    }
}
