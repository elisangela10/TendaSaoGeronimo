using CasaDeAxe.Domain.Entities;

namespace CasaDeAxe.Domain.Interfaces
{
    public interface IGiraRepository
    {
        Task<IEnumerable<Gira>> GetAllAsync();
        Task<Gira?> GetByIdAsync(int id);
        Task<Gira> AddAsync(Gira gira);
        Task<Gira?> UpdateAsync(int id, Gira gira);
        Task<bool> DeleteAsync(int id);
    }
}
