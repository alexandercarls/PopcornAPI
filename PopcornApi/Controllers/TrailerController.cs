using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PopcornApi.Services.Caching;
using PopcornApi.Services.Logging;
using YoutubeExtractor;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using PopcornApi.Models.Trailer;

namespace PopcornApi.Controllers
{
    [Route("api/[controller]")]
    public class TrailerController : Controller
    {
        /// <summary>
        /// The logging service
        /// </summary>
        private readonly ILoggingService _loggingService;

        /// <summary>
        /// The caching service
        /// </summary>
        private readonly ICachingService _cachingService;

        /// <summary>
        /// Movies
        /// </summary>
        /// <param name="loggingService">The logging service</param>
        /// <param name="cachingService">The caching service</param>
        public TrailerController(ILoggingService loggingService, ICachingService cachingService)
        {
            _loggingService = loggingService;
            _cachingService = cachingService;
        }

        /// <summary>
        /// Youtube quality
        /// </summary>
        public enum YoutubeStreamingQuality
        {
            Low = 0,
            Medium = 1,
            High = 2
        }

         /// <summary>
        /// Map for defining youtube video quality
        /// </summary>
        private static readonly IReadOnlyDictionary<YoutubeStreamingQuality, IEnumerable<int>>
            StreamingQualityMap =
                new Dictionary<YoutubeStreamingQuality, IEnumerable<int>>
                {
                    {YoutubeStreamingQuality.High, new HashSet<int> {1080, 720}},
                    {YoutubeStreamingQuality.Medium, new HashSet<int> {480}},
                    {YoutubeStreamingQuality.Low, new HashSet<int> {360, 240}}
                };

                        /// <summary>
        /// Get VideoInfo of a youtube video
        /// </summary>
        /// <param name="youtubeLink">The youtube link of a movie</param>
        /// <param name="qualitySetting">The youtube quality settings</param>
        /// <returns>The trailer's video info</returns>
        private async Task<VideoInfo> GetVideoInfoForStreamingAsync(string youtubeLink,
            YoutubeStreamingQuality qualitySetting)
        {
            IEnumerable<VideoInfo> videoInfos = new List<VideoInfo>();

            // Get video infos of the requested video
            await Task.Run(() => videoInfos = DownloadUrlResolver.GetDownloadUrls(youtubeLink, false));

            // We only want video matching criterias : only mp4 and no adaptive
            var filtered = videoInfos
                .Where(info => info.VideoType == VideoType.Mp4 && !info.Is3D && info.AdaptiveType == AdaptiveType.None);

            return GetVideoByStreamingQuality(filtered, qualitySetting);
        }

        /// <summary>
        /// Get youtube video depending of choosen quality settings
        /// </summary>
        /// <param name="videosToProcess">List of VideoInfo</param>
        /// <param name="quality">The youtube quality settings</param>
        /// <returns>The trailer's video info</returns>
        private VideoInfo GetVideoByStreamingQuality(IEnumerable<VideoInfo> videosToProcess,
            YoutubeStreamingQuality quality)
        {
            while (true)
            {
                var videos = videosToProcess.ToList(); // Prevent multiple enumeration

                if (quality == YoutubeStreamingQuality.High)
                    // Choose high quality Youtube video
                    return videos.OrderByDescending(x => x.Resolution).FirstOrDefault();

                // Pick the video with the requested quality settings
                var preferredResolutions = StreamingQualityMap[quality];

                var preferredVideos =
                    videos.Where(info => preferredResolutions.Contains(info.Resolution))
                        .OrderByDescending(info => info.Resolution);

                var video = preferredVideos.FirstOrDefault();

                if (video != null) return video;
                videosToProcess = videos;
                quality = (YoutubeStreamingQuality) (((int) quality) - 1);
            }
        }

        // GET api/trailer/q4Zd-LxoK1c
        [HttpGet("{ytTrailerCode}")]
        public async Task<IActionResult> Get(string ytTrailerCode)
        {
            var cachedTrailer = _cachingService.GetCache(ytTrailerCode);
            if (cachedTrailer == null)
            {
                var trailer = await GetVideoInfoForStreamingAsync("http://www.youtube.com/watch?v=" + ytTrailerCode, YoutubeStreamingQuality.High);
                if (trailer != null && trailer.RequiresDecryption)
                {
                    await Task.Run(() => DownloadUrlResolver.DecryptDownloadUrl(trailer));
                    var response = new TrailerResponse {TrailerUrl = trailer.DownloadUrl};
                    _cachingService.SetCache(ytTrailerCode, JsonConvert.SerializeObject(response));
                    return Json(response);
                }

                if (trailer != null && !trailer.RequiresDecryption)
                {
                    var response = new TrailerResponse { TrailerUrl = trailer.DownloadUrl };
                    _cachingService.SetCache(ytTrailerCode, JsonConvert.SerializeObject(response));
                    return Json(response);
                }

                return BadRequest();
            }

            return Json(JsonConvert.DeserializeObject<TrailerResponse>(cachedTrailer));
        }
    }
}