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
using Rocket.Core.Plugins.NuGet.Client.V3;
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
        private readonly IRuntime runtime;
        private readonly NuGetClientV3 client;
        private IConfiguration configuration;
        private IConfiguration packagesConfiguration;

        public virtual string PackagesDirectory { get; protected set; }

        public NuGetPluginLoader(IDependencyContainer container,
                                  IEventBus eventBus,
                                  ILogger logger,
                                  IRuntime runtime) :
            base(container, eventBus, logger)
        {
            this.runtime = runtime;
            client = new NuGetClientV3(container);
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

        protected virtual IConfiguration Configuration
        {
            get
            {
                if (configuration != null)
                    return configuration;

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
                            Url = "http://nuget.rocketmod.net/",
                            Enabled = true
                        },
                        new Repository
                        {
                            Name = "nuget",
                            Url = "https://www.nuget.org/api/v2",
                            Enabled = true
                        }
                    }
                }).GetAwaiter().GetResult();
                configuration.SaveAsync().GetAwaiter().GetResult();

                return configuration;
            }
        }

        public virtual IEnumerable<Repository> Repositories
            => Configuration["Repositories"].Get<Repository[]>();

        public async Task<NuGetInstallResult> Update(string packageName, string version = null, string repoName = null, bool isPreRelease = false)
        {
            return await InstallOrUpdate(packageName, version, repoName, isPreRelease, true);
        }

        public async Task<NuGetInstallResult> Install(string packageName, string version = null, string repoName = null, bool isPreRelease = false)
        {
            return await InstallOrUpdate(packageName, version, repoName, isPreRelease);
        }

        protected virtual async Task<NuGetInstallResult> InstallOrUpdate(string packageName, string version = null, string repoName = null, bool isPreRelease = false, bool isUpdate = false)
        {
            if (isUpdate && !PackageExists(packageName))
                return NuGetInstallResult.PackageNotFound;

            if (repoName == null)
            {
                repoName = FindRepositoryForPackage(packageName, isPreRelease);
            }

            if (repoName == null)
            {
                return NuGetInstallResult.PackageNotFound;
            }

            if (isUpdate
                || Packages.NugetPackages.Any(c => c.Id.Equals(packageName, StringComparison.OrdinalIgnoreCase))
                || PackageExists(packageName))
                await Uninstall(packageName);

            var configRepo = Repositories.FirstOrDefault(c => c.Name.Equals(repoName, StringComparison.OrdinalIgnoreCase));

            if (configRepo == null)
                return NuGetInstallResult.RepositoryNotFound;

            if (!configRepo.Enabled)
                return NuGetInstallResult.RepositoryNotFound;

            var repo = client.FetchRepository(configRepo.Url);
            List<NuGetPackage> packages = client.QueryPackages(repo, new NuGetQuery
            {
                Name = packageName
            }, isPreRelease)
            //Todo: remove after harbor queries are fixed                                    
            .Where(c => c.Id.ToLower().Contains(packageName.ToLower())).ToList();

            if (packages.Count == 0)
                return NuGetInstallResult.PackageNotFound;

            if (packages.Count > 1)
                return NuGetInstallResult.MultipleMatch;

            var package = packages.First();
            version = version ?? package.Version;
            var targetVersion =
                package.Versions.FirstOrDefault(c => c.Version.Equals(version, StringComparison.OrdinalIgnoreCase));

            if (targetVersion == null)
                return NuGetInstallResult.VersionNotFound;

            byte[] data = client.DownloadPackage(repo, targetVersion);

            string uid = package.Id + "." + targetVersion.Version;
            if (!Directory.Exists(PackagesDirectory))
                Directory.CreateDirectory(PackagesDirectory);

            File.WriteAllBytes(Path.Combine(PackagesDirectory, uid + ".nupkg"), data);
            PackagesConfiguration config = Packages;

            bool wasInstalled = false;
            foreach (var np in config.NugetPackages)
            {
                if (!np.Id.Equals(package.Id, StringComparison.OrdinalIgnoreCase))
                    continue;

                wasInstalled = true;
                np.Version = package.Version;
            }

            if (!wasInstalled)
            {
                var list = config.NugetPackages.ToList();
                list.Add(new ConfigurationNuGetPackage
                {
                    Id = package.Id,
                    Version = package.Version
                });
                config.NugetPackages = list.ToArray();
            }

            PackagesConfiguration.Set(config);
            await PackagesConfiguration.SaveAsync();

            return NuGetInstallResult.Success;
        }

        public string FindRepositoryForPackage(string packageName, bool includePreReleases = false)
        {
            //todo: return repository with highest version

            throw new NotImplementedException();
        }

        public virtual async Task<bool> Uninstall(string packageName)
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
            var assemblies = await LoadAssembliesFromNugetPackageAync(packagePath);
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
        protected override async Task<IEnumerable<Assembly>> LoadAssemblies()
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

                assemblies.AddRange(await LoadAssembliesFromNugetPackageAync(nugetPackageFile));
            }

            return assemblies;
        }

        public override async Task InitAsync()
        {
            Logger.LogDebug($"[{GetType().Name}] Initializing Nuget plugins.");

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

                var result = await Install(package.Id, isLatest ? null : package.Version, null, true);
                if (result != NuGetInstallResult.Success)
                    Logger.LogWarning($"Package \"{package.Id}\" failed to install: " + result);
                else
                    await LoadPluginFromNugetAsync(package.Id);
            }
        }

        public virtual async Task<IEnumerable<Assembly>> LoadAssembliesFromNugetPackageAync(string nugetPackageFile)
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