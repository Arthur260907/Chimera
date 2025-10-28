using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StreamingRecommenderAPI.Data;
using StreamingRecommenderAPI.Models.User;
using StreamingRecommenderAPI.Services; // Adicionado using para UsuarioService
using StreamingRecommenderAPI.Models.User.DTOs; // Adicionado using para DTOs

namespace StreamingRecommenderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UsuarioService _usuarioService;

        public UsuariosController(ApplicationDbContext context, UsuarioService usuarioService)
        {
            _context = context;
            _usuarioService = usuarioService;
        }

        // --- Endpoints GET, PUT, DELETE (Mantidos) ---
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();
            return usuario;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Id) return BadRequest();
            _context.Entry(usuario).State = EntityState.Modified;
            _context.Entry(usuario).Property(x => x.Senha).IsModified = false; // Não permite mudar senha aqui
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException) { if (!UsuarioExists(id)) { return NotFound(); } else { throw; } }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) { return NotFound(); }
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // --- Endpoint de Cadastro ---
        [HttpPost("cadastro")]
        public async Task<IActionResult> CadastrarUsuario([FromBody] CadastroRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Nome) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Senha))
                return BadRequest(new { message = "Nome, email e senha são obrigatórios." });
            try
            {
                bool sucesso = await _usuarioService.CadastrarUsuarioAsync(request.Nome, request.Email, request.Senha);
                if (sucesso) return StatusCode(StatusCodes.Status201Created, new { message = "Usuário cadastrado com sucesso." });
                else return BadRequest(new { message = "Email já cadastrado." });
            }
            catch (Exception ex) { Console.WriteLine($"Erro ao cadastrar: {ex.Message}"); return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno ao cadastrar." }); }
        }

        // --- Endpoint de Login ---
        [HttpPost("login")]
        public async Task<IActionResult> LoginUsuario([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Senha))
                return BadRequest(new { message = "Email e senha são obrigatórios." });
            try
            {
                var usuario = await _usuarioService.LoginAsync(request.Email, request.Senha);
                if (usuario != null) return Ok(new { username = usuario.Nome, email = usuario.Email });
                else return Unauthorized(new { message = "Email ou senha inválidos." });
            }
            catch (Exception ex) { Console.WriteLine($"Erro no login: {ex.Message}"); return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno durante o login." }); }
        }

        // --- Endpoint de Solicitar Recuperação ---
        // POST: api/Usuarios/recuperar-senha
        [HttpPost("recuperar-senha")]
        public async Task<IActionResult> SolicitarRecuperacao([FromBody] RecuperarSenhaRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new { message = "Email é obrigatório." });
            }
            try
            {
                await _usuarioService.SolicitarRecuperacaoSenhaAsync(request.Email);
                // Retorna Ok mesmo se o email não existir por segurança
                return Ok(new { message = "Se o email estiver cadastrado, instruções foram enviadas." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao solicitar recuperação: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno." });
            }
        }

        // --- Endpoint de Redefinir Senha ---
        // POST: api/Usuarios/redefinir-senha
        [HttpPost("redefinir-senha")]
        public async Task<IActionResult> RedefinirSenha([FromBody] RedefinirSenhaRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.NovaSenha))
            {
                return BadRequest(new { message = "Token e nova senha são obrigatórios." });
            }
            try
            {
                bool sucesso = await _usuarioService.RedefinirSenhaAsync(request.Token, request.NovaSenha);
                if (sucesso)
                {
                    return Ok(new { message = "Senha redefinida com sucesso." });
                }
                else
                {
                    return BadRequest(new { message = "Token inválido ou expirado." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao redefinir senha: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno." });
            }
        }

        // --- Endpoint de Perfil ---
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest(new { message = "Email é obrigatório." });
            try
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
                if (usuario == null) return NotFound(new { message = "Usuário não encontrado." });
                return Ok(new { nome = usuario.Nome, email = usuario.Email, dataCadastro = usuario.Data_Cadastro });
            }
            catch (Exception ex) { Console.WriteLine($"Erro ao buscar perfil: {ex.Message}"); return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno." }); }
        }

        // --- Endpoint de Excluir Conta ---
        [HttpDelete("delete-by-email")]
        public async Task<IActionResult> DeleteUsuarioByEmail([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest(new { message = "Email é obrigatório." });
            try
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
                if (usuario == null) return NotFound(new { message = "Usuário não encontrado." });
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Conta excluída com sucesso." });
            }
            catch (Exception ex) { Console.WriteLine($"Erro ao excluir conta: {ex.Message}"); return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno." }); }
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }

    // --- Classes Auxiliares (mantidas aqui para simplicidade) ---
    public class CadastroRequest
    {
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Senha { get; set; }
    }

    public class LoginRequest
    {
        public string? Email { get; set; }
        public string? Senha { get; set; }
    }

    // Nota: RecuperarSenhaRequest e RedefinirSenhaRequest são esperados
    // de StreamingRecommenderAPI.Models.User.DTOs via 'using' no topo.
}