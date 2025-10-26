using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Dapper;
using StreamingRecommenderAPI.Models.User;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration; 
using System; 

namespace StreamingRecommenderAPI.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly string? _connectionString;

        public UsuarioRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            }
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            using var conn = new MySqlConnection(_connectionString);
            // Assume coluna 'Email'
            return await conn.QueryFirstOrDefaultAsync<Usuario>("SELECT * FROM usuarios WHERE Email = @email", new { email });
        }

        public async Task<Usuario?> GetByTokenAsync(string token)
        {
            using var conn = new MySqlConnection(_connectionString);
            // CORREÇÃO: Usa TokenRecuperacao e TokenExpiraEm (PascalCase)
            return await conn.QueryFirstOrDefaultAsync<Usuario>("SELECT * FROM usuarios WHERE TokenRecuperacao = @token AND TokenExpiraEm > NOW()", new { token });
        }

        public async Task SalvarTokenAsync(string email, string token, DateTime expiraEm)
        {
            using var conn = new MySqlConnection(_connectionString);
             // CORREÇÃO: Usa TokenRecuperacao e TokenExpiraEm (PascalCase)
            await conn.ExecuteAsync("UPDATE usuarios SET TokenRecuperacao = @token, TokenExpiraEm = @expiraEm WHERE Email = @email", new { token, expiraEm, email });
        }

        public async Task AtualizarSenhaAsync(string token, string novaSenhaHash)
        {
            using var conn = new MySqlConnection(_connectionString);
            // CORREÇÃO: Usa Senha, TokenRecuperacao e TokenExpiraEm (PascalCase)
            await conn.ExecuteAsync("UPDATE usuarios SET Senha = @novaSenhaHash, TokenRecuperacao = NULL, TokenExpiraEm = NULL WHERE TokenRecuperacao = @token", new { novaSenhaHash, token });
        }

        public async Task CadastrarUsuarioAsync(Usuario usuario)
        {
            using var conn = new MySqlConnection(_connectionString);
            // Usa Nome, Email, Senha, Data_Cadastro, Foto_Perfil (PascalCase)
            var sql = "INSERT INTO usuarios (Nome, Email, Senha, Data_Cadastro, Foto_Perfil) VALUES (@Nome, @Email, @Senha, @Data_Cadastro, @Foto_Perfil)";
            await conn.ExecuteAsync(sql, usuario);
        }
    }
}