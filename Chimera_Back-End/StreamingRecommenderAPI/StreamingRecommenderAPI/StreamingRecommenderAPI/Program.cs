

// Localização: StreamingRecommenderAPI / Program.cs

// Adicione estes usings no topo do arquivo
using Microsoft.EntityFrameworkCore;
using StreamingRecommenderAPI.Data;
using StreamingRecommenderAPI.Repositories;
using StreamingRecommenderAPI.Services;
using Microsoft.EntityFrameworkCore.SqlServer;

var builder = WebApplication.CreateBuilder(args);

// --- INÍCIO DA CONFIGURAÇÃO DO DBCONTEXT ---

// 1. Pega a string de conexão que vamos adicionar no appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Adiciona o DbContext ao contêiner de serviços
//    Aqui estamos dizendo para ele usar o SQL Server.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- FIM DA CONFIGURAÇÃO DO DBCONTEXT ---


// Adicionar serviços ao container.
builder.Services.AddControllers();

// Serviços para documentação da API (Swagger), muito útil para testes.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Streaming Recommender API",
        Version = "v1",
        Description = "API de recomendação de streaming"
    });
});

// Registrar seus serviços e repositórios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddHttpClient<OmdbService>();
builder.Services.AddScoped<OmdbService>();

// Configurar CORS para permitir requisições do front-end
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("AllowAll"); // Habilitar CORS

app.UseAuthorization();

// Mapeia as rotas definidas nos controllers.
app.MapControllers();


app.Run();
