using System;
using NUnit.Framework;
using Vertigo.Gameplay;

namespace Vertigo.Tests.EditMode
{
    public class SpinResolverTests
    {
        #region Fakes

        // Always returns the same value and remembers the range it was asked for.
        private sealed class FakeRandomSource : IRandomSource
        {
            private readonly int _valueToReturn;

            public FakeRandomSource(int valueToReturn)
            {
                _valueToReturn = valueToReturn;
            }

            public int LastMinInclusive { get; private set; }

            public int LastMaxExclusive { get; private set; }

            public int Range(int minInclusive, int maxExclusive)
            {
                LastMinInclusive = minInclusive;
                LastMaxExclusive = maxExclusive;
                return _valueToReturn;
            }
        }

        #endregion

        #region Constants

        private const int SLICE_COUNT = 8;

        #endregion

        #region Fields

        private int _sliceCountUnderTest;

        #endregion

        #region Tests

        [Test]
        public void PickSliceIndex_ReturnsIndexChosenByRandomSource()
        {
            SpinResolver resolver = new SpinResolver(new FakeRandomSource(3));

            Assert.AreEqual(3, resolver.PickSliceIndex(SLICE_COUNT));
        }

        [Test]
        public void PickSliceIndex_AsksForEverySlice()
        {
            FakeRandomSource random = new FakeRandomSource(0);
            SpinResolver resolver = new SpinResolver(random);

            resolver.PickSliceIndex(SLICE_COUNT);

            // 0 to 8 (exclusive) means slices 0..7: the last slice must be able to win too.
            Assert.AreEqual(0, random.LastMinInclusive);
            Assert.AreEqual(SLICE_COUNT, random.LastMaxExclusive);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void PickSliceIndex_WithoutSlices_Throws(int sliceCount)
        {
            _sliceCountUnderTest = sliceCount;

            Assert.Throws<ArgumentOutOfRangeException>(PickWithSliceCountUnderTest);
        }

        #endregion

        #region Helpers

        private void PickWithSliceCountUnderTest()
        {
            new SpinResolver(new FakeRandomSource(0)).PickSliceIndex(_sliceCountUnderTest);
        }

        #endregion
    }
}
