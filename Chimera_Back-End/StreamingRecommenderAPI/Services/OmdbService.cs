using Newtonsoft.Json;
using StreamingRecommenderAPI.Models;
using StreamingRecommenderAPI.Models.Midia;
using StreamingRecommenderAPI.Repositories;
using System.Text.Json;
using Stj = System.Text.Json;
using Microsoft.Extensions.Configuration;
using StreamingRecommenderAPI.Data;
using System.Linq; // Adicionado para Any()
using System.Collections.Generic; // Adicionado para List<>
using System; // Adicionado para Math.Ceiling, StringComparison, Exception
using System.Net.Http; // Adicionado para HttpClient, HttpResponseMessage
using System.Threading.Tasks; // Adicionado para Task<>

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
            _baseUrl = configuration["ApiUrls:Omdb"] ?? "https://www.omdbapi.com/"; // Usa HTTPS por padrão se não especificado
            _dbContext = dbContext;
        }

        public async Task<OmdbMovie> GetMovieByTitleAsync(string title)
        {
            try
            {
                if (string.IsNullOrEmpty(_apiKey) || _apiKey.Contains("SUA_CHAVE") || _apiKey.Length < 8)
                {
                    return new OmdbMovie { Response = "False", Error = "Chave API da OMDB não configurada. Configure em appsettings.json" };
                }

                var url = $"{_baseUrl}?t={Uri.EscapeDataString(title)}&apikey={_apiKey}";
                Console.WriteLine($"🔍 Request URL (GetMovieByTitleAsync): {url}");

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return new OmdbMovie { Response = "False", Error = $"Erro HTTP: {response.StatusCode}" };
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"📄 JSON Response (GetMovieByTitleAsync): {jsonResponse}");

                var options = new Stj.JsonSerializerOptions { PropertyNameCaseInsensitive = true, AllowTrailingCommas = true };
                var movie = Stj.JsonSerializer.Deserialize<OmdbMovie>(jsonResponse, options);

                if (movie == null)
                {
                    return new OmdbMovie { Response = "False", Error = "Falha na deserialização do JSON" };
                }

                await SaveMovieToDbAsync(movie);
                return movie;
            }
            catch (Exception ex)
            {
                return new OmdbMovie { Response = "False", Error = $"Erro: {ex.Message}" };
            }
        }

        public async Task<OmdbMovie> GetMovieByIdAsync(string imdbId)
        {
            try
            {
                if (string.IsNullOrEmpty(_apiKey) || _apiKey.Contains("SUA_CHAVE") || _apiKey.Length < 8)
                {
                    return new OmdbMovie { Response = "False", Error = "Chave API da OMDB não configurada" };
                }

                var url = $"{_baseUrl}?i={Uri.EscapeDataString(imdbId)}&apikey={_apiKey}";
                Console.WriteLine($"🔍 Request URL (GetMovieByIdAsync): {url}");

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return new OmdbMovie { Response = "False", Error = $"Erro HTTP: {response.StatusCode}" };
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"📄 JSON Response (GetMovieByIdAsync): {jsonResponse}");

                var options = new Stj.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var movie = Stj.JsonSerializer.Deserialize<OmdbMovie>(jsonResponse, options);

                if (movie == null)
                {
                    return new OmdbMovie { Response = "False", Error = "Falha na deserialização" };
                }

                await SaveMovieToDbAsync(movie);
                return movie;
            }
            catch (Exception ex)
            {
                return new OmdbMovie { Response = "False", Error = $"Erro: {ex.Message}" };
            }
        }

        // --- MÉTODO MODIFICADO COM PAGINAÇÃO E LOGS ---
        public async Task<OmdbSearchResult?> SearchMediaByTitleAsync(string title, string? type = null, int maxResults = 20)
        {
            Console.WriteLine($"[LOG] Iniciando SearchMediaByTitleAsync para '{title}', tipo='{type ?? "any"}', maxResults={maxResults}"); // Log inicial
            List<OmdbMovie> allResults = new List<OmdbMovie>();
            string? totalResultsStr = "0";
            string? firstError = null; // Use Nullable<string> or string?
            int page = 1;
            int resultsFetched = 0;
            int maxPagesToFetch = (int)Math.Ceiling((double)maxResults / 10.0);
            Console.WriteLine($"[LOG] Calculado maxPagesToFetch = {maxPagesToFetch}");

            while (resultsFetched < maxResults && page <= maxPagesToFetch)
            {
                Console.WriteLine($"[LOG] Loop: Tentando buscar página {page}");
                string apiUrl = $"{_baseUrl}?apikey={_apiKey}&s={Uri.EscapeDataString(title)}&page={page}";
                if (!string.IsNullOrEmpty(type)) { apiUrl += $"&type={type}"; }
                Console.WriteLine($"[LOG] URL da página {page}: {apiUrl}");

                try
                {
                    HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
                    Console.WriteLine($"[LOG] Página {page}: Status HTTP = {response.StatusCode}");

                    if (!response.IsSuccessStatusCode)
                    {
                        if (page == 1)
                        {
                            Console.WriteLine($"[ERRO] HTTP (página {page}) para '{title}': {response.StatusCode}. Retornando erro.");
                            return new OmdbSearchResult { Search = new List<OmdbMovie>(), totalResults = "0", Response = "False", Error = $"HTTP Error: {response.StatusCode}" };
                        }
                        Console.WriteLine($"[AVISO] HTTP (página {page}) para '{title}': {response.StatusCode}. Parando busca de páginas adicionais.");
                        break;
                    }

                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    // Console.WriteLine($"[LOG] Página {page}: JSON Response = {jsonResponse}"); // Descomente se precisar ver o JSON cru

                    var options = new Stj.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var searchResultPage = Stj.JsonSerializer.Deserialize<OmdbSearchResult>(jsonResponse, options);

                    // Verifica erro na resposta da API
                    if (searchResultPage != null && searchResultPage.Response.Equals("False", StringComparison.OrdinalIgnoreCase))
                    {
                        if (page == 1)
                        {
                            Console.WriteLine($"[ERRO] OMDB API (página {page}) para '{title}': {searchResultPage.Error}. Retornando erro.");
                            if (firstError == null) firstError = searchResultPage.Error; // Guarda o primeiro erro
                            // Mesmo com erro na pag 1, retornamos a estrutura com o erro dentro
                            return new OmdbSearchResult { Search = allResults, totalResults = totalResultsStr, Response = "False", Error = firstError };
                        }
                        Console.WriteLine($"[INFO] OMDB API respondeu 'False' (página {page}) para '{title}'. Parando busca. Erro: {searchResultPage.Error}");
                        break;
                    }

                    // Se a busca deu certo nesta página
                    if (searchResultPage?.Search != null && searchResultPage.Search.Any())
                    {
                        int countPagina = searchResultPage.Search.Count;
                        Console.WriteLine($"[LOG] Página {page}: Encontrados {countPagina} resultados. Response='{searchResultPage.Response}', TotalReportado='{searchResultPage.totalResults}'");
                        if (page == 1)
                        {
                            totalResultsStr = searchResultPage.totalResults;
                        }

                        int addedCount = 0;
                        foreach (var item in searchResultPage.Search)
                        {
                            if (resultsFetched < maxResults)
                            {
                                await SaveMovieToDbAsync(item);
                                allResults.Add(item);
                                resultsFetched++;
                                addedCount++;
                            }
                            else
                            {
                                Console.WriteLine($"[LOG] Limite de {maxResults} resultados atingido ao adicionar item. Parando foreach.");
                                break;
                            }
                        }
                        Console.WriteLine($"[LOG] Adicionados {addedCount} resultados da página {page}. Total acumulado: {resultsFetched}");

                        if (resultsFetched >= maxResults)
                        {
                            Console.WriteLine($"[LOG] Limite de {maxResults} resultados atingido após processar página {page}. Saindo do while.");
                            break;
                        }

                        if (int.TryParse(totalResultsStr, out int total) && allResults.Count >= total)
                        {
                            Console.WriteLine($"[LOG] Todos os {total} resultados reportados pela API foram buscados. Saindo do while.");
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"[LOG] Página {page}: Nenhum resultado encontrado (Search foi nulo ou vazio), apesar de Response='{searchResultPage?.Response}'. Parando busca.");
                        // Se nem a primeira página retornou, retorna o resultado (que pode ser Response="True" mas Search=null)
                        if (page == 1) return searchResultPage ?? new OmdbSearchResult { Search = new List<OmdbMovie>(), totalResults = "0", Response = "True" };
                        break;
                    }

                    page++;
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"[ERRO] HttpRequestException (página {page}) para '{title}': {e.Message}");
                    if (page == 1) return null;
                    else break;
                }
                catch (Stj.JsonException e)
                {
                    Console.WriteLine($"[ERRO] JsonException (página {page}) para '{title}': {e.Message}");
                    if (page == 1) return null;
                    else break;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[ERRO] Exceção inesperada (página {page}) ao buscar '{title}': {e.ToString()}"); // Log completo da exceção
                    if (page == 1) return null;
                    else break;
                }
            }

            Console.WriteLine($"[LOG] Fim do loop while. Total de resultados coletados: {allResults.Count}");
            return new OmdbSearchResult
            {
                Search = allResults,
                totalResults = totalResultsStr,
                Response = allResults.Any() ? "True" : (firstError != null ? "False" : "True"), // Se teve erro na pag 1, Response=False
                Error = firstError
            };
        }
        // --- FIM DO MÉTODO MODIFICADO ---

        public class OmdbSearchResult
        {
            public List<OmdbMovie>? Search { get; set; }
            public string? totalResults { get; set; }
            public string Response { get; set; } = "False";
            public string? Error { get; set; }
        }

        private async Task SaveMovieToDbAsync(OmdbMovie movie)
        {
            if (movie == null || string.IsNullOrEmpty(movie.ImdbID))
                return;

            try
            {
                var existing = await _dbContext.Filmes.FindAsync(movie.ImdbID);
                if (existing == null)
                {
                    await _dbContext.Filmes.AddAsync(movie);
                }
                else
                {
                    // Atualiza campos
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
                    // Note: Não estamos atualizando existing.Ratings aqui, pois [NotMapped]

                    _dbContext.Filmes.Update(existing);
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar filme no banco: {ex.Message}");
                // Considerar relançar ou logar com mais detalhes dependendo da política de erro
            }
        }
    }
}