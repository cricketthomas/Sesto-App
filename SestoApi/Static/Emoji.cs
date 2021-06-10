using System;
using System.Text.Json.Serialization;

namespace sesto.api.Static
{
    public class Emoji
    {

        [JsonPropertyName("emoji_description")]
        public string Description { get; set; }

        [JsonPropertyName("emoji")]
        public string Character { get; set; }

        [JsonPropertyName("aliases")]
        public string[] Aliases { get; set; }

        [JsonPropertyName("tags")]
        public string[] Tags { get; set; }

    }
}
