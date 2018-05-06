using System.IO;
using Rocket.API;
using Rocket.API.DependencyInjection;

namespace Rocket.Core.Migration
{
    public class PackagesMigration : IMigrationStep
    {
        public void Migrate(IDependencyContainer container, string basePath)
        {
            var runtime = container.Resolve<IRuntime>();
            string source = Path.Combine(basePath, "Libraries");
            string target = Path.Combine(runtime.WorkingDirectory, "Packages");

            Copy(source, target);
        }

        private void Copy(string sourceDir, string targetDir)
        {
            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir))
                File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)));

            foreach (var directory in Directory.GetDirectories(sourceDir))
                Copy(directory, Path.Combine(targetDir, Path.GetFileName(directory)));
        }
    }
}