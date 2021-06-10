using JsonPropName = System.Text.Json.Serialization.JsonPropertyNameAttribute;

namespace sesto.api.Models.Crunchbase
{
    public class Metadata
    {
        [JsonPropName("version")]
        public int Version { get; set; }

        [JsonPropName("www_path_prefix")]
        public string WwwPathPrefix { get; set; }

        [JsonPropName("api_path_prefix")]
        public string ApiPathPrefix { get; set; }

        [JsonPropName("image_path_prefix")]
        public string ImagePathPrefix { get; set; }
    }

}