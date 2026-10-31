namespace Vertigo.Gameplay
{
    // Rotates the wheel to a given slice and publishes SpinFinishedEvent when it stops.
    public interface IWheelSpinner
    {
        void SpinTo(int sliceIndex, int sliceCount);
    }
}
