// Services/Interfaces/ISearchService.cs
using StreamingRecommenderAPI.Models; // Ajuste se OmdbMovie estiver em Models.Midia
using StreamingRecommenderAPI.Models.Midia;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StreamingRecommenderAPI.Services.Interfaces
{
    public interface ISearchService
    {
        Task<IEnumerable<OmdbMovie>> SearchAsync(string query);
    }
}
