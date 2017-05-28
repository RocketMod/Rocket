using System.Collections.Generic;
using System.Reflection;
using Rocket.API.Commands;
using Rocket.API.Player;
using Rocket.API.Providers.Plugins;

namespace Rocket.Plugins.Native
{
    public class RocketAttributeCommand : IRocketCommand
    {
        public IRocketPluginProvider Manager { get; }

        public RocketAttributeCommand(IRocketPluginProvider manager, string name, string help, string syntax, AllowedCaller allowedCaller, List<string> permissions, List<string> aliases, MethodInfo method)
        {
            Manager = manager;
            Name = name;
            Help = help;
            Syntax = syntax;
            AllowedCaller = allowedCaller;
            Permissions = permissions;
            Method = method;
        }

        public List<string> Aliases { get; private set; }
        public AllowedCaller AllowedCaller { get; }
        public string Help { get; }
        public string Name { get; }
        public string Identifier { get; private set; }
        public string Syntax { get; }
        public List<string> Permissions { get; }
        public MethodInfo Method { get; }

        public void Execute(ICommandContext ctx)
        {
            ParameterInfo[] methodParameters = Method.GetParameters();
            IRocketPlugin plugin = Manager.GetPlugin(Method.ReflectedType.Assembly.GetName().Name);
            switch (methodParameters.Length)
            {
                case 0:
                    Method.Invoke(plugin, null);
                    break;

                case 1:
                    if (methodParameters[0].ParameterType == typeof(IRocketPlayer))
                        Method.Invoke(plugin, new object[] { ctx.Caller });
                    else if (methodParameters[0].ParameterType == typeof(string[]))
                        Method.Invoke(plugin, new object[] { ctx.Parameters });
                    else if (typeof(ICommandContext).IsAssignableFrom(methodParameters[0].ParameterType))
                        Method.Invoke(plugin, new object[] { ctx });
                    break;

                case 2:
                    if (methodParameters[0].ParameterType == typeof(IRocketPlayer) && methodParameters[1].ParameterType == typeof(string[]))
                        Method.Invoke(plugin, new object[] { ctx.Caller, ctx.Parameters });
                    else if (methodParameters[0].ParameterType == typeof(string[]) && methodParameters[1].ParameterType == typeof(IRocketPlayer))
                        Method.Invoke(plugin, new object[] { ctx.Parameters, ctx.Caller });
                    break;
            }
        }
    }

}
