using JsonPropName = System.Text.Json.Serialization.JsonPropertyNameAttribute;
namespace sesto.api.Models
{
    public class FirebaseUser
    {

        [JsonPropName("identities")]
        public FirebaseIdentities Identities { get; set; }

        [JsonPropName("sign_in_provider")]
        public string SignInProvider { get; set; }
    }
}


