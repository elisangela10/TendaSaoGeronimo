using CasaDeAxe.Domain.Entities;
using CasaDeAxe.Domain.Interfaces;
using CasaDeAxe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CasaDeAxe.Infrastructure.Repositories
{
    public class GiraRepository : IGiraRepository
    {
        private readonly ApplicationDbContext _context;

        public GiraRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Gira>> GetAllAsync()
        {
            return await _context.Giras
                .OrderBy(g => g.DataHora)
                .ToListAsync();
        }

        public async Task<Gira?> GetByIdAsync(int id)
        {
            return await _context.Giras.FindAsync(id);
        }

        public async Task<Gira> AddAsync(Gira gira)
        {
            _context.Giras.Add(gira);
            await _context.SaveChangesAsync();
            return gira;
        }

        public async Task<Gira?> UpdateAsync(int id, Gira gira)
        {
            var existing = await _context.Giras.FindAsync(id);
            if (existing == null)
                return null;

            existing.Descricao = gira.Descricao;
            existing.Cura = gira.Cura;
            existing.Responsavel = gira.Responsavel;
            existing.DataHora = gira.DataHora;
            existing.Status = gira.Status;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Giras.FindAsync(id);
            if (existing == null)
                return false;

            _context.Giras.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
