using CasaDeAxe.Domain.Entities;

namespace CasaDeAxe.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> AddAsync(User user);
        Task<User?> GetByIdAsync(int id);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task DeleteAsync(int id);
        Task<Role?> GetRoleByIdAsync(int roleId);
        Task<StatusUsuario?> GetStatusByIdAsync(int statusId);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByLoginAsync(string login);


    }
}
