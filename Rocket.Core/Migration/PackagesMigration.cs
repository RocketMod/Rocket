using System.IO;

namespace Rocket.Core.Migration
{
    public class PackagesMigration : DirectoryMigration
    {
        public override string Name => "Packages";

        protected override string GetSourcePath(string basePath) => Path.Combine(basePath, "Libraries");

        protected override string GetTargetPath(string basePath) => Path.Combine(basePath, "Packages");
    }
}