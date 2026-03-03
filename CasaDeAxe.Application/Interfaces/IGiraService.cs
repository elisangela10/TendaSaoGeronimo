using CasaDeAxe.Application.DTOs;

namespace CasaDeAxe.Application.Interfaces
{
    public interface IGiraService
    {
        Task<IEnumerable<GiraResponse>> GetAllAsync();
        Task<GiraResponse?> GetByIdAsync(int id);
        Task<GiraResponse> CreateAsync(CreateGiraRequest request);
        Task<GiraResponse?> UpdateAsync(int id, UpdateGiraRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
