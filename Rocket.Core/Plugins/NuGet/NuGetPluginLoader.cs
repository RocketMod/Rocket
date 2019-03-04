using MoreLinq.Extensions;
using NuGet.Frameworks;
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
        private readonly NuGetFramework currentFramework;
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

            string frameworkName = Assembly.GetExecutingAssembly().GetCustomAttributes(true)
                                           .OfType<System.Runtime.Versioning.TargetFrameworkAttribute>()
                                           .Select(x => x.FrameworkName)
                                           .FirstOrDefault();

            currentFramework = frameworkName == null
                ? NuGetFramework.AnyFramework
                : NuGetFramework.ParseFrameworkName(frameworkName, new DefaultFrameworkNameProvider());
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
            if (isUpdate && !PackageExists(packageName))
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

            if (isUpdate || PackageExists(packageName))
                await UninstallAsync(packageName);

            var packages = (await nugetInstaller.QueryPackagesAsync(repoUrls, packageName, version, isPreRelease))
                .ToList();

            if (packages.Count == 0)
            {
                return new NuGetInstallResult(NuGetInstallCode.PackageNotFound);
            }

            var package = packages.FirstOrDefault(d => packages.Count == 1 || d.Metadata.Identity.Id.Equals(packageName, StringComparison.OrdinalIgnoreCase));
            if (package == null)
            {
                // we have no exact match but multiple results
                return new NuGetInstallResult(NuGetInstallCode.MultipleMatch);
            }

            if (version == null)
            {
                version = package.Metadata.Identity.Version.OriginalVersion;
            }

            string uid = packageName + "." + version;
            string installPath = Path.Combine(PackagesDirectory, uid);

            var result = await nugetInstaller.InstallAsync(repo, package, installPath, version, isPreRelease);
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

        public virtual async Task<bool> LoadPluginFromNugetAsync(string pluginName)
        {
            var pkg = GetNugetPackageFile(pluginName);
            if (pkg == null)
                throw new Exception($"Plugin \"{pluginName}\" was not found.");

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
            return File.Exists(GetNugetPackageFile(packageName));
        }

        public virtual string GetNugetPackageFile(string packageName)
        {
            if (!Directory.Exists(PackagesDirectory))
            {
                return null;
            }

            foreach (var dir in Directory.GetDirectories(PackagesDirectory))
            {
                var dirName = new DirectoryInfo(dir).Name;
                if (dirName.ToLower().StartsWith(packageName.ToLower() + "."))
                {
                    return Path.Combine(dir, dirName + ".nupkg");
                }
            }

            return null;
        }

        public override string ServiceName => "NuGet Plugins";

        protected override async Task<IEnumerable<Assembly>> LoadAssembliesAsync()
        {
            List<Assembly> assemblies = new List<Assembly>();
            if (!Directory.Exists(PackagesDirectory))
                return Array.Empty<Assembly>();

            foreach (var dir in Directory.GetDirectories(PackagesDirectory))
            {
                var dirName = new DirectoryInfo(dir).Name;
                var nugetPackageFile = Path.Combine(dir, dirName + "nupkg");
                if (!File.Exists(nugetPackageFile))
                    continue;

                assemblies.AddRange(await nugetInstaller.LoadAssembliesFromNugetPackageAsync(nugetPackageFile));
            }

            return assemblies;
        }
    }
}