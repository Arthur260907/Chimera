using System.Collections.Generic;
using System.Threading.Tasks;
using StreamingRecommenderAPI.Models.Midia;
using StreamingRecommenderAPI.Services.Interfaces; // para ISearchService

namespace StreamingRecommenderAPI.Services
{
    /// <summary>
    /// Adapter que expõe um ISearchService como um IFilterService.
    /// </summary>
    public class SearchFilterAdapter : StreamingRecommenderAPI.Interfaces.IFilterService
    {
        private readonly ISearchService _searchService;

        // Construtor usado pelo SearchController: recebe o serviço de busca
        public SearchFilterAdapter(ISearchService searchService)
        {
            _searchService = searchService;
        }

        // Executa a busca via ISearchService e retorna os resultados
        public async Task<IEnumerable<OmdbMovie>> ExecuteAsync(string query)
        {
            if (_searchService == null) return await Task.FromResult<IEnumerable<OmdbMovie>>(new List<OmdbMovie>());
            return await _searchService.SearchAsync(query);
        }
    }
}
