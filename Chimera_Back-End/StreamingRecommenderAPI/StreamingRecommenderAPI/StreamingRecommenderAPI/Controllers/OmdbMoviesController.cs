using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StreamingRecommenderAPI.Data;
using StreamingRecommenderAPI.Models.Midia;

namespace StreamingRecommenderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OmdbMoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OmdbMoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/OmdbMovies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OmdbMovie>>> GetFilmes()
        {
            return await _context.Filmes.ToListAsync();
        }

        // GET: api/OmdbMovies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OmdbMovie>> GetOmdbMovie(string id)
        {
            var omdbMovie = await _context.Filmes.FindAsync(id);

            if (omdbMovie == null)
            {
                return NotFound();
            }

            return omdbMovie;
        }

        // PUT: api/OmdbMovies/5
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

        // POST: api/OmdbMovies
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

        // DELETE: api/OmdbMovies/5
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

        private bool OmdbMovieExists(string id)
        {
            return _context.Filmes.Any(e => e.ImdbID == id);
        }
    }
}
