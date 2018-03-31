using System;

namespace Rocket.API.Eventing
{
    public abstract class Event
    {
        /// <summary>
        /// Name of the event
        /// </summary>
        public virtual string Name { get; }

        /// <summary>
        /// True if the event should be fired async
        /// Notice: Some APIs do not work correctly async 
        /// </summary>
        public virtual bool IsAsync { get; }

        /// <param name="name">The name of the event. Will be auto set if null</param>
        protected Event(string name = null) : this(false, name) { }

        /// <param name="isAsync">Should the event be called from an async thread? See <see cref="IsAsync"/></param>
        /// <param name="name">The name of the event. Will be auto set if null</param>
        protected Event(bool isAsync, string name = null)
        {
            Name = name ?? GetType().Name;
            IsAsync = isAsync;
        }
    }
}