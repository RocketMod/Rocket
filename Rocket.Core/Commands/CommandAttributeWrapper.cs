using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;

namespace Rocket.Core.Commands
{
    public class CommandAttributeWrapper : ICommand
    {
        public CommandAttribute Attribute { get; }

        public CommandAttributeWrapper(object instance, MethodBase method, CommandAttribute attribute)
        {
            Instance = instance;
            Method = method;
            Attribute = attribute;
        }

        public string Name => Attribute.Name;
        public string Description => Attribute.Description;
        public string Permission => Attribute.Permission;
        public string Syntax => Attribute.Syntax;
        public List<ISubCommand> ChildCommands { get; } //todo
        public List<string> Aliases => Attribute.Aliases.ToList();
        public object Instance { get; }
        public MethodBase Method { get; set; }

        public bool SupportsCaller(ICommandCaller caller)
        {
            return Attribute.SupportedCommandCallers.Any(c => c.IsInstanceOfType(caller));
        }

        public void Execute(ICommandContext context)
        {
            List<object> @params = new List<object>();

            int index = 0;
            foreach (var param in Method.GetParameters())
            {
                var type = param.ParameterType;
                if (type == typeof(ICommandContext))
                    @params.Add(context);

                else if (type == typeof(ICommandCaller))
                    @params.Add(context);

                else if (type == typeof(string[]))
                    @params.Add(context.Parameters.ToArray());

                else if (type == typeof(ICommandParameters))
                    @params.Add(context.Parameters);

                else if (type == typeof(IDependencyContainer))
                    @params.Add(context.Container);
                else
                {
                    @params.Add(context.Parameters.Get(index, type));
                    index++;
                }
            }

            Method.Invoke(Instance, @params.ToArray());
        }
    }
}