using System;
using System.ComponentModel;
using System.Threading;
using Rocket.API.DependencyInjection;
using Rocket.API.Player;
using Rocket.Core.DependencyInjection;
using Rocket.Core.Player;

namespace Rocket.Core.Extensions
{
    public static class TypeConverterExtensions
    {
        public static TypeConverter GetConverter(Type type)
        {
            if (typeof(IPlayer).IsAssignableFrom(type))
                return new PlayerTypeConverter();
            return TypeDescriptor.GetConverter(type);
        }

        public static object ConvertFromWithContext(this TypeConverter converter, IDependencyContainer container,
                                                    object value) => converter.ConvertFrom(
            UnityDescriptorContext.From(container), Thread.CurrentThread.CurrentCulture, value);
    }
}