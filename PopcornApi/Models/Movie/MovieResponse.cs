using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PopcornApi.Models.Movie
{
    public class MovieResponse
    {
        [JsonProperty("totalMovies")]
        public int TotalMovies { get; set; }

        [JsonProperty("movies")]
        public IEnumerable<MovieJson> Movies { get; set; }
    }
}