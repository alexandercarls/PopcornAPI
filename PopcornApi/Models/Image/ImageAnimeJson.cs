using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PopcornApi.Models.Image
{
    public class ImageAnimeJson
    {
        [JsonProperty("poster_kitsu")]
        public ImageAnimeTypeJson Poster { get; set; }

        [JsonProperty("cover_kitsu")]
        public ImageAnimeTypeJson Cover { get; set; }
    }

    public class ImageAnimeTypeJson
    {
        [JsonProperty("tiny")]
        public string Tiny { get; set; }

        [JsonProperty("small")]
        public string Small { get; set; }

        [JsonProperty("medium")]
        public string Medium { get; set; }

        [JsonProperty("large")]
        public string Large { get; set; }

        [JsonProperty("original")]
        public string Original { get; set; }
    }
}
