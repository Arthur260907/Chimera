

// Localização: StreamingRecommenderAPI / Program.cs

// Adicione estes usings no topo do arquivo
using Microsoft.EntityFrameworkCore;
using StreamingRecommenderAPI.Data;
using StreamingRecommenderAPI.Repositories;
using StreamingRecommenderAPI.Services;
using StreamingRecommenderAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// --- INÍCIO DA CONFIGURAÇÃO DO DBCONTEXT ---

// 1. Pega a string de conexão que vamos adicionar no appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Adiciona o DbContext ao contêiner de serviços
//    Aqui estamos usando InMemory database para desenvolvimento
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("ChimeraDB"));

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
        Description = "API de recomendação de streaming para o projeto Chimera",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Chimera Team",
            Email = "contato@chimera.com"
        }
    });
});

// Registrar seus serviços e repositórios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddHttpClient<OmdbService>();
builder.Services.AddScoped<OmdbService>();
builder.Services.AddScoped<ISearchService, OmdbSearchService>();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Streaming Recommender API v1");
        c.RoutePrefix = "swagger"; // Define que o Swagger estará em /swagger
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll"); // Habilitar CORS

app.UseAuthorization();

// Mapeia as rotas definidas nos controllers.
app.MapControllers();


app.Run();
