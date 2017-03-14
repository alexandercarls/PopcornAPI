namespace PopcornApi.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class EpisodeAnime
    {
        public int Id { get; set; }
        public string Overview { get; set; }
        public string Title { get; set; }
        public int EpisodeNumber { get; set; }
        public int Season { get; set; }
        public string TvdbId { get; set; }
    
        public virtual TorrentNode Torrents { get; set; }
    }
}
