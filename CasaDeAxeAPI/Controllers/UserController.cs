using CasaDeAxeAPI.Data;
using CasaDeAxeAPI.Models;
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
        public IActionResult Login([FromBody] User user)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Username == user.Username);
            if (existingUser == null || !BCrypt.Net.BCrypt.Verify(user.Password, existingUser.Password))
                return Unauthorized("Usuário ou senha inválidos.");

            if (existingUser.Status != "Ativo")
                return Unauthorized("Usuário não está ativo.");

            var token = GenerateJwtToken(existingUser);
            return Ok(new { Token = token });
         
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim("role", user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: "CasaDeAxeAPI",
                audience: "CasaDeAxeAPI",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            if (_context.Users.Any(u => u.Username == user.Username))
                return BadRequest("Nome de usuário já existe.");

            // Gera um ID de 3 dígitos (incremental a partir do maior existente)
            int nextId = _context.Users.Any() ? _context.Users.Max(u => u.Id) + 1 : 100;
            user.Id = nextId;
            user.Status = "Ativo";


            // Criptografa a senha
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            _context.Users.Add(user);
            _context.SaveChanges();

            return CreatedAtAction(nameof(Login), new { username = user.Username }, user);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] User updatedUser)
        {
            var existingUser = _context.Users.Find(id);
            if (existingUser == null)
                return NotFound("Usuário não encontrado.");

            existingUser.Username = updatedUser.Username;

            // Atualiza senha só se vier preenchida
            if (!string.IsNullOrWhiteSpace(updatedUser.Password))
                existingUser.Password = BCrypt.Net.BCrypt.HashPassword(updatedUser.Password);

            existingUser.Role = updatedUser.Role;

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
                    u.Role,
                    u.Status
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

            if (status != "Ativo" && status != "Inativo")
                return BadRequest("Status inválido. Use 'Ativo' ou 'Inativo'.");

            user.Status = status;
            _context.SaveChanges();
            return Ok($"Status atualizado para '{status}'.");
        }




    }
}
