using Microsoft.AspNetCore.Mvc;
using StreamingRecommenderAPI.Models; // Namespace de OmdbMovie
using StreamingRecommenderAPI.Services; // Para Adaptador e talvez Composite
using StreamingRecommenderAPI.Services.Filters; // Para Decoradores
using StreamingRecommenderAPI.Services.Interfaces; // Para ISearchService, IFilterService
using System; // Para StringComparison e Exception
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StreamingRecommenderAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;
        // Ou injete CompositeSearchService se você o criou e registrou:
        // private readonly CompositeSearchService _searchService;

        // Ajuste o construtor dependendo do que você injetou no Program.cs
        public SearchController(ISearchService searchService)
        // public SearchController(CompositeSearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StreamingRecommenderAPI.Models.Midia.OmdbMovie>>> Search(
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

                // Retorna NotFound se a lista de resultados estiver vazia após filtros/busca
                if (!results.Any())
                {
                    return NotFound("Nenhum resultado encontrado para os critérios fornecidos.");
                }


                return Ok(results);
            }
            catch (Exception ex)
            {
                // TODO: Adicionar log do erro ex
                Console.WriteLine($"Erro na API de busca: {ex.Message}"); // Log simples
                return StatusCode(500, "Ocorreu um erro interno ao processar a busca.");
            }
        }
    }
}