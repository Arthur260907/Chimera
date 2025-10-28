// Services/Filters/TypeFilterDecorator.cs
using StreamingRecommenderAPI.Models; // Namespace de OmdbMovie
using StreamingRecommenderAPI.Services.Interfaces;
using System; // Para StringComparison
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StreamingRecommenderAPI.Services.Filters
{
    public class TypeFilterDecorator : FilterDecorator
    {
        private readonly string _mediaTypeToFilter; // Ex: "movie", "series"

        public TypeFilterDecorator(IFilterService innerFilter, string mediaTypeToFilter)
            : base(innerFilter)
        {
            _mediaTypeToFilter = mediaTypeToFilter;
        }

        public override async Task<IEnumerable<Models.Midia.OmdbMovie>> ExecuteAsync(string query)
        {
            var results = await _innerFilter.ExecuteAsync(query);
            return results.Where(item =>
                !string.IsNullOrEmpty(item.Type) && // Garante que Type não é nulo/vazio
                item.Type.Equals(_mediaTypeToFilter, StringComparison.OrdinalIgnoreCase));
        }
    }
}
