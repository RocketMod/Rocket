using System;
using System.ComponentModel;
using System.Globalization;
using Rocket.API.DependencyInjection;
using Rocket.API.Player;

namespace Rocket.Core.Player
{
    public class PlayerTypeConverter : TypeConverter
    {
        //private readonly IDependencyContainer container;

        //public PlayerTypeConverter(IDependencyContainer container)
        //{
        //    this.container = container;
        //}

        public override bool CanConvertFrom(ITypeDescriptorContext context,
                                            Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
                                           CultureInfo culture, object value)
        {
            throw new NotImplementedException();

            if (value is string)
            {
            //    return container.Get<IPlayerManager>().GetPlayer((string) value);
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}