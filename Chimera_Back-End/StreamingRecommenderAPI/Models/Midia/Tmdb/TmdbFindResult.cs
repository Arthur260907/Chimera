// Caminho: Models/Midia/Tmdb/TmdbFindResult.cs
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace StreamingRecommenderAPI.Models.Midia.Tmdb
{
    public class TmdbFindResult
    {
        [JsonPropertyName("movie_results")]
        public List<TmdbMinimalMovie>? MovieResults { get; set; }
        // Pode adicionar tv_results se precisar buscar séries também no futuro
    }

    public class TmdbMinimalMovie
    {
        [JsonPropertyName("id")]
        public int Id { get; set; } // O ID interno do TMDB
    }
}