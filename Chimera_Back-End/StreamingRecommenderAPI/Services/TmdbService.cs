// Caminho: Services/TmdbService.cs
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using StreamingRecommenderAPI.Models.Midia.Tmdb; // Using corrigido

namespace StreamingRecommenderAPI.Services
{
    public class TmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl = "https://api.themoviedb.org/3";
        private TmdbImageConfiguration? _config;
        private DateTime _configLastFetched;

        public TmdbService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["ApiKeys:Tmdb"];
            if (string.IsNullOrEmpty(_apiKey) || _apiKey.Contains("SUA_CHAVE"))
            {
                Console.Error.WriteLine("API Key do TMDB não configurada corretamente em appsettings.json!");
            }
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("ChimeraApp/1.0"); // Adiciona User-Agent
        }

        private async Task<TmdbImageConfiguration?> GetConfigurationAsync()
        {
            if (_config != null && (DateTime.UtcNow - _configLastFetched).TotalHours < 24) return _config;
            if (string.IsNullOrEmpty(_apiKey)) return null;

            try
            {
                var url = $"{_baseUrl}/configuration?api_key={_apiKey}";
                _config = await _httpClient.GetFromJsonAsync<TmdbImageConfiguration>(url);
                _configLastFetched = DateTime.UtcNow;
                return _config;
            }
            catch (HttpRequestException ex) { Console.Error.WriteLine($"Erro HTTP config TMDB: {ex.StatusCode} - {ex.Message}"); return null; }
            catch (Exception ex) { Console.Error.WriteLine($"Erro config TMDB: {ex.Message}"); return null; }
        }

        public async Task<int?> FindMovieByImdbIdAsync(string imdbId)
        {
            if (string.IsNullOrEmpty(imdbId) || string.IsNullOrEmpty(_apiKey)) return null;
            try
            {
                var url = $"{_baseUrl}/find/{imdbId}?api_key={_apiKey}&external_source=imdb_id";
                var result = await _httpClient.GetFromJsonAsync<TmdbFindResult>(url);
                return result?.MovieResults?.FirstOrDefault()?.Id;
            }
            catch (HttpRequestException ex) { Console.Error.WriteLine($"Erro HTTP find TMDB '{imdbId}': {ex.StatusCode} - {ex.Message}"); return null; }
            catch (Exception ex) { Console.Error.WriteLine($"Erro find TMDB '{imdbId}': {ex.Message}"); return null; }
        }

        public async Task<TmdbImagesResponse?> GetMovieImagesAsync(int tmdbId)
        {
            if (tmdbId <= 0 || string.IsNullOrEmpty(_apiKey)) return null;
            try
            {
                var url = $"{_baseUrl}/movie/{tmdbId}/images?api_key={_apiKey}";
                return await _httpClient.GetFromJsonAsync<TmdbImagesResponse>(url);
            }
            catch (HttpRequestException ex) { Console.Error.WriteLine($"Erro HTTP images TMDB '{tmdbId}': {ex.StatusCode} - {ex.Message}"); return null; }
            catch (Exception ex) { Console.Error.WriteLine($"Erro images TMDB '{tmdbId}': {ex.Message}"); return null; }
        }

        public async Task<TmdbVideosResponse?> GetMovieVideosAsync(int tmdbId, string language = "en-US")
        {
            if (tmdbId <= 0 || string.IsNullOrEmpty(_apiKey)) return null;
            try
            {
                var url = $"{_baseUrl}/movie/{tmdbId}/videos?api_key={_apiKey}&language={language}";
                return await _httpClient.GetFromJsonAsync<TmdbVideosResponse>(url);
            }
            catch (HttpRequestException ex) { Console.Error.WriteLine($"Erro HTTP videos TMDB '{tmdbId}': {ex.StatusCode} - {ex.Message}"); return null; }
            catch (Exception ex) { Console.Error.WriteLine($"Erro videos TMDB '{tmdbId}': {ex.Message}"); return null; }
        }

        public async Task<string?> GetFullPosterUrlAsync(string? filePath, string size = "w500")
        {
            if (string.IsNullOrEmpty(filePath)) return null;
            var config = await GetConfigurationAsync();
            if (config?.Images?.SecureBaseUrl == null || config.Images.PosterSizes == null) return null;

            string selectedSize = size;
            if (!config.Images.PosterSizes.Contains(size))
            {
                selectedSize = config.Images.PosterSizes.ElementAtOrDefault(^2) ?? config.Images.PosterSizes.LastOrDefault() ?? "original";
            }
            if (!filePath.StartsWith("/")) { filePath = "/" + filePath; }
            return $"{config.Images.SecureBaseUrl}{selectedSize}{filePath}";
        }

         public string? FindBestTrailerUrl(TmdbVideosResponse? videosResponse)
         {
             if (videosResponse?.Results == null || !videosResponse.Results.Any()) return null;

             var trailer = videosResponse.Results
                 .Where(v => v != null && v.Site != null && v.Site.Equals("YouTube", StringComparison.OrdinalIgnoreCase) &&
                             v.Type != null && v.Type.Equals("Trailer", StringComparison.OrdinalIgnoreCase) &&
                             !string.IsNullOrEmpty(v.Key))
                 .OrderByDescending(v => v.Official)
                 .ThenByDescending(v => v.Language != null && v.Language.Equals("en", StringComparison.OrdinalIgnoreCase))
                 .FirstOrDefault();

             if (trailer != null) return $"https://www.youtube.com/embed/{trailer.Key}";
             return null;
         }
    }
}