namespace PagamentoSeguroAPI.Models
{
    // Classe para representar os dados que recebemos do frontend.
    // Contém apenas o ID seguro do método de pagamento.
    public class PagamentoRequest
    {
        public string PaymentMethodId { get; set; }
        public string UserEmail { get; set; } // Adicionamos o email para criar o cliente
    }

    // Classe para representar a resposta que enviamos de volta para o frontend.
    public class PagamentoResponse
    {
        public string Status { get; set; }
        public string CustomerId { get; set; }
        public string SubscriptionId { get; set; }
        public string ErrorMessage { get; set; }
    }
}