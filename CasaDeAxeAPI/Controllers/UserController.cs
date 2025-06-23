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

        // Roles permitidas no sistema
        private static readonly string[] RolesValidas = { "ADM", "PaiDeSanto", "Filho", "Assistencia" };

        public UserController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _secretKey = config["JwtSettings:SecretKey"];
        }

        // ✅ LOGIN: Apenas username e senha
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest login)
        {
            try
            {
                var existingUser = _context.Users.FirstOrDefault(u => u.Username == login.Username);
                if (existingUser == null || !BCrypt.Net.BCrypt.Verify(login.Password, existingUser.Password))
                    return Unauthorized("Usuário ou senha inválidos.");

                if (existingUser.Status != "Ativo")
                    return Unauthorized("Usuário não está ativo.");

                existingUser.UltimoLogin = DateTime.UtcNow;
                _context.SaveChanges();

                var token = GenerateJwtToken(existingUser);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        // ✅ REGISTRO de usuário
        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            if (_context.Users.Any(u => u.Username == user.Username))
                return BadRequest("Nome de usuário já existe.");

            if (!RolesValidas.Contains(user.Role))
                return BadRequest("Role inválida. Use: " + string.Join(", ", RolesValidas));

            if (!IsPasswordStrong(user.Password))
                return BadRequest("A senha deve conter no mínimo 8 caracteres, incluindo letra maiúscula, minúscula, número e caractere especial.");

            int nextId = _context.Users.Any() ? _context.Users.Max(u => u.Id) + 1 : 100;
            user.Id = nextId;
            user.Status = "Ativo";
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
                user.Status,
                user.DataCriacao
            });
        }

        // ✅ Validação de senha forte
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

        // ✅ Geração de token JWT
        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim("role", user.Role),
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

        // ✅ Atualiza dados do usuário
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] User updatedUser)
        {
            var existingUser = _context.Users.Find(id);
            if (existingUser == null)
                return NotFound("Usuário não encontrado.");

            existingUser.Username = updatedUser.Username;
            existingUser.NomeCompleto = updatedUser.NomeCompleto;
            existingUser.Email = updatedUser.Email;
            existingUser.Telefone = updatedUser.Telefone;
            existingUser.Role = updatedUser.Role;
            existingUser.Status = updatedUser.Status;

            if (!string.IsNullOrWhiteSpace(updatedUser.Password))
                existingUser.Password = BCrypt.Net.BCrypt.HashPassword(updatedUser.Password);

            _context.SaveChanges();
            return NoContent();
        }

        // ✅ Lista todos os usuários
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
                    u.Status,
                    u.DataCriacao,
                    u.UltimoLogin
                })
                .ToList();

            return Ok(users);
        }

        // ✅ Atualiza status do usuário (Ativo/Inativo)
        [HttpPatch("{id}/status")]
        public IActionResult UpdateStatus(int id, [FromQuery] string status)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound("Usuário não encontrado.");

            if (status != "Ativo" && status != "Inativo")
                return BadRequest("Status inválido. Use 'Ativo' ou 'Inativo'.");

            user.Status = status;
            _context.SaveChanges();
            return Ok($"Status atualizado para '{status}'.");
        }

        // ✅ Consulta perfil do usuário autenticado via token
        [HttpGet("profile")]
        [Authorize]
        public IActionResult GetProfile()
        {
            var username = User.Identity?.Name;
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
                return NotFound("Usuário não encontrado.");

            return Ok(new
            {
                user.Id,
                user.Username,
                user.NomeCompleto,
                user.Email,
                user.Telefone,
                user.Role,
                user.Status,
                user.DataCriacao,
                user.UltimoLogin
            });
        }
    }
}
