using NuGet.Common;
using NuGet.Configuration;
using NuGet.Packaging;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using Rocket.NuGet;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.ProjectManagement;

namespace Rocket
{
    public class RocketDynamicBootstrapper
    {
        private const string DefaultNugetRepository = "https://api.nuget.org/v3/index.json";

        public async Task BootstrapAsync(string rocketFolder, string repository)
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

            string packageId = "Rocket.Core";
            var nugetInstaller = new NuGetInstaller(logger, packagesDirectory);

            if (nugetInstaller.PackageExists(packagesDirectory, packageId))
            {
                await InitializeRuntimeAsync();
                return;
            }

            var rocketPackage = await nugetInstaller.QueryPackageExactAsync(repository, packageId);

            var installResult = await nugetInstaller.InstallAsync(repository, rocketPackage.Metadata.Identity, NuGetActionType.Install, false);
            var packageIdentity = installResult.Identity;

            var resolver = new PackageResolver();
            var context = new PackageResolverContext(
                DependencyBehavior.Lowest,
                new[] { packageIdentity.Id },
                Enumerable.Empty<string>(),
                Enumerable.Empty<PackageReference>(),
                new[] { packageIdentity },
                Enumerable.Empty<SourcePackageDependencyInfo>(),
                Enumerable.Empty<PackageSource>(),
                logger
            );

            var results = resolver.Resolve(context, CancellationToken.None);
            foreach (var result in results)
            {
                await LoadPackageAsync(nugetInstaller, packagesDirectory, result.Id);
            }

            await LoadPackageAsync(nugetInstaller, packagesDirectory, packageId);
            await InitializeRuntimeAsync();
        }

        private async Task LoadPackageAsync(NuGetInstaller nugetInstaller, string packagesDirectory, string packageId)
        {
            string nupkgFile = nugetInstaller.GetNugetPackageFile(packagesDirectory, packageId);
            await nugetInstaller.LoadAssembliesFromNugetPackageAsync(nupkgFile);
        }

        public async Task BootstrapAsync(string rocketFolder, bool useLocalRocketAssemblies)
        {
            await BootstrapAsync(rocketFolder, useLocalRocketAssemblies ? null : DefaultNugetRepository);
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
            Console.WriteLine($"[{message.Level}] [NuGet] {message.Message}");
        }

        public override async Task LogAsync(ILogMessage message)
        {
            Log(message);
        }
    }
}