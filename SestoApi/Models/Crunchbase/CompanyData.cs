using System.Collections.Generic;
using JsonPropName = System.Text.Json.Serialization.JsonPropertyNameAttribute;


namespace sesto.api.Models.Crunchbase
{
    public class CompanyData
    {
        [JsonPropName("paging")]
        public Paging Paging { get; set; }

        [JsonPropName("items")]
        public List<Item> Items { get; set; }
    }

}