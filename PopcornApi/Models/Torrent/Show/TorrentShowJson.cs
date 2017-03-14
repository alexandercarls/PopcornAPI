using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PopcornApi.Models.Torrent.Show
{
    public class TorrentShowJson
    {
        [JsonProperty("provider")]
        public string Provider { get; set; }

        [JsonProperty("peers")]
        public int? Peers { get; set; }

        [JsonProperty("seeds")]
        public int? Seeds { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
