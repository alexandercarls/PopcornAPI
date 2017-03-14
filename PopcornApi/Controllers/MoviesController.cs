using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;
using PopcornApi.Attributes;
using PopcornApi.Database;
using PopcornApi.Models.Cast;
using PopcornApi.Models.Movie;
using PopcornApi.Models.Torrent.Movie;
using PopcornApi.Services.Caching;
using PopcornApi.Services.Logging;

namespace PopcornApi.Controllers
{
    [Route("api/[controller]")]
    public class MoviesController : Controller
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
        public MoviesController(ILoggingService loggingService, ICachingService cachingService)
        {
            _loggingService = loggingService;
            _cachingService = cachingService;
        }

        // GET api/movies
        [HttpGet]
        public async Task<IActionResult> Get([RequiredFromQuery] int page, [FromQuery] int limit,
            [FromQuery] int minimum_rating, [FromQuery] string query_term,
            [FromQuery] string genre, [FromQuery] string sort_by)
        {
            var nbMoviesPerPage = 20;
            if (limit >= 20 && limit <= 50)
                nbMoviesPerPage = limit;

            var currentPage = 1;
            if (page >= 1)
            {
                currentPage = page;
            }

            using (var context = new PopcornContextFactory().Create(new DbContextFactoryOptions()))
            {
                var query =
                    context.MovieSet.Include(movie => movie.Torrents)
                        .Include(movie => movie.Cast)
                        .Include(movie => movie.Genres)
                        .AsQueryable();

                if (minimum_rating > 0 && minimum_rating < 10)
                {
                    query = query.Where(movie => movie.Rating > minimum_rating);
                }

                if (!string.IsNullOrWhiteSpace(query_term))
                {
                    query =
                        query.Where(
                            movie =>
                                movie.Title.ToLower().Contains(query_term.ToLower()));
                }

                if (!string.IsNullOrWhiteSpace(genre))
                {
                    query = query.Where(movie => movie.Genres.Any(a => a.Name.ToLower().Contains(genre.ToLower())));
                }

                if (!string.IsNullOrWhiteSpace(sort_by))
                {
                    switch (sort_by)
                    {
                        case "title":
                            query = query.OrderBy(movie => movie.Title);
                            break;
                        case "year":
                            query = query.OrderByDescending(movie => movie.Year);
                            break;
                        case "rating":
                            query = query.OrderByDescending(movie => movie.Rating);
                            break;
                        case "peers":
                            query =
                                query.OrderByDescending(
                                    movie => movie.Torrents.Max(torrent => torrent.Peers));
                            break;
                        case "seeds":
                            query =
                                query.OrderByDescending(
                                    movie => movie.Torrents.Max(torrent => torrent.Seeds));
                            break;
                        case "download_count":
                            query = query.OrderByDescending(movie => movie.DownloadCount);
                            break;
                        case "like_count":
                            query = query.OrderByDescending(movie => movie.LikeCount);
                            break;
                        case "date_added":
                            query = query.OrderByDescending(movie => movie.DateUploadedUnix);
                            break;
                    }
                }
                else
                {
                    query = query.OrderByDescending(movie => movie.DateUploadedUnix);
                }

                var count = query.Count();
                var skip = (currentPage - 1) * nbMoviesPerPage;
                if (count <= nbMoviesPerPage)
                {
                    skip = 0;
                }

                var movies = query.Skip(skip).Take(nbMoviesPerPage).ToList();
                return
                    Json(new MovieResponse
                    {
                        TotalMovies = count,
                        Movies = movies.Select(ConvertMovieToJson)
                    });
            }
        }

        // GET api/movies/tt3640424
        [HttpGet("{imdb}")]
        public async Task<IActionResult> Get(string imdb)
        {
            var cachedMovie = _cachingService.GetCache(imdb);
            if (cachedMovie == null)
            {
                using (var context = new PopcornContextFactory().Create(new DbContextFactoryOptions()))
                {
                    var movie =
                        context.MovieSet.Include(a => a.Torrents)
                            .Include(a => a.Cast)
                            .Include(a => a.Genres).AsQueryable()
                            .FirstOrDefault(
                                document => document.ImdbCode.ToLower() == imdb.ToLower());
                    if (movie == null) return BadRequest();

                    var movieJson = ConvertMovieToJson(movie);
                    _cachingService.SetCache(imdb, JsonConvert.SerializeObject(movieJson));
                    return Json(movieJson);
                }
            }

            return Json(JsonConvert.DeserializeObject<MovieJson>(cachedMovie));
        }

        /// <summary>
        /// Convert a <see cref="Movie"/> to a <see cref="MovieJson"/>
        /// </summary>
        /// <param name="movie"></param>
        /// <returns></returns>
        private MovieJson ConvertMovieToJson(Movie movie)
        {
            return new MovieJson
            {
                Rating = movie.Rating,
                Torrents = movie.Torrents.Select(torrent => new TorrentMovieJson
                {
                    DateUploadedUnix = torrent.DateUploadedUnix,
                    Peers = torrent.Peers,
                    Seeds = torrent.Seeds,
                    Quality = torrent.Quality,
                    Url = torrent.Url,
                    DateUploaded = torrent.DateUploaded,
                    Hash = torrent.Hash,
                    Size = torrent.Size,
                    SizeBytes = torrent.SizeBytes
                }).ToList(),
                Title = movie.Title,
                DateUploadedUnix = movie.DateUploadedUnix,
                Genres = movie.Genres.Select(genre => genre.Name).ToList(),
                Cast = movie.Cast.Select(cast => new CastJson
                {
                    CharacterName = cast.CharacterName,
                    Name = cast.Name,
                    ImdbCode = cast.ImdbCode,
                    SmallImage = cast.SmallImage
                }).ToList(),
                Runtime = movie.Runtime,
                Url = movie.Url,
                Year = movie.Year,
                Slug = movie.Slug,
                LikeCount = movie.LikeCount,
                DownloadCount = movie.DownloadCount,
                ImdbCode = movie.ImdbCode,
                DateUploaded = movie.DateUploaded,
                BackdropImage = movie.BackdropImage,
                BackgroundImage = movie.BackgroundImage,
                DescriptionFull = movie.DescriptionFull,
                DescriptionIntro = movie.DescriptionIntro,
                Language = movie.Language,
                LargeCoverImage = movie.LargeCoverImage,
                LargeScreenshotImage1 = movie.LargeScreenshotImage1,
                LargeScreenshotImage2 = movie.LargeScreenshotImage2,
                LargeScreenshotImage3 = movie.LargeScreenshotImage3,
                MediumCoverImage = movie.MediumCoverImage,
                MediumScreenshotImage1 = movie.MediumScreenshotImage1,
                MediumScreenshotImage2 = movie.MediumScreenshotImage2,
                MediumScreenshotImage3 = movie.MediumScreenshotImage3,
                MpaRating = movie.MpaRating,
                PosterImage = movie.PosterImage,
                SmallCoverImage = movie.SmallCoverImage,
                TitleLong = movie.TitleLong,
                YtTrailerCode = movie.YtTrailerCode
            };
        }
    }
}