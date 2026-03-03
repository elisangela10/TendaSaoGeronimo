using CasaDeAxe.Application.DTOs;
using CasaDeAxe.Application.Interfaces;
using CasaDeAxe.Domain.Entities;
using CasaDeAxe.Domain.Enums;
using CasaDeAxe.Domain.Interfaces;

namespace CasaDeAxe.Application.Service
{
    public class GiraService : IGiraService
    {
        private readonly IGiraRepository _giraRepository;

        public GiraService(IGiraRepository giraRepository)
        {
            _giraRepository = giraRepository;
        }

        public async Task<IEnumerable<GiraResponse>> GetAllAsync()
        {
            var giras = await _giraRepository.GetAllAsync();
            return giras.Select(MapToResponse);
        }

        public async Task<GiraResponse?> GetByIdAsync(int id)
        {
            var gira = await _giraRepository.GetByIdAsync(id);
            return gira == null ? null : MapToResponse(gira);
        }

        public async Task<GiraResponse> CreateAsync(CreateGiraRequest request)
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

            var created = await _giraRepository.AddAsync(gira);
            return MapToResponse(created);
        }

        public async Task<GiraResponse?> UpdateAsync(int id, UpdateGiraRequest request)
        {
            var gira = new Gira
            {
                Descricao = request.Descricao,
                Cura = request.Cura,
                Responsavel = request.Responsavel,
                DataHora = request.DataHora,
                Status = (StatusGira)request.Status
            };

            var updated = await _giraRepository.UpdateAsync(id, gira);
            return updated == null ? null : MapToResponse(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _giraRepository.DeleteAsync(id);
        }

        private static GiraResponse MapToResponse(Gira gira)
        {
            return new GiraResponse
            {
                Id = gira.Id,
                Nome = gira.Nome,
                Descricao = gira.Descricao,
                Cura = gira.Cura,
                Responsavel = gira.Responsavel,
                DataHora = gira.DataHora,
                Status = gira.Status.ToString(),
                DataCriacao = gira.DataCriacao
            };
        }
    }
}
