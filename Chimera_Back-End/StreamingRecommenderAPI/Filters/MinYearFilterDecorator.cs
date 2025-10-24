using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StreamingRecommenderAPI.Models.Midia;
using StreamingRecommenderAPI.Interfaces;

namespace StreamingRecommenderAPI.Filters
{
    /// <summary>
    /// Filtra por ano mínimo (exclui itens com ano menor que minYear).
    /// </summary>
    public class MinYearFilterDecorator : FilterDecorator
    {
        private readonly int _minYear;

        public MinYearFilterDecorator(IFilterService? inner, int minYear) : base(inner)
        {
            _minYear = minYear;
        }

        public override async Task<IEnumerable<OmdbMovie>> ExecuteAsync(string query)
        {
            var result = await base.ExecuteAsync(query);

            return result.Where(m =>
            {
                if (m == null) return false;
                if (string.IsNullOrWhiteSpace(m.Year)) return false;
                var yearPart = m.Year.Trim();
                if (yearPart.Length >= 4 && int.TryParse(yearPart.Substring(0, 4), out var y))
                {
                    return y >= _minYear;
                }
                return false;
            });
        }
    }
}