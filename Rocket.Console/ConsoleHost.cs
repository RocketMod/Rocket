using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.Logging;
using Rocket.API.Plugins;
using Rocket.API.Scheduling;
using Rocket.Core.Logging;
using Rocket.Core.Scheduling;
using Rocket.Core.User;
using Color = System.Drawing.Color;

namespace Rocket.Console
{
    public class ConsoleHost : IHost
    {
        public ConsoleHost(IRuntime runtime)
        {
            Console = new StdConsole(runtime.Container);
        }

        private ILogger logger;
        private AsyncTaskRunner syncTaskRunner;
        private AsyncTaskRunner physicsTaskRunner;

        public string Name => "Rocket.Console";

        public string WorkingDirectory { get; set; } = Path.Combine(Environment.CurrentDirectory, "Rocket");

        public async Task InitAsync(IRuntime runtime)
        {
            logger = runtime.Container.Resolve<ILogger>();

            await runtime.Container.Resolve<IPluginLoader>().InitAsync();
            var taskScheduler = runtime.Container.Resolve<ITaskScheduler>();
            syncTaskRunner = new AsyncTaskRunner(taskScheduler, ExecutionTargetSide.SyncFrame);
            syncTaskRunner.Start();

            physicsTaskRunner = new AsyncTaskRunner(taskScheduler, ExecutionTargetSide.SyncFrame);
            physicsTaskRunner.Start();

            ICommandHandler cmdHandler = runtime.Container.Resolve<ICommandHandler>();

            Directory.SetCurrentDirectory(WorkingDirectory);

            logger.LogInformation("Loaded; type \"help\" for help or \"exit\" to exit.");

            System.Console.ForegroundColor = ConsoleColor.Gray;
            System.Console.Write("> ");
            System.Console.ForegroundColor = ConsoleColor.White;

            string line;
            while (!(line = System.Console.ReadLine())?.Equals("exit", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                try
                {
                    if (!await cmdHandler.HandleCommandAsync(Console, line, ""))
                        System.Console.WriteLine("Command not found: " + line);
                    System.Console.Write("> ");
                }
                catch (Exception ex)

                {
                    Console.WriteLine(ex.ToString(), Color.DarkRed);
                }
            }

            Environment.Exit(0);
        }

        public async Task ShutdownAsync()
        {
            syncTaskRunner?.Dispose();
            physicsTaskRunner?.Dispose();

            Environment.Exit(0);
        }

        public Version HostVersion => new Version(FileVersionInfo.GetVersionInfo(GetType().Assembly.Location).FileVersion);
        public Version GameVersion => HostVersion;
        public string ServerName => "Rocket Console";
        public ushort ServerPort => 0;

        public async Task ReloadAsync() { }

        public IConsole Console { get; set; }

        public string GameName => "Rocket.Console";

        public bool IsAlive => true;

        public string ConfigurationName => "ConsoleHost";
    }
}