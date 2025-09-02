using Microsoft.AspNetCore.Mvc;
using StreamingRecommenderAPI.Models.User.DTOs;
using StreamingRecommenderAPI.Services;
using System.Threading.Tasks;

namespace StreamingRecommenderAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _service;

        public UsuarioController(UsuarioService service)
        {
            _service = service;
        }

        [HttpPost("recuperar-senha")]
        public async Task<IActionResult> RecuperarSenha([FromBody] RecuperarSenhaRequest request)
        {
            await _service.SolicitarRecuperacaoSenhaAsync(request.Email);
            return Ok(new { message = "Se o e-mail estiver cadastrado, instruções serão enviadas." });
        }

        [HttpPost("redefinir-senha")]
        public async Task<IActionResult> RedefinirSenha([FromBody] RedefinirSenhaRequest request)
        {
            var sucesso = await _service.RedefinirSenhaAsync(request.Token, request.NovaSenha);
            if (!sucesso) return BadRequest(new { message = "Token inválido ou expirado." });
            
            return Ok(new { message = "Senha redefinida com sucesso." });
        }

        [HttpPost("cadastrar")]
        public async Task<IActionResult> Cadastrar([FromBody] CadastroRequest request)
        {
            var sucesso = await _service.CadastrarUsuarioAsync(request.Nome, request.Email, request.Senha);
            if (!sucesso)
            {
                return BadRequest(new { message = "E-mail já cadastrado." });
            }
            return Ok(new { message = "Usuário cadastrado com sucesso!" });
        }
    }

    // DTO para o request de cadastro
    public class CadastroRequest
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
    }
}