
using JsonPropName = System.Text.Json.Serialization.JsonPropertyNameAttribute;
namespace sesto.api.Models.Crunchbase
{
    public class Properties
    {
        [JsonPropName("permalink")]
        public string Permalink { get; set; }

        [JsonPropName("api_path")]
        public string ApiPath { get; set; }

        [JsonPropName("web_path")]
        public string WebPath { get; set; }

        [JsonPropName("api_url")]
        public string ApiUrl { get; set; }

        [JsonPropName("name")]
        public string Name { get; set; }

        [JsonPropName("stock_exchange")]
        public object StockExchange { get; set; }

        [JsonPropName("stock_symbol")]
        public object StockSymbol { get; set; }

        [JsonPropName("primary_role")]
        public string PrimaryRole { get; set; }

        [JsonPropName("short_description")]
        public string ShortDescription { get; set; }

        [JsonPropName("profile_image_url")]
        public string ProfileImageUrl { get; set; }

        [JsonPropName("domain")]
        public string Domain { get; set; }

        [JsonPropName("homepage_url")]
        public string HomepageUrl { get; set; }

        [JsonPropName("facebook_url")]
        public string FacebookUrl { get; set; }

        [JsonPropName("twitter_url")]
        public string TwitterUrl { get; set; }

        [JsonPropName("linkedin_url")]
        public string LinkedinUrl { get; set; }

        [JsonPropName("city_name")]
        public string CityName { get; set; }

        [JsonPropName("region_name")]
        public string RegionName { get; set; }

        [JsonPropName("country_code")]
        public string CountryCode { get; set; }

        [JsonPropName("created_at")]
        public int CreatedAt { get; set; }

        [JsonPropName("updated_at")]
        public int UpdatedAt { get; set; }
    }

}