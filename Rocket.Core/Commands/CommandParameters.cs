using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.API.Player;
using Rocket.Core.DependencyInjection;
using Rocket.Core.Player;

namespace Rocket.Core.Commands
{
    public class CommandParameters : ICommandParameters
    {
        private readonly UnityDescriptorContext descriptorContext;

        protected internal string[] Parameters { get; }

        public CommandParameters(IDependencyContainer container, string[] parameters)
        {
            descriptorContext = UnityDescriptorContext.From(container);
            Parameters = parameters;
        }

        public string this[int index] => Parameters[index];

        public int Length => Parameters.Length;

        public T Get<T>(int index)
        {
            return (T) Get(index, typeof(T));
        }

        public object Get(int index, Type type)
        {
            if(type == null)
                throw new ArgumentNullException(nameof(type));

            TypeConverter converter;
            if (typeof(IPlayer).IsAssignableFrom(type))
            {
                converter = new PlayerTypeConverter();
            }
            else if (typeof(IOnlinePlayer).IsAssignableFrom(type))
            {
                converter = new OnlinePlayerTypeConverter();
            }
            else
            {
                converter = TypeDescriptor.GetConverter(type);
            }

            if (converter.CanConvertFrom(typeof(string)))
            {
                return converter.ConvertFrom(descriptorContext, Thread.CurrentThread.CurrentCulture, Parameters[index]);
            }

            throw new NotSupportedException($"Converting \"{Parameters[index]}\" to \"{type.FullName}\" is not supported!");
        }

        public T Get<T>(int index, T defaultValue)
        {
            return (T) Get(index, typeof(T), defaultValue);
        }

        public object Get(int index, Type type, object defaultValue)
        {
            if (TryGet(index, type, out object val))
                return val;
            return defaultValue;
        }

        public bool TryGet<T>(int index, out T value)
        {
            bool result = TryGet(index, typeof(T), out var tmp);
            value = (T) tmp;
            return result;
        }

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

        public string[] ToArray()
        {
            return Parameters.ToArray(); // send copy
        }
    }
}