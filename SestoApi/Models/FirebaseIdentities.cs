using System.Collections.Generic;
using JsonPropName = System.Text.Json.Serialization.JsonPropertyNameAttribute;

namespace sesto.api.Models
{
    public class FirebaseIdentities
    {
        [JsonPropName("email")]
        public List<string> Emails { get; set; }
    }
}

