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
builder.Services.AddHttpClient<OmdbService>();
builder.Services.AddScoped<ISearchService, OmdbSearchService>();

// --- Configuração e Registro do EmailService ---
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();
// --- Fim Configuração EmailService ---

// --- Configuração do CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevAllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// --- Aplicar Migrações do EF ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetService<ILogger<Program>>();
        logger?.LogError(ex, "An error occurred while migrating or initializing the database.");
    }
}

// --- Configurar o HTTP request pipeline ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Streaming Recommender API v1");
        c.RoutePrefix = "swagger";
    });
}

// app.UseHttpsRedirection(); // Comentado para simplificar testes locais com http://
app.UseRouting(); // Adicionado para garantir a ordem correta

app.UseCors("DevAllowAll"); // CORS deve vir antes de UseAuthorization e MapControllers

app.UseAuthorization();

app.MapControllers();

app.Run();