using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.PackageManagement;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Packaging.Signing;
using NuGet.ProjectManagement;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using NuGet.Versioning;

namespace Rocket.NuGet
{
    public class NuGetInstaller
    {
        private readonly ILogger logger;
        private readonly string packagesDirectory;
        private readonly List<Lazy<INuGetResourceProvider>> providers;
        private readonly ISettings nugetSettings;
        private readonly NuGetFramework currentFramework;

        public NuGetInstaller(ILogger logger, string packagesDirectory)
        {
            this.logger = logger;
            this.packagesDirectory = packagesDirectory;
            providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3()); // Add v3 API support

            nugetSettings = Settings.LoadDefaultSettings(packagesDirectory, null, new MachineWideSettings());

            var frameworkName = Assembly.GetExecutingAssembly().GetCustomAttributes(true)
                                           .OfType<System.Runtime.Versioning.TargetFrameworkAttribute>()
                                           .Select(x => x.FrameworkName)
                                           .FirstOrDefault();
            currentFramework = frameworkName == null
                ? NuGetFramework.AnyFramework
                : NuGetFramework.ParseFrameworkName(frameworkName, new DefaultFrameworkNameProvider());
        }

        public async Task<NuGetInstallResult> InstallAsync(string repo, QueriedNuGetPackage queryResult, string installPath, string version, bool allowPrereleaseVersions = false)
        {
            var packageSource = new PackageSource(repo);
            var sourceRepository = new SourceRepository(packageSource, providers);
            var packageSourceProvider = new PackageSourceProvider(nugetSettings);
            var sourceRepositoryProvider = new SourceRepositoryProvider(packageSourceProvider, providers);

            var project = new FolderNuGetProject(packagesDirectory);
            
            NuGetPackageManager packageManager = new NuGetPackageManager(sourceRepositoryProvider, nugetSettings, packagesDirectory)
            {
                PackagesFolderNuGetProject = project
            };

            INuGetProjectContext projectContext = new EmptyNuGetProjectContext()
            {
                PackageExtractionContext = new PackageExtractionContext(
                    PackageSaveMode.Nupkg,
                    XmlDocFileSaveMode.Skip,
                    ClientPolicyContext.GetClientPolicy(nugetSettings, logger),
                    logger),
                ActionType = NuGetActionType.Install,
            };

            ResolutionContext resolutionContext = new ResolutionContext(
                DependencyBehavior.Lowest,
                allowPrereleaseVersions,
                false,
                VersionConstraints.None);

            var identity = new PackageIdentity(queryResult.Metadata.Identity.Id, new NuGetVersion(version));

            await packageManager.InstallPackageAsync(
                project,
                identity,
                resolutionContext,
                projectContext,
                sourceRepository,
                Array.Empty<SourceRepository>(),
                CancellationToken.None);
            return new NuGetInstallResult(version);
        }

        public async Task<IEnumerable<QueriedNuGetPackage>> QueryPackagesAsync(IEnumerable<string> repositories, string packageName, string version = null, bool includePreRelease = false)
        {
            var matches = new List<QueriedNuGetPackage>();

            foreach (var repo in repositories)
            {
                var packageSource = new PackageSource(repo);
                var sourceRepository = new SourceRepository(packageSource, providers);
                var searchResource = await sourceRepository.GetResourceAsync<PackageSearchResource>();
                var searchFilter = new SearchFilter(includePreRelease)
                {
                    IncludeDelisted = false,
                    SupportedFrameworks = new []{ currentFramework .DotNetFrameworkName }
                };

                var searchResult = await searchResource.SearchAsync(packageName, searchFilter, 0, 10, logger, CancellationToken.None);

                if (version == null)
                {
                    matches.AddRange(searchResult.Select(d => new QueriedNuGetPackage
                    {
                        Metadata = d,
                        Repository = repo,
                        Versions = null
                    }));
                    continue;
                }

                foreach (var packageMeta in searchResult)
                {
                    var versions = (await packageMeta.GetVersionsAsync()).ToList();
                    if (versions.Any(d => d.Version.OriginalVersion.Equals(version, StringComparison.OrdinalIgnoreCase)))
                    {
                        matches.Add(new QueriedNuGetPackage
                        {
                            Metadata = packageMeta,
                            Repository = repo,
                            Versions = versions
                        });
                    }
                }
            }

            return matches;
        }

        public async Task<string> FindRepositoryForPackageAsync(IEnumerable<string> repositories, string packageName, string version = null, bool includePreReleases = false)
        {
            var repoList = repositories.ToList();

            foreach (var repository in repoList)
            {
                try
                {
                    var results = (await QueryPackagesAsync(new[] { repository }, packageName, version, includePreReleases)).ToList();

                    if (results.Count == 0)
                    {
                        continue;
                    }

                    return repository;
                }
                catch (Exception ex)
                {
                    logger.LogWarning($"Failed to fetch from repository: {repository}: " + ex.Message);
                }
            }

            return null;
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

#if NETSTANDARD2_0
                if (assemblies.Count < 1) // if no NET461 assemblies were found; continue to check.
#endif
#elif NETSTANDARD2_0 || NET461
                foreach (ZipEntry entry in zf)
                    if (entry.Name.ToLower().StartsWith("lib/netstandard2.0") && entry.Name.EndsWith(".dll"))
                        assemblies.Add(entry);

#endif

                if (assemblies.Count == 0)
                {
                    logger.LogWarning("No assemblies found in: " + nugetPackageFile);
                    return allAssemblies;
                }

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

    public class MachineWideSettings : IMachineWideSettings
    {
        public MachineWideSettings()
        {
            var baseDirectory = NuGetEnvironment.GetFolderPath(NuGetFolderPath.MachineWideConfigDirectory);
            Settings = global::NuGet.Configuration.Settings.LoadMachineWideSettings(baseDirectory);
        }

        public ISettings Settings { get; }
    }

    public class QueriedNuGetPackage
    {
        public IPackageSearchMetadata Metadata { get; set; }

        public IEnumerable<VersionInfo> Versions { get; set; }

        public string Repository { get; set; }
    }
}