using System;
using System.Collections.Generic;
using System.IO;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.Logging;
using Rocket.API.Plugins;
using Rocket.Core.Logging;

namespace Rocket.ConsoleImplementation
{
    public class ConsoleImplementation : IImplementation
    {
        public IEnumerable<string> Capabilities => new List<string>();
        public string Name => "ConsoleHost";

        public string WorkingDirectory { get; set; } = Path.Combine(Environment.CurrentDirectory, "Rocket");

        public void Init(IRuntime runtime)
        {
            runtime.Container.Resolve<IPluginManager>().Init();
            ICommandHandler cmdHandler = runtime.Container.Resolve<ICommandHandler>();

            Directory.SetCurrentDirectory(WorkingDirectory);

            ILogger logger = runtime.Container.Resolve<ILogger>();
            Console = new RocketConsole(runtime.Container);

            logger.LogInformation("Loaded; type \"help\" for help or \"exit\" to exit.");

            System.Console.Write(">");

            string line;
            while (!(line = System.Console.ReadLine())?.Equals("exit", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                if (!cmdHandler.HandleCommand(Console, line, ""))
                    System.Console.WriteLine("Command not found: " + line);
                System.Console.Write(">");
            }
        }

        public void Shutdown()
        {
            Environment.Exit(0);
        }

        public string InstanceId => "console";

        public void Reload() { }

        public IConsole Console { get; private set; }

        public bool IsAlive => true;

        public string ConfigurationName => "ConsoleHost";
    }
}