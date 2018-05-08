using System.IO;

namespace Rocket.Core.Migration
{
    public class LogsMigration : DirectoryMigration
    {
        public override string Name => "Logs";

        protected override string GetSourcePath(string basePath) => Path.Combine(basePath, "Logs");

        protected override string GetTargetPath(string basePath) => Path.Combine(basePath, "Logs");
    }
}