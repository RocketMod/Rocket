using ICSharpCode.SharpZipLib.Zip;
using MoreLinq.Extensions;
using Rocket.API;
using Rocket.API.Configuration;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.Logging;
using Rocket.Core.Configuration;
using Rocket.Core.Logging;
using Rocket.Core.Plugins.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NuGet.Frameworks;
using NuGet.Protocol.Core.Types;

//todo: find and install dependencies
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

        protected IConfiguration PackagesConfiguration
        {
            get
            {
                if (packagesConfiguration != null)
                    return packagesConfiguration;

                packagesConfiguration = Container.Resolve<IConfiguration>();

                PackagesDirectory = Path.Combine(runtime.WorkingDirectory, "Packages");
                if (!Directory.Exists(PackagesDirectory))
                    Directory.CreateDirectory(PackagesDirectory);

                ConfigurationContext context = new ConfigurationContext(PackagesDirectory, "packages");
                packagesConfiguration.LoadAsync(context, new PackagesConfiguration()).GetAwaiter().GetResult();
                packagesConfiguration.SaveAsync().GetAwaiter().GetResult();
                return packagesConfiguration;
            }
        }

        public virtual PackagesConfiguration Packages => PackagesConfiguration.Get<PackagesConfiguration>();
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
            nugetInstaller = new NuGetInstaller(logger, PackagesDirectory);

            PluginManagerInitEvent pluginManagerInitEvent =
                new PluginManagerInitEvent(this, EventExecutionTargetContext.Sync);
            EventBus.Emit(runtime, pluginManagerInitEvent);

            if (pluginManagerInitEvent.IsCancelled)
            {
                Logger.LogDebug($"[{GetType().Name}] Loading of plugins was cancelled.");
                return;
            }

            foreach (var package in Packages.NugetPackages.DistinctBy(d => d.Id.Trim().ToLowerInvariant()))
            {
                if (!Directory.Exists(PackagesDirectory))
                    Directory.CreateDirectory(PackagesDirectory);

                bool isInstalled = false;
                bool isLatest = package.Version == null || package.Version == "*" || package.Version == "latest";

                foreach (var dir in Directory.GetDirectories(PackagesDirectory))
                {
                    var dirName = new DirectoryInfo(dir).Name;
                    if (isLatest)
                    {
                        if (!dirName.StartsWith(package.Id + "."))
                            continue;
                    }
                    else
                    {
                        if (!dirName.Equals(package.Id + "." + package.Version))
                            continue;
                    }

                    foreach (var file in Directory.GetFiles(dir))
                    {
                        if (file.EndsWith(".nupkg"))
                        {
                            await LoadPluginFromNugetPackageAsync(file);
                            isInstalled = true;
                            break;
                        }
                    }

                    if (isInstalled)
                        break;
                }

                if (isInstalled)
                    continue;

                var result = await InstallAsync(package.Id, isLatest ? null : package.Version, null, true);
                if (result.Code != NuGetInstallCode.Success)
                    Logger.LogWarning($"Package \"{package.Id}\" failed to install: " + result);
                else
                    await LoadPluginFromNugetAsync(package.Id);
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

            Repository repo;
            if (repoName == null)
            {
                repo = await nugetInstaller.FindRepositoryForPackageAsync(enabledRepos, packageName, version, isPreRelease);
            }
            else
            {
                repo = enabledRepos.FirstOrDefault(c => c.Name.Equals(repoName, StringComparison.OrdinalIgnoreCase));
            }

            if (repo == null)
            {
                return new NuGetInstallResult(NuGetInstallCode.PackageNotFound);
            }

            if (isUpdate
                || Packages.NugetPackages.Any(c => c.Id.Equals(packageName, StringComparison.OrdinalIgnoreCase))
                || PackageExists(packageName))
                await UninstallAsync(packageName);

            if (!repo.IsEnabled)
            {
                return new NuGetInstallResult(NuGetInstallCode.RepositoryNotFound);
            }

            var packages = (await nugetInstaller.QueryPackagesAsync(enabledRepos, packageName, version, isPreRelease))
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

            var installedVersion = result.InstalledVersion;

            PackagesConfiguration config = Packages;

            bool wasInstalled = false;
            foreach (var np in config.NugetPackages)
            {
                if (!np.Id.Equals(packageName, StringComparison.OrdinalIgnoreCase))
                    continue;

                wasInstalled = true;
                np.Version = installedVersion;
            }

            if (!wasInstalled)
            {
                var list = config.NugetPackages.ToList();
                list.Add(new ConfigurationNuGetPackage
                {
                    Id = packageName,
                    Version = installedVersion
                });
                config.NugetPackages = list.ToArray();
            }

            PackagesConfiguration.Set(config);
            await PackagesConfiguration.SaveAsync();
            return result;
        }

        public virtual async Task<bool> UninstallAsync(string packageName)
        {
            PackagesConfiguration config = Packages;
            var list = config.NugetPackages.ToList();
            list.RemoveAll(d => d.Id.Equals(packageName, StringComparison.OrdinalIgnoreCase));
            config.NugetPackages = list.ToArray();
            PackagesConfiguration.Set(config);
            await PackagesConfiguration.SaveAsync();

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
            var assemblies = await LoadAssembliesFromNugetPackageAsync(packagePath);
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
                return null;

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

                assemblies.AddRange(await LoadAssembliesFromNugetPackageAsync(nugetPackageFile));
            }

            return assemblies;
        }

        public virtual async Task<IEnumerable<Assembly>> LoadAssembliesFromNugetPackageAsync(string nugetPackageFile)
        {
            List<Assembly> allAssemblies = new List<Assembly>();
            using (Stream fs = new FileStream(nugetPackageFile, FileMode.Open))
            {
                ZipFile zf = new ZipFile(fs);
                List<ZipEntry> assemblies = new List<ZipEntry>();

#if NET461
                foreach (ZipEntry entry in zf)
                    if (entry.Name.ToLower().StartsWith("lib/net461") && entry.Name.EndsWith(".dll"))
                        assemblies.Add(entry);

                if (assemblies.Count < 1) // if no NET461 assemblies were found; continue to check.
#elif NETSTANDARD2_0 || NET461
                foreach (ZipEntry entry in zf)
                    if (entry.Name.ToLower().StartsWith("lib/netstandard2.0") && entry.Name.EndsWith(".dll"))
                        assemblies.Add(entry);

#endif

                if (assemblies.Count == 0)
                    throw new Exception("No compatible assemblies were found.");

                foreach (var ze in assemblies)
                {
                    Stream inputStream = zf.GetInputStream(ze);
                    MemoryStream ms = new MemoryStream();

                    await inputStream.CopyToAsync(ms);
                    var asm = Assembly.Load(ms.ToArray());

                    ms.Close();
                    inputStream.Close();
                    zf.Close();

                    allAssemblies.Add(asm);
                }
            }

            return allAssemblies;
        }
    }
}