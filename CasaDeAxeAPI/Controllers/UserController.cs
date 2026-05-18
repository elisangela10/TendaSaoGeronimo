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
            return StatusCode(201, user);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userRepository.GetByLoginAsync(request .Login);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                _logger.LogWarning("Falha de autenticação para o login: {Login}", request.Login);
                return Unauthorized("Usuário ou senha inválidos.");
            }

            var token = _jwtService.GenerateToken(user);
            _logger.LogInformation("Autenticação realizada com sucesso para o login: {Login}", request.Login);
            return Ok(new { Token = token });   
        }


        [HttpGet("GetUser")]
        [AllowAnonymous]
        public async Task<IActionResult>GetAll()
        {
            var users = await _service.GetAllAsync();
            _logger.LogInformation("Consulta de usuários cadastrados realizada com sucesso.");
            return Ok(users);
        }
    }
}
