using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.Core.Extensions;

namespace Rocket.Core.Commands
{
    /// <inheritdoc/>
    public class CommandParameters : ICommandParameters
    {
        private readonly IDependencyContainer container;

        /// <summary>
        /// The internal stored raw parameter list
        /// </summary>
        protected internal string[] Parameters { get; }

        /// <param name="container">The IoC container of the <see cref="ICommandContext">command context</see></param>
        /// <param name="parameters">The raw parameters</param>
        public CommandParameters(IDependencyContainer container, string[] parameters)
        {
            this.container = container;
            Parameters = parameters;
        }

        /// <inheritdoc/>
        public string this[int index] => Parameters[index];

        /// <inheritdoc/>
        public int Length => Parameters.Length;

        /// <inheritdoc/>
        public T Get<T>(int index)
        {
            return (T) Get(index, typeof(T));
        }

        /// <inheritdoc/>
        public object Get(int index, Type type)
        {
            if(type == null)
                throw new ArgumentNullException(nameof(type));

            TypeConverter converter = TypeConverterExtensions.GetConverter(type);

            if (converter.CanConvertFrom(typeof(string)))
            {
                return converter.ConvertFromWithContext(container, Parameters[index]);
            }

            throw new NotSupportedException($"Converting \"{Parameters[index]}\" to \"{type.FullName}\" is not supported!");
        }

        /// <inheritdoc/>
        public T Get<T>(int index, T defaultValue)
        {
            return (T) Get(index, typeof(T), defaultValue);
        }

        /// <inheritdoc/>
        public object Get(int index, Type type, object defaultValue)
        {
            if (TryGet(index, type, out object val))
                return val;
            return defaultValue;
        }

        /// <inheritdoc/>
        public bool TryGet<T>(int index, out T value)
        {
            bool result = TryGet(index, typeof(T), out var tmp);
            value = (T) tmp;
            return result;
        }

        /// <inheritdoc/>
        public bool TryGet(int index, Type type, out object value)
        {
            value = null;
            try
            {
                value = Get(index, type);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public string[] ToArray()
        {
            return Parameters.ToArray(); // send copy
        }

        /// <inheritdoc/>
        public List<string> ToList()
        {
            return Parameters.ToList(); // send copy
        }

        /// <inheritdoc/>
        public IEnumerator<string> GetEnumerator()
        {
            return ToList().GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}