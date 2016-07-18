using Rocket.API.Commands;
using Rocket.API.Plugins;
using System;

namespace Rocket.API.Extensions
{
    public static class IRocketCommandExtensions
    {
        public static Type GetCommandType<T>(this IRocketCommand<T> command) where T : IRocketPlugin
        {
            if (command is RocketAttributeCommand<T>)
            {
                return ((RocketAttributeCommand<T>)command).Method.ReflectedType;
            }
            else if (command.GetType().ReflectedType != null)
            {
                return command.GetType().ReflectedType;
            }
            else
            {
                return command.GetType();
            }
        }
    }
}
