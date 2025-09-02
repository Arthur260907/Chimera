using StreamingRecommenderAPI.Repositories;
using StreamingRecommenderAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao container.
// O AddControllers() registra os controllers que criamos.
builder.Services.AddControllers();

// Serviços para documentação da API (Swagger), muito útil para testes.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar seus serviços e repositórios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>(); // Esta linha conecta a interface à classe
builder.Services.AddScoped<UsuarioService>();

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

// Configura o pipeline de requisições HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll"); // Habilitar CORS

app.UseAuthorization();

// Mapeia as rotas definidas nos controllers.
app.MapControllers();

app.Run();
