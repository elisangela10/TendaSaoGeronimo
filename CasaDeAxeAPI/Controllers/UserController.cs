using CasaDeAxe.Application.DTOs;
using CasaDeAxe.Application.Interfaces;
using CasaDeAxe.Domain.Entities;
using CasaDeAxe.Domain.Interfaces;
using CasaDeAxe.Infrastructure.Data;
using CasaDeAxe.Infrastructure.Repositories;
using CasaDeAxe.Infrastructure.Services;
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
        private readonly IUserService _service;
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        public UserController(IUserService service, IUserRepository userRepository, IJwtService jwtService)
        {
            _service = service;
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterRequest request)
        {
            await _service.RegisterAsync(request);
            return Ok("Usuário registrado.");
        }
        
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userRepository.GetByLoginAsync(request.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                return Unauthorized("Usuário ou senha inválidos.");

            var token = _jwtService.GenerateToken(user);
            return Ok(new { Token = token });
        }


    }
}
