using MoreLinq.Extensions;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.ProjectManagement;
using NuGet.Versioning;
using Rocket.API;
using Rocket.API.Configuration;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.Logging;
using Rocket.Core.Configuration;
using Rocket.Core.Logging;
using Rocket.Core.Plugins.Events;
using Rocket.NuGet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Rocket.Core.Plugins.NuGet
{
    public class NuGetPluginLoader : ClrPluginLoader
    {
        private readonly ILogger logger;
        private readonly IRuntime runtime;
        private IConfiguration configuration;
        private IConfiguration packagesConfiguration;
        private NuGetInstaller nugetInstaller;

        public virtual string PackagesDirectory { get; protected set; }

        public NuGetPluginLoader(IDependencyContainer container,
                                  IEventBus eventBus,
                                  ILogger logger,
                                  IRuntime runtime) :
            base(container, eventBus, logger)
        {
            this.logger = logger;
            this.runtime = runtime;
        }

        public virtual IEnumerable<Repository> Repositories => Configuration["Repositories"].Get<Repository[]>();

        protected virtual IConfiguration Configuration
        {
            get
            {
                if (configuration != null)
                    return configuration;

                CreateConfiguration();
                return configuration;
            }
        }

        public override async Task InitAsync()
        {
            Logger.LogDebug($"[{GetType().Name}] Initializing NuGet.");

            CreateConfiguration();

            var adapter = new NuGetLoggerAdapter(logger);
            nugetInstaller = new NuGetInstaller(adapter, PackagesDirectory);

            PluginManagerInitEvent pluginManagerInitEvent =
                new PluginManagerInitEvent(this, EventExecutionTargetContext.Sync);
            EventBus.Emit(runtime, pluginManagerInitEvent);

            if (pluginManagerInitEvent.IsCancelled)
            {
                Logger.LogDebug($"[{GetType().Name}] Loading of NuGet packages was cancelled.");
                return;
            }

            foreach (var dir in Directory.GetDirectories(PackagesDirectory))
            {
                foreach (var file in Directory.GetFiles(dir))
                {
                    if (file.EndsWith(".nupkg", StringComparison.OrdinalIgnoreCase))
                        await LoadPluginFromNugetPackageAsync(file);
                }
            }
        }

        private void CreateConfiguration()
        {
            configuration = Container.Resolve<IConfiguration>();

            PackagesDirectory = Path.Combine(runtime.WorkingDirectory, "Packages");
            if (!Directory.Exists(PackagesDirectory))
                Directory.CreateDirectory(PackagesDirectory);

            ConfigurationContext context = new ConfigurationContext(PackagesDirectory, "repositories");
            configuration.LoadAsync(context, new
            {
                Repositories = new[]
                {
                    new Repository
                    {
                        Name = "rocketmod",
                        Url = "https://hangar.rocketmod.net/index.json",
                        IsEnabled = true
                    },
                    new Repository
                    {
                        Name = "nuget",
                        Url = "https://api.nuget.org/v3/index.json",
                        IsEnabled = true
                    }
                }
            }).GetAwaiter().GetResult();
            configuration.SaveAsync().GetAwaiter().GetResult();
        }

        public async Task<NuGetInstallResult> UpdateAsync(string packageName, string version = null, string repoName = null, bool isPreRelease = false)
        {
            return await InstallOrUpdateAsync(packageName, version, repoName, isPreRelease, true);
        }

        public async Task<NuGetInstallResult> InstallAsync(string packageName, string version = null, string repoName = null, bool isPreRelease = false)
        {
            return await InstallOrUpdateAsync(packageName, version, repoName, isPreRelease);
        }

        protected virtual async Task<NuGetInstallResult> InstallOrUpdateAsync(string packageName, string version = null, string repoName = null, bool isPreRelease = false, bool isUpdate = false)
        {
            bool packageExists = PackageExists(packageName);

            if (isUpdate && !packageExists)
                return new NuGetInstallResult(NuGetInstallCode.PackageNotFound);

            var enabledRepos = Repositories.Where(d => d.IsEnabled).ToList();
            var repoUrls = enabledRepos.Select(c => c.Url).ToList();

            string repo;
            if (repoName == null)
            {
                repo = await nugetInstaller.FindRepositoryForPackageAsync(repoUrls, packageName, version, isPreRelease);
                if (repo == null)
                {
                    return new NuGetInstallResult(NuGetInstallCode.PackageNotFound);
                }
            }
            else
            {
                repo = enabledRepos.FirstOrDefault(c => c.Name.Equals(repoName, StringComparison.OrdinalIgnoreCase))?.Url;
                if (repo == null)
                {
                    return new NuGetInstallResult(NuGetInstallCode.RepositoryNotFound);
                }
            }

            if (isUpdate || packageExists)
                await UninstallAsync(packageName);

            var packages = (await nugetInstaller.QueryPackagesAsync(repoUrls, packageName, version, isPreRelease)).ToList();

            if (packages.Count == 0)
            {
                return new NuGetInstallResult(NuGetInstallCode.PackageNotFound);
            }

            var package = packages.FirstOrDefault(d => packages.Count == 1 || d.Identity.Id.Equals(packageName, StringComparison.OrdinalIgnoreCase));
            if (package == null)
            {
                // we have no exact match but multiple results
                return new NuGetInstallResult(NuGetInstallCode.MultipleMatch);
            }

            if (version == null)
            {
                version = package.Identity.Version.OriginalVersion;
            }

            var packageIdentity = new PackageIdentity(package.Identity.Id, new NuGetVersion(version));

            var result = await nugetInstaller.InstallAsync(repo, packageIdentity, isUpdate ? NuGetActionType.Update : NuGetActionType.Install, isPreRelease);
            if (result.Code != NuGetInstallCode.Success)
            {
                return result;
            }

            return result;
        }

        public virtual async Task<bool> UninstallAsync(string packageName)
        {
            foreach (var directory in Directory.GetDirectories(PackagesDirectory))
            {
                var name = new DirectoryInfo(directory).Name;
                if (name.ToLowerInvariant().StartsWith(packageName.ToLowerInvariant()))
                {
                    Directory.Delete(directory, true);
                    return true;
                }
            }

            return false;
        }

        public virtual async Task<bool> LoadPluginFromNugetAsync(PackageIdentity identity)
        {
            var pkg = nugetInstaller.GetNugetPackageFile(identity);
            if (pkg == null)
                throw new Exception($"Plugin {identity.Id} v{identity.Version} was not found.");

            return await LoadPluginFromNugetPackageAsync(pkg);
        }

        protected virtual async Task<bool> LoadPluginFromNugetPackageAsync(string packagePath)
        {
            var assemblies = await nugetInstaller.LoadAssembliesFromNugetPackageAsync(packagePath);
            bool success = false;
            foreach (var asm in assemblies)
            {
                if (LoadPluginFromAssembly(asm, out var pluginChildContainer) != null)
                {
                    success = await RegisterAndLoadPluginFromContainer(pluginChildContainer);
                }
            }

            return success;
        }

        public virtual bool PackageExists(string packageName)
        {
            return nugetInstaller.PackageExists(PackagesDirectory, packageName);
        }

        public override string ServiceName => "NuGet Plugins";

        protected override async Task<IEnumerable<Assembly>> LoadAssembliesAsync()
        {
            List<Assembly> assemblies = new List<Assembly>();
            if (!Directory.Exists(PackagesDirectory))
                return Array.Empty<Assembly>();

            foreach (var dir in Directory.GetDirectories(PackagesDirectory))
            {
                var directoryName = new DirectoryInfo(dir).Name;
                var nupkgFile = Path.Combine(dir, directoryName + ".nupkg");
                if (!File.Exists(nupkgFile))
                {
                    continue;
                }

                assemblies.AddRange(await nugetInstaller.LoadAssembliesFromNugetPackageAsync(nupkgFile));
            }

            return assemblies;
        }
    }
}