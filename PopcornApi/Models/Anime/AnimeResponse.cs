using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PopcornApi.Models.Anime
{
    public class AnimeResponse
    {
        [JsonProperty("totalAnimes")]
        public long TotalAnimes { get; set; }

        [JsonProperty("animes")]
        public IEnumerable<AnimeJson> Animes { get; set; }
    }
}