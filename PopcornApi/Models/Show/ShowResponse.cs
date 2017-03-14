using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PopcornApi.Models.Show
{
    public class ShowResponse
    {
        [JsonProperty("totalShows")]
        public long TotalShows { get; set; }

        [JsonProperty("shows")]
        public IEnumerable<ShowJson> Shows { get; set; }
    }
}