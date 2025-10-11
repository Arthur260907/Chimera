namespace StreamingRecommenderAPI.Models.User.DTOs
{
    public class RedefinirSenhaRequest
    {
        public string Token { get; set; }
        public string NovaSenha { get; set; }
    }
}