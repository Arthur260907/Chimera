<Project Sdk="Microsoft.NET.Sdk.Web">
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