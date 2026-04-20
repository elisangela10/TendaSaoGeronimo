using CasaDeAxe.Domain.Entities;
using CasaDeAxe.Domain.Interfaces;
using CasaDeAxe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CasaDeAxe.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.StatusUsuario)
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }
        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Role?> GetRoleByIdAsync(int roleId)
        {
            return await _context.Roles.FindAsync(roleId);
        }

        public async Task<StatusUsuario?> GetStatusByIdAsync(int statusId)
        {
            return await _context.StatusUsuarios.FindAsync(statusId);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
            .Include(u => u.Role)
            .Include(u => u.StatusUsuario)
            .FirstOrDefaultAsync(u => u.Username == username);
            
    
        }
        public async Task<User?> GetByLoginAsync(string login)
        {
            login = login.ToLower();

            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.StatusUsuario)
                .FirstOrDefaultAsync(u =>
                    u.Username == login ||
                    u.Email.ToLower() == login
                );
        }
    
    }
}
