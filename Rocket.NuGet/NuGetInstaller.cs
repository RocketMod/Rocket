using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Packaging.Signing;
using NuGet.ProjectManagement;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Rocket.NuGet
{
    public class NuGetInstaller
    {
        private readonly ILogger logger;
        private readonly string packagesDirectory;
        private readonly List<Lazy<INuGetResourceProvider>> providers;
        private readonly ISettings nugetSettings;
        private readonly NuGetFramework currentFramework;
        private readonly FrameworkReducer frameworkReducer;
        private readonly PackagePathResolver packagePathResolver;
        private readonly PackageResolver packageResolver;

        public NuGetInstaller(ILogger logger, string packagesDirectory)
        {
            this.logger = logger;
            this.packagesDirectory = packagesDirectory;
            providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3()); // Add v3 API support

            nugetSettings = Settings.LoadDefaultSettings(null);

            frameworkReducer = new FrameworkReducer();

            var frameworkName = Assembly.GetExecutingAssembly().GetCustomAttributes(true)
                                           .OfType<System.Runtime.Versioning.TargetFrameworkAttribute>()
                                           .Select(x => x.FrameworkName)
                                           .FirstOrDefault();

            currentFramework = frameworkName == null
                ? NuGetFramework.AnyFramework
                : NuGetFramework.ParseFrameworkName(frameworkName, new DefaultFrameworkNameProvider());

            packagePathResolver = new PackagePathResolver(packagesDirectory);
            packageResolver = new PackageResolver();

            InstallAssemblyResolver();
        }

        public async Task<NuGetInstallResult> InstallAsync(string repo, PackageIdentity packageIdentity, NuGetActionType action = NuGetActionType.Update, bool allowPrereleaseVersions = false)
        {
            using (var cacheContext = new SourceCacheContext())
            {
                var packageSource = new PackageSource(repo);
                var sourceRepository = new SourceRepository(packageSource, providers);

                var packagesToInstall = await GetDependenciesAsync(packageIdentity, cacheContext);

                var packageExtractionContext = new PackageExtractionContext(
                    PackageSaveMode.Nupkg,
                    XmlDocFileSaveMode.None,
                    ClientPolicyContext.GetClientPolicy(nugetSettings, logger),
                    logger);

                foreach (var packageToInstall in packagesToInstall)
                {
                    var installedPath = packagePathResolver.GetInstalledPath(packageToInstall);
                    if (installedPath == null)
                    {
                        var downloadResource = await packageToInstall.Source.GetResourceAsync<DownloadResource>(CancellationToken.None);
                        var downloadResult = await downloadResource.GetDownloadResourceResultAsync(
                            packageToInstall,
                            new PackageDownloadContext(cacheContext),
                            SettingsUtility.GetGlobalPackagesFolder(nugetSettings),
                            logger, CancellationToken.None);

                        await PackageExtractor.ExtractPackageAsync(
                            downloadResult.PackageSource,
                            downloadResult.PackageStream,
                            packagePathResolver,
                            packageExtractionContext,
                            CancellationToken.None);
                    }
                }

                return new NuGetInstallResult(packageIdentity);
            }
        }

        private async Task GetPackageDependencies(PackageIdentity package,
                                          SourceCacheContext cacheContext,
                                          IEnumerable<SourceRepository> repositories,
                                          ISet<SourcePackageDependencyInfo> availablePackages)
        {
            if (availablePackages.Contains(package))
            {
                return;
            }

            var repos = repositories.ToList();
            foreach (var sourceRepository in repos)
            {
                var dependencyInfoResource = await sourceRepository.GetResourceAsync<DependencyInfoResource>();
                var dependencyInfo = await dependencyInfoResource.ResolvePackage(
                    package, currentFramework, cacheContext, logger, CancellationToken.None);

                if (dependencyInfo == null) continue;

                availablePackages.Add(dependencyInfo);
                foreach (var dependency in dependencyInfo.Dependencies)
                {
                    await GetPackageDependencies(
                        new PackageIdentity(dependency.Id, dependency.VersionRange.MinVersion), cacheContext, repos, availablePackages);
                }
            }
        }

        public bool PackageExists(string rootDirectory, string packageId)
        {
            return GetLatestPackageIdentity(packageId) != null;
        }

        public async Task<IPackageSearchMetadata> QueryPackageExactAsync(
            string repository, string packageId, string version = null, bool includePreRelease = false)
        {
            var matches = await QueryPackagesAsync(repository, packageId, version, includePreRelease);
            return matches.FirstOrDefault(d => d.Identity.Id.Equals(packageId));
        }

        public async Task<IEnumerable<IPackageSearchMetadata>> QueryPackagesAsync(
            string repository, string packageId, string version = null, bool includePreRelease = false)
        {
            return await QueryPackagesAsync(new[] { repository }, packageId, version, includePreRelease);
        }

        public async Task<IPackageSearchMetadata> QueryPackageExactAsync(
            IEnumerable<string> repositories, string packageId, string version = null, bool includePreRelease = false)
        {
            var matches = await QueryPackagesAsync(repositories, packageId, version, includePreRelease);
            return matches.FirstOrDefault(d => d.Identity.Id.Equals(packageId));
        }

        public async Task<IEnumerable<IPackageSearchMetadata>> QueryPackagesAsync(IEnumerable<string> repositories, string packageId, string version = null, bool includePreRelease = false)
        {
            var matches = new List<IPackageSearchMetadata>();

            foreach (var repo in repositories)
            {
                var packageSource = new PackageSource(repo);
                var sourceRepository = new SourceRepository(packageSource, providers);
                var searchResource = await sourceRepository.GetResourceAsync<PackageSearchResource>();
                var searchFilter = new SearchFilter(includePreRelease)
                {
                    IncludeDelisted = false,
                    SupportedFrameworks = new[] { currentFramework.DotNetFrameworkName }
                };

                var searchResult = await searchResource.SearchAsync(packageId, searchFilter, 0, 10, logger, CancellationToken.None);

                if (version == null)
                {
                    matches.AddRange(searchResult);
                    continue;
                }

                foreach (var packageMeta in searchResult)
                {
                    var versions = (await packageMeta.GetVersionsAsync()).ToList();
                    if (versions.Any(d => d.Version.OriginalVersion.Equals(version, StringComparison.OrdinalIgnoreCase)))
                    {
                        matches.Add(packageMeta);
                    }
                }
            }

            return matches;
        }

        public async Task<string> FindRepositoryForPackageAsync(IEnumerable<string> repositories, string packageId, string version = null, bool includePreReleases = false)
        {
            var repoList = repositories.ToList();

            foreach (var repository in repoList)
            {
                try
                {
                    var results = (await QueryPackagesAsync(repository, packageId, version, includePreReleases)).ToList();

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

        private readonly Dictionary<string, List<CachedAssembly>> loadedPackageAssemblies = new Dictionary<string, List<CachedAssembly>>();
        public virtual async Task<IEnumerable<Assembly>> LoadAssembliesFromNugetPackageAsync(string nupkgFile)
        {
            var fullPath = Path.GetFullPath(nupkgFile).ToLower();

            if (loadedPackageAssemblies.ContainsKey(fullPath))
            {
                return loadedPackageAssemblies[nupkgFile].Select(d => d.Assembly);
            }

            List<CachedAssembly> assemblies = new List<CachedAssembly>();

            var packageReader = new PackageArchiveReader(nupkgFile);
            var libItems = packageReader.GetLibItems().ToList();
            var nearest = frameworkReducer.GetNearest(currentFramework, libItems.Select(x => x.TargetFramework));

            foreach (var file in libItems.Where(x => x.TargetFramework.Equals(nearest)))
            {
                foreach (var item in file.Items)
                {
                    if (!item.EndsWith(".dll"))
                    {
                        continue;
                    }

                    var entry = packageReader.GetEntry(item);
                    using (var stream = entry.Open())
                    {
                        MemoryStream ms = new MemoryStream();
                        await stream.CopyToAsync(ms);

                        try
                        {
                            var asm = Assembly.Load(ms.ToArray());
                          
                            var name = GetVersionIndependentName(asm.FullName, out string extractedVersion);
                            var parsedVersion = new Version(extractedVersion);

                            assemblies.Add(new CachedAssembly
                            {
                                Assembly = asm,
                                AssemblyName = name,
                                Version = parsedVersion
                            });
                        }
                        catch (Exception ex)
                        {
                            logger.LogError("Failed to load assembly: " + item);
                            logger.LogError(ex.ToString());
                        }
                        finally
                        {
                            ms.Close();
                            stream.Close();
                        }
                    }
                }
            }

            loadedPackageAssemblies.Add(fullPath, assemblies);
            packageReader.Dispose();
            return assemblies.Select(d => d.Assembly);
        }

        public PackageIdentity GetLatestPackageIdentity(string packageId)
        {
            if (!Directory.Exists(packagesDirectory))
            {
                return null;
            }

            List<PackageIdentity> packageIdentities = new List<PackageIdentity>();
            foreach (var dir in Directory.GetDirectories(packagesDirectory))
            {
                var dirName = new DirectoryInfo(dir).Name;
                if (dirName.StartsWith(packageId + ".", StringComparison.OrdinalIgnoreCase))
                {
                    var directoryName = new DirectoryInfo(dir).Name;
                    var nupkgFile = Path.Combine(dir, directoryName + ".nupkg");
                    if (!File.Exists(nupkgFile))
                    {
                        return null;
                    }

                    using (var reader = new PackageArchiveReader(nupkgFile))
                    {
                        var identity = reader.NuspecReader.GetIdentity();

                        if (identity.Id.Equals(packageId))
                        {
                            packageIdentities.Add(identity);
                        }
                    }
                }
            }

            return packageIdentities.OrderByDescending(c => c.Version).FirstOrDefault();
        }

        public string GetNugetPackageFile(PackageIdentity identity)
        {
            var dir = packagePathResolver.GetInstallPath(identity);
            var dirName = new DirectoryInfo(dir).Name;

            return Path.Combine(dir, dirName + ".nupkg");
        }

        public async Task<IEnumerable<SourcePackageDependencyInfo>> GetDependenciesAsync(PackageIdentity identity, SourceCacheContext cacheContext)
        {
            var packageSourceProvider = new PackageSourceProvider(nugetSettings);
            var sourceRepositoryProvider = new SourceRepositoryProvider(packageSourceProvider, providers);

            var repositories = sourceRepositoryProvider.GetRepositories();
            var availablePackages = new HashSet<SourcePackageDependencyInfo>(PackageIdentityComparer.Default);
            await GetPackageDependencies(identity, cacheContext, repositories, availablePackages);

            var resolverContext = new PackageResolverContext(
                DependencyBehavior.Lowest,
                new[] { identity.Id },
                Enumerable.Empty<string>(),
                Enumerable.Empty<PackageReference>(),
                Enumerable.Empty<PackageIdentity>(),
                availablePackages,
                sourceRepositoryProvider.GetRepositories().Select(s => s.PackageSource),
                logger);

            return packageResolver.Resolve(resolverContext, CancellationToken.None).Select(p => availablePackages.Single(x => PackageIdentityComparer.Default.Equals(x, p)));
        }

        private void InstallAssemblyResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve += delegate (object sender, ResolveEventArgs args)
            {
                var name = GetVersionIndependentName(args.Name, out _);
                var matchingAssemblies = 
                    loadedPackageAssemblies.Values.SelectMany(d => d)
                        .Where(d => d.AssemblyName.Equals(name, StringComparison.OrdinalIgnoreCase))
                        .OrderByDescending(d => d.Version);

                return matchingAssemblies.FirstOrDefault()?.Assembly;
            };
        }

        private static readonly Regex versionRegex = new Regex("Version=(?<version>.+?), ", RegexOptions.Compiled);
        private static string GetVersionIndependentName(string fullAssemblyName, out string extractedVersion)
        {
            var match = versionRegex.Match(fullAssemblyName);
            extractedVersion = match.Groups[1].Value;
            return versionRegex.Replace(fullAssemblyName, "");
        }
    }

    public class CachedAssembly
    {
        public string AssemblyName { get; set; }
        public Version Version { get; set; }
        public Assembly Assembly { get; set; }
    }
}