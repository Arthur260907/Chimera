
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StreamingRecommenderAPI.Models.Midia;

public class OmdbMovie
{
    [JsonPropertyName("imdbID")]
    [Key]
    public string? ImdbID { get; set; }
    [JsonPropertyName("Title")]
    public string? Title { get; set; }

    [JsonPropertyName("Year")]
    public string? Year { get; set; }

    [JsonPropertyName("Rated")]
    public string? Rated { get; set; }

    [JsonPropertyName("Released")]
    public string? Released { get; set; }

    [JsonPropertyName("Runtime")]
    public string? Runtime { get; set; }

    [JsonPropertyName("Genre")]
    public string? Genre { get; set; }

    [JsonPropertyName("Director")]
    public string? Director { get; set; }

    [JsonPropertyName("Writer")]
    public string? Writer { get; set; }

    [JsonPropertyName("Actors")]
    public string? Actors { get; set; }

    [JsonPropertyName("Plot")]
    public string? Plot { get; set; }

    [JsonPropertyName("Language")]
    public string? Language { get; set; }

    [JsonPropertyName("Country")]
    public string? Country { get; set; }

    [JsonPropertyName("Awards")]
    public string? Awards { get; set; }

    [JsonPropertyName("Poster")]
    public string? Poster { get; set; }

    [JsonPropertyName("imdbRating")]
    public string? ImdbRating { get; set; }

    [JsonPropertyName("Type")]
    public string? Type { get; set; }
    [JsonPropertyName("Response")]
    public string? Response { get; set; }

    [JsonPropertyName("Error")]
    public string? Error { get; set; }
}