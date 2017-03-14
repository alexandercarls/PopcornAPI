using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PopcornApi.Models.Trailer
{
    public class TrailerResponse
    {
        [JsonProperty("trailer_url")]
        public string TrailerUrl { get; set; }
    }
}
