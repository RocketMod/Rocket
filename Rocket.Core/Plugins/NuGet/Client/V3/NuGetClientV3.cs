using RestSharp;
using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Rocket.Core.Plugins.NuGet.Client.V3
{
    public class NuGetClientV3
    {
        private readonly IDependencyContainer container;

        public NuGetClientV3(IDependencyContainer container)
        {
            this.container = container;
        }

        public string UserAgent { get; set; } = null;

        public NuGetRepository FetchRepository(string repositoryBaseUrl)
        {
            var client = GetRestClient(repositoryBaseUrl);
            var request = new RestRequest("index.json", Method.GET);
            var result = client.Execute<NuGetRepository>(request);

            if (result.ErrorException != null)
                throw new Exception("Fetch failed", result.ErrorException);

            var repo = result.Data;
            repo.BaseUrl = repositoryBaseUrl;
            return repo;
        }

        public IEnumerable<NuGetPackage> QueryPackages(NuGetRepository repository, NuGetQuery query)
        {
            var client = GetRestClient(repository.BaseUrl);

            RestRequest request = new RestRequest("query", Method.GET);
            if (query.PreRelease)
            {
                request.AddQueryParameter("prerelease", "true");
            }

            if (!string.IsNullOrEmpty(query.Name))
            {
                request.AddQueryParameter("q", query.Name);
            }

            if (!string.IsNullOrEmpty(query.Version))
            {
                request.AddQueryParameter("semVerLevel", query.Version);
            }

            var result = client.Execute<NuGetQueryResult>(request);

            if (result.ErrorException != null)
                throw result.ErrorException;

            return result.Data.Data;
        }

        public byte[] DownloadPackage(NuGetRepository repo, NuGetPackage package)
        {
            var currentVersion = package.Version;
            return DownloadPackage(repo, package.Versions.First(c => c.Version.Equals(currentVersion, System.StringComparison.OrdinalIgnoreCase)));
        }

        public byte[] DownloadPackage(NuGetRepository repo, NuGetPackageVersion version)
        {
            var isHttps = repo.BaseUrl.Contains("https");

            string url = version.Id.
                                 Replace("https://", "http://")
                                 .Replace(repo.BaseUrl.Replace("https://", "http://"), "");
            if (isHttps)
                url = url.Replace("http://", "https://");

            if (!url.Contains("http://") && !url.Contains("https://"))
            {
                url = repo.BaseUrl + url;
            }

            var client = GetRestClient(url);
            var request = new RestRequest(Method.GET);
            var result = client.Execute<NuGetPackageVersionRegistration>(request);

            if (result.ErrorException != null)
            {
                var response = Encoding.UTF8.GetString(result.RawBytes);

                var logger = container.Resolve<ILogger>();
                logger.LogDebug("Failed to parse response: ");
                logger.LogDebug(response);
                throw result.ErrorException;
            }

            var registration = result.Data;

            WebClient req = new WebClient();
            if (UserAgent != null)
                req.Headers.Add("user-agent", UserAgent);

            return req.DownloadData(registration.PackageContent);
        }

        private RestClient GetRestClient(string baseUrl = null)
        {
            RestClient client = baseUrl == null
                ? new RestClient()
                : new RestClient(baseUrl);

            if (UserAgent != null)
                client.UserAgent = UserAgent;
            return client;
        }
    }
}