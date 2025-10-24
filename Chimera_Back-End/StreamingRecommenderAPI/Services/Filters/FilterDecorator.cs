// Services/Filters/FilterDecorator.cs
using StreamingRecommenderAPI.Models; // Namespace de OmdbMovie
using StreamingRecommenderAPI.Models.Midia;
using StreamingRecommenderAPI.Services.Interfaces;
using System; // Para ArgumentNullException
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StreamingRecommenderAPI.Services.Filters
{
    public abstract class FilterDecorator : IFilterService
    {
        protected readonly IFilterService _innerFilter;

        protected FilterDecorator(IFilterService innerFilter)
        {
            _innerFilter = innerFilter ?? throw new ArgumentNullException(nameof(innerFilter));
        }

        public abstract Task<IEnumerable<OmdbMovie>> ExecuteAsync(string query);

        // Explicit interface implementation delegates to the abstract method so every decorator only needs to
        // implement ExecuteAsync once.
        Task<IEnumerable<OmdbMovie>> IFilterService.ExecuteAsync(string query)
        {
            return ExecuteAsync(query);
        }
    }
}