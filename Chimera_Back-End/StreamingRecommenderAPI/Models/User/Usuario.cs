namespace StreamingRecommenderAPI.Models.User
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public DateTime Data_Cadastro { get; set; }
        public string Foto_Perfil { get; set; }
        public string? TokenRecuperacao { get; set; }
        public DateTime? TokenExpiraEm { get; set; }
    }
}