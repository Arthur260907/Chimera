<<<<<<< HEAD
﻿using System.Collections.Generic;
using System.Threading.Tasks;
using StreamingRecommenderAPI.Models.Midia;

namespace StreamingRecommenderAPI.Services.Interfaces
{
    /// <summary>
    /// Serviço de busca que retorna uma coleção de OmdbMovie para uma query.
    /// </summary>
=======
// Services/Interfaces/ISearchService.cs
using StreamingRecommenderAPI.Models; // Ajuste se OmdbMovie estiver em Models.Midia
using StreamingRecommenderAPI.Models.Midia;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StreamingRecommenderAPI.Services.Interfaces
{
>>>>>>> 449ee9dbd286b960d244b48ad90eeb5a3c674493
    public interface ISearchService
    {
        Task<IEnumerable<OmdbMovie>> SearchAsync(string query);
    }
}
