using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Rocket.API.Utils.Debugging;

namespace Rocket.API.Providers.Database
{
    public class DatabaseTable : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();


        /// <summary>
        /// Gets the value of a property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        protected T Get<T>(string name)
        {
            Assert.NotNull(name);
            object value;
            if (_properties.TryGetValue(name, out value))
                return value == null ? default(T) : (T)value;
            return default(T);
        }

        /// <summary>
        /// Sets the value of a property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="name"></param>
        /// <remarks>Use this overload when implicitly naming the property</remarks>
        protected void Set<T>(T value, string name)
        {
            Assert.NotNull(name);
            if (Equals(value, Get<T>(name)))
                return;
            OnPropertyChanging(name);
            _properties[name] = value;
            OnPropertyChanged(name);
        }

        protected virtual void OnPropertyChanging(string propertyName = null)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }


        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}