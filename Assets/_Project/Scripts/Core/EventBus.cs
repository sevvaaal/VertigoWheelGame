using System;
using System.Collections.Generic;

namespace Vertigo.Core
{
    // A plain object instead of a static class: it is created once and passed in,
    // so dependencies stay visible and every test gets its own clean bus.
    public sealed class EventBus : IEventBus
    {
        #region Fields

        private readonly Dictionary<Type, Delegate> _handlers = new Dictionary<Type, Delegate>();

        #endregion

        #region Public Methods

        public void Subscribe<T>(Action<T> handler) where T : struct, IGameEvent
        {
            Type eventType = typeof(T);
            _handlers.TryGetValue(eventType, out Delegate existing);
            _handlers[eventType] = Delegate.Combine(existing, handler);
        }

        public void Unsubscribe<T>(Action<T> handler) where T : struct, IGameEvent
        {
            Type eventType = typeof(T);
            if (!_handlers.TryGetValue(eventType, out Delegate existing))
            {
                return;
            }

            Delegate remaining = Delegate.Remove(existing, handler);
            if (remaining == null)
            {
                _handlers.Remove(eventType);
            }
            else
            {
                _handlers[eventType] = remaining;
            }
        }

        public void Publish<T>(T gameEvent) where T : struct, IGameEvent
        {
            // Delegates are immutable, so a handler that subscribes or unsubscribes
            // during this call does not change the list being invoked.
            if (_handlers.TryGetValue(typeof(T), out Delegate handlers))
            {
                ((Action<T>)handlers).Invoke(gameEvent);
            }
        }

        #endregion
    }
}
