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

namespace StreamingRecommenderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        // --- CORREÇÃO: Mover campos para DENTRO da classe ---
        private readonly ApplicationDbContext _context;
        private readonly UsuarioService _usuarioService; 

        // Construtor corrigido para injetar UsuarioService
        public UsuariosController(ApplicationDbContext context, UsuarioService usuarioService)
        {
            _context = context;
            _usuarioService = usuarioService; // Serviço injetado
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        // PUT: api/Usuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return BadRequest();
            }
            _context.Entry(usuario).State = EntityState.Modified;
             _context.Entry(usuario).Property(x => x.Senha).IsModified = false; // Impede alteração de senha aqui

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id)) { return NotFound(); } else { throw; }
            }
            return NoContent();
        }

        // // POST: api/Usuarios (Comentado ou Removido para evitar conflito com /cadastro)
    // // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    // [HttpPost]
    // public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
    // {
    //     // Adicionar hashing de senha se este endpoint for usado diretamente
    //     // usuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha); 
    //     _context.Usuarios.Add(usuario);
    //     await _context.SaveChangesAsync();
    //     return CreatedAtAction("GetUsuario", new { id = usuario.Id }, usuario);
    // }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) { return NotFound(); }
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // --- Endpoint de Cadastro Adicionado ---
        // POST: api/Usuarios/cadastro
        [HttpPost("cadastro")]
        public async Task<IActionResult> CadastrarUsuario([FromBody] CadastroRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Nome) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Senha))
            {
                return BadRequest(new { message = "Nome, email e senha são obrigatórios." });
            }
            try
            {
                bool sucesso = await _usuarioService.CadastrarUsuarioAsync(request.Nome, request.Email, request.Senha);
                if (sucesso)
                {
                    return StatusCode(StatusCodes.Status201Created, new { message = "Usuário cadastrado com sucesso." });
                }
                else
                {
                    return BadRequest(new { message = "Email já cadastrado." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao cadastrar: {ex.Message}"); 
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno ao cadastrar." });
            }
        }

        // --- Endpoint de Login Adicionado ---
        // POST: api/Usuarios/login
        [HttpPost("login")]
        public async Task<IActionResult> LoginUsuario([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Senha))
            {
                return BadRequest(new { message = "Email e senha são obrigatórios." });
            }
            try
            {
                var usuario = await _usuarioService.LoginAsync(request.Email, request.Senha);
                if (usuario != null)
                {
                    return Ok(new { username = usuario.Nome }); // Retorna nome de usuário
                }
                else
                {
                    return Unauthorized(new { message = "Email ou senha inválidos." });
                }
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"Erro no login: {ex.Message}"); 
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno durante o login." });
            }
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }

    // --- Classes Auxiliares Adicionadas ---
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
}