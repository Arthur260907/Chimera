// Arquivo: Program.cs
using Microsoft.EntityFrameworkCore;
using StreamingRecommenderAPI.Data;
using StreamingRecommenderAPI.Repositories;
using StreamingRecommenderAPI.Services;
using StreamingRecommenderAPI.Models; // Para EmailSettings
using StreamingRecommenderAPI.Services.Interfaces; // Para ISearchService e IEmailService
using Microsoft.Extensions.Options; // Para IOptions

var builder = WebApplication.CreateBuilder(args);

// --- Configuração do DbContext ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// --- Adicionar serviços ao container ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Streaming Recommender API", Version = "v1" });
});

// --- Registrar Repositórios e Serviços ---
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<UsuarioService>();

// Registra OmdbService e TmdbService via HttpClientFactory
builder.Services.AddHttpClient<OmdbService>();
builder.Services.AddHttpClient<TmdbService>(); // Adiciona o registro do TmdbService

// Registra a implementação do ISearchService (que usa OmdbService)
builder.Services.AddScoped<ISearchService, OmdbSearchService>();

// --- Configuração e Registro do EmailService ---
// Configura EmailSettings para ser injetável via IOptions<EmailSettings>
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
// Registra EmailService como a implementação de IEmailService
builder.Services.AddTransient<IEmailService, EmailService>(); // Use AddTransient para serviços leves ou AddScoped se ele tiver dependências Scoped
// --- Fim Configuração EmailService ---

// --- Configuração do CORS ---
builder.Services.AddCors(options =>
{
    // Política mais permissiva para desenvolvimento
    options.AddPolicy("DevAllowAll", policy =>
    {
        policy.AllowAnyOrigin() // Permite qualquer origem (frontend rodando em qualquer porta)
              .AllowAnyMethod() // Permite qualquer método HTTP (GET, POST, etc.)
              .AllowAnyHeader(); // Permite qualquer cabeçalho HTTP
    });
    // Você pode criar políticas mais restritivas para produção
});

var app = builder.Build();

// --- Aplicar Migrações do EF ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        // Aplica quaisquer migrações pendentes ao banco de dados ao iniciar
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        // Loga um erro se a migração falhar
        var logger = services.GetService<ILogger<Program>>();
        logger?.LogError(ex, "An error occurred while migrating or initializing the database.");
        // Considerar lançar a exceção ou parar a aplicação em caso de falha crítica na migração
    }
}

// --- Configurar o HTTP request pipeline ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Streaming Recommender API v1");
        // Rota para acessar o Swagger UI (ex: https://localhost:7196/swagger)
        c.RoutePrefix = "swagger";
    });
    // Adiciona Developer Exception Page para melhor visualização de erros em dev
    app.UseDeveloperExceptionPage();
}
else
{
    // Em produção, usar HTTPS e HSTS
    app.UseHttpsRedirection();
    app.UseHsts(); // Header HSTS para segurança
}

app.UseRouting(); // Essencial para mapear rotas

// Aplica a política CORS definida ("DevAllowAll")
app.UseCors("DevAllowAll"); // Deve vir ANTES de UseAuthorization e MapControllers

app.UseAuthorization(); // Habilita autorização (se você usar)

// Mapeia os controllers para as rotas (ex: /api/Search)
app.MapControllers();

app.Run();