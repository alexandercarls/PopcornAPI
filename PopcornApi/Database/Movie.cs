namespace PopcornApi.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class Movie
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Movie()
        {
            this.Genres = new HashSet<Genre>();
            this.Cast = new HashSet<Cast>();
            this.Torrents = new HashSet<TorrentMovie>();
        }
    
        public int Id { get; set; }
        public string Url { get; set; }
        public string ImdbCode { get; set; }
        public string Title { get; set; }
        public string TitleLong { get; set; }
        public string Slug { get; set; }
        public int Year { get; set; }
        public double Rating { get; set; }
        public string Language { get; set; }
        public string MpaRating { get; set; }
        public int DownloadCount { get; set; }
        public int LikeCount { get; set; }
        public int Runtime { get; set; }
        public string DescriptionIntro { get; set; }
        public string DescriptionFull { get; set; }
        public string YtTrailerCode { get; set; }
        public string DateUploaded { get; set; }
        public int DateUploadedUnix { get; set; }
        public string PosterImage { get; set; }
        public string BackdropImage { get; set; }
        public string BackgroundImage { get; set; }
        public string SmallCoverImage { get; set; }
        public string MediumCoverImage { get; set; }
        public string LargeCoverImage { get; set; }
        public string MediumScreenshotImage1 { get; set; }
        public string MediumScreenshotImage2 { get; set; }
        public string MediumScreenshotImage3 { get; set; }
        public string LargeScreenshotImage1 { get; set; }
        public string LargeScreenshotImage2 { get; set; }
        public string LargeScreenshotImage3 { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Genre> Genres { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cast> Cast { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TorrentMovie> Torrents { get; set; }
    }
}
