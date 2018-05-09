using System.IO;
using Rocket.API;
using Rocket.API.DependencyInjection;

namespace Rocket.Core.Migration
{
    public abstract class DirectoryMigration : IMigrationStep
    {
        public abstract string Name { get; }

        public void Migrate(IDependencyContainer container, string basePath)
        {
            IRuntime runtime = container.Resolve<IRuntime>();
            string source = GetSourcePath(basePath);
            string target = GetTargetPath(runtime.WorkingDirectory);

            Copy(source, target);
        }

        protected abstract string GetTargetPath(string basePath);

        protected abstract string GetSourcePath(string basePath);

        private void Copy(string sourceDir, string targetDir)
        {
            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string targetFile = Path.Combine(targetDir, Path.GetFileName(file));
                if (!File.Exists(targetFile))
                    File.Copy(file, targetFile);
            }

            foreach (string directory in Directory.GetDirectories(sourceDir))
                Copy(directory, Path.Combine(targetDir, Path.GetFileName(directory)));
        }
    }
}