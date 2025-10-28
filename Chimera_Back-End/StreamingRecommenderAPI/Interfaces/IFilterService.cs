using System.Collections.Generic;
using System.Threading.Tasks;
using StreamingRecommenderAPI.Models.Midia;

namespace StreamingRecommenderAPI.Interfaces
{
    /// <summary>
    /// Interface que aplica uma cadeia de filtros sobre um termo de busca (query)
    /// e retorna a coleção de OmdbMovie resultante.
    /// </summary>
    public interface IFilterService
    {
        /// <summary>
        /// Executa o filtro para a query fornecida e retorna os filmes filtrados.
        /// </summary>
        Task<IEnumerable<OmdbMovie>> ExecuteAsync(string query);
    }
}