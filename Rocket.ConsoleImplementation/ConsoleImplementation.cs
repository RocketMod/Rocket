using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.Configuration;
using Rocket.API.Logging;
using Rocket.API.Plugins;
using Rocket.Core.Configuration;
using Rocket.Core.Logging;

namespace Rocket.ConsoleImplementation
{
    public class ConsoleImplementation : IImplementation
    {
        private ConsoleUser consoleUser;

        public IEnumerable<string> Capabilities => new List<string>();
        public string Name => "ConsoleHost";

        public string WorkingDirectory { get; set; } = Path.Combine(Environment.CurrentDirectory, "Rocket");

        public void Init(IRuntime runtime)
        {
            runtime.Container.Resolve<IPluginManager>().Init();
            ICommandHandler cmdHandler = runtime.Container.Resolve<ICommandHandler>();

            Directory.SetCurrentDirectory(WorkingDirectory);

            ILogger logger = runtime.Container.Resolve<ILogger>();
            consoleUser = new ConsoleUser(logger);

            logger.LogInformation("Loaded; type \"help\" for help or \"exit\" to exit.");

            Console.Write(">");

            string line;
            while (!(line = Console.ReadLine())?.Equals("exit", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                if(!cmdHandler.HandleCommand(ConsoleUser, line, ""))
                    Console.WriteLine("Command not found: " + line);
                Console.Write(">");
            }
        }

        public void Shutdown()
        {
            Environment.Exit(0);
        }

        public string InstanceId => "console";

        public void Reload() { }

        public IConsoleUser ConsoleUser => consoleUser;

        public bool IsAlive => true;

        public string ConfigurationName => "ConsoleHost";
    }
}