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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        public NuGetInstaller(ILogger logger, string packagesDirectory)
        {
            this.logger = logger;
            this.packagesDirectory = packagesDirectory;
            providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3()); // Add v3 API support

            nugetSettings = Settings.LoadDefaultSettings(packagesDirectory, null, null);

            var frameworkName = Assembly.GetExecutingAssembly().GetCustomAttributes(true)
                                           .OfType<System.Runtime.Versioning.TargetFrameworkAttribute>()
                                           .Select(x => x.FrameworkName)
                                           .FirstOrDefault();
            currentFramework = frameworkName == null
                ? NuGetFramework.AnyFramework
                : NuGetFramework.ParseFrameworkName(frameworkName, new DefaultFrameworkNameProvider());
        }

        public async Task<NuGetInstallResult> InstallAsync(string repo, PackageIdentity packageIdentity, NuGetActionType action = NuGetActionType.Update, bool allowPrereleaseVersions = false)
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

            INuGetProjectContext projectContext = new EmptyNuGetProjectContext
            {
                PackageExtractionContext = new PackageExtractionContext(
                         PackageSaveMode.Nupkg,
                         XmlDocFileSaveMode.Skip,
                         ClientPolicyContext.GetClientPolicy(nugetSettings, logger),
                         logger),
                ActionType = action
            };

            ResolutionContext resolutionContext = new ResolutionContext(
                DependencyBehavior.Lowest,
                allowPrereleaseVersions,
                false,
                VersionConstraints.None);

            await packageManager.InstallPackageAsync(
                project,
                packageIdentity,
                resolutionContext,
                projectContext,
                sourceRepository,
                Array.Empty<SourceRepository>(),
                CancellationToken.None);

            //var dependencies = GetDependencies(packageIdentity).ToList();

            //List<NuGetInstallResult> installedDependencies = new List<NuGetInstallResult>();
            //List<PackageIdentity> packagesToRemove = new List<PackageIdentity>();

            //foreach (var dependency in dependencies)
            //{
            //    var result = await InstallAsync(repo, dependency, allowPrereleaseVersions);
            //    if (result.Code != NuGetInstallCode.Success)
            //    {
            //        logger.LogError($"Failed to install dependency {dependency.Id} v{dependency.Version}: " + result.Code);
            //        RollbackInstallation(packageIdentity, installedDependencies.Select(d => d.Identity));
            //        return new NuGetInstallResult(NuGetInstallCode.DependencyFail);
            //    }

            //    //todo: remove older dependencies
            //    installedDependencies.Add(result);
            //}

            //foreach (var package in packagesToRemove)
            //{
            //    DeletePackage(package);
            //}

            return new NuGetInstallResult(packageIdentity);
        }

        public bool PackageExists(string rootDirectory, string packageId)
        {
            return File.Exists(GetNugetPackageFile(rootDirectory, packageId));
        }

        public async Task<QueriedNuGetPackage> QueryPackageExactAsync(
            string repository, string packageId, string version = null, bool includePreRelease = false)
        {
            var matches = await QueryPackagesAsync(repository, packageId, version, includePreRelease);
            return matches.FirstOrDefault(d => d.Metadata.Identity.Id.Equals(packageId));
        }

        public async Task<IEnumerable<QueriedNuGetPackage>> QueryPackagesAsync(
            string repository, string packageId, string version = null, bool includePreRelease = false)
        {
            return await QueryPackagesAsync(new[] { repository }, packageId, version, includePreRelease);
        }

        public async Task<QueriedNuGetPackage> QueryPackageExactAsync(
            IEnumerable<string> repositories, string packageId, string version = null, bool includePreRelease = false)
        {
            var matches = await QueryPackagesAsync(repositories, packageId, version, includePreRelease);
            return matches.FirstOrDefault(d => d.Metadata.Identity.Id.Equals(packageId));
        }

        public async Task<IEnumerable<QueriedNuGetPackage>> QueryPackagesAsync(IEnumerable<string> repositories, string packageId, string version = null, bool includePreRelease = false)
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
                    SupportedFrameworks = new[] { currentFramework.DotNetFrameworkName }
                };

                var searchResult = await searchResource.SearchAsync(packageId, searchFilter, 0, 10, logger, CancellationToken.None);

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
                    var results = (await QueryPackagesAsync(repository, packageName, version, includePreReleases)).ToList();

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
                List<ZipEntry> assemblyEntries = new List<ZipEntry>();
                List<NuGetFramework> frameworks = new List<NuGetFramework>();

                foreach (ZipEntry entry in zf)
                {
                    if (entry.Name.StartsWith("lib/", StringComparison.OrdinalIgnoreCase) && entry.Name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                    {
                        var parts = entry.Name.Split('/');
                        var frameworkName = parts[1];
                        var framework = NuGetFramework.ParseFolder(frameworkName, new DefaultFrameworkNameProvider());

                        if (NuGetFrameworkUtility.IsCompatibleWithFallbackCheck(currentFramework, framework))
                        {
                            frameworks.Add(framework);
                        }
                    }
                }

                var targetFramework = frameworks.OrderByDescending(d => d.Version).FirstOrDefault();
                if (targetFramework == null)
                {
                    logger.LogWarning("No compatible framework found in: " + nugetPackageFile);
                    return allAssemblies;
                }

                foreach (ZipEntry entry in zf)
                {
                    string frameworkPrefix = "lib/" + targetFramework.GetShortFolderName() + "/";

                    if (!entry.Name.StartsWith(frameworkPrefix, StringComparison.OrdinalIgnoreCase)
                        || !entry.Name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    assemblyEntries.Add(entry);
                }

                if (assemblyEntries.Count == 0)
                {
                    logger.LogWarning("No assemblies found in: " + nugetPackageFile);
                    return allAssemblies;
                }

                foreach (var ze in assemblyEntries)
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

        public string GetNugetPackageFile(string rootDirectory, string packageId)
        {
            if (!Directory.Exists(rootDirectory))
            {
                return null;
            }

            foreach (var dir in Directory.GetDirectories(rootDirectory))
            {
                var dirName = new DirectoryInfo(dir).Name;
                if (dirName.ToLower().StartsWith(packageId.ToLower() + "."))
                {
                    return Path.Combine(dir, dirName + ".nupkg");
                }
            }

            return null;
        }

        private IEnumerable<PackageIdentity> GetDependencies(PackageIdentity identity)
        {
            var resolver = new PackageResolver();

            var context = new PackageResolverContext(
                DependencyBehavior.Lowest,
                new[] { identity.Id },
                Enumerable.Empty<string>(),
                Enumerable.Empty<PackageReference>(),
                new[] { identity },
                Enumerable.Empty<SourcePackageDependencyInfo>(),
                Enumerable.Empty<PackageSource>(),
                logger
            );

            return resolver.Resolve(context, CancellationToken.None);
        }

        private void RollbackInstallation(PackageIdentity packageIdentity, IEnumerable<PackageIdentity> dependencies)
        {
            DeletePackage(packageIdentity);

            foreach (var dep in dependencies)
            {
                DeletePackage(dep);
            }
        }

        private void DeletePackage(PackageIdentity package)
        {
            var packagePath = Path.Combine(packagesDirectory, $"{package.Id}.{package.Version.OriginalVersion}");
            if (Directory.Exists(packagePath))
            {
                logger.LogWarning("Path not found: " + packagePath);
                return;
            }

            Directory.Delete(packagePath, true);
        }
    }
}