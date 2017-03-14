namespace PopcornApi.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class Anime
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Anime()
        {
            this.Episodes = new HashSet<EpisodeAnime>();
            this.Genres = new HashSet<Genre>();
        }
    
        public int Id { get; set; }
        public string MalId { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public string Slug { get; set; }
        public string Synopsis { get; set; }
        public string Runtime { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public long LastUpdated { get; set; }
        public int NumSeasons { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EpisodeAnime> Episodes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Genre> Genres { get; set; }
        public virtual CollectionImageAnime Images { get; set; }
        public virtual Rating Rating { get; set; }
    }
}
