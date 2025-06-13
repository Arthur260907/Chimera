namespace PagamentoSeguroAPI.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using PagamentoSeguroAPI.Models; // Importa as classes do nosso modelo
    using Stripe; // Importa a biblioteca do Stripe

    [ApiController]
    [Route("api/[controller]")]
    public class PagamentoController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        // O construtor recebe a configuração para podermos ler a chave secreta do Stripe.
        public PagamentoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("processar-assinatura")]
        public async Task<IActionResult> ProcessarAssinatura([FromBody] PagamentoRequest request)
        {
            // Validação inicial dos dados recebidos do frontend.
            if (request == null || string.IsNullOrWhiteSpace(request.PaymentMethodId) || string.IsNullOrWhiteSpace(request.UserEmail))
            {
                return BadRequest(new PagamentoResponse { Status = "falha", ErrorMessage = "Dados inválidos." });
            }

            try
            {
                // Configura a chave SECRETA do Stripe. Ela é lida do arquivo `appsettings.json`
                // e NUNCA deve ser exposta no código do frontend.
                StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

                // --- PASSO 1: CRIAR O CLIENTE (CUSTOMER) NO STRIPE ---
                // Um "Customer" no Stripe representa o seu usuário.
                // Isso permite salvar métodos de pagamento para cobranças futuras.
                var customerOptions = new CustomerCreateOptions
                {
                    Email = request.UserEmail,
                    PaymentMethod = request.PaymentMethodId, // Associa o cartão ao cliente
                    InvoiceSettings = new CustomerInvoiceSettingsOptions
                    {
                        // Define este cartão como o método de pagamento padrão para futuras faturas.
                        DefaultPaymentMethod = request.PaymentMethodId,
                    },
                };
                var customerService = new CustomerService();
                Customer customer = await customerService.CreateAsync(customerOptions);

                Console.WriteLine($"Stripe Customer criado com sucesso: {customer.Id} para o email {customer.Email}");

                // --- PASSO 2: SALVAR O ID DO CLIENTE NO SEU BANCO DE DADOS ---
                // ESTE é o momento de interagir com o seu `DataBase.cs` ou DbContext.
                // Você vai associar o ID do cliente do Stripe ("cus_...") ao seu usuário.
                //
                // Exemplo de como seria com Entity Framework:
                //
                // var usuario = _meuDbContext.Usuarios.FirstOrDefault(u => u.Email == request.UserEmail);
                // if (usuario != null)
                // {
                //     usuario.StripeCustomerId = customer.Id;
                //     await _meuDbContext.SaveChangesAsync();
                // }
                //
                // NUNCA SALVE o PaymentMethodId. Ele é de uso único para criar o cliente.

                // --- PASSO 3: CRIAR A ASSINATURA (SUBSCRIPTION) ---
                // Agora que temos o cliente, podemos criar uma assinatura para cobrar um valor recorrente.
                var subscriptionOptions = new SubscriptionCreateOptions
                {
                    Customer = customer.Id, // Cobra o cliente que acabamos de criar
                    Items = new List<SubscriptionItemOptions>
                    {
                        new SubscriptionItemOptions
                        {
                            // O "Price ID" é o ID do plano (ex: R$ 59,90/mês) que você cria
                            // no seu painel (dashboard) do Stripe.
                            Price = _configuration["Stripe:PriceId"],
                        },
                    },
                    // Expande para recebermos o status do pagamento da primeira fatura imediatamente.
                    PaymentSettings = new SubscriptionPaymentSettingsOptions
                    {
                        PaymentMethodTypes = new List<string> { "card" },
                        SaveDefaultPaymentMethod = "on_subscription",
                    },
                    Expand = new List<string> { "latest_invoice.payment_intent" },
                };
                var subscriptionService = new SubscriptionService();
                Subscription subscription = await subscriptionService.CreateAsync(subscriptionOptions);
                
                Console.WriteLine($"Stripe Subscription criada com sucesso: {subscription.Id}");

                // Retorna uma resposta de sucesso para o frontend com os IDs gerados.
                return Ok(new PagamentoResponse
                {
                    Status = "sucesso",
                    CustomerId = customer.Id,
                    SubscriptionId = subscription.Id
                });
            }
            catch (StripeException e)
            {
                // Se a API do Stripe retornar um erro (ex: cartão recusado), nós o capturamos.
                Console.WriteLine($"Erro do Stripe: {e.StripeError.Message}");
                return BadRequest(new PagamentoResponse { Status = "falha", ErrorMessage = e.StripeError.Message });
            }
            catch (Exception ex)
            {
                // Captura qualquer outro erro inesperado no nosso servidor.
                 Console.WriteLine($"Erro inesperado: {ex.Message}");
                return StatusCode(500, new PagamentoResponse { Status = "falha", ErrorMessage = "Ocorreu um erro interno no servidor." });
            }
        }
    }
}