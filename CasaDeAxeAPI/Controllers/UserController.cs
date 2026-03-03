using CasaDeAxe.Application.DTOs;
using CasaDeAxe.Application.Interfaces;
using CasaDeAxe.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasaDeAxeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IUserService service,
            IUserRepository userRepository,
            IJwtService jwtService,
            ILogger<UserController> logger)
        {
            _service = service;
            _userRepository = userRepository;
            _jwtService = jwtService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest request)
        {
            var user = await _service.RegisterAsync(request);
            _logger.LogInformation("Usuário registrado com sucesso. Id: {UserId}, Username: {Username}", user.Id, user.Username);
            return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                _logger.LogWarning("Falha de autenticação para o username: {Username}", request.Username);
                return Unauthorized("Usuário ou senha inválidos.");
            }

            var token = _jwtService.GenerateToken(user);
            _logger.LogInformation("Autenticação realizada com sucesso para o username: {Username}", request.Username);
            return Ok(new { Token = token });
        }
    }
}
