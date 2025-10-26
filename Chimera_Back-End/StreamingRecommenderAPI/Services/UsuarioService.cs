// Cole este código em Chimera_Back-End\StreamingRecommenderAPI\Services\UsuarioService.cs
using StreamingRecommenderAPI.Models.User;
using StreamingRecommenderAPI.Repositories;
using System; 
using System.Threading.Tasks; 

namespace StreamingRecommenderAPI.Services
{
    public class UsuarioService
    {
        private readonly IUsuarioRepository _repo;

        public UsuarioService(IUsuarioRepository repo)
        {
            _repo = repo;
        }

        // --- Método de Recuperação de Senha ---
        public async Task SolicitarRecuperacaoSenhaAsync(string email)
        {
            var usuario = await _repo.GetByEmailAsync(email);
            if (usuario == null) return; 

            var token = Guid.NewGuid().ToString(); 
            var expiraEm = DateTime.UtcNow.AddHours(1); 
            await _repo.SalvarTokenAsync(email, token, expiraEm);

            Console.WriteLine($"[DEV] Token de recuperação para {email}: {token}");
        }

        // --- Método de Redefinir Senha ---
        public async Task<bool> RedefinirSenhaAsync(string token, string novaSenha)
        {
            var usuario = await _repo.GetByTokenAsync(token);
            if (usuario == null) return false;

            var novaSenhaHash = BCrypt.Net.BCrypt.HashPassword(novaSenha);
            await _repo.AtualizarSenhaAsync(token, novaSenhaHash);
            return true;
        }

        // --- Método de Cadastro ---
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
                Senha = BCrypt.Net.BCrypt.HashPassword(senha), // Hash da senha
                Data_Cadastro = DateTime.UtcNow,
                Foto_Perfil = "" // Valor padrão inicial
            };

            await _repo.CadastrarUsuarioAsync(novoUsuario);
            return true;
        }

        // --- Método de Login ---
        public async Task<Usuario?> LoginAsync(string email, string senha)
        {
            var usuario = await _repo.GetByEmailAsync(email);
            if (usuario != null)
            {
                // Verifica senha com o hash
                bool senhaValida = BCrypt.Net.BCrypt.Verify(senha, usuario.Senha);
                if (senhaValida)
                {
                    return usuario; 
                }
            }
            return null; 
        }
    }
}