using System.Collections.Generic;
using System.Threading.Tasks;
using StreamingRecommenderAPI.Models.Midia;

namespace StreamingRecommenderAPI.Services.Interfaces
{
    /// <summary>
    /// Interface que representa um serviço de filtros encadeáveis que recebe
    /// uma query e retorna os filmes resultantes.
    /// </summary>
    public interface IFilterService
    {
        Task<IEnumerable<OmdbMovie>> ExecuteAsync(string query);
    }
}
