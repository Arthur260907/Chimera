<Project Sdk="Microsoft.NET.Sdk.Web">
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

// Usa CORS
app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();
app.Run();

[ApiController]
[Route("api/catalogo")]
public class CatalogoController : ControllerBase
{
    private readonly CatalogoServices _catalog;

    public CatalogoController(CatalogoServices catalog)
    {
        _catalog = catalog;
    }

    [HttpGet]
    public async Task<ActionResult<List<CatalogoItem>>> GetCatalogo(
        [FromQuery] string filtro = null,
        [FromQuery] double? notaMin = null)
    {
        var lista = await _catalog.GetCatalog(filtro, notaMin);
        return Ok(lista);
    }
}