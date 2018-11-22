using System;
using System.Collections.Generic;
using System.Diagnostics;
using Rocket.API.Drawing;
using System.IO;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.Logging;
using Rocket.Core.Extensions;
using Rocket.Core.Logging;
using Rocket.Core.Migration;
using Rocket.Core.User;
using Rocket.API.User;

namespace Rocket.Core.Commands.RocketCommands
{
    public class CommandLegacyMigration : ICommand
    {
        public string Name => "LegacyMigration";
        public string[] Aliases => null;
        public string Summary => "Migrates from old RocketMod 4";
        public string Description => null;
        public string Syntax => "[step]";
        public IChildCommand[] ChildCommands => null;

        public bool SupportsUser(IUser user) => user is IConsole;

        public async Task ExecuteAsync(ICommandContext context)
        {
            ILogger logger = context.Container.Resolve<ILogger>();
            IRuntime runtime = context.Container.Resolve<IRuntime>();
            IEnumerable<Type> migrationSteps = GetType().Assembly.FindTypes<IMigrationStep>();

            string parentPath = Directory.GetParent(runtime.WorkingDirectory).FullName;
            string basePath = Path.Combine(parentPath, "Rocket.old");
            basePath = Path.GetFullPath(basePath);

            if (!Directory.Exists(basePath))
            {
                await context.User.SendMessageAsync($"Migration failed: Path \"{basePath}\" does not exist.", Color.Red);
                return;
            }

            string targetStep = context.Parameters[0];

            foreach (Type migrationStep in migrationSteps)
            {
                IMigrationStep step = (IMigrationStep) Activator.CreateInstance(migrationStep);

                if (targetStep != null && !step.Name.Equals(targetStep, StringComparison.OrdinalIgnoreCase))
                    continue;

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
                        logger.LogWarning($"Use \"{context.CommandPrefix}{Name} {step.Name}\" to retry.");
                    }
            }

            logger.LogInformation("Legacy migration done.", Color.DarkGreen);
        }
    }
}