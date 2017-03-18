using System.Collections.Generic;
using System.Reflection;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.Plugins;
using Rocket.API.Providers;

namespace Rocket.Plugins.Native
{
    public class RocketAttributeCommand : IRocketCommand
    {
        public IRocketPluginProvider Manager { get; private set; }

        public RocketAttributeCommand(IRocketPluginProvider manager,string name, string help, string syntax, AllowedCaller allowedCaller, List<string> permissions, List<string> aliases, MethodInfo method)
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
        public AllowedCaller AllowedCaller { get; private set; }
        public string Help { get; private set; }
        public string Name { get; private set; }
        public string Identifier { get; private set; }
        public string Syntax { get; private set; }
        public List<string> Permissions { get; private set; }
        public MethodInfo Method { get; private set; }

        public void Execute(IRocketPlayer caller, string[] parameters)
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
                        Method.Invoke(plugin, new object[] { caller });
                    else if (methodParameters[0].ParameterType == typeof(string[]))
                        Method.Invoke(plugin, new object[] { parameters });
                    break;

                case 2:
                    if (methodParameters[0].ParameterType == typeof(IRocketPlayer) && methodParameters[1].ParameterType == typeof(string[]))
                        Method.Invoke(plugin, new object[] { caller, parameters });
                    else if (methodParameters[0].ParameterType == typeof(string[]) && methodParameters[1].ParameterType == typeof(IRocketPlayer))
                        Method.Invoke(plugin, new object[] { parameters, caller });
                    break;
            }
        }
    }

}
