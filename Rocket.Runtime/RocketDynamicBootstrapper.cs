using NuGet.Common;
using NuGet.Packaging.Core;
using Rocket.NuGet;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Rocket
{
    // NOTE TO DEVELOPER: THIS CLASS SHOULD NOT REFERENCE *ANY* OTHER ROCKET CODE!
    // RocketMod is not loaded at this sage.
    /// <summary>
    ///     This class is responsible for downloading Rocket.Core, Rocket.API and the host assembly. <br/>
    ///     After download, it will boot RocketMod and then initialize the IHost interface.
    /// </summary>
    public class RocketDynamicBootstrapper
    {
        public const string DefaultNugetRepository = "https://api.nuget.org/v3/index.json";

        public async Task BootstrapAsync(
            string rocketFolder,
            string packageId,
            bool allowPrereleaseVersions = false,
            string repository = DefaultNugetRepository,
            ILogger logger = null)
        {
            await BootstrapAsync(rocketFolder, new List<string> { packageId }, allowPrereleaseVersions, repository, logger);
        }

        public void Bootstrap(string rocketFolder,
                              IEnumerable<string> packageIds,
                              bool allowPrereleaseVersions = false,
                              string repository = DefaultNugetRepository,
                              ILogger logger = null)
        {
            AsyncContext.Run(() => BootstrapAsync(rocketFolder, packageIds, allowPrereleaseVersions, repository, logger));
        }

        public async Task BootstrapAsync(
            string rocketFolder,
            IEnumerable<string> packageIds,
            bool allowPrereleaseVersions = false,
            string repository = DefaultNugetRepository,
            ILogger logger = null)
        {
            logger = logger ?? new NuGetConsoleLogger();
            logger.LogInformation("Bootstrap has started.");

            rocketFolder = Path.GetFullPath(rocketFolder);

            if (string.IsNullOrEmpty(repository))
            {
                await InitializeRuntimeAsync();
                return;
            }

            var packagesDirectory = Path.Combine(rocketFolder, "Packages");

            if (!Directory.Exists(packagesDirectory))
            {
                Directory.CreateDirectory(packagesDirectory);
            }

            var nugetInstaller = new NuGetPackageManager(logger, packagesDirectory);

            foreach (var packageId in packageIds)
            {
                PackageIdentity packageIdentity;
                if (!await nugetInstaller.PackageExistsAsync(packageId))
                {
                    logger.LogInformation("Searching for: " + packageId);
                    var rocketPackage = await nugetInstaller.QueryPackageExactAsync(packageId, null, allowPrereleaseVersions);

                    logger.LogInformation($"Downloading {rocketPackage.Identity.Id} v{rocketPackage.Identity.Version} via NuGet, this might take a while...");
                    var installResult = await nugetInstaller.InstallAsync(rocketPackage.Identity, allowPrereleaseVersions);
                    if (installResult.Code != NuGetInstallCode.Success)
                    {
                        logger.LogInformation($"Downloading has failed for {rocketPackage.Identity.Id}: " + installResult.Code);
                        return;
                    }

                    packageIdentity = installResult.Identity;
                    logger.LogInformation($"Finished downloading \"{packageId}\"");
                }
                else
                {
                    packageIdentity = await nugetInstaller.GetLatestPackageIdentityAsync(packageId);
                }

                logger.LogInformation($"Loading {packageId}.");
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
}