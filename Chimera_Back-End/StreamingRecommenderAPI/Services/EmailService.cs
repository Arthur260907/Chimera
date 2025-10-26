// Arquivo: Services/EmailService.cs
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using StreamingRecommenderAPI.Models;
using StreamingRecommenderAPI.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace StreamingRecommenderAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettingsAccessor)
        {
            _emailSettings = emailSettingsAccessor.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            if (string.IsNullOrEmpty(toEmail) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(htmlMessage))
            {
                 Console.WriteLine("[ERRO EMAIL] Destinatário, Assunto ou Mensagem vazios.");
                 throw new ArgumentException("Destinatário, Assunto ou Mensagem não podem ser vazios.");
            }
             if (string.IsNullOrEmpty(_emailSettings.SmtpServer) || string.IsNullOrEmpty(_emailSettings.SenderEmail) || string.IsNullOrEmpty(_emailSettings.SmtpPass) || _emailSettings.SmtpPort <= 0)
             {
                 Console.WriteLine("[ERRO EMAIL] Configurações de Email (EmailSettings) incompletas ou inválidas no appsettings.");
                 throw new InvalidOperationException("Configurações de Email incompletas ou inválidas.");
             }

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            emailMessage.To.Add(MailboxAddress.Parse(toEmail));
            emailMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                Console.WriteLine($"[EMAIL] Conectando a {_emailSettings.SmtpServer}:{_emailSettings.SmtpPort}...");
                // Conecta - StartTlsWhenAvailable tenta a conexão segura primeiro
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTlsWhenAvailable);
                Console.WriteLine("[EMAIL] Conectado. Autenticando...");

                // Autentica (se o servidor exigir)
                await client.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpPass);
                Console.WriteLine("[EMAIL] Autenticado. Enviando email...");

                // Envia
                await client.SendAsync(emailMessage);
                Console.WriteLine($"[EMAIL] Email enviado com sucesso para {toEmail}");

            }
            catch (AuthenticationException ex) {
                 Console.WriteLine($"[ERRO EMAIL] Autenticação SMTP falhou: {ex.Message}. Verifique usuário/senha/senha de app e configurações de segurança.");
                 throw;
            }
            catch (SmtpCommandException ex) {
                Console.WriteLine($"[ERRO EMAIL] Comando SMTP falhou: {ex.StatusCode} - {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO EMAIL] Erro GERAL ao enviar email para {toEmail}: {ex.ToString()}");
                throw;
            }
            finally // Garante a desconexão
            {
                 if (client.IsConnected)
                 {
                     await client.DisconnectAsync(true);
                     Console.WriteLine("[EMAIL] Desconectado do servidor SMTP.");
                 }
            }
        }
    }
}