using JsonPropName = System.Text.Json.Serialization.JsonPropertyNameAttribute;

namespace sesto.api.Models.Crunchbase
{
    public class CrunchbaseRoot
    {
        [JsonPropName("metadata")]
        public Metadata Metadata { get; set; }

        [JsonPropName("data")]
        public CompanyData Data { get; set; }
    }

}