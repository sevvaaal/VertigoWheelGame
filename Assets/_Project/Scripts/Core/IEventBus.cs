using System;

namespace Vertigo.Core
{
    // Events are structs so that publishing one does not allocate garbage.
    public interface IEventBus
    {
        void Subscribe<T>(Action<T> handler) where T : struct, IGameEvent;

        void Unsubscribe<T>(Action<T> handler) where T : struct, IGameEvent;

        void Publish<T>(T gameEvent) where T : struct, IGameEvent;
    }
}
