using CasaDeAxe.Application.DTOs;

namespace CasaDeAxe.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse> RegisterAsync(UserRegisterRequest request);
        Task<IEnumerable<UserResponse>> GetAllAsync();
    }
}
