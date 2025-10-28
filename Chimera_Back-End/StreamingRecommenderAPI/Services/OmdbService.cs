using Newtonsoft.Json;
using StreamingRecommenderAPI.Models;
using StreamingRecommenderAPI.Models.Midia;
using StreamingRecommenderAPI.Repositories;
using System.Text.Json;
using Stj = System.Text.Json;
using Microsoft.Extensions.Configuration;
using StreamingRecommenderAPI.Data;


namespace StreamingRecommenderAPI.Services
{
    public class OmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly ApplicationDbContext _dbContext;

        public OmdbService(HttpClient httpClient, IConfiguration configuration, ApplicationDbContext dbContext)
        {
            _httpClient = httpClient;
            _apiKey = configuration["ApiKeys:Omdb"];
            _baseUrl = configuration["ApiUrls:Omdb"] ?? "https://www.omdbapi.com/";
            _dbContext = dbContext;
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

                // Salva/atualiza no banco de dados
                await SaveMovieToDbAsync(movie);

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

                if (movie == null)
                {
                    return new OmdbMovie
                    {
                        Response = "False",
                        Error = "Falha na deserialização"
                    };
                }

                // Salva/atualiza no banco de dados
                await SaveMovieToDbAsync(movie);

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

                // Salva os itens retornados (se houver) no banco de dados
                if (searchResult?.Search != null)
                {
                    foreach (var item in searchResult.Search)
                    {
                        // Salva/atualiza cada item
                        await SaveMovieToDbAsync(item);
                    }
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

        // Helper para inserir/atualizar filmes no banco
        private async Task SaveMovieToDbAsync(OmdbMovie movie)
        {
            if (movie == null || string.IsNullOrEmpty(movie.ImdbID))
                return;

            try
            {
                var existing = await _dbContext.Filmes.FindAsync(movie.ImdbID);
                if (existing == null)
                {
                    // Insere novo registro
                    await _dbContext.Filmes.AddAsync(movie);
                }
                else
                {
                    // Atualiza campos relevantes
                    existing.Title = movie.Title ?? existing.Title;
                    existing.Year = movie.Year ?? existing.Year;
                    existing.Rated = movie.Rated ?? existing.Rated;
                    existing.Released = movie.Released ?? existing.Released;
                    existing.Runtime = movie.Runtime ?? existing.Runtime;
                    existing.Genre = movie.Genre ?? existing.Genre;
                    existing.Director = movie.Director ?? existing.Director;
                    existing.Writer = movie.Writer ?? existing.Writer;
                    existing.Actors = movie.Actors ?? existing.Actors;
                    existing.Plot = movie.Plot ?? existing.Plot;
                    existing.Language = movie.Language ?? existing.Language;
                    existing.Country = movie.Country ?? existing.Country;
                    existing.Awards = movie.Awards ?? existing.Awards;
                    existing.Poster = movie.Poster ?? existing.Poster;
                    existing.ImdbRating = movie.ImdbRating ?? existing.ImdbRating;
                    existing.Type = movie.Type ?? existing.Type;
                    existing.Response = movie.Response ?? existing.Response;
                    existing.Error = movie.Error ?? existing.Error;

                    _dbContext.Filmes.Update(existing);
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar filme no banco: {ex.Message}");
            }
        }
    }
}
