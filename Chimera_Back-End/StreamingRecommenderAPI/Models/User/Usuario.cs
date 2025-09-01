namespace Usuario.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        
        public string TokenRecuperacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataUltimoAcesso { get; set; }
    }
}