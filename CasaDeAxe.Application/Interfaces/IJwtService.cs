using CasaDeAxe.Domain.Entities;

namespace CasaDeAxe.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
