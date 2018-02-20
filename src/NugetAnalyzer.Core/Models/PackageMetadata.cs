using Newtonsoft.Json;

namespace NugetAnalyzer.Core.Models
{
    public sealed class PackageMetadata
    {
        [JsonProperty("items")]
        public PackageItem[] PackageItems { get; set; }
    }
}