using NUnit.Framework;
using Vertigo.Core;

namespace Vertigo.Tests.EditMode
{
    public class EventBusTests
    {
        #region Test Events

        private readonly struct NumberEvent : IGameEvent
        {
            public NumberEvent(int value)
            {
                Value = value;
            }

            public int Value { get; }
        }

        private struct OtherEvent : IGameEvent
        {
        }

        #endregion

        #region Fields

        private EventBus _bus;
        private int _receivedValue;
        private int _receivedCount;

        #endregion

        #region Setup

        [SetUp]
        public void CreateBus()
        {
            _bus = new EventBus();
            _receivedValue = 0;
            _receivedCount = 0;
        }

        #endregion

        #region Tests

        [Test]
        public void Publish_ReachesSubscriberWithData()
        {
            _bus.Subscribe<NumberEvent>(HandleNumber);

            _bus.Publish(new NumberEvent(7));

            Assert.AreEqual(1, _receivedCount);
            Assert.AreEqual(7, _receivedValue);
        }

        [Test]
        public void Publish_AfterUnsubscribe_DoesNotReachHandler()
        {
            _bus.Subscribe<NumberEvent>(HandleNumber);
            _bus.Unsubscribe<NumberEvent>(HandleNumber);

            _bus.Publish(new NumberEvent(7));

            Assert.AreEqual(0, _receivedCount);
        }

        [Test]
        public void Publish_WithoutSubscribers_DoesNotThrow()
        {
            Assert.DoesNotThrow(PublishNumber);
        }

        [Test]
        public void Publish_OtherEventType_DoesNotReachHandler()
        {
            _bus.Subscribe<NumberEvent>(HandleNumber);

            _bus.Publish(new OtherEvent());

            Assert.AreEqual(0, _receivedCount);
        }

        #endregion

        #region Helpers

        private void HandleNumber(NumberEvent gameEvent)
        {
            _receivedCount++;
            _receivedValue = gameEvent.Value;
        }

        private void PublishNumber()
        {
            _bus.Publish(new NumberEvent(1));
        }

        #endregion
    }
}
