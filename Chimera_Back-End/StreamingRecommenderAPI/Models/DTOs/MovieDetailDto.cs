// Caminho: Models/DTOs/MovieDetailDto.cs
using System.Collections.Generic;
using StreamingRecommenderAPI.Models.Midia; // Precisa disso para List<Rating>

namespace StreamingRecommenderAPI.Models.DTOs
{
    public class MovieDetailDto
    {
        // Propriedades vindas principalmente da OMDB (OmdbMovie)
        public string? ImdbID { get; set; }
        public string? Title { get; set; }
        public string? Year { get; set; }
        public string? Rated { get; set; }
        public string? Released { get; set; }
        public string? Runtime { get; set; }
        public string? Genre { get; set; }
        public string? Director { get; set; }
        public string? Writer { get; set; }
        public string? Actors { get; set; }
        public string? Plot { get; set; }
        public string? Language { get; set; }
        public string? Country { get; set; }
        public string? Awards { get; set; }
        public string? ImdbRating { get; set; }
        public List<Rating>? Ratings { get; set; }
        public string? Type { get; set; }
        public string? OriginalPosterUrl { get; set; } // Poster original OMDB

        // --- Novas Propriedades vindas da TMDB ---
        public string? BestPosterUrl { get; set; } // Melhor poster TMDB (ou fallback)
        public string? TrailerUrl { get; set; } // Trailer YouTube
    }
}