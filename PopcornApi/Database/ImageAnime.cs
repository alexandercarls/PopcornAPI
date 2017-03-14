namespace PopcornApi.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class ImageAnime
    {
        public int Id { get; set; }
        public string Tiny { get; set; }
        public string Small { get; set; }
        public string Medium { get; set; }
        public string Large { get; set; }
        public string Original { get; set; }
    }
}
