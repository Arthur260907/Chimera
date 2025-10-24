using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StreamingRecommenderAPI.Models.Midia;
using StreamingRecommenderAPI.Interfaces;

namespace StreamingRecommenderAPI.Filters
{
    /// <summary>
    /// Filtra por tipo (por exemplo "movie", "series").
    /// </summary>
    public class TypeFilterDecorator : FilterDecorator
    {
        private readonly string _type;

        public TypeFilterDecorator(IFilterService? inner, string type) : base(inner)
        {
            _type = type ?? string.Empty;
        }

        public override async Task<IEnumerable<OmdbMovie>> ExecuteAsync(string query)
        {
            var result = await base.ExecuteAsync(query);
            if (string.IsNullOrWhiteSpace(_type)) return result;

            return result.Where(m =>
                !string.IsNullOrWhiteSpace(m?.Type) &&
                string.Equals(m.Type.Trim(), _type.Trim(), StringComparison.OrdinalIgnoreCase)
            );
        }
    }
}