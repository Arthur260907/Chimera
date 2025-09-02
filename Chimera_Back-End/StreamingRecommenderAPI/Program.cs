using StreamingRecommenderAPI.Repositories;
using StreamingRecommenderAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços ao contêiner.
// O AddControllers() registra os controllers que criamos.
builder.Services.AddControllers();

// Serviços para documentação da API (Swagger), muito útil para testes.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configura o pipeline de requisições HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Mapeia as rotas definidas nos controllers.
app.MapControllers();

app.Run();

var builder = WebApplication.CreateBuilder(args);

// Adiciona os serviços essenciais do ASP.NET Core para a API.
builder.Services.AddControllers();

// Adiciona o Swagger para documentação e teste fácil da API.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Em ambiente de desenvolvimento, habilita a interface do Swagger.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseAuthorization();

// Mapeia as rotas definidas nos controllers.
app.MapControllers();

app.Run();



var builder = WebApplication.CreateBuilder(args);

// ...
// Registrar seus serviços e repositórios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<UsuarioService>();
