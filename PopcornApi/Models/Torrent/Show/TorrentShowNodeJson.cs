using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PopcornApi.Models.Torrent.Show
{
    public class TorrentShowNodeJson
    {
        [JsonProperty("0")]
        public TorrentShowJson Torrent_0 { get; set; }

        [JsonProperty("480p")]
        public TorrentShowJson Torrent_480p { get; set; }

        [JsonProperty("720p")]
        public TorrentShowJson Torrent_720p { get; set; }

        [JsonProperty("1080p")]
        public TorrentShowJson Torrent_1080p { get; set; }
    }
}
