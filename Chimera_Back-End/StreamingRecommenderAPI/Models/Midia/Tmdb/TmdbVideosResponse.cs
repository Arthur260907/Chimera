// Caminho: Models/Midia/Tmdb/TmdbVideosResponse.cs
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace StreamingRecommenderAPI.Models.Midia.Tmdb
{
    public class TmdbVideosResponse
    {
        [JsonPropertyName("results")]
        public List<TmdbVideo>? Results { get; set; }
    }

    public class TmdbVideo
    {
        [JsonPropertyName("iso_639_1")]
        public string? Language { get; set; }

        [JsonPropertyName("key")]
        public string? Key { get; set; } // A chave do vídeo (ex: YouTube ID)

        [JsonPropertyName("name")]
        public string? Name { get; set; } // Nome/título do vídeo

        [JsonPropertyName("site")]
        public string? Site { get; set; } // Onde está hospedado (ex: "YouTube")

        [JsonPropertyName("type")]
        public string? Type { get; set; } // Tipo (ex: "Trailer", "Teaser")

        [JsonPropertyName("official")]
        public bool Official { get; set; }
    }
}