using Newtonsoft.Json;
using PopcornApi.Models.Torrent.Show;

namespace PopcornApi.Models.Episode
{
    public class EpisodeShowJson
    {
        [JsonProperty("torrents")]
        public TorrentShowNodeJson Torrents { get; set; }

        [JsonProperty("first_aired")]
        public long FirstAired { get; set; }

        [JsonProperty("date_based")]
        public bool DateBased { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("episode")]
        public int EpisodeNumber { get; set; }

        [JsonProperty("season")]
        public int Season { get; set; }

        [JsonProperty("tvdb_id")]
        public int? TvdbId { get; set; }
    }
}
