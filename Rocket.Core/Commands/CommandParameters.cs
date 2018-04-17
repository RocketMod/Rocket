using System;
using System.ComponentModel;
using System.Linq;
using Rocket.API.Commands;

namespace Rocket.Core.Commands
{
    public class CommandParameters : ICommandParameters
    {
        protected internal string[] Parameters { get; }

        public CommandParameters(string[] parameters)
        {
            Parameters = parameters;
        }

        public string this[int index] => Parameters[index];

        public int Length => Parameters.Length;

        public T Get<T>(int index)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter.CanConvertFrom(typeof(string)))
            {
                return (T) converter.ConvertFrom(Parameters[index]);
            }

            throw new NotSupportedException($"Converting \"{Parameters[index]}\" to \"{typeof(T).FullName}\" is not supported!");
        }

        public T Get<T>(int index, T defaultValue)
        {
            if (TryGet(index, out T val))
                return val;
            return defaultValue;
        }

        public bool TryGet<T>(int index, out T value)
        {
            value = default(T);
            try
            {
                value = Get<T>(index);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string[] ToArray()
        {
            return Parameters.ToArray(); // send copy
        }
    }
}