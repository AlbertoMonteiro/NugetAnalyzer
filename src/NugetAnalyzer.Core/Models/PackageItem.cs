using Newtonsoft.Json;

namespace NugetAnalyzer.Core.Models
{
    public sealed class PackageItem : PackageItemVersion
    {
        [JsonProperty("items")]
        public PackageItemVersion[] PackageItemsVersion { get; set; }
        [JsonProperty("@id")]
        public string Id { get; set; }
    }

    public class PackageItemVersion
    {
        [JsonProperty("catalogEntry")]
        public CatalogEntry CatalogEntry { get; set; }
    }

    public sealed class CatalogEntry
    {
        [JsonProperty("listed")]
        public bool Listed { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }
    }
}