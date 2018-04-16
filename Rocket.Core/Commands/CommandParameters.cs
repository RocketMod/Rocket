using System;
using System.ComponentModel;
using Rocket.API.Commands;

namespace Rocket.Core.Commands
{
    public class CommandParameters : ICommandParameters
    {
        public string[] Parameters { get; }

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
    }
}