using System.Collections.Generic;
using Newtonsoft.Json;
using PopcornApi.Models.Episode;
using PopcornApi.Models.Image;
using PopcornApi.Models.Rating;

namespace PopcornApi.Models.Show
{
    public class ShowJson
    {
        [JsonProperty("imdb_id")]
        public string ImdbId { get; set; }

        [JsonProperty("tvdb_id")]
        public string TvdbId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("synopsis")]
        public string Synopsis { get; set; }

        [JsonProperty("runtime")]
        public string Runtime { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("network")]
        public string Network { get; set; }

        [JsonProperty("air_day")]
        public string AirDay { get; set; }

        [JsonProperty("air_time")]
        public string AirTime { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("num_seasons")]
        public int NumSeasons { get; set; }

        [JsonProperty("last_updated")]
        public long LastUpdated { get; set; }

        [JsonProperty("episodes")]
        public List<EpisodeShowJson> Episodes { get; set; }

        [JsonProperty("genres")]
        public IEnumerable<string> Genres { get; set; }

        [JsonProperty("images")]
        public ImageShowJson Images { get; set; }

        [JsonProperty("rating")]
        public RatingJson Rating { get; set; }

        [JsonProperty("similar")]
        public List<string> Similar { get; set; }
    }
}
