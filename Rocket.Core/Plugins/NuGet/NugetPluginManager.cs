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
using Rocket.Core.Configuration;
using Rocket.Core.Plugins.NuGet.Client.V3;
#if NET35
using Theraot.Core;
#endif

namespace Rocket.Core.Plugins.NuGet
{
    public class NuGetPluginManager : CLRPluginManager
    {
        private readonly NuGetClientV3 client;
        private IConfiguration configuration;

        public virtual string RepositoriesDirectory { get; protected set; }

        public NuGetPluginManager(IDependencyContainer container,
                                  IEventManager eventManager,
                                  ILogger logger) :
            base(container, eventManager, logger)
        {
            client = new NuGetClientV3();
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
                            Enabled = true
                        },
                        new Repository
                        {
                            Name = "unturned",
                            Url = "http://unturned.nuget.rocketmod.net/",
                            Enabled = false
                        },
                        new Repository
                        {
                            Name = "eco",
                            Url = "http://eco.nuget.rocketmod.net/",
                            Enabled = false
                        }
                    }
                });

                return configuration;
            }
        }

        public virtual IEnumerable<Repository> Repositories
            => Configuration["Repositories"].Get<Repository[]>();

        public bool Update(string repoName, string packageName, string version = null)
        {
            return InstallInternal(repoName, packageName, version, true);
        }

        public bool Install(string repoName, string packageName, string version = null)
        {
            return InstallInternal(repoName, packageName, version);
        }

        protected virtual bool InstallInternal(string repoName, string packageName, string version = null, bool isUpdate = false)
        {
            if (isUpdate != PluginExists(repoName, packageName))
                return false;

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
            })
            //Todo: remove after queries are fixed                                    
            .Where(c => c.Id.ToLower().Contains(packageName.ToLower())).ToList();

            if (packages.Count == 0)
                return false;

            if (packages.Count > 1)
                throw new Exception("Multiple packages matched.");

            var package = packages.First();
            version = version ?? package.Version;
            var targetVersion =
                package.Versions.FirstOrDefault(c => c.Version.Equals(version, StringComparison.OrdinalIgnoreCase));

            if (targetVersion == null)
                throw new Exception("Version not found");

            byte[] data = client.DownloadPackage(repo, targetVersion);

            string uid = package.Id + "." + targetVersion.Version;
            var targetDir = Path.Combine(Path.Combine(RepositoriesDirectory, configRepo.Name), uid);
            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            File.WriteAllBytes(Path.Combine(targetDir, uid + ".nupkg"), data);
            return true;
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
            var assemblies = LoadAssembliesFromNugetPackage(pkg);

            bool success = false;
            foreach (var asm in assemblies)
            {
                if (LoadPluginFromAssembly(asm, out _) != null)
                    success = true;
            }

            return success;
        }

        public virtual bool PluginExists(string repo, string pluginName)
        {
            return File.Exists(GetNugetPackageFile(repo, pluginName));
        }

        public virtual string GetNugetPackageFile(string repo, string pluginName)
        {
            foreach (var dir in Directory.GetDirectories(Path.Combine(RepositoriesDirectory, repo)))
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

        public virtual IEnumerable<Assembly> LoadAssembliesFromNugetPackage(string nugetPackageFile)
        {
            using (Stream fs = new FileStream(nugetPackageFile, FileMode.Open))
            {
                ZipFile zf = new ZipFile(fs);
                ZipEntry ze = null;
#if NET35
                foreach (ZipEntry entry in zf)
                    if (entry.Name.ToLower().StartsWith("lib/net35") && entry.Name.EndsWith(".dll"))
                    {
                        ze = entry;
                        break;
                    }

#elif NETSTANDARD2_0
                foreach (ZipEntry entry in zf)
                    if (entry.Name.ToLower().StartsWith("lib/netstandard2.0") && entry.Name.EndsWith(".dll"))
                    {
                        ze = entry;
                        break;
                    }
#elif NET461 || NETSTANDARD2_0
#if NETSTANDARD2_0
                if(ze == null) 
                {
#endif
                    foreach (ZipEntry entry in zf)
                        if (entry.Name.ToLower().StartsWith("lib/net461") && entry.Name.EndsWith(".dll"))
                        {
                            ze = entry;
                            break;
                        }
#if NETSTANDARD2_0
                }
#endif
#else
#error Not supported runtime
#endif

                if (ze == null)
                    throw new Exception("No dll found in package");

                Stream inputStream = zf.GetInputStream(ze);
                MemoryStream ms = new MemoryStream();
                inputStream.CopyTo(ms);

                var asm = Assembly.Load(ms.ToArray());

                ms.Close();
                inputStream.Close();
                zf.Close();

                return new List<Assembly> { asm };
            }
        }
    }
}