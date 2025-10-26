// Caminho: Controllers/OmdbMoviesController.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StreamingRecommenderAPI.Data;
using StreamingRecommenderAPI.Models.Midia;
using StreamingRecommenderAPI.Services;
using StreamingRecommenderAPI.Models.DTOs; // Adicionado Using para DTO
using System.Globalization;              // Adicionado Using para CultureInfo

namespace StreamingRecommenderAPI.Controllers
{
    [ApiController]
    [Route("api/movies")]
    public class OmdbMoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly OmdbService _omdbService;
        private readonly TmdbService _tmdbService; // <<< INJETADO
        private readonly IHttpClientFactory _httpClientFactory;

        // Construtor modificado
        public OmdbMoviesController(
            ApplicationDbContext context,
            OmdbService omdbService,
            TmdbService tmdbService, // <<< INJETADO
            IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _omdbService = omdbService;
            _tmdbService = tmdbService; // <<< INJETADO
            _httpClientFactory = httpClientFactory;
        }

        // GET: api/movies/title/{title} (OMDB) - Mantido como está
        [HttpGet("title/{title}")]
        public async Task<ActionResult<OmdbMovie>> SearchMovie([FromRoute] string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return BadRequest("O título não pode ser vazio.");
            var movie = await _omdbService.GetMovieByTitleAsync(title);
            if (movie == null || movie.Response == "False") return NotFound(movie?.Error ?? "Filme não encontrado na OMDB.");
            return Ok(movie);
        }

        // GET: api/movies/byId/{imdbId} (OMDB + TMDB) - MÉTODO MODIFICADO
        [HttpGet("byId/{imdbId}")]
        public async Task<ActionResult<MovieDetailDto>> GetMovieById(string imdbId) // Retorna DTO
        {
            if (string.IsNullOrWhiteSpace(imdbId)) return BadRequest("O ID IMDb não pode ser vazio.");

            // 1. Busca OMDB
            var omdbMovie = await _omdbService.GetMovieByIdAsync(imdbId);
            if (omdbMovie == null || omdbMovie.Response == "False") return NotFound(omdbMovie?.Error ?? $"Filme com IMDb ID '{imdbId}' não encontrado na OMDB.");

            // 2. Busca TMDB
            string? bestPosterUrl = null;
            string? trailerUrl = null;
            int? tmdbId = await _tmdbService.FindMovieByImdbIdAsync(imdbId);

            if (tmdbId.HasValue)
            {
                // Imagens
                var images = await _tmdbService.GetMovieImagesAsync(tmdbId.Value);
                if (images?.Posters != null && images.Posters.Any())
                {
                    var bestPoster = images.Posters
                        .OrderBy(p => Math.Abs(p.AspectRatio - (2.0 / 3.0))) // Prioriza proporção 2:3
                        .ThenByDescending(p => string.IsNullOrEmpty(p.Language) || p.Language.Equals("en", StringComparison.OrdinalIgnoreCase))
                        .FirstOrDefault();
                    if (bestPoster?.FilePath != null)
                    {
                        bestPosterUrl = await _tmdbService.GetFullPosterUrlAsync(bestPoster.FilePath, "w500");
                    }
                }

                // Vídeos (Tenta idioma local, fallback inglês)
                string acceptLanguage = Request.Headers["Accept-Language"].ToString().Split(',').FirstOrDefault()?.Trim() ?? "pt-BR";
                var videos = await _tmdbService.GetMovieVideosAsync(tmdbId.Value, acceptLanguage);
                if (videos?.Results == null || !videos.Results.Any()) {
                    videos = await _tmdbService.GetMovieVideosAsync(tmdbId.Value, "en-US");
                }
                trailerUrl = _tmdbService.FindBestTrailerUrl(videos);
            }

            // 3. Monta DTO
            var movieDetailDto = new MovieDetailDto
            {
                ImdbID = omdbMovie.ImdbID,
                Title = omdbMovie.Title,
                Year = omdbMovie.Year,
                Rated = omdbMovie.Rated,
                Released = omdbMovie.Released,
                Runtime = omdbMovie.Runtime,
                Genre = omdbMovie.Genre,
                Director = omdbMovie.Director,
                Writer = omdbMovie.Writer,
                Actors = omdbMovie.Actors,
                Plot = omdbMovie.Plot,
                Language = omdbMovie.Language,
                Country = omdbMovie.Country,
                Awards = omdbMovie.Awards,
                ImdbRating = omdbMovie.ImdbRating,
                Ratings = omdbMovie.Ratings, // Lista de Ratings (RT, Metacritic)
                Type = omdbMovie.Type,
                OriginalPosterUrl = omdbMovie.Poster, // Poster original OMDB

                // Dados TMDB (com fallback para OMDB se TMDB falhar)
                BestPosterUrl = bestPosterUrl ?? await _tmdbService.GetFullPosterUrlAsync(omdbMovie.Poster) ?? omdbMovie.Poster,
                TrailerUrl = trailerUrl
            };

            // 4. Retorna DTO
            return Ok(movieDetailDto);
        }


        // GET: api/movies (Busca todos do DB Local) - Mantido
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OmdbMovie>>> GetFilmes()
        {
            return await _context.Filmes.ToListAsync();
        }

        // GET: api/movies/{id} (Busca por ID no DB Local) - Mantido
        [HttpGet("{id}")]
        public async Task<ActionResult<OmdbMovie>> GetOmdbMovie(string id)
        {
            var omdbMovie = await _context.Filmes.FindAsync(id);
            if (omdbMovie == null) return NotFound();
            return omdbMovie;
        }

        // PUT: api/movies/{id} - Mantido
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOmdbMovie(string id, OmdbMovie omdbMovie)
        {
            if (id != omdbMovie.ImdbID) return BadRequest();
            _context.Entry(omdbMovie).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException) { if (!OmdbMovieExists(id)) return NotFound(); else throw; }
            return NoContent();
        }

        // POST: api/movies - Mantido
        [HttpPost]
        public async Task<ActionResult<OmdbMovie>> PostOmdbMovie(OmdbMovie omdbMovie)
        {
            _context.Filmes.Add(omdbMovie);
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateException) { if (OmdbMovieExists(omdbMovie.ImdbID)) return Conflict(); else throw; }
            return CreatedAtAction("GetOmdbMovie", new { id = omdbMovie.ImdbID }, omdbMovie);
        }

        // DELETE: api/movies/{id} - Mantido
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOmdbMovie(string id)
        {
            var omdbMovie = await _context.Filmes.FindAsync(id);
            if (omdbMovie == null) return NotFound();
            _context.Filmes.Remove(omdbMovie);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/movies/poster - Mantido
        [HttpGet("poster")]
        public async Task<IActionResult> GetPoster([FromQuery] string url, CancellationToken cancellationToken = default)
        {
             if (string.IsNullOrWhiteSpace(url)) return BadRequest("url is required.");
             if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return BadRequest("invalid url.");
             try
             {
                 var client = _httpClientFactory.CreateClient();
                 client.DefaultRequestHeaders.UserAgent.ParseAdd("Chimera/1.0");
                 using var resp = await client.GetAsync(uri, cancellationToken);
                 if (!resp.IsSuccessStatusCode) return NotFound();
                 var contentType = resp.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";
                 var bytes = await resp.Content.ReadAsByteArrayAsync(cancellationToken);
                 return File(bytes, contentType);
             }
             catch (OperationCanceledException) { return StatusCode(499); }
             catch (Exception) { return StatusCode(500); }
        }

        private bool OmdbMovieExists(string id)
        {
            return _context.Filmes.Any(e => e.ImdbID == id);
        }

        // GET: api/movies/byGenre/{genre} (Busca por gênero no DB Local) - Mantido
        [HttpGet("byGenre/{genre}")]
        public async Task<ActionResult<IEnumerable<OmdbMovie>>> GetMoviesByGenre(string genre)
        {
            if (string.IsNullOrWhiteSpace(genre)) return BadRequest("O nome do gênero não pode ser vazio.");
            try
            {
                var movies = await _context.Filmes
                    .Where(m => m.Genre != null && m.Genre.Contains(genre, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(m => m.ImdbRating)
                    .Take(15)
                    .ToListAsync();
                if (movies == null || !movies.Any()) return NotFound($"Nenhum filme encontrado no banco de dados local para o gênero '{genre}'.");
                return Ok(movies);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar filmes por gênero '{genre}': {ex.Message}");
                return StatusCode(500, "Ocorreu um erro interno ao buscar filmes por gênero.");
            }
        }
    }
}