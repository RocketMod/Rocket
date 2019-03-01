using NuGet.Configuration;
using NuGet.PackageManagement;
using NuGet.ProjectManagement;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using Rocket.API.Logging;
using Rocket.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Packaging.Core;
using NuGet.Versioning;
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
            nugetSettings = Settings.LoadDefaultSettings(packagesDirectory, null, new XPlatMachineWideSetting());
            sourceCacheContext = new SourceCacheContext();
        }

        public async Task<NuGetInstallResult> InstallAsync(Repository repo, NuGetQueryResult queryResult, string path, string version, bool allowPrereleaseVersions = false)
        {
            ISourceRepositoryProvider sourceRepositoryProvider = new SourceRepositoryProvider(nugetSettings, providers);
            NuGetPackageManager packageManager = new NuGetPackageManager(sourceRepositoryProvider, nugetSettings, packagesDirectory)
            {
                PackagesFolderNuGetProject = new FolderNuGetProject(path)
            };

            ResolutionContext resolutionContext = new ResolutionContext(DependencyBehavior.Lowest, allowPrereleaseVersions, false, VersionConstraints.None);
            INuGetProjectContext projectContext = new EmptyNuGetProjectContext();

            PackageSource packageSource = new PackageSource(repo.Url);
            SourceRepository sourceRepository = new SourceRepository(packageSource, providers);

            var packageIdentity = new PackageIdentity(queryResult.Metadata.Identity.Id, new NuGetVersion(version));

            IEnumerable<SourceRepository> sourceRepositories = new[] { sourceRepository };
            await packageManager.InstallPackageAsync(
                    packageManager.PackagesFolderNuGetProject,
                    packageIdentity,
                    resolutionContext,
                    projectContext,
                    sourceRepositories,
                    Array.Empty<SourceRepository>(),
                    CancellationToken.None);

            return new NuGetInstallResult(version);
        }

        public async Task<IEnumerable<NuGetQueryResult>> QueryPackagesAsync(IEnumerable<Repository> repositories, string packageName, string version = null, bool includePreRelease = false)
        {
            var matches = new List<NuGetQueryResult>();

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
                    matches.AddRange(metadatas.Select(d => new NuGetQueryResult
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
                        matches.Add(new NuGetQueryResult
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

    public class NuGetQueryResult
    {
        public IPackageSearchMetadata Metadata { get; set; }

        public IEnumerable<VersionInfo> Versions { get; set; }

        public Repository Repository { get; set; }
    }
}