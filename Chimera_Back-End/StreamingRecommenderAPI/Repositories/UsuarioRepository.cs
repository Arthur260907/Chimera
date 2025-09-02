using Dapper;
using MySql.Data.MySqlClient;
using StreamingRecommenderAPI.Models.User;
using System.Threading.Tasks;

namespace StreamingRecommenderAPI.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly string _connectionString;

        public UsuarioRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<Usuario> GetByEmailAsync(string email)
        {
            using var conn = new MySqlConnection(_connectionString);
            return await conn.QueryFirstOrDefaultAsync<Usuario>("SELECT * FROM usuarios WHERE email = @email", new { email });
        }

        public async Task<Usuario> GetByTokenAsync(string token)
        {
            using var conn = new MySqlConnection(_connectionString);
            return await conn.QueryFirstOrDefaultAsync<Usuario>("SELECT * FROM usuarios WHERE token_recuperacao = @token AND token_expira_em > NOW()", new { token });
        }

        public async Task SalvarTokenAsync(string email, string token, DateTime expiraEm)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.ExecuteAsync("UPDATE usuarios SET token_recuperacao = @token, token_expira_em = @expiraEm WHERE email = @email", new { token, expiraEm, email });
        }

        public async Task AtualizarSenhaAsync(string token, string novaSenhaHash)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.ExecuteAsync("UPDATE usuarios SET senha = @novaSenhaHash, token_recuperacao = NULL, token_expira_em = NULL WHERE token_recuperacao = @token", new { novaSenhaHash, token });
        }

        public async Task CadastrarUsuarioAsync(Usuario usuario)
        {
            using var conn = new MySqlConnection(_connectionString);
            var sql = "INSERT INTO usuarios (Nome, Email, Senha, Data_Cadastro) VALUES (@Nome, @Email, @Senha, @Data_Cadastro)";
            await conn.ExecuteAsync(sql, usuario);
        }
    }
}