using NuGet.Packaging.Core;
using NuGet.Versioning;
using Rocket.API;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.Logging;
using Rocket.Core.Logging;
using Rocket.Core.Plugins.Events;
using Rocket.NuGet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Rocket.Core.Plugins.NuGet
{
    public class NuGetPluginLoader : ClrPluginLoader
    {
        private readonly ILogger logger;
        private readonly IRuntime runtime;
        private NuGetPackageManager nugetPackageManager;

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

        public override async Task InitAsync()
        {
            Logger.LogDebug($"[{GetType().Name}] Initializing NuGet.");
            PackagesDirectory = Path.Combine(runtime.WorkingDirectory, "Packages");
            if (!Directory.Exists(PackagesDirectory))
            {
                Directory.CreateDirectory(PackagesDirectory);
            }

            var adapter = new NuGetLoggerAdapter(logger);
            nugetPackageManager = new NuGetPackageManager(adapter, PackagesDirectory);

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

        public virtual async Task<NuGetInstallResult> UpdateAsync(string packageName, string version = null, bool isPreRelease = false)
        {
            return await InstallOrUpdateAsync(packageName, version, isPreRelease, true);
        }

        public virtual async Task<NuGetInstallResult> InstallAsync(string packageName, string version = null, bool isPreRelease = false)
        {
            bool exists = await nugetPackageManager.PackageExistsAsync(packageName);
            return await InstallOrUpdateAsync(packageName, version, isPreRelease, exists);
        }

        protected virtual async Task<NuGetInstallResult> InstallOrUpdateAsync(string packageId, string version = null, bool isPreRelease = false, bool isUpdate = false)
        {
            PackageIdentity previousVersion = await nugetPackageManager.GetLatestPackageIdentityAsync(packageId);

            if (isUpdate && previousVersion == null)
            {
                return new NuGetInstallResult(NuGetInstallCode.PackageOrVersionNotFound);
            }

            var package = (await nugetPackageManager.QueryPackageExactAsync(packageId, version, isPreRelease));
            if (package == null)
            {
                return new NuGetInstallResult(NuGetInstallCode.PackageOrVersionNotFound);
            }

            if (version == null)
            {
                version = package.Identity.Version.OriginalVersion; // set to latest version
            }

            var packageIdentity = new PackageIdentity(package.Identity.Id, new NuGetVersion(version));

            var result = await nugetPackageManager.InstallAsync(packageIdentity, isPreRelease);
            if (result.Code != NuGetInstallCode.Success)
            {
                return result;
            }

            if (isUpdate)
            {
                await nugetPackageManager.RemoveAsync(previousVersion);
            }
            return result;
        }

        public virtual async Task<bool> RemoveAsync(string packageId)
        {
            var package = await nugetPackageManager.GetLatestPackageIdentityAsync(packageId);
            if (package == null)
            {
                return false;
            }

            return await nugetPackageManager.RemoveAsync(package);
        }

        public virtual async Task<bool> LoadPluginFromNugetAsync(PackageIdentity identity)
        {
            var pkg = nugetPackageManager.GetNugetPackageFile(identity);
            if (pkg == null)
                throw new Exception($"Plugin {identity.Id} v{identity.Version} was not found.");

            return await LoadPluginFromNugetPackageAsync(pkg);
        }

        protected virtual async Task<bool> LoadPluginFromNugetPackageAsync(string packagePath)
        {
            var assemblies = await nugetPackageManager.LoadAssembliesFromNugetPackageAsync(packagePath);
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

        public virtual async Task<bool> PackageExistsAsync(string packageName)
        {
            return await nugetPackageManager.PackageExistsAsync(packageName);
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

                assemblies.AddRange(await nugetPackageManager.LoadAssembliesFromNugetPackageAsync(nupkgFile));
            }

            return assemblies;
        }
    }
}