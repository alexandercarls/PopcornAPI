using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PopcornApi.Models.Torrent.Movie
{
    public class TorrentMovieJson
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("quality")]
        public string Quality { get; set; }

        [JsonProperty("seeds")]
        public int Seeds { get; set; }

        [JsonProperty("peers")]
        public int Peers { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("size_bytes")]
        public long? SizeBytes { get; set; }

        [JsonProperty("date_uploaded")]
        public string DateUploaded { get; set; }

        [JsonProperty("date_uploaded_unix")]
        public int DateUploadedUnix { get; set; }
    }
}
