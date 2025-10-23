using Newtonsoft.Json;
using StreamingRecommenderAPI.Models;
using StreamingRecommenderAPI.Models.Midia;
using StreamingRecommenderAPI.Repositories;
using System.Text.Json;
using Stj = System.Text.Json;
using Microsoft.Extensions.Configuration;


namespace StreamingRecommenderAPI.Services
{
    public class OmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public OmdbService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["ApiKeys:Omdb"];
            _baseUrl = configuration["ApiUrls:Omdb"] ?? "https://www.omdbapi.com/";
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
        // Dentro da classe OmdbService
        public async Task<OmdbSearchResult?> SearchMediaByTitleAsync(string title, string? type = null) // Adiciona type opcional
        {
            string apiUrl = $"{_baseUrl}?apikey={_apiKey}&s={Uri.EscapeDataString(title)}"; // Busca por título (s=)

            if (!string.IsNullOrEmpty(type)) // Adiciona o tipo se fornecido
            {
                apiUrl += $"&type={type}"; // ex: &type=movie ou &type=series
            }

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode(); // Lança exceção se a resposta não for 2xx

                string jsonResponse = await response.Content.ReadAsStringAsync();

                // Desserializa a resposta JSON
                var searchResult = Stj.JsonSerializer.Deserialize<OmdbSearchResult>(jsonResponse, new Stj.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Lida com diferenças de maiúsculas/minúsculas
                });

                // Verifica se a resposta da API indica um erro
                if (searchResult != null && searchResult.Response.Equals("False", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"OMDB API Error for query '{title}' (type: {type ?? "any"}): {searchResult.Error}");
                    return new OmdbSearchResult { Search = new List<OmdbMovie>(), totalResults = "0", Response = "False", Error = searchResult.Error }; // Retorna resultado vazio com erro
                }


                return searchResult;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Erro na requisição HTTP para buscar '{title}': {e.Message}");
                return null; // Ou lançar uma exceção específica
            }
            catch (Stj.JsonException e)
            {
                Console.WriteLine($"Erro ao desserializar JSON para buscar '{title}': {e.Message}");
                return null; // Ou lançar uma exceção específica
            }
            catch (Exception e) // Captura outras exceções inesperadas
            {
                Console.WriteLine($"Erro inesperado ao buscar '{title}': {e.Message}");
                return null;
            }
        }

        // Você pode manter o método antigo ou removê-lo se o novo cobrir todos os casos
        public async Task<OmdbSearchResult?> SearchMoviesByTitleAsync(string title)
        {
            // Pode simplesmente chamar o novo método
            return await SearchMediaByTitleAsync(title, "movie");
        }

        // Adicione também o OmdbSearchResult se ainda não existir
        public class OmdbSearchResult
        {
            public List<OmdbMovie>? Search { get; set; }
            public string? totalResults { get; set; }
            public string Response { get; set; } = "False"; // Default para False
            public string? Error { get; set; } // Para capturar mensagens de erro da API
        }
    }
}
