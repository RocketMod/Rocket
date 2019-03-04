using NuGet.Configuration;
using NuGet.PackageManagement;
using NuGet.ProjectManagement;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using Rocket.Core.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Packaging.Signing;
using NuGet.Versioning;
using ILogger = Rocket.API.Logging.ILogger;
using Types_Repository = NuGet.Protocol.Core.Types.Repository;

namespace Rocket.Core.Plugins.NuGet
{
    public class NuGetInstaller
    {
        private readonly ILogger logger;
        private readonly string packagesDirectory;
        private readonly List<Lazy<INuGetResourceProvider>> providers;
        private readonly ISettings nugetSettings;
        private readonly SourceCacheContext sourceCacheContext;
        private readonly NuGetLoggerAdapter nuGetLoggerAdapter;

        public NuGetInstaller(ILogger logger, string packagesDirectory)
        {
            this.logger = logger;
            this.packagesDirectory = packagesDirectory;
            providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Types_Repository.Provider.GetCoreV3()); // Add v3 API support

            nuGetLoggerAdapter = new NuGetLoggerAdapter(logger);
            nugetSettings = Settings.LoadDefaultSettings(packagesDirectory, null, new MachineWideSettings());
            sourceCacheContext = new SourceCacheContext();
        }

        public async Task<NuGetInstallResult> InstallAsync(Repository repo, QueriedNuGetPackage queryResult, string installPath, string version, bool allowPrereleaseVersions = false)
        {
            var packageSource = new PackageSource(repo.Url);
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
                    ClientPolicyContext.GetClientPolicy(nugetSettings, nuGetLoggerAdapter),
                    nuGetLoggerAdapter),
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

        public async Task<IEnumerable<QueriedNuGetPackage>> QueryPackagesAsync(IEnumerable<Repository> repositories, string packageName, string version = null, bool includePreRelease = false)
        {
            var matches = new List<QueriedNuGetPackage>();

            foreach (var repo in repositories)
            {
                PackageSource packageSource = new PackageSource(repo.Url);

                SourceRepository sourceRepository = new SourceRepository(packageSource, providers);
                PackageMetadataResource packageMetadataResource = await sourceRepository.GetResourceAsync<PackageMetadataResource>();

                var metadatas = await packageMetadataResource.GetMetadataAsync(
                    packageName, 
                    includePreRelease, 
                    false, 
                    sourceCacheContext,
                    nuGetLoggerAdapter, 
                    CancellationToken.None);

                if (version == null)
                {
                    matches.AddRange(metadatas.Select(d => new QueriedNuGetPackage
                    {
                        Metadata = d,
                        Repository = repo,
                        Versions = null
                    }));
                    continue;
                }

                foreach (var meta in metadatas)
                {
                    var versions = (await meta.GetVersionsAsync()).ToList();
                    if (versions.Any(d => d.Version.OriginalVersion.Equals(version, StringComparison.OrdinalIgnoreCase)))
                    {
                        matches.Add(new QueriedNuGetPackage
                        {
                            Metadata = meta,
                            Repository = repo,
                            Versions = versions
                        });
                    }
                }
            }

            return matches;
        }

        public async Task<Repository> FindRepositoryForPackageAsync(IEnumerable<Repository> repositories, string packageName, string version = null, bool includePreReleases = false)
        {
            var repoList = repositories.ToList();

            foreach (Repository repository in repoList)
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
                    logger.LogWarning($"Failed to fetch from repository: {repository.Name}", ex);
                }
            }

            return null;
        }
    }

    public class MachineWideSettings : IMachineWideSettings
    {
        private readonly ISettings _settings;

        public MachineWideSettings()
        {
            var baseDirectory = NuGetEnvironment.GetFolderPath(NuGetFolderPath.MachineWideConfigDirectory);
            _settings = global::NuGet.Configuration.Settings.LoadMachineWideSettings(baseDirectory);
        }

        public ISettings Settings => _settings ;
    }

    public class QueriedNuGetPackage
    {
        public IPackageSearchMetadata Metadata { get; set; }

        public IEnumerable<VersionInfo> Versions { get; set; }

        public Repository Repository { get; set; }
    }
}