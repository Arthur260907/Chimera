<<<<<<< HEAD
﻿using System.Collections.Generic;
using System.Threading.Tasks;
using StreamingRecommenderAPI.Models.Midia;
=======
// Services/Interfaces/IFilterService.cs
using StreamingRecommenderAPI.Models; // Ajuste se OmdbMovie estiver em Models.Midia
using StreamingRecommenderAPI.Models.Midia;
using System.Collections.Generic;
using System.Threading.Tasks;
>>>>>>> 449ee9dbd286b960d244b48ad90eeb5a3c674493

namespace StreamingRecommenderAPI.Services.Interfaces
{
    public interface IFilterService
    {
<<<<<<< HEAD
        // método assíncrono que aplica o filtro e retorna coleção filtrada
        Task<IEnumerable<OmdbMovie>> ExecuteAsync(IEnumerable<OmdbMovie> source);
=======
        Task<IEnumerable<OmdbMovie>> ExecuteAsync(string query);
>>>>>>> 449ee9dbd286b960d244b48ad90eeb5a3c674493
    }
}
