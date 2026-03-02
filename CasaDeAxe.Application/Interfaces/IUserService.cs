using CasaDeAxe.Domain.Entities;

namespace CasaDeAxe.Application.Interfaces
{
    public interface IUserService
    {
        Task RegisterAsync(UserRegisterRequest request);
        Task<User?> GetByLoginAsync(string login);
    }


}
