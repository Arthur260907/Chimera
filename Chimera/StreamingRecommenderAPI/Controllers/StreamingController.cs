using Microsoft.AspNetCore.Mvc;
using StreamingRecommenderAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StreamingRecommenderAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StreamingController : ControllerBase
    {
        // Nossa "base de dados" simulada, agora usando Dictionaries em C#
        private static readonly Dictionary<string, StreamingService> ServicesData = new Dictionary<string, StreamingService>(StringComparer.OrdinalIgnoreCase)
        {
            { "Netflix", new StreamingService { Price = 44.90M, Plan = "Padrão (sem anúncios)" } },
            { "Max", new StreamingService { Price = 39.90M, Plan = "Padrão (sem anúncios)" } },
            { "Prime Video", new StreamingService { Price = 19.90M, Plan = "Padrão" } },
            { "Disney+", new StreamingService { Price = 43.90M, Plan = "Padrão" } }
        };

        private static readonly Dictionary<string, List<string>> TitlesData = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
        {
            { "breaking bad", new List<string> { "Netflix" } },
            { "hora de aventura", new List<string> { "Max", "Prime Video" } },
            { "ultimate homem aranha", new List<string> { "Disney+" } },
            { "ozark", new List<string> { "Netflix" } },
            { "the boys", new List<string> { "Prime Video" } },
            { "a casa do dragão", new List<string> { "Max" } }
        };

        [HttpGet("recommendation-dynamic")]
        public IActionResult GetDynamicRecommendation([FromQuery] string titles, [FromQuery] decimal budget = 80.0M)
        {
            if (string.IsNullOrWhiteSpace(titles))
            {
                return BadRequest(new { error = "Nenhum título fornecido. Use o parâmetro 'titles' (separados por vírgula)." });
            }

            var requestedTitles = titles.Split(',').Select(t => t.Trim().ToLower()).ToList();

            // 1. Mapear os serviços necessários para os títulos desejados
            var requiredServices = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var unfoundTitles = new List<string>();

            foreach (var title in requestedTitles)
            {
                if (TitlesData.ContainsKey(title))
                {
                    // Adiciona todos os serviços que contêm o título
                    TitlesData[title].ForEach(service => requiredServices.Add(service));
                }
                else
                {
                    unfoundTitles.Add(title);
                }
            }

            // 2. Encontrar a combinação de serviços mais barata que cobre tudo
            HashSet<string> bestCombo = null;
            decimal minCost = decimal.MaxValue;

            var allAvailableServices = ServicesData.Keys.ToList();
            // Itera sobre todas as combinações possíveis de serviços (de 1 até N)
            for (int i = 1; i <= allAvailableServices.Count; i++)
            {
                foreach (var combo in GetCombinations(allAvailableServices, i))
                {
                    var currentCombo = new HashSet<string>(combo, StringComparer.OrdinalIgnoreCase);
                    // Verifica se a combinação atual cobre todos os serviços necessários
                    if (requiredServices.IsSubsetOf(currentCombo))
                    {
                        var cost = currentCombo.Sum(serviceName => ServicesData[serviceName].Price);
                        if (cost < minCost)
                        {
                            minCost = cost;
                            bestCombo = currentCombo;
                        }
                    }
                }
            }

            // 3. Construir a resposta com base na melhor combinação encontrada
            var response = new RecommendationResponse
            {
                RequestedTitles = requestedTitles,
                Budget = budget,
                UnfoundTitles = unfoundTitles
            };

            if (bestCombo != null)
            {
                var comboDetails = new RecommendedCombo
                {
                    Services = bestCombo.Select(s => new ServiceInfo { Name = s, Plan = ServicesData[s].Plan, Price = ServicesData[s].Price }).ToList(),
                    TotalCost = minCost
                };

                if (minCost <= budget)
                {
                    response.Status = "Combinação encontrada dentro do orçamento!";
                    response.Recommendation = comboDetails;
                }
                else
                {
                    response.Status = "Não foi possível encontrar uma combinação dentro do orçamento.";
                    response.Suggestion = "Considere alternar assinaturas ou aumentar o orçamento.";
                    response.CheapestOptionFound = comboDetails;
                }
            }
            else
            {
                 response.Status = "Não foi possível encontrar os títulos solicitados em nossa base.";
            }

            return Ok(response);
        }

        // Função auxiliar para gerar combinações, já que C# não tem um `itertools.combinations` nativo.
        private static IEnumerable<IEnumerable<T>> GetCombinations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });

            return GetCombinations(list, length - 1)
                .SelectMany(t => list.Where(o => Comparer<T>.Default.Compare(o, t.Last()) > 0),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }
    }
}