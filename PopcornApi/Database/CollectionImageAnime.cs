namespace PopcornApi.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class CollectionImageAnime
    {
        public int Id { get; set; }
    
        public virtual ImageAnime Poster { get; set; }
        public virtual ImageAnime Cover { get; set; }
    }
}
