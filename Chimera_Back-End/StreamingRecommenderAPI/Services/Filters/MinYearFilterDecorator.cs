// Services/Filters/MinYearFilterDecorator.cs
using StreamingRecommenderAPI.Models; // Namespace de OmdbMovie
using StreamingRecommenderAPI.Models.Midia;
using StreamingRecommenderAPI.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StreamingRecommenderAPI.Services.Filters
{
    public class MinYearFilterDecorator : FilterDecorator
    {
        private readonly int _minimumYear;

        public MinYearFilterDecorator(IFilterService innerFilter, int minimumYear)
            : base(innerFilter)
        {
            _minimumYear = minimumYear;
        }

        public override async Task<IEnumerable<OmdbMovie>> ExecuteAsync(string query)
        {
            var results = await _innerFilter.ExecuteAsync(query);
            return results.Where(item =>
            {
                // Lógica para extrair o ano (pode ser complexo se o formato for "YYYY-YYYY")
                string yearString = item.Year?.Split('–')[0].Trim() ?? ""; // Pega a primeira parte antes de '–' se existir
                return int.TryParse(yearString, out int year) && year >= _minimumYear;
            });
        }
    }
}
