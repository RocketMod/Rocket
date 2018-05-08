using System.IO;

namespace Rocket.Core.Migration
{
    public class LogsMigration : DirectoryMigration
    {
        protected override string GetSourcePath(string basePath)
        {
            return Path.Combine(basePath, "Logs");
        }

        public override string Name => "Logs";

        protected override string GetTargetPath(string basePath)
        {
            return Path.Combine(basePath, "Logs");
        }
    }
}