namespace Rocket.Core.Plugins.NuGet
{
    public class NuGetInstallResult
    {
        public string Version { get; }

        public NuGetInstallResult(NuGetInstallCode code)
        {
            Code = code;
        }

        public NuGetInstallResult(string version)
        {
            Code = NuGetInstallCode.Success;
            Version = version;
        }

        public NuGetInstallCode Code { get; set; }

        public string InstalledVersion { get; set; }
    }

    public enum NuGetInstallCode
    {
        Success,
        VersionNotFound,
        PackageNotFound,
        RepositoryNotFound,
        MultipleMatch
    }
}