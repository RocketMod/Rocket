using Rocket.API.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Rocket.API.Commands
{
    public class RocketAttributeCommand<T> : IRocketCommand<T> where T : IRocketPlugin
    {
        IRocketPluginManager<T> manager;
        public IRocketPluginManager<T> Manager { get { return manager; } }

        internal RocketAttributeCommand(IRocketPluginManager<T> manager,string Name, string Help, string Syntax, AllowedCaller AllowedCaller, List<string> Permissions, List<string> Aliases, MethodInfo Method)
        {
            this.manager = manager;
            name = Name;
            help = Help;
            syntax = Syntax;
            permissions = Permissions;
            aliases = Aliases;
            method = Method;
            allowedCaller = AllowedCaller;
        }

        private List<string> aliases;
        public List<string> Aliases { get { return aliases; } }

        private AllowedCaller allowedCaller;
        public AllowedCaller AllowedCaller { get { return allowedCaller; } }

        private string help;
        public string Help { get { return help; } }

        private string name;
        public string Name { get { return name; } }

        private string identifier;
        public string Identifier { get { return identifier; } }

        private string syntax;
        public string Syntax { get { return syntax; } }

        private List<string> permissions;
        public List<string> Permissions { get { return permissions; } }

        private MethodInfo method;
        public MethodInfo Method { get { return method; } }

        public void Execute(IRocketPlayer caller, string[] parameters)
        {
            ParameterInfo[] methodParameters = method.GetParameters();
            IRocketPlugin plugin = manager.GetPlugin(method.ReflectedType.Assembly.GetName().Name);
            switch (methodParameters.Length)
            {
                case 0:
                    method.Invoke(plugin, null);
                    break;

                case 1:
                    if (methodParameters[0].ParameterType == typeof(IRocketPlayer))
                        method.Invoke(plugin, new object[] { caller });
                    else if (methodParameters[0].ParameterType == typeof(string[]))
                        method.Invoke(plugin, new object[] { parameters });
                    break;

                case 2:
                    if (methodParameters[0].ParameterType == typeof(IRocketPlayer) && methodParameters[1].ParameterType == typeof(string[]))
                        method.Invoke(plugin, new object[] { caller, parameters });
                    else if (methodParameters[0].ParameterType == typeof(string[]) && methodParameters[1].ParameterType == typeof(IRocketPlayer))
                        method.Invoke(plugin, new object[] { parameters, caller });
                    break;
            }
        }
    }

}
