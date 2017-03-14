using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PopcornApi.Models.Rating
{
    public class RatingJson
    {
        [JsonProperty("percentage")]
        public int? Percentage { get; set; }

        [JsonProperty("watching")]
        public int? Watching { get; set; }

        [JsonProperty("votes")]
        public int? Votes { get; set; }

        [JsonProperty("loved")]
        public int? Loved { get; set; }

        [JsonProperty("hated")]
        public int? Hated { get; set; }
    }
}
