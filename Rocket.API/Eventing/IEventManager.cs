using System;
using Rocket.API.Plugin;

namespace Rocket.API.Eventing
{
    public interface IEventManager
    {
        void Subscribe(IEventListener listener, IRegisterableObject @object);

        void Unsubscribe(IEventListener listener, IRegisterableObject @object);

        void Subscribe<T>(IRegisterableObject @object, Action<T> callback);

        void Subscribe(IRegisterableObject @object, string eventName, Action<Object> callback);

        void Emit(Event @event);

        void UnsubcribeAllEvents(IRegisterableObject plugin);

        void SubscribeAllEvents(IRegisterableObject plugin);
    }
}