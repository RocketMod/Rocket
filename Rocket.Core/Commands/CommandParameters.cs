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
    /// <inheritdoc />
    public class CommandParameters : ICommandParameters
    {
        private readonly IDependencyContainer container;

        /// <param name="container">The IoC container of the <see cref="ICommandContext">command context</see></param>
        /// <param name="parameters">The raw parameters</param>
        public CommandParameters(IDependencyContainer container, string[] parameters)
        {
            this.container = container;
            RawParameters = parameters;
        }

        /// <summary>
        ///     The internal stored raw parameter list
        /// </summary>
        protected internal string[] RawParameters { get; }

        /// <inheritdoc />
        public string this[int index] => ToArray()[index];

        /// <inheritdoc />
        public int Length => ToArray().Length;

        /// <inheritdoc />
        public T Get<T>(int index) => (T) Get(index, typeof(T));

        /// <inheritdoc />
        public object Get(int index, Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            TypeConverter converter = TypeConverterExtensions.GetConverter(type);

            if (converter.CanConvertFrom(typeof(string)))
                return converter.ConvertFromWithContext(container, ToArray()[index]);

            throw new NotSupportedException(
                $"Converting \"{ToArray()[index]}\" to \"{type.FullName}\" is not supported!");
        }

        /// <inheritdoc />
        public T Get<T>(int index, T defaultValue) => (T) Get(index, typeof(T), defaultValue);

        /// <inheritdoc />
        public object Get(int index, Type type, object defaultValue)
        {
            if (TryGet(index, type, out object val))
                return val;
            return defaultValue;
        }

        /// <inheritdoc />
        public bool TryGet<T>(int index, out T value)
        {
            bool result = TryGet(index, typeof(T), out object tmp);
            value = (T) tmp;
            return result;
        }

        public string GetArgumentLine(int startPosition)
        {
            if (startPosition > Length)
                throw new IndexOutOfRangeException();

            return string.Join(" ", ToArray().Skip(startPosition).ToArray());
        }

        public string GetArgumentLine(int startPosition, int endPosition)
        {
            if (startPosition > Length)
                throw new IndexOutOfRangeException();

            if (endPosition > Length)
                throw new IndexOutOfRangeException();

            if (endPosition - startPosition < 1)
                throw new ArgumentException();

            return string.Join(" ", ToArray().Skip(startPosition).Take(endPosition - startPosition).ToArray());
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public string[] ToArray() => RawParameters.ToArray();

        /// <inheritdoc />
        public List<string> ToList() => ToArray().ToList();

        /// <inheritdoc />
        public IEnumerator<string> GetEnumerator() => ToList().GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}