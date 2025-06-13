using System.Collections.Generic;

// Namespace para organizar nossas classes
namespace StreamingRecommenderAPI.Models
{
    // Representa um único serviço de streaming
    public class StreamingService
    {
        public string Plan { get; set; }
        public decimal Price { get; set; }
    }

    // Representa a resposta da nossa recomendação
    public class RecommendationResponse
    {
        public List<string> RequestedTitles { get; set; }
        public decimal Budget { get; set; }
        public string Status { get; set; }
        public RecommendedCombo Recommendation { get; set; }
        public RecommendedCombo CheapestOptionFound { get; set; } // Usado se estourar o orçamento
        public List<string> UnfoundTitles { get; set; }
        public string Suggestion { get; set; }
    }

    // Representa a combinação de serviços recomendada
    public class RecommendedCombo
    {
        public List<ServiceInfo> Services { get; set; }
        public decimal TotalCost { get; set; }
    }

    // Informações detalhadas de um serviço na resposta
    public class ServiceInfo
    {
        public string Name { get; set; }
        public string Plan { get; set; }
        public decimal Price { get; set; }
    }
}