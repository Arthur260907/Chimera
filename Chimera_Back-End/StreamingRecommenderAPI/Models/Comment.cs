namespace StreamingRecommenderAPI.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string MovieId { get; set; } = string.Empty;
        public string Username { get; set; } = "Anonymous";
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}