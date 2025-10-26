// Caminho: Models/Midia/Tmdb/TmdbImagesResponse.cs
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace StreamingRecommenderAPI.Models.Midia.Tmdb
{
    public class TmdbImagesResponse
    {
        [JsonPropertyName("posters")]
        public List<TmdbPoster>? Posters { get; set; }
        // Pode adicionar backdrops, logos etc.
    }

    public class TmdbPoster
    {
        [JsonPropertyName("aspect_ratio")]
        public double AspectRatio { get; set; }

        [JsonPropertyName("file_path")]
        public string? FilePath { get; set; } // Caminho relativo

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("iso_639_1")]
        public string? Language { get; set; } // Idioma
    }
}