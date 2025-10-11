// Localização: StreamingRecommenderAPI/Data/ApplicationDbContext.cs

using Microsoft.EntityFrameworkCore;
using StreamingRecommenderAPI.Models.Midia;
using StreamingRecommenderAPI.Models.User;

namespace StreamingRecommenderAPI.Data
{
    // Nossa classe herda da classe DbContext do Entity Framework.
    public class ApplicationDbContext : DbContext
    {
        // Construtor necessário para a injeção de dependência.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // --- Mapeamento das Classes para Tabelas ---
        // Cada DbSet<T> representa uma tabela no banco de dados.
        // O nome da propriedade (Usuarios) será o nome da tabela.
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<OmdbMovie> Filmes { get; set; }
        // Adicione aqui outros DbSets para futuras tabelas que você precise salvar,
        // como avaliações, favoritos, etc.
        // public DbSet<Avaliacao> Avaliacoes { get; set; }
    }
}