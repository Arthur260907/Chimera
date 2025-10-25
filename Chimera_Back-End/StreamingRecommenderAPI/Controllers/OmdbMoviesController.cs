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

namespace StreamingRecommenderAPI.Controllers
{
    [ApiController]
    [Route("api/movies")]
    public class OmdbMoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly OmdbService _omdbService;
        private readonly IHttpClientFactory _httpClientFactory;

        public OmdbMoviesController(ApplicationDbContext context, OmdbService omdbService, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _omdbService = omdbService;
            _httpClientFactory = httpClientFactory;
        }

        // --- NOVO ENDPOINT: Buscar filme da API OMDB pelo título ---
        // GET: api/movies/title/Inception
        [HttpGet("title/{title}")]
        public async Task<ActionResult<OmdbMovie>> SearchMovie([FromRoute] string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return BadRequest("O título não pode ser vazio.");

            var movie = await _omdbService.GetMovieByTitleAsync(title);

            if (movie == null || movie.Response == "False")
                return NotFound(movie?.Error ?? "Filme não encontrado na OMDB.");

            return Ok(movie);
        }

        // --- NOVO ENDPOINT: Buscar filme da API OMDB pelo ID ---
        // GET: api/movies/byId/tt0372784
        [HttpGet("byId/{imdbId}")]
        public async Task<ActionResult<OmdbMovie>> GetMovieById(string imdbId)
        {
            if (string.IsNullOrWhiteSpace(imdbId))
                return BadRequest("O ID não pode ser vazio.");

            var movie = await _omdbService.GetMovieByIdAsync(imdbId);

            if (movie == null || movie.Response == "False")
                return NotFound(movie?.Error ?? "Filme não encontrado na OMDB.");

            return Ok(movie);
        }

        // --- Seus endpoints atuais que usam o banco ainda ficam ---
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OmdbMovie>>> GetFilmes()
        {
            return await _context.Filmes.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OmdbMovie>> GetOmdbMovie(string id)
        {
            var omdbMovie = await _context.Filmes.FindAsync(id);

            if (omdbMovie == null)
                return NotFound();

            return omdbMovie;
        }

        // PUT: api/movies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOmdbMovie(string id, OmdbMovie omdbMovie)
        {
            if (id != omdbMovie.ImdbID)
            {
                return BadRequest();
            }

            _context.Entry(omdbMovie).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OmdbMovieExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/movies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OmdbMovie>> PostOmdbMovie(OmdbMovie omdbMovie)
        {
            _context.Filmes.Add(omdbMovie);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (OmdbMovieExists(omdbMovie.ImdbID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetOmdbMovie", new { id = omdbMovie.ImdbID }, omdbMovie);
        }

        // DELETE: api/movies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOmdbMovie(string id)
        {
            var omdbMovie = await _context.Filmes.FindAsync(id);
            if (omdbMovie == null)
            {
                return NotFound();
            }

            _context.Filmes.Remove(omdbMovie);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Proxy que retorna o conteúdo binário do poster indicado pela url.
        /// Ex: GET /api/movies/poster?url={encodedUrl}
        /// </summary>
        [HttpGet("poster")]
        public async Task<IActionResult> GetPoster([FromQuery] string url, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(url))
                return BadRequest("url is required.");

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return BadRequest("invalid url.");

            try
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Chimera/1.0");

                using var resp = await client.GetAsync(uri, cancellationToken);
                if (!resp.IsSuccessStatusCode)
                    return NotFound();

                var contentType = resp.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";
                var bytes = await resp.Content.ReadAsByteArrayAsync(cancellationToken);
                return File(bytes, contentType);
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499); // client closed request
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        private bool OmdbMovieExists(string id)
        {
            return _context.Filmes.Any(e => e.ImdbID == id);
        }
    }
}
