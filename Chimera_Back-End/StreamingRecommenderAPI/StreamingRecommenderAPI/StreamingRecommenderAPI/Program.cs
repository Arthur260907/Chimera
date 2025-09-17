

// Localiza��o: StreamingRecommenderAPI / Program.cs

// Adicione estes usings no topo do arquivo
using Microsoft.EntityFrameworkCore;
using StreamingRecommenderAPI.Data;
using StreamingRecommenderAPI.Repositories;
using StreamingRecommenderAPI.Services;
using Microsoft.EntityFrameworkCore.SqlServer;

var builder = WebApplication.CreateBuilder(args);

// --- IN�CIO DA CONFIGURA��O DO DBCONTEXT ---

// 1. Pega a string de conex�o que vamos adicionar no appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Adiciona o DbContext ao cont�iner de servi�os
//    Aqui estamos dizendo para ele usar o SQL Server.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- FIM DA CONFIGURA��O DO DBCONTEXT ---


// Adicionar servi�os ao container.
builder.Services.AddControllers();

// Servi�os para documenta��o da API (Swagger), muito �til para testes.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Streaming Recommender API",
        Version = "v1",
        Description = "API de recomenda��o de streaming"
    });
});

// Registrar seus servi�os e reposit�rios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddHttpClient<OmdbService>();
builder.Services.AddScoped<OmdbService>();

// Configurar CORS para permitir requisi��es do front-end
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

// Configura o pipeline de requisi��es HTTP.
// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Streaming Recommender API v1");
        c.RoutePrefix = string.Empty; // Abre Swagger direto em http://localhost:5000/
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll"); // Habilitar CORS

app.UseAuthorization();

// Mapeia as rotas definidas nos controllers.
app.MapControllers();

app.Run();