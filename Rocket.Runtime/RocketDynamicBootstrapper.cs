using NuGet.Common;
using NuGet.Packaging.Core;
using NuGet.ProjectManagement;
using NuGet.Protocol.Core.Types;
using Rocket.NuGet;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Rocket
{
    public class RocketDynamicBootstrapper
    {
        private const string DefaultNugetRepository = "https://api.nuget.org/v3/index.json";

        public async Task BootstrapAsync(string rocketFolder, string packageId, bool allowPrereleaseVersions = false, string repository = DefaultNugetRepository)
        {
            rocketFolder = Path.GetFullPath(rocketFolder);

            if (string.IsNullOrEmpty(repository))
            {
                await InitializeRuntimeAsync();
                return;
            }

            var logger = new NuGetConsoleLogger();
            var packagesDirectory = Path.Combine(rocketFolder, "Packages");

            if (!Directory.Exists(packagesDirectory))
            {
                Directory.CreateDirectory(packagesDirectory);
            }

            var nugetInstaller = new NuGetInstaller(logger, packagesDirectory);
            PackageIdentity packageIdentity;
            if (!nugetInstaller.PackageExists(packagesDirectory, packageId))
            {
                var rocketPackage =
                    await nugetInstaller.QueryPackageExactAsync(repository, packageId, null, allowPrereleaseVersions);

                Console.WriteLine($"Downloading {rocketPackage.Identity.Id} v{rocketPackage.Identity.Version} via NuGet, this can take a while...");
                var installResult = await nugetInstaller.InstallAsync(repository, rocketPackage.Identity, NuGetActionType.Install, allowPrereleaseVersions);
                if (installResult.Code != NuGetInstallCode.Success)
                {
                    Console.WriteLine($"Downloading finished. Loading runtime for {rocketPackage.Identity.Id}.");
                    return;
                }

                packageIdentity = installResult.Identity;
                Console.WriteLine($"Downloading finished.");
            }
            else
            {
                packageIdentity = nugetInstaller.GetLatestPackageIdentity(packageId);
            }

            Console.WriteLine($"Loading runtime for {packageId}.");

            using (var cache = new SourceCacheContext())
            {
                var dependencies = await nugetInstaller.GetDependenciesAsync(packageIdentity, cache);
                foreach (var dependency in dependencies)
                {
                    await LoadPackageAsync(nugetInstaller, dependency);
                }
            }

            await InitializeRuntimeAsync();
        }

        private async Task LoadPackageAsync(NuGetInstaller installer, PackageIdentity identity)
        {
            Console.WriteLine($"Loading: {identity.Id} v{identity.Version}");
            var pkg = installer.GetNugetPackageFile(identity);
            await installer.LoadAssembliesFromNugetPackageAsync(pkg);
        }

        private async Task InitializeRuntimeAsync()
        {
            var runtime = new Runtime();
            await runtime.InitAsync();
        }
    }

    public class NuGetConsoleLogger : LoggerBase
    {
        public override void Log(ILogMessage message)
        {
            if (message.Level < LogLevel.Minimal)
                return;

            Console.WriteLine($"[{message.Level}] [NuGet] {message.Message}");
        }

        public override async Task LogAsync(ILogMessage message)
        {
            Log(message);
        }
    }
}