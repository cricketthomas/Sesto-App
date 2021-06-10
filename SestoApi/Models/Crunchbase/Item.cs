using JsonPropName = System.Text.Json.Serialization.JsonPropertyNameAttribute;

namespace sesto.api.Models.Crunchbase
{

    public class Item
    {
        [JsonPropName("type")]
        public string Type { get; set; }

        [JsonPropName("uuid")]
        public string Uuid { get; set; }

        [JsonPropName("properties")]
        public Properties Properties { get; set; }
    }

}