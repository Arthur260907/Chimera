
using Newtonsoft.Json;
using Apianime.models.Catalogo;
using Apianime.models.Mal;
namespace Apianime.Services;

public class Malservices
{
    private readonly HttpClient _httpClient;
    private const string ClientId = "SEU_CLIENT_ID_MAL";

    public Malservices(HttpClient client)
    {
        _httpClient = client;
        _httpClient.BaseAddress = new Uri("https://api.myanimelist.net/v2/");
        _httpClient.DefaultRequestHeaders.Add("X-MAL-CLIENT-ID", ClientId);
    }

    public async Task<List<CatalogoAnime>> GetTopAnimes()
    {
        var json = await _httpClient.GetStringAsync("anime/ranking?ranking_type=all&limit=10");
        var animes = JsonConvert.DeserializeObject<AnimeListResponse>(json);

        return animes.data.Select(a => new CatalogoAnime
        {
            Id = a.node.id,
            Titulo = a.node.title,
            Tipo = "Anime",
            Imagem = a.node.main_picture?.large,
            Nota = a.node.mean,
            Episodios = null,
            Origem = "MAL"
        }).ToList();
    }
}
