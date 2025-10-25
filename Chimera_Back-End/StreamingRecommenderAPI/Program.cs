// Localização: StreamingRecommenderAPI / Program.cs

// Adicione estes usings no topo do arquivo
using Microsoft.EntityFrameworkCore;
using StreamingRecommenderAPI.Data;
using StreamingRecommenderAPI.Repositories;
using StreamingRecommenderAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// --- INÍCIO DA CONFIGURAÇÃO DO DBCONTEXT ---

// 1. Pega a string de conexão que vamos adicionar no appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Adiciona o DbContext ao contêiner de serviços
//    Agora usando MySQL via Pomelo.EntityFrameworkCore.MySql
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

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
// Registra OmdbService via HttpClientFactory — não é necessário AddScoped adicional
builder.Services.AddHttpClient<OmdbService>();

// Adicione isto para permitir requisições do front (dev). Substitua AllowAnyOrigin por WithOrigins("http://127.0.0.1:5500") se souber a origem do five-server.
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

// Aplicar migrações do EF automaticamente ao iniciar a aplicação
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
        // Não interrompe a inicialização — dependendo do cenário você pode querer rethrow
    }
}

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

// Habilitar CORS antes dos endpoints
app.UseCors("DevAllowAll");

app.UseAuthorization();

// Mapeia as rotas definidas nos controllers.
app.MapControllers();


app.Run();
