using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;
using PopcornApi.Attributes;
using PopcornApi.Database;
using PopcornApi.Models.Episode;
using PopcornApi.Models.Image;
using PopcornApi.Models.Rating;
using PopcornApi.Models.Show;
using PopcornApi.Models.Torrent.Show;
using PopcornApi.Services.Caching;
using PopcornApi.Services.Logging;
using Microsoft.AspNetCore.Cors;

namespace PopcornApi.Controllers
{
    [Route("api/[controller]")]
    public class ShowsController : Controller
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
        public ShowsController(ILoggingService loggingService, ICachingService cachingService)
        {
            _loggingService = loggingService;
            _cachingService = cachingService;
        }

        // GET api/shows
        [HttpGet]
        public async Task<IActionResult> Get([RequiredFromQuery] int page, [FromQuery] int limit,
            [FromQuery] int minimum_rating, [FromQuery] string query_term,
            [FromQuery] string genre, [FromQuery] string sort_by)
        {
            var nbShowsPerPage = 20;
            if (limit >= 20 && limit <= 50)
                nbShowsPerPage = limit;

            var currentPage = 1;
            if (page >= 1)
            {
                currentPage = page;
            }

            using (var context = new PopcornContextFactory().Create(new DbContextFactoryOptions()))
            {
                var query = context.ShowSet.Include(show => show.Rating)
                    .Include(show => show.Episodes)
                    .ThenInclude(episode => episode.Torrents)
                    .ThenInclude(torrent => torrent.Torrent0)
                    .Include(show => show.Episodes)
                    .ThenInclude(episode => episode.Torrents)
                    .ThenInclude(torrent => torrent.Torrent1080p)
                    .Include(show => show.Episodes)
                    .ThenInclude(episode => episode.Torrents)
                    .ThenInclude(torrent => torrent.Torrent480p)
                    .Include(show => show.Episodes)
                    .ThenInclude(episode => episode.Torrents)
                    .ThenInclude(torrent => torrent.Torrent720p)
                    .Include(show => show.Genres)
                    .Include(show => show.Images).AsQueryable();

                if (minimum_rating > 0 && minimum_rating < 100)
                {
                    query = query.Where(show => show.Rating.Percentage > minimum_rating);
                }

                if (!string.IsNullOrWhiteSpace(query_term))
                {
                    query =
                        query.Where(
                            show =>
                                show.Title.ToLower().Contains(query_term.ToLower()));
                }

                if (!string.IsNullOrWhiteSpace(genre))
                {
                    query = query.Where(show => show.Genres.Any(a => a.Name.ToLower().Contains(genre.ToLower())));
                }

                if (!string.IsNullOrWhiteSpace(sort_by))
                {
                    switch (sort_by)
                    {
                        case "title":
                            query = query.OrderBy(show => show.Title);
                            break;
                        case "year":
                            query = query.OrderByDescending(show => show.Year);
                            break;
                        case "rating":
                            query = query.OrderByDescending(show => show.Rating.Percentage);
                            break;
                        case "loved":
                            query = query.OrderByDescending(show => show.Rating.Loved);
                            break;
                        case "votes":
                            query = query.OrderByDescending(show => show.Rating.Votes);
                            break;
                        case "watching":
                            query = query.OrderByDescending(show => show.Rating.Watching);
                            break;
                        case "date_added":
                            query = query.OrderByDescending(show => show.LastUpdated);
                            break;
                    }
                }
                else
                {
                    query = query.OrderByDescending(movie => movie.LastUpdated);
                }

                var count = query.Count();
                var skip = (currentPage - 1) * nbShowsPerPage;
                if (count <= nbShowsPerPage)
                {
                    skip = 0;
                }

                var result = query.Skip(skip).Take(nbShowsPerPage).ToList();

                return
                    Json(new ShowResponse
                    {
                        TotalShows = count,
                        Shows = result.Select(ConvertShowToJson)
                    });
            }
        }

        // GET api/shows/tt3640424
        [HttpGet("{imdb}")]
        public async Task<IActionResult> Get(string imdb)
        {
            var cachedShow = _cachingService.GetCache(imdb);
            if (cachedShow == null)
            {
                using (var context = new PopcornContextFactory().Create(new DbContextFactoryOptions()))
                {
                    var show = context.ShowSet.Include(a => a.Rating)
                        .Include(a => a.Episodes)
                        .ThenInclude(episode => episode.Torrents)
                        .ThenInclude(torrent => torrent.Torrent0)
                        .Include(a => a.Episodes)
                        .ThenInclude(episode => episode.Torrents)
                        .ThenInclude(torrent => torrent.Torrent1080p)
                        .Include(a => a.Episodes)
                        .ThenInclude(episode => episode.Torrents)
                        .ThenInclude(torrent => torrent.Torrent480p)
                        .Include(a => a.Episodes)
                        .ThenInclude(episode => episode.Torrents)
                        .ThenInclude(torrent => torrent.Torrent720p)
                        .Include(a => a.Genres)
                        .Include(a => a.Images).AsQueryable()
                        .FirstOrDefault(a => a.ImdbId.ToLower() == imdb.ToLower());
                    if (show == null) return BadRequest();

                    var showJson = ConvertShowToJson(show);
                    _cachingService.SetCache(imdb, JsonConvert.SerializeObject(showJson));
                    return Json(showJson);
                }
            }

            return Json(JsonConvert.DeserializeObject<ShowJson>(cachedShow));
        }

        /// <summary>
        /// Convert a <see cref="Show"/> to a <see cref="ShowJson"/>
        /// </summary>
        /// <param name="show"></param>
        /// <returns></returns>
        private ShowJson ConvertShowToJson(Show show)
        {
            return new ShowJson
            {
                AirDay = show.AirDay,
                Rating = new RatingJson
                {
                    Hated = show.Rating?.Hated,
                    Loved = show.Rating?.Loved,
                    Percentage = show.Rating?.Percentage,
                    Votes = show.Rating?.Votes,
                    Watching = show.Rating?.Watching
                },
                Title = show.Title,
                Genres = show.Genres.Select(genre => genre.Name),
                Year = show.Year,
                ImdbId = show.ImdbId,
                Episodes = show.Episodes.Select(episode => new EpisodeShowJson
                {
                    DateBased = episode.DateBased,
                    EpisodeNumber = episode.EpisodeNumber,
                    Torrents = new TorrentShowNodeJson
                    {
                        Torrent_0 = new TorrentShowJson
                        {
                            Peers = episode.Torrents?.Torrent0?.Peers,
                            Seeds = episode.Torrents?.Torrent0?.Seeds,
                            Provider = episode.Torrents?.Torrent0?.Provider,
                            Url = episode.Torrents?.Torrent0?.Url
                        },
                        Torrent_1080p = new TorrentShowJson
                        {
                            Peers = episode.Torrents?.Torrent1080p?.Peers,
                            Seeds = episode.Torrents?.Torrent1080p?.Seeds,
                            Provider = episode.Torrents?.Torrent1080p?.Provider,
                            Url = episode.Torrents?.Torrent1080p?.Url
                        },
                        Torrent_720p = new TorrentShowJson
                        {
                            Peers = episode.Torrents?.Torrent720p?.Peers,
                            Seeds = episode.Torrents?.Torrent720p?.Seeds,
                            Provider = episode.Torrents?.Torrent720p?.Provider,
                            Url = episode.Torrents?.Torrent720p?.Url
                        },
                        Torrent_480p = new TorrentShowJson
                        {
                            Peers = episode.Torrents?.Torrent480p?.Peers,
                            Seeds = episode.Torrents?.Torrent480p?.Seeds,
                            Provider = episode.Torrents?.Torrent480p?.Provider,
                            Url = episode.Torrents?.Torrent480p?.Url
                        }
                    },
                    FirstAired = episode.FirstAired,
                    Title = episode.Title,
                    Overview = episode.Overview,
                    Season = episode.Season,
                    TvdbId = episode.TvdbId
                }).ToList(),
                TvdbId = show.TvdbId,
                AirTime = show.AirTime,
                Country = show.Country,
                Images = new ImageShowJson
                {
                    Banner = show.Images?.Banner,
                    Fanart = show.Images?.Fanart,
                    Poster = show.Images?.Poster
                },
                LastUpdated = show.LastUpdated,
                Network = show.Network,
                NumSeasons = show.NumSeasons,
                Runtime = show.Runtime,
                Slug = show.Slug,
                Status = show.Status,
                Synopsis = show.Synopsis
            };
        }
    }
}