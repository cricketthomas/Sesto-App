
using JsonPropName = System.Text.Json.Serialization.JsonPropertyNameAttribute;

namespace sesto.api.Models.Crunchbase
{
    public class Paging
    {
        [JsonPropName("total_items")]
        public int TotalItems { get; set; }

        [JsonPropName("number_of_pages")]
        public int NumberOfPages { get; set; }

        [JsonPropName("current_page")]
        public int CurrentPage { get; set; }

        [JsonPropName("sort_order")]
        public string SortOrder { get; set; }

        [JsonPropName("items_per_page")]
        public int ItemsPerPage { get; set; }

        [JsonPropName("next_page_url")]
        public object NextPageUrl { get; set; }

        [JsonPropName("prev_page_url")]
        public object PrevPageUrl { get; set; }

        [JsonPropName("key_set_url")]
        public object KeySetUrl { get; set; }

        [JsonPropName("collection_url")]
        public string CollectionUrl { get; set; }

        [JsonPropName("updated_since")]
        public object UpdatedSince { get; set; }
    }

}