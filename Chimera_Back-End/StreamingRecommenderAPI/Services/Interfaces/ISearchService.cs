using System.Collections.Generic;
using System.Threading.Tasks;
using StreamingRecommenderAPI.Models.Midia;

namespace StreamingRecommenderAPI.Services.Interfaces
{
    /// <summary>
    /// Serviço de busca que retorna uma coleção de OmdbMovie para uma query.
    /// </summary>
    public interface ISearchService
    {
        Task<IEnumerable<OmdbMovie>> SearchAsync(string query);
    }
}
