using System;

namespace Rocket.Core.Util
{
    /// <summary>
    /// A generic <see cref="WeakReference"/> implementation.
    /// </summary>
    public class WeakReference<T>
    {
        private readonly WeakReference weakReference;

        public WeakReference(T target)
        {
            weakReference = new WeakReference(target);
        }

        /// <summary>
        /// <inheritdoc cref="WeakReference.IsAlive"/>
        /// </summary>
        public bool IsAlive
        {
            get { return weakReference.IsAlive; }
        }

        /// <summary>
        /// <inheritdoc cref="WeakReference.Target"/>
        /// </summary>
        public T Target
        {
            get { return (T) weakReference.Target; }
        }
    }
}