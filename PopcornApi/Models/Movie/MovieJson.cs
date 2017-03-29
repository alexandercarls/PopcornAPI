using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PopcornApi.Models.Cast;
using PopcornApi.Models.Torrent.Movie;

namespace PopcornApi.Models.Movie
{
    public class MovieJson
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("imdb_code")]
        public string ImdbCode { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("title_long")]
        public string TitleLong { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("rating")]
        public double Rating { get; set; }

        [JsonProperty("runtime")]
        public int Runtime { get; set; }

        [JsonProperty("genres")]
        public List<string> Genres { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("mpa_rating")]
        public string MpaRating { get; set; }

        [JsonProperty("download_count")]
        public int DownloadCount { get; set; }

        [JsonProperty("like_count")]
        public int LikeCount { get; set; }

        [JsonProperty("description_intro")]
        public string DescriptionIntro { get; set; }

        [JsonProperty("description_full")]
        public string DescriptionFull { get; set; }

        [JsonProperty("yt_trailer_code")]
        public string YtTrailerCode { get; set; }

        [JsonProperty("cast")]
        public List<CastJson> Cast { get; set; }

        [JsonProperty("torrents")]
        public List<TorrentMovieJson> Torrents { get; set; }

        [JsonProperty("date_uploaded")]
        public string DateUploaded { get; set; }

        [JsonProperty("date_uploaded_unix")]
        public int DateUploadedUnix { get; set; }

        [JsonProperty("poster_image")]
        public string PosterImage { get; set; }

        [JsonProperty("backdrop_image")]
        public string BackdropImage { get; set; }

        [JsonProperty("background_image")]
        public string BackgroundImage { get; set; }

        [JsonProperty("small_cover_image")]
        public string SmallCoverImage { get; set; }

        [JsonProperty("medium_cover_image")]
        public string MediumCoverImage { get; set; }

        [JsonProperty("large_cover_image")]
        public string LargeCoverImage { get; set; }

        [JsonProperty("medium_screenshot_image1")]
        public string MediumScreenshotImage1 { get; set; }

        [JsonProperty("medium_screenshot_image2")]
        public string MediumScreenshotImage2 { get; set; }

        [JsonProperty("medium_screenshot_image3")]
        public string MediumScreenshotImage3 { get; set; }

        [JsonProperty("large_screenshot_image1")]
        public string LargeScreenshotImage1 { get; set; }

        [JsonProperty("large_screenshot_image2")]
        public string LargeScreenshotImage2 { get; set; }

        [JsonProperty("large_screenshot_image3")]
        public string LargeScreenshotImage3 { get; set; }

        [JsonProperty("similar")]
        public List<string> Similar { get; set; }
    }
}
