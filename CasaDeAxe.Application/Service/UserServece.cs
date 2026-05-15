using CasaDeAxe.Application.DTOs;
using CasaDeAxe.Application.Interfaces;
using CasaDeAxe.Domain.Entities;
using CasaDeAxe.Domain.Interfaces;

namespace CasaDeAxe.Application.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserResponse> RegisterAsync(UserRegisterRequest request)
        {
            var user = new User
            {
                NomeCompleto = request.NomeCompleto,
                Email = request.Email,
                Telefone = request.Telefone,
                Username = request.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                RoleId = request.RoleId > 0 ? request.RoleId : 1,
                StatusUsuarioId = request.StatusUsuarioId > 0 ? request.StatusUsuarioId : 1,
                DataCriacao = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user)    ;
            var role = await _userRepository.GetRoleByIdAsync(user.RoleId);
            var status = await _userRepository.GetStatusByIdAsync(user.StatusUsuarioId);

            if (role == null || status == null)
                throw new Exception("Role ou Status inválido.");

            return new UserResponse
            {
                Id = user.Id,
                NomeCompleto = user.NomeCompleto,
                Email = user.Email,
                Telefone = user.Telefone,
                Username = user.Username,
                RoleNome = role.Nome,
                StatusNome = status.Nome,
                DataCriacao = user.DataCriacao
            };
        }

        public async Task<IEnumerable<UserResponse>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();

            return users.Select(user => new UserResponse
            {
                Id = user.Id,
                NomeCompleto = user.NomeCompleto,
                Email = user.Email,
                Telefone = user.Telefone,
                Username = user.Username,
                RoleNome = user.Role?.Nome ?? string.Empty,
                StatusNome = user.StatusUsuario?.Nome ?? string.Empty,
                DataCriacao = user.DataCriacao,
                UtimoLogin = user.UltimoLogin
            });
        }
    }
}
