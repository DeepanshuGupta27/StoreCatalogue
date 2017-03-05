using Newtonsoft.Json;
using System;

namespace StoreCatalogue.Models
{
    public class Product
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "subCategoryId")]
        public Guid SubCategoryId { get; set; }
    }
}