using Newtonsoft.Json;
using Apianime.models.Catalogo;
using Apianime.models.Movel;

namespace Apianime.Services;

public class Movelservices
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey = "SUA_API_KEY_TMDB";

    public Movelservices(HttpClient client)
    {
        _httpClient = client;
        _httpClient.BaseAddress = new Uri("https://api.themoviedb.org/3/");
    }

    public async Task<List<CatalogoMovel>> GetPopularMovies()
    {
        var json = await _httpClient.GetStringAsync($"movie/popular?api_key={_apiKey}&language=pt-BR");
        var filmes = JsonConvert.DeserializeObject<MovieListResponse>(json);

        return filmes.results.Select(f => new CatalogoMovel
        {
            Id = f.id,
            Titulo = f.title,
            Tipo = "Filme",
            Sinopse = f.overview,
            Imagem = $"https://image.tmdb.org/t/p/w500{f.poster_path}",
            Nota = f.vote_average,
            DataLancamento = f.release_date,
            DuracaoMinutos = null,
            Origem = "TMDb"
        }).ToList();
    }
}
