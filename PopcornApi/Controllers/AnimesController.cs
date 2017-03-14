using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;
using PopcornApi.Attributes;
using PopcornApi.Database;
using PopcornApi.Models.Anime;
using PopcornApi.Models.Episode;
using PopcornApi.Models.Image;
using PopcornApi.Models.Rating;
using PopcornApi.Models.Torrent.Show;
using PopcornApi.Services.Caching;
using PopcornApi.Services.Logging;

namespace PopcornApi.Controllers
{
    [Route("api/[controller]")]
    public class AnimesController : Controller
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
        public AnimesController(ILoggingService loggingService, ICachingService cachingService)
        {
            _loggingService = loggingService;
            _cachingService = cachingService;
        }

        // GET api/animes
        [HttpGet]
        public async Task<IActionResult> Get([RequiredFromQuery] int page, [FromQuery] int limit,
            [FromQuery] int minimum_rating, [FromQuery] string query_term,
            [FromQuery] string genre, [FromQuery] string sort_by)
        {
            var nbAnimesPerPage = 20;
            if (limit >= 20 && limit <= 50)
                nbAnimesPerPage = limit;

            var currentPage = 1;
            if (page >= 1)
            {
                currentPage = page;
            }

            using (var context = new PopcornContextFactory().Create(new DbContextFactoryOptions()))
            {
                var query =
                    context.AnimeSet.Include(anime => anime.Rating)
                        .Include(anime => anime.Episodes)
                        .ThenInclude(episode => episode.Torrents)
                        .ThenInclude(torrent => torrent.Torrent0)
                        .Include(anime => anime.Episodes)
                        .ThenInclude(episode => episode.Torrents)
                        .ThenInclude(torrent => torrent.Torrent1080p)
                        .Include(anime => anime.Episodes)
                        .ThenInclude(episode => episode.Torrents)
                        .ThenInclude(torrent => torrent.Torrent480p)
                        .Include(anime => anime.Episodes)
                        .ThenInclude(episode => episode.Torrents)
                        .ThenInclude(torrent => torrent.Torrent720p)
                        .Include(anime => anime.Genres)
                        .Include(anime => anime.Images)
                        .AsQueryable();

                if (minimum_rating > 0 && minimum_rating < 100)
                {
                    query = query.Where(anime => anime.Rating.Percentage > minimum_rating);
                }

                if (!string.IsNullOrWhiteSpace(query_term))
                {
                    query =
                        query.Where(
                            anime =>
                                anime.Title.ToLower().Contains(query_term.ToLower()));
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
                            query = query.OrderBy(anime => anime.Title);
                            break;
                        case "year":
                            query = query.OrderByDescending(anime => anime.Year);
                            break;
                        case "rating":
                            query = query.OrderByDescending(anime => anime.Rating.Percentage);
                            break;
                        case "loved":
                            query = query.OrderByDescending(anime => anime.Rating.Loved);
                            break;
                        case "votes":
                            query = query.OrderByDescending(anime => anime.Rating.Votes);
                            break;
                        case "watching":
                            query = query.OrderByDescending(anime => anime.Rating.Watching);
                            break;
                        case "date_added":
                            query = query.OrderByDescending(anime => anime.LastUpdated);
                            break;
                    }
                }
                else
                {
                    query = query.OrderByDescending(movie => movie.LastUpdated);
                }

                var count = query.Count();
                var skip = (currentPage - 1) * nbAnimesPerPage;
                if (count <= nbAnimesPerPage)
                {
                    skip = 0;
                }

                var result = query.Skip(skip).Take(nbAnimesPerPage).ToList();

                return
                    Json(new AnimeResponse
                    {
                        TotalAnimes = count,
                        Animes = result.Select(ConvertAnimeToJson)
                    });
            }
        }

        // GET api/animes/tt3640424
        [HttpGet("{malid}")]
        public async Task<IActionResult> Get(string malid)
        {
            var cachedAnime = _cachingService.GetCache(malid);
            if (cachedAnime == null)
            {
                using (var context = new PopcornContextFactory().Create(new DbContextFactoryOptions()))
                {
                    var anime = context.AnimeSet.Include(a => a.Rating)
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
                        .Include(a => a.Images).AsQueryable().FirstOrDefault(a => a.MalId.ToLower() == malid.ToLower());
                    if (anime == null) return BadRequest();

                    var animeJson = ConvertAnimeToJson(anime);
                    _cachingService.SetCache(malid, JsonConvert.SerializeObject(animeJson));
                    return Json(animeJson);
                }
            }

            return Json(JsonConvert.DeserializeObject<AnimeJson>(cachedAnime));
        }

        /// <summary>
        /// Convert a <see cref="Anime"/> to a <see cref="AnimeJson"/>
        /// </summary>
        /// <param name="anime"></param>
        /// <returns></returns>
        private AnimeJson ConvertAnimeToJson(Anime anime)
        {
            return new AnimeJson
            {
                Episodes = anime.Episodes.Select(episode => new EpisodeAnimeJson
                {
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
                    EpisodeNumber = episode.EpisodeNumber,
                    Overview = episode.Overview,
                    Title = episode.Title,
                    TvdbId = episode.TvdbId,
                    Season = episode.Season
                }).ToList(),
                Rating = new RatingJson
                {
                    Hated = anime.Rating?.Hated,
                    Loved = anime.Rating?.Loved,
                    Percentage = anime.Rating?.Percentage,
                    Votes = anime.Rating?.Votes,
                    Watching = anime.Rating?.Watching
                },
                Title = anime.Title,
                Genres = anime.Genres.Select(genre => genre.Name),
                Year = anime.Year,
                Images = new ImageAnimeJson
                {
                    Poster = new ImageAnimeTypeJson
                    {
                        Large = anime.Images?.Poster?.Large,
                        Medium = anime.Images?.Poster?.Medium,
                        Original = anime.Images?.Poster?.Original,
                        Small = anime.Images?.Poster?.Small,
                        Tiny = anime.Images?.Poster?.Tiny
                    },
                    Cover = new ImageAnimeTypeJson
                    {
                        Large = anime.Images?.Cover?.Large,
                        Medium = anime.Images?.Cover?.Medium,
                        Original = anime.Images?.Cover?.Original,
                        Small = anime.Images?.Cover?.Small,
                        Tiny = anime.Images?.Cover?.Tiny
                    }
                },
                LastUpdated = anime.LastUpdated,
                MalId = anime.MalId,
                NumSeasons = anime.NumSeasons,
                Runtime = anime.Runtime,
                Slug = anime.Slug,
                Status = anime.Status,
                Synopsis = anime.Synopsis,
                Type = anime.Type
            };
        }
    }
}