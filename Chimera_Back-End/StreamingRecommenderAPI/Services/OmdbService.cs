using Stj = System.Text.Json;
using Newtonsoft.Json;
using StreamingRecommenderAPI.Models;
using StreamingRecommenderAPI.Models.Midia;
using StreamingRecommenderAPI.Repositories;


namespace StreamingRecommenderAPI.Services
{
    public class OmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OmdbService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["ApiKeys:Omdb"];
        }

        public async Task<OmdbMovie> GetMovieByTitleAsync(string title)
        {
            try
            {
                // Verificação melhorada da chave API
                if (string.IsNullOrEmpty(_apiKey) || _apiKey.Contains("SUA_CHAVE") || _apiKey.Length < 8)
                {
                    return new OmdbMovie
                    {
                        Response = "False",
                        Error = "Chave API da OMDB não configurada. Configure em appsettings.json"
                    };
                }

                // ✅ CORRIGIDO: Usar HTTPS
                var url = $"https://www.omdbapi.com/?t={Uri.EscapeDataString(title)}&apikey={_apiKey}";

                Console.WriteLine($"🔍 Request URL: {url}"); // Debug

                var response = await _httpClient.GetAsync(url);

                // Verifica se a requisição foi bem-sucedida
                if (!response.IsSuccessStatusCode)
                {
                    return new OmdbMovie
                    {
                        Response = "False",
                        Error = $"Erro HTTP: {response.StatusCode}"
                    };
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"📄 JSON Response: {jsonResponse}"); // Debug

                // Configurações para deserialização
                var options = new Stj.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true
                };

                var movie = Stj.JsonSerializer.Deserialize<OmdbMovie>(jsonResponse, options);

                if (movie == null)
                {
                    return new OmdbMovie
                    {
                        Response = "False",
                        Error = "Falha na deserialização do JSON"
                    };
                }

                return movie;
            }
            catch (Exception ex)
            {
                return new OmdbMovie
                {
                    Response = "False",
                    Error = $"Erro: {ex.Message}"
                };
            }
        }

        public async Task<OmdbMovie> GetMovieByIdAsync(string imdbId)
        {
            try
            {
                if (string.IsNullOrEmpty(_apiKey) || _apiKey.Contains("SUA_CHAVE") || _apiKey.Length < 8)
                {
                    return new OmdbMovie
                    {
                        Response = "False",
                        Error = "Chave API da OMDB não configurada"
                    };
                }

                // ✅ CORRIGIDO: HTTPS
                var url = $"https://www.omdbapi.com/?i={Uri.EscapeDataString(imdbId)}&apikey={_apiKey}";

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return new OmdbMovie
                    {
                        Response = "False",
                        Error = $"Erro HTTP: {response.StatusCode}"
                    };
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var options = new Stj.JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var movie = Stj.JsonSerializer.Deserialize<OmdbMovie>(jsonResponse, options);

                return movie ?? new OmdbMovie
                {
                    Response = "False",
                    Error = "Falha na deserialização"
                };
            }
            catch (Exception ex)
            {
                return new OmdbMovie
                {
                    Response = "False",
                    Error = $"Erro: {ex.Message}"
                };
            }
        }
    }
}
