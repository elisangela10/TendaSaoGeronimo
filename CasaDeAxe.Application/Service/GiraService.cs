using CasaDeAxe.Application.DTOs;
using CasaDeAxe.Domain.Entities;
using CasaDeAxe.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using CasaDeAxe.Infrastructure.Data;

public class GiraService
{
    private readonly ApplicationDbContext _context;

    public GiraService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GiraResponse> CriarAsync(CreateGiraRequest request)
    {
        var gira = new Gira
        {
            Nome = request.Nome,
            Descricao = request.Descricao,
            Cura = request.Cura,
            Responsavel = request.Responsavel,
            DataHora = request.DataHora,
            Status = StatusGira.Ativo
        };

        _context.Giras.Add(gira);
        await _context.SaveChangesAsync();

        return Map(gira);
    }

    public async Task AtualizarAsync(int id, UpdateGiraRequest request)
    {
        var gira = await _context.Giras.FindAsync(id);
        if (gira == null)
            throw new Exception("Gira não encontrada.");

        gira.Descricao = request.Descricao;
        gira.Cura = request.Cura;
        gira.Responsavel = request.Responsavel;
        gira.DataHora = request.DataHora;
        gira.Status = (StatusGira)request.Status;

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<GiraResponse>> ObterTodasAsync()
    {
        return await _context.Giras
            .Select(g => Map(g))
            .ToListAsync();
    }

    private static GiraResponse Map(Gira g) =>
        new()
        {
            Id = g.Id,
            Nome = g.Nome,
            Descricao = g.Descricao,
            Cura = g.Cura,
            Responsavel = g.Responsavel,
            DataHora = g.DataHora,
            Status = g.Status.ToString()
        };
}