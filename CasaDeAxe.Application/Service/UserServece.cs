using CasaDeAxe.Application.DTOs;
using CasaDeAxe.Application.Interfaces;
using CasaDeAxe.Domain.Entities;
using CasaDeAxe.Domain.Interfaces;


using System.Data;

namespace CasaDeAxe.Application.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User> CreateAsync(User user)
        {
            return await _userRepository.CreateAsync(user);
        }

        public async Task<User> UpdateAsync(User user)
        {
            return await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteAsync(int id)
        {
            await _userRepository.DeleteAsync(id);
        }



        public async Task<UserResponse> RegisterAsync(UserRegisterRequest request)
        {
            var roleId = request.RoleId > 0 ? request.RoleId : 1;
            var statusId = request.StatusUsuarioId > 0 ? request.StatusUsuarioId : 1;

           
            var role = await _userRepository.GetRoleByIdAsync(roleId);
            if (role == null)
                throw new Exception("Role inválido.");

            var status = await _userRepository.GetStatusByIdAsync(statusId);
            if (status == null)
                throw new Exception("Status inválido.");

            var user = new User
            {
                NomeCompleto = request.NomeCompleto,
                Email = request.Email,
                Telefone = request.Telefone,
                Username = request.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                RoleId = roleId,
                StatusUsuarioId = statusId,
                DataCriacao = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

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
        Task IUserService.RegisterAsync(UserRegisterRequest request)
        {
            return RegisterAsync(request);
        }

        public Task<User?> GetByLoginAsync(string login)
        {
            throw new NotImplementedException();
        }
    }
}
