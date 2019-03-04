using NuGet.Common;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using Rocket.NuGet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Rocket
{
    public class RocketDynamicBootstrapper
    {
        private const string DefaultNugetRepository = "https://api.nuget.org/v3/index.json";

        public async Task BootstrapAsync(string rocketFolder, string packageId,
                                         bool allowPrereleaseVersions = false,
                                         string repository = DefaultNugetRepository)
        {
            await BootstrapAsync(rocketFolder, new List<string> {packageId}, allowPrereleaseVersions, repository);
        }

        public async Task BootstrapAsync(string rocketFolder, IEnumerable<string> packageIds, bool allowPrereleaseVersions = false, string repository = DefaultNugetRepository)
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

            var nugetInstaller = new NuGetPackageManager(logger, packagesDirectory, new []{ repository });

            foreach (var packageId in packageIds)
            {
                PackageIdentity packageIdentity;
                if (!await nugetInstaller.PackageExistsAsync(packageId))
                {
                    var rocketPackage =
                        await nugetInstaller.QueryPackageExactAsync(packageId, null, allowPrereleaseVersions);

                    Console.WriteLine(
                        $"Downloading {rocketPackage.Identity.Id} v{rocketPackage.Identity.Version} via NuGet, this might take a while...");
                    var installResult =
                        await nugetInstaller.InstallAsync(rocketPackage.Identity, allowPrereleaseVersions);
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
                    packageIdentity = await nugetInstaller.GetLatestPackageIdentityAsync(packageId);
                }

                Console.WriteLine($"Loading runtime for {packageId}.");

                await LoadPackageAsync(nugetInstaller, packageIdentity);
            }

            await InitializeRuntimeAsync();
        }

        private async Task LoadPackageAsync(NuGetPackageManager packageManager, PackageIdentity identity)
        {
            var pkg = packageManager.GetNugetPackageFile(identity);
            await packageManager.LoadAssembliesFromNugetPackageAsync(pkg);
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

            if (message.Message.Contains("Resolving dependency information took"))
                return;

            Console.WriteLine($"[{message.Level}] [NuGet] {message.Message}");
        }

        public override async Task LogAsync(ILogMessage message)
        {
            Log(message);
        }
    }
}