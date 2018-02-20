using System.Linq;
using System.Net;
using Newtonsoft.Json;

namespace NugetAnalyzer.Core.Models
{
    public sealed class Package
    {
        public Package(string id, string version, string targetFramework)
        {
            Id = id;
            Version = version;
            TargetFramework = targetFramework;
            PackageMetadata packageMetadata;
            bool gettingCatalogPages;
            using (var webClient = new WebClient())
            {
                packageMetadata = JsonConvert.DeserializeObject<PackageMetadata>(webClient.DownloadString($"https://api.nuget.org/v3/registration3/{id.ToLower()}/index.json"));
                gettingCatalogPages = true;
                if (packageMetadata.PackageItems.Any(p => p.PackageItemsVersion?.Any() != true))
                {
                    packageMetadata = JsonConvert.DeserializeObject<PackageMetadata>(webClient.DownloadString(packageMetadata.PackageItems.Last().Id));
                    gettingCatalogPages = false;
                }
            }
            var packageItemVersions = gettingCatalogPages
                 ? packageMetadata.PackageItems.SelectMany(p => p.PackageItemsVersion)
                 : packageMetadata.PackageItems;

            LastestVersion = packageItemVersions
                .Select(p => p.CatalogEntry)
                .Last(p => p.Listed)
                .Version;
            IsOutdated = LastestVersion > Version;
        }

        public string Id { get; }

        public Version Version { get; }

        public string TargetFramework { get; }

        public Version LastestVersion { get; }

        public bool IsOutdated { get; }

        private bool Equals(Package other)
            => string.Equals(Id, other.Id) &&
               Version.Equals(other.Version) &&
               string.Equals(TargetFramework, other.TargetFramework);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Package package && Equals(package);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Version.GetHashCode();
                hashCode = (hashCode * 397) ^ (TargetFramework != null ? TargetFramework.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
            => $"{Id} on {Version}, targeting {TargetFramework}";
    }
}