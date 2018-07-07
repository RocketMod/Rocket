using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ICSharpCode.SharpZipLib.Zip;
using Rocket.API;
using Rocket.API.Configuration;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.Logging;
using Rocket.API.Plugins;
using Rocket.Core.Configuration;
using Rocket.Core.Logging;
using Rocket.Core.Plugins.Events;
using Rocket.Core.Plugins.NuGet.Client.V3;
#if NET35
using Theraot.Core;
#endif

namespace Rocket.Core.Plugins.NuGet
{
    public class NuGetPluginManager : CLRPluginManager
    {
        private readonly IRuntime runtime;
        private readonly NuGetClientV3 client;
        private IConfiguration configuration;

        public virtual string RepositoriesDirectory { get; protected set; }

        public NuGetPluginManager(IDependencyContainer container,
                                  IEventManager eventManager,
                                  ILogger logger,
                                  IRuntime runtime) :
            base(container, eventManager, logger)
        {
            this.runtime = runtime;
            client = new NuGetClientV3(container);
        }

        protected virtual IConfiguration Configuration
        {
            get
            {
                if (configuration != null)
                    return configuration;

                var runtime = Container.Resolve<IRuntime>();
                configuration = Container.Resolve<IConfiguration>();

                RepositoriesDirectory = Path.Combine(runtime.WorkingDirectory, "Repositories");
                if (!Directory.Exists(RepositoriesDirectory))
                    Directory.CreateDirectory(RepositoriesDirectory);

                ConfigurationContext context = new ConfigurationContext(RepositoriesDirectory, "Repositories");
                configuration.Load(context, new
                {
                    Repositories = new[]
                    {
                        new Repository
                        {
                            Name = "universal",
                            Url = "http://nuget.rocketmod.net/",
                            Enabled = true,
                            Type = "Plugins"
                        },
                        new Repository
                        {
                            Name = "unturned",
                            Url = "http://unturned.nuget.rocketmod.net/",
                            Enabled = false,
                            Type = "Plugins"
                        },
                        new Repository
                        {
                            Name = "eco",
                            Url = "http://eco.nuget.rocketmod.net/",
                            Enabled = false,
                            Type = "Plugins"
                        },
                        new Repository
                        {
                            Name = "nuget",
                            Url = "",
                            Enabled = true,
                            Type = "Libraries"
                        }
                    }
                });

                return configuration;
            }
        }

        public virtual IEnumerable<Repository> Repositories
            => Configuration["Repositories"].Get<Repository[]>();

        public NuGetInstallResult Update(string repoName, string packageName, string version = null, bool isPreRelease = false)
        {
            return InstallInternal(repoName, packageName, version, isPreRelease, true);
        }

        public NuGetInstallResult Install(string repoName, string packageName, string version = null, bool isPreRelease = false)
        {
            return InstallInternal(repoName, packageName, version, isPreRelease);
        }

        protected virtual NuGetInstallResult InstallInternal(string repoName, string packageName, string version = null, bool isPreRelease = false, bool isUpdate = false)
        {
            if (isUpdate != PluginExists(repoName, packageName))
                return NuGetInstallResult.PackageNotFound;

            if (isUpdate)
                Uninstall(repoName, packageName);

            var configRepo = Repositories
                .FirstOrDefault(c => c.Name.Equals(repoName, StringComparison.OrdinalIgnoreCase));

            if (configRepo == null)
                throw new ArgumentException("Repo not found: " + repoName, nameof(repoName));

            if (!configRepo.Enabled)
                throw new ArgumentException("Repo not enabled: " + repoName, nameof(repoName));

            var repo = client.FetchRepository(configRepo.Url);
            List<NuGetPackage> packages = client.QueryPackages(repo, new NuGetQuery
            {
                Name = packageName
            }, isPreRelease)
            //Todo: remove after queries are fixed                                    
            .Where(c => c.Id.ToLower().Contains(packageName.ToLower())).ToList();

            if (packages.Count == 0)
                return NuGetInstallResult.PackageNotFound;

            if (packages.Count > 1)
                throw new Exception("Multiple packages matched.");

            var package = packages.First();
            version = version ?? package.Version;
            var targetVersion =
                package.Versions.FirstOrDefault(c => c.Version.Equals(version, StringComparison.OrdinalIgnoreCase));

            if (targetVersion == null)
                return NuGetInstallResult.VersionNotFound;

            byte[] data = client.DownloadPackage(repo, targetVersion);

            string uid = package.Id + "." + targetVersion.Version;
            var targetDir = Path.Combine(Path.Combine(RepositoriesDirectory, configRepo.Name), uid);
            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            File.WriteAllBytes(Path.Combine(targetDir, uid + ".nupkg"), data);
            return NuGetInstallResult.Success;
        }

        public virtual bool Uninstall(string repoName, string packageName, string version = null)
        {
            var repoDir = Path.Combine(RepositoriesDirectory, repoName);
            if (!Directory.Exists(repoDir))
                return true;

            foreach (var directory in Directory.GetDirectories(repoDir))
            {
                var name = new DirectoryInfo(directory).Name;
                if (version == null
                    && name.ToLowerInvariant().StartsWith(packageName.ToLowerInvariant()))
                {
                    Directory.Delete(directory, true);
                    return true;
                }

                if (version != null
                    && name.Equals(packageName + "." + version, StringComparison.OrdinalIgnoreCase))
                {
                    Directory.Delete(directory, true);
                    return true;
                }
            }

            return false;
        }

        public virtual bool LoadPlugin(string repo, string pluginName)
        {
            var pkg = GetNugetPackageFile(repo, pluginName);
            if (pkg == null)
                throw new Exception($"Plugin \"{pluginName}\" was not found in repo \"{repo}\".");

            return LoadPluginFromNugetPackage(pkg);
        }

        protected virtual bool LoadPluginFromNugetPackage(string packagePath)
        {
            var assemblies = LoadAssembliesFromNugetPackage(packagePath);
            bool success = false;
            foreach (var asm in assemblies)
            {
                if (LoadPluginFromAssembly(asm, out var pluginChildContainer) != null)
                {
                    success = RegisterAndLoadPluginFromContainer(pluginChildContainer);
                }
            }

            return success;
        }

        public virtual bool PluginExists(string repo, string pluginName)
        {
            return File.Exists(GetNugetPackageFile(repo, pluginName));
        }

        public virtual string GetNugetPackageFile(string repo, string pluginName)
        {
            var repoDir = Path.Combine(RepositoriesDirectory, repo);
            if (!Directory.Exists(repoDir))
                return null;

            foreach (var dir in Directory.GetDirectories(repoDir))
            {
                var dirName = new DirectoryInfo(dir).Name;
                if (dirName.ToLower().StartsWith(pluginName.ToLower() + "."))
                {
                    return Path.Combine(dir, dirName + ".nupkg");
                }
            }

            return null;
        }

        public override string ServiceName => "NuGet Plugins";
        protected override IEnumerable<Assembly> LoadAssemblies()
        {
            List<Assembly> assemblies = new List<Assembly>();
            foreach (var repo in Repositories.Where(c => c.Enabled))
            {
                var repoDir = Path.Combine(RepositoriesDirectory, repo.Name);
                if (!Directory.Exists(repoDir))
                    continue;

                foreach (var dir in Directory.GetDirectories(repoDir))
                {
                    var dirName = new DirectoryInfo(dir).Name;
                    var nugetPackageFile = Path.Combine(dir, dirName + "nupkg");
                    if (!File.Exists(nugetPackageFile))
                        continue;

                    assemblies.AddRange(LoadAssembliesFromNugetPackage(nugetPackageFile));
                }
            }

            return assemblies;
        }

        public override void Init()
        {
            Logger.LogDebug($"[{GetType().Name}] Initializing Nuget plugins.");

            PluginManagerInitEvent pluginManagerInitEvent =
                new PluginManagerInitEvent(this, EventExecutionTargetContext.Sync);
            EventManager.Emit(runtime, pluginManagerInitEvent);

            if (pluginManagerInitEvent.IsCancelled)
            {
                Logger.LogDebug($"[{GetType().Name}] Loading of plugins was cancalled.");
                return;
            }

            foreach (var repo in Repositories)
            {
                var repoDir = Path.Combine(RepositoriesDirectory, repo.Name);
                if(!Directory.Exists(repoDir))
                    continue;

                foreach (var dir in Directory.GetDirectories(repoDir))
                {
                    foreach (var file in Directory.GetFiles(dir))
                    {
                        if (file.EndsWith(".nupkg"))
                            LoadPluginFromNugetPackage(file);
                    }
                }
            }
        }

        public virtual IEnumerable<Assembly> LoadAssembliesFromNugetPackage(string nugetPackageFile)
        {
            using (Stream fs = new FileStream(nugetPackageFile, FileMode.Open))
            {
                ZipFile zf = new ZipFile(fs);
                List<ZipEntry> assemblies = new List<ZipEntry>();
#if NET35
                foreach (ZipEntry entry in zf)
                    if (entry.Name.ToLower().StartsWith("lib/net35") && entry.Name.EndsWith(".dll"))
                        assemblies.Add(entry);

#elif NETSTANDARD2_0
                foreach (ZipEntry entry in zf)
                    if (entry.Name.ToLower().StartsWith("lib/netstandard2.0") && entry.Name.EndsWith(".dll"))
                        assemblies.Add(entry);
#elif NET461 || NETSTANDARD2_0
#if NETSTANDARD2_0
                if(ze == null) 
                {
#endif
                    foreach (ZipEntry entry in zf)
                        if (entry.Name.ToLower().StartsWith("lib/net461") && entry.Name.EndsWith(".dll"))
                        assemblies.Add(entry);
#if NETSTANDARD2_0
                }
#endif
#else
#error Not supported runtime
#endif

                if (assemblies.Count == 0)
                    throw new Exception("No compatible assemblies were found.");

                foreach (var ze in assemblies)
                {
                    Stream inputStream = zf.GetInputStream(ze);
                    MemoryStream ms = new MemoryStream();
                    inputStream.CopyTo(ms);

                    var asm = Assembly.Load(ms.ToArray());

                    ms.Close();
                    inputStream.Close();
                    zf.Close();

                    yield return asm;
                }
            }
        }
    }
}