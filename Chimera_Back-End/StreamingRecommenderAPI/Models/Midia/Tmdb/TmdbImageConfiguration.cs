// Caminho: Models/Midia/Tmdb/TmdbImageConfiguration.cs
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace StreamingRecommenderAPI.Models.Midia.Tmdb
{
    public class TmdbImageConfiguration
    {
        [JsonPropertyName("images")]
        public TmdbImageBaseConfiguration? Images { get; set; }
    }

    public class TmdbImageBaseConfiguration
    {
        [JsonPropertyName("secure_base_url")]
        public string? SecureBaseUrl { get; set; }

        [JsonPropertyName("poster_sizes")]
        public List<string>? PosterSizes { get; set; }
        // Pode adicionar backdrop_sizes, logo_sizes etc. se precisar
    }
}