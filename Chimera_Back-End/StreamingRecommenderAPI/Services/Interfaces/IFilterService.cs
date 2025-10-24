using System.Collections.Generic;
using System.Threading.Tasks;
using StreamingRecommenderAPI.Models.Midia;

namespace StreamingRecommenderAPI.Services.Interfaces
{
    public interface IFilterService
    {
        // método assíncrono que aplica o filtro e retorna coleção filtrada
        Task<IEnumerable<OmdbMovie>> ExecuteAsync(IEnumerable<OmdbMovie> source);
    }
}
