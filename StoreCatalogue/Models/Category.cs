using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace StoreCatalogue.Models
{
    public class Category
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

    }
}