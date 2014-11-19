using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractWeiboData
{
    class WeiboData
    {
        [JsonProperty(PropertyName = "id")]
        public string Id {get;set;}
        [JsonProperty(PropertyName = "created_at")]
        public string CreatedAt { get; set; }
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
        //[JsonProperty(PropertyName = "pic_ids")]
        //public string PicIds { get; set; }
        [JsonProperty(PropertyName = "pic_count")]
        public string PicCount { get; set; }
        [JsonProperty(PropertyName = "lng")]
        public string Lng { get; set; }
        [JsonProperty(PropertyName = "lat")]
        public string Lat { get; set; }
        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; }
        [JsonProperty(PropertyName = "user_province")]
        public string UserProvince { get; set; }
        [JsonProperty(PropertyName = "user_city")]
        public string UserCity { get; set; }
        [JsonProperty(PropertyName = "annotations_id")]
        public string AnnotationsId { get; set; }
        [JsonProperty(PropertyName = "annotations_title")]
        public string AnnotationsTitle { get; set; }
        [JsonProperty(PropertyName = "annotations_lng")]
        public string AnnotationsLng { get; set; }
        [JsonProperty(PropertyName = "annotations_lat")]
        public string AnnotationsLat { get; set; }
    }
}
