// Arquivo: Services/UsuarioService.cs
using StreamingRecommenderAPI.Models.User;
using StreamingRecommenderAPI.Repositories;
using System;
using System.Threading.Tasks;
using StreamingRecommenderAPI.Services.Interfaces; // Adicionado para IEmailService

namespace StreamingRecommenderAPI.Services
{
    public class UsuarioService
    {
        private readonly IUsuarioRepository _repo;
        private readonly IEmailService _emailService; // Injetado

        // Construtor atualizado
        public UsuarioService(IUsuarioRepository repo, IEmailService emailService)
        {
            _repo = repo;
            _emailService = emailService; // Atribuído
        }

        // --- Método de Recuperação de Senha (Atualizado) ---
        public async Task SolicitarRecuperacaoSenhaAsync(string email)
        {
            var usuario = await _repo.GetByEmailAsync(email);
            if (usuario == null)
            {
                 Console.WriteLine($"[DEV] Tentativa de recuperação para email não cadastrado: {email}");
                 return; // Retorna silenciosamente
            }

            var token = Guid.NewGuid().ToString();
            var expiraEm = DateTime.UtcNow.AddHours(1);
            await _repo.SalvarTokenAsync(email, token, expiraEm);

            // --- Envio de Email ---
            var subject = "Recuperação de Senha - Chimera";
            var message = $@"
        <html><body>
            <p>Olá {usuario.Nome ?? "Usuário"},</p>
            <p>Você solicitou a redefinição de sua senha para a plataforma Chimera.</p>
            <p>Use o seguinte token para criar uma nova senha (válido por 1 hora):</p>
            <p style='font-size: 1.2em; font-weight: bold; background-color: #f0f0f0; padding: 10px; border-radius: 5px; display: inline-block;'>{token}</p>
            <p>Se você não solicitou esta alteração, ignore este email.</p><br>
            <p>Atenciosamente,<br>Equipe Chimera</p>
        </body></html>";

            try
            {
                await _emailService.SendEmailAsync(email, subject, message);
                Console.WriteLine($"[INFO] Email de recuperação (com token {token}) potencialmente enviado para {email}.");
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"[ERRO] Falha ao tentar ENVIAR email de recuperação para {email}: {ex.Message}");
                 // Não lançar a exceção, para que o controller retorne Ok( ) mesmo assim
                 // (Medida de segurança para não vazar se o email existe ou se o envio falhou)
            }
            // --- Fim do Envio de Email ---
        }

        // --- Método de Redefinir Senha ---
        public async Task<bool> RedefinirSenhaAsync(string token, string novaSenha)
        {
            var usuario = await _repo.GetByTokenAsync(token);
            if (usuario == null) return false; // Token inválido ou expirado

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