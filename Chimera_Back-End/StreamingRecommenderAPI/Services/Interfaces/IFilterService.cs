// Services/Interfaces/IFilterService.cs
using StreamingRecommenderAPI.Models; // Ajuste se OmdbMovie estiver em Models.Midia
using StreamingRecommenderAPI.Models.Midia;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StreamingRecommenderAPI.Services.Interfaces
{
    public interface IFilterService
    {
        Task<IEnumerable<OmdbMovie>> ExecuteAsync(string query);
    }
}
