using Org.BouncyCastle.Crypto.Generators;
using StreamingRecommenderAPI.Models.User;
using StreamingRecommenderAPI.Repositories;


namespace StreamingRecommenderAPI.Services
{
    public class UsuarioService
    {
        private readonly IUsuarioRepository _repo;

        public UsuarioService(IUsuarioRepository repo)
        {
            _repo = repo;
        }

        public async Task SolicitarRecuperacaoSenhaAsync(string email)
        {
            var usuario = await _repo.GetByEmailAsync(email);
            if (usuario == null) return;

            var token = Guid.NewGuid().ToString();
            var expiraEm = DateTime.UtcNow.AddHours(1);
            await _repo.SalvarTokenAsync(email, token, expiraEm);

            Console.WriteLine($"[DEV] Token para {email}: {token}");
        }

        public async Task<bool> RedefinirSenhaAsync(string token, string novaSenha)
        {
            var usuario = await _repo.GetByTokenAsync(token);
            if (usuario == null) return false;

            var novaSenhaHash = BCrypt.Net.BCrypt.HashPassword(novaSenha);
            await _repo.AtualizarSenhaAsync(token, novaSenhaHash);
            return true;
        }

        public async Task<bool> CadastrarUsuarioAsync(string nome, string email, string senha)
        {
            var usuarioExistente = await _repo.GetByEmailAsync(email);
            if (usuarioExistente != null)
            {
                return false; // Usuário já existe
            }

            var novoUsuario = new Usuario
            {
                Nome = nome,
                Email = email,
                Senha = BCrypt.Net.BCrypt.HashPassword(senha), // Hashing da senha
                Data_Cadastro = DateTime.UtcNow
            };

            await _repo.CadastrarUsuarioAsync(novoUsuario);
            return true;
        }
    }
}