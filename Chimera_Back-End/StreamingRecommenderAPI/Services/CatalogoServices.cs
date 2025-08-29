using Apianime.models.Catalogo;

namespace Apianime.Services;

public class CatalogoServices
{
    private readonly Movelservices _tmdb;
    private readonly Malservices _mal;

    public CatalogoServices(Movelservices tmdb, Malservices mal)
    {
        _tmdb = tmdb;
        _mal = mal;
    }

    public async Task<List<CatalogoItem>> GetCatalog(string filtro = null, double? notaMin = null)
    {
        var catalogo = new List<CatalogoItem>();

        var animes = await _mal.GetTopAnimes();
        catalogo.AddRange(animes);

        var filmes = await _tmdb.GetPopularMovies();
        catalogo.AddRange(filmes);

        if (!string.IsNullOrEmpty(filtro))
            catalogo = catalogo.Where(c => c.Titulo.Contains(filtro, StringComparison.OrdinalIgnoreCase)).ToList();

        if (notaMin.HasValue)
            catalogo = catalogo.Where(c => c.Nota.HasValue && c.Nota.Value >= notaMin.Value).ToList();

        return catalogo.OrderByDescending(c => c.Nota).ToList();
    }
}
