using Microsoft.EntityFrameworkCore;
using StreamingRecommenderAPI.Models.User;
using System.Threading.Tasks;

namespace StreamingRecommenderAPI.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario> GetByEmailAsync(string email);
        Task<Usuario> GetByTokenAsync(string token);
        Task SalvarTokenAsync(string email, string token, DateTime expiraEm);
        Task AtualizarSenhaAsync(string token, string novaSenhaHash);
        Task CadastrarUsuarioAsync(Usuario usuario);
    }
}