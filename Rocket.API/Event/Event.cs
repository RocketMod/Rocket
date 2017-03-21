using System;

namespace Rocket.API.Event
{
    public class Event
    {
        public string Name { get; private set; }

        /// <summary>
        /// True if the event should be fired async
        /// Notice: Unity's APIs may not work correctly in async1s
        /// </summary>
        public bool IsAsync { get; private set; }

        protected Event() : this(false) { }

        /// <param name="isAsync">Should the event be called from an async thread? See <see cref="IsAsync"/></param>
        protected Event(bool isAsync)
        {
            Name = GetType().Name;
            IsAsync = isAsync;
        }

        public void Fire()
        {
            if (EventManager.Instance == null) throw new Exception("EventManager instance is null");
            EventManager.Instance.CallEvent(this);
        }
    }
}