var builder = WebApplication.CreateBuilder(args);

// Adiciona CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
// ...outras configurações...

var app = builder.Build();

app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();
app.Run();
