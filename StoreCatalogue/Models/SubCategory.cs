using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace StoreCatalogue.Models
{
    public class SubCategory
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "categoryId")]
        public Guid CategoryId { get; set; }

    }
}