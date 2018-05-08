using System.IO;

namespace Rocket.Core.Migration
{
    public class PackagesMigration : DirectoryMigration
    {
        protected override string GetSourcePath(string basePath)
        {
            return Path.Combine(basePath, "Libraries");
        }

        public override string Name => "Packages";

        protected override string GetTargetPath(string basePath)
        {
            return Path.Combine(basePath, "Packages");
        }
    }
}