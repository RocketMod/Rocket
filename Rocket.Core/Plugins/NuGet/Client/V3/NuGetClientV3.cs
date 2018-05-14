using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using RestSharp;

namespace Rocket.Core.Plugins.NuGet.Client.V3
{
    public class NuGetClientV3
    {
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

        public IEnumerable<NuGetPackage> QueryPackages(NuGetRepository repository, NuGetQuery query = null)
        {
            var client = GetRestClient(repository.BaseUrl);
            var request = new RestRequest(query == null ? "query" : "query?q={QUERY}", Method.GET);
            if (query != null)
                request.AddParameter("QUERY", query.Name, ParameterType.UrlSegment);

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

            var client = GetRestClient(repo.BaseUrl);
            var request = new RestRequest(url, Method.GET);
            var result = client.Execute<NuGetPackageVersionRegistration>(request);

            if (result.ErrorException != null)
                throw result.ErrorException;

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