using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PopcornApi.Models.Cast
{
    public class CastJson
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("character_name")]
        public string CharacterName { get; set; }

        [JsonProperty("url_small_image")]
        public string SmallImage { get; set; }

        [JsonProperty("imdb_code")]
        public string ImdbCode { get; set; }
    }
}
