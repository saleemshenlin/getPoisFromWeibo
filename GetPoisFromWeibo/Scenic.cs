using NetDimension.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPoisFromWeibo
{
    public class Scenic
    {
        [JsonProperty(PropertyName = "poiid")]
        public string PoiID { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "lon")]
        public string Lng { get; set; }
        [JsonProperty(PropertyName = "lat")]
        public string Lat { get; set; }
        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }
        [JsonProperty(PropertyName = "category_name")]
        public string CategoryName { get; set; }
        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }

    }
}
