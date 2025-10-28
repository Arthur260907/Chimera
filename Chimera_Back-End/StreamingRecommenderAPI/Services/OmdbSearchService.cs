// Localização: Services/OmdbMovieSearchService.cs
using StreamingRecommenderAPI.Models.Midia; // Corrigido: usar o namespace que contém OmdbMovie
using StreamingRecommenderAPI.Services.Interfaces; // Para ISearchService
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; // Para Enumerable.Empty<>()

namespace StreamingRecommenderAPI.Services // Namespace correto
{
    // Implementa a interface ISearchService
    public class OmdbSearchService : ISearchService // Nome pode ser OmdbSearchService ou OmdbMovieSearchService
    {
        // Dependência do OmdbService (que faz a chamada à API)
        private readonly OmdbService _omdbService;

        // Construtor para injeção de dependência
        public OmdbSearchService(OmdbService omdbService)
        {
            _omdbService = omdbService;
        }

        // Implementação do método da interface ISearchService
        public async Task<IEnumerable<OmdbMovie>> SearchAsync(string query)
        {
            // Chama o método do OmdbService que busca por título (s=)
            // sem especificar o tipo, para obter filmes E séries.
            // Assumindo que você criou/modificou SearchMediaByTitleAsync em OmdbService.
            var searchResult = await _omdbService.SearchMediaByTitleAsync(query);

            // Retorna a lista de resultados da busca (propriedade 'Search' do OmdbSearchResult)
            // Se searchResult for nulo ou a lista 'Search' for nula, retorna uma lista vazia.
            return searchResult?.Search ?? Enumerable.Empty<OmdbMovie>();
        }
    }
}