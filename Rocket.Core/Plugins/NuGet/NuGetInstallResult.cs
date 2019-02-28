namespace Rocket.Core.Plugins.NuGet
{
    public enum NuGetInstallResult
    {
        Success,
        VersionNotFound,
        PackageNotFound,
        RepositoryNotFound,
        MultipleMatch
    }
}