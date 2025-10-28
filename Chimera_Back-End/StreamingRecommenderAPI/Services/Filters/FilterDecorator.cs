using System.Collections.Generic;
using System.Threading.Tasks;
using StreamingRecommenderAPI.Models.Midia;
using StreamingRecommenderAPI.Interfaces;

namespace StreamingRecommenderAPI.Filters
{
    /// <summary>
    /// Base para decorators de filtro. Encapsula outro IFilterService e delega por padrão a execução.
    /// </summary>
    public abstract class FilterDecorator : IFilterService
    {
        protected readonly IFilterService? _inner;

        protected FilterDecorator(IFilterService? inner)
        {
            _inner = inner;
        }

        /// <summary>
        /// Por padrão delega para o inner (se houver) passando a query.
        /// </summary>
        public virtual async Task<IEnumerable<OmdbMovie>> ExecuteAsync(string query)
        {
            if (_inner == null) return await Task.FromResult<IEnumerable<OmdbMovie>>(new List<OmdbMovie>());
            return await _inner.ExecuteAsync(query);
        }
    }
}