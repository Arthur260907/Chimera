namespace StreamingRecommenderAPI.Models.Midia;
using System.ComponentModel.DataAnnotations.Schema;

[NotMapped]
public class Rating
{
    public string Source { get; set; }
    public string Value { get; set; }
}