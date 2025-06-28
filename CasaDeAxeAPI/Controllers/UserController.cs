using CasaDeAxeAPI.Data;    
using CasaDeAxeAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CasaDeAxeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _secretKey;

        public UserController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _secretKey = config["JwtSettings:SecretKey"];
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest login)
        {
            try
            {
                var user = _context.Users
                    .FirstOrDefault(u => u.Username == login.Username);

                if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
                    return Unauthorized("Usuário ou senha inválidos.");

                var status = _context.StatusUsuarios.FirstOrDefault(s => s.Id == user.StatusUsuarioId);
                if (status == null || status.Nome != "Ativo")
                    return Unauthorized("Usuário não está ativo.");

                user.UltimoLogin = DateTime.UtcNow;
                _context.SaveChanges();

                var token = GenerateJwtToken(user);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            if (_context.Users.Any(u => u.Username == user.Username))
                return BadRequest("Nome de usuário já existe.");

            if (!_context.Roles.Any(r => r.Nome == user.Role.Nome))
                return BadRequest("Role inválida.");

            if (!IsPasswordStrong(user.Password))
                return BadRequest("A senha deve conter no mínimo 8 caracteres, incluindo letra maiúscula, minúscula, número e caractere especial.");

            var statusAtivo = _context.StatusUsuarios.FirstOrDefault(s => s.Nome == "Ativo");
            if (statusAtivo == null)
                return BadRequest("Status padrão 'Ativo' não encontrado.");

            int nextId = _context.Users.Any() ? _context.Users.Max(u => u.Id) + 1 : 100;
            user.Id = nextId;
            user.StatusUsuarioId = statusAtivo.Id;
            user.DataCriacao = DateTime.UtcNow;
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            _context.Users.Add(user);
            _context.SaveChanges();

            return CreatedAtAction(nameof(Login), new { username = user.Username }, new
            {
                user.Id,
                user.Username,
                user.NomeCompleto,
                user.Email,
                user.Telefone,
                user.Role,
                Status = statusAtivo.Nome,
                user.DataCriacao
            });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] User updatedUser)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound("Usuário não encontrado.");

            user.Username = updatedUser.Username;
            user.NomeCompleto = updatedUser.NomeCompleto;
            user.Email = updatedUser.Email;
            user.Telefone = updatedUser.Telefone;
            user.Role = updatedUser.Role;
            user.StatusUsuarioId = updatedUser.StatusUsuarioId;

            if (!string.IsNullOrWhiteSpace(updatedUser.Password))
                user.Password = BCrypt.Net.BCrypt.HashPassword(updatedUser.Password);

            _context.SaveChanges();
            return NoContent();
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.NomeCompleto,
                    u.Email,
                    u.Telefone,
                    u.Role,
                    Status = _context.StatusUsuarios.FirstOrDefault(s => s.Id == u.StatusUsuarioId)!.Nome,
                    u.DataCriacao,
                    u.UltimoLogin
                })
                .ToList();

            return Ok(users);
        }

        [HttpPatch("{id}/status")]
        public IActionResult UpdateStatus(int id, [FromQuery] string status)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound("Usuário não encontrado.");

            var statusObj = _context.StatusUsuarios.FirstOrDefault(s => s.Nome == status);
            if (statusObj == null)
                return BadRequest("Status inválido.");

            user.StatusUsuarioId = statusObj.Id;
            _context.SaveChanges();
            return Ok($"Status atualizado para '{statusObj.Nome}'.");
        }

        [HttpGet("profile")]
        [Authorize]
        public IActionResult GetProfile()
        {
            var username = User.Identity?.Name;
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
                return NotFound("Usuário não encontrado.");

            var status = _context.StatusUsuarios.FirstOrDefault(s => s.Id == user.StatusUsuarioId);

            return Ok(new
            {
                user.Id,
                user.Username,
                user.NomeCompleto,
                user.Email,
                user.Telefone,
                user.Role,
                Status = status?.Nome ?? "Desconhecido",
                user.DataCriacao,
                user.UltimoLogin
            });
        }

        [HttpGet("roles")]
        [AllowAnonymous]
        public IActionResult GetRoles()
        {
            var roles = _context.Roles.Select(r => r.Nome).ToList();
            return Ok(roles);
        }

        [HttpGet("status")]
        [AllowAnonymous]
        public IActionResult GetStatus()
        {
            var status = _context.StatusUsuarios.Select(s => s.Nome).ToList();
            return Ok(status);
        }

        private bool IsPasswordStrong(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return false;

            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

            return hasUpper && hasLower && hasDigit && hasSpecial;
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim("role", user.Role.Nome),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var token = new JwtSecurityToken(
                issuer: "CasaDeAxeAPI",
                audience: "CasaDeAxeAPI",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
