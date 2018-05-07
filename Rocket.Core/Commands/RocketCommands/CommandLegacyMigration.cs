using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.Logging;
using Rocket.Core.Extensions;
using Rocket.Core.Logging;
using Rocket.Core.Migration;

namespace Rocket.Core.Commands.RocketCommands
{
    public class CommandLegacyMigration : ICommand
    {
        public string Name => "LegacyMigration";
        public string[] Aliases => null;
        public string Summary => "Migrates from old RocketMod 4";
        public string Description => null;
        public string Permission => "Rocket.Migrate.Legacy";
        public string Syntax => "";
        public ISubCommand[] ChildCommands => null;
        public bool SupportsCaller(Type commandCaller)
        {
            return typeof(IConsoleCommandCaller).IsAssignableFrom(commandCaller);
        }

        public void Execute(ICommandContext context)
        {
            var logger = context.Container.Resolve<ILogger>();
            var runtime = context.Container.Resolve<IRuntime>();
            var migrationSteps = GetType().Assembly.FindTypes<IMigrationStep>();

            var parentPath = Directory.GetParent(runtime.WorkingDirectory).FullName;
            var basePath = Path.Combine(parentPath, "Rocket.old");
            basePath = Path.GetFullPath(basePath);

            if (!Directory.Exists(basePath))
            {
                context.Caller.SendMessage($"Migration failed: Path \"{basePath}\" does not exist.", ConsoleColor.Red);
                return;
            }

            foreach (var migrationStep in migrationSteps)
            {
                IMigrationStep step = (IMigrationStep)Activator.CreateInstance(migrationStep);
                logger.LogInformation($"Executing migration step \"{step.Name}\".");

                if (Debugger.IsAttached)
                    step.Migrate(context.Container, basePath);
                else
                    try
                    {
                        step.Migrate(context.Container, basePath);
                    }
                    catch (Exception e)
                    {
                        logger.LogError($"Failed at migration step \"{step.Name}\": ", e);
                    }
            }

            logger.LogInformation("Legacy migration done.", ConsoleColor.DarkGreen);
        }
    }
}