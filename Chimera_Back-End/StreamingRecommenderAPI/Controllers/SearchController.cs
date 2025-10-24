using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// usar namespaces das implementações que ajustamos
using StreamingRecommenderAPI.Models.Midia;           // OmdbMovie
using StreamingRecommenderAPI.Services;               // SearchFilterAdapter, ISearchService implementation
using StreamingRecommenderAPI.Filters;                // TypeFilterDecorator, MinYearFilterDecorator
using StreamingRecommenderAPI.Interfaces;             // IFilterService

namespace StreamingRecommenderAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly StreamingRecommenderAPI.Services.Interfaces.ISearchService _searchService;

        public SearchController(StreamingRecommenderAPI.Services.Interfaces.ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OmdbMovie>>> Search(
            [FromQuery] string q,
            [FromQuery] string? type, // "movie" ou "series"
            [FromQuery] int? minYear)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return BadRequest("O parâmetro de busca 'q' é obrigatório.");
            }

            try
            {
                // usar IFilterService do namespace StreamingRecommenderAPI.Interfaces
                IFilterService filterChain = new SearchFilterAdapter(_searchService);

                if (!string.IsNullOrEmpty(type) &&
                    (type.Equals("movie", StringComparison.OrdinalIgnoreCase) ||
                     type.Equals("series", StringComparison.OrdinalIgnoreCase)))
                {
                    filterChain = new TypeFilterDecorator(filterChain, type);
                }

                if (minYear.HasValue)
                {
                    filterChain = new MinYearFilterDecorator(filterChain, minYear.Value);
                }

                var results = await filterChain.ExecuteAsync(q);

                if (results == null || !System.Linq.Enumerable.Any(results))
                {
                    return NotFound("Nenhum resultado encontrado para os critérios fornecidos.");
                }

                return Ok(results);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na API de busca: {ex.Message}");
                return StatusCode(500, "Ocorreu um erro interno ao processar a busca.");
            }
        }
    }
}