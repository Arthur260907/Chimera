// Arquivo: Services/Interfaces/IEmailService.cs
using System.Threading.Tasks;

namespace StreamingRecommenderAPI.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlMessage);
    }
}