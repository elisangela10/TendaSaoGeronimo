using CasaDeAxe.Application.DTOs;
using CasaDeAxe.Application.Interfaces;
using CasaDeAxe.Domain.Entities;
using CasaDeAxe.Domain.Enums;
using CasaDeAxe.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CasaDeAxe.Application.Service
{
    public class GiraService : IGiraService
    {
        private readonly IGiraRepository _giraRepository;
        private readonly ILogger<GiraService> _logger;

        public GiraService(IGiraRepository giraRepository, ILogger<GiraService> logger)
        {
            _giraRepository = giraRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<GiraResponse>> GetAllAsync()
        {
            var giras = await _giraRepository.GetAllAsync();
            _logger.LogInformation("Giras listadas com sucesso. Quantidade: {Count}", giras.Count());
            return giras.Select(MapToResponse);
        }

        public async Task<GiraResponse?> GetByIdAsync(int id)
        {
            var gira = await _giraRepository.GetByIdAsync(id);
            if (gira == null)
            {
                _logger.LogWarning("Gira não encontrada. Id: {GiraId}", id);
                return null;
            }

            _logger.LogInformation("Gira recuperada com sucesso. Id: {GiraId}", id);
            return MapToResponse(gira);
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
            _logger.LogInformation("Gira criada com sucesso. Id: {GiraId}, Nome: {Nome}", created.Id, created.Nome);
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
            if (updated == null)
            {
                _logger.LogWarning("Tentativa de atualização em Gira inexistente. Id: {GiraId}", id);
                return null;
            }

            _logger.LogInformation("Gira atualizada com sucesso. Id: {GiraId}", id);
            return MapToResponse(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var deleted = await _giraRepository.DeleteAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("Tentativa de exclusão em Gira inexistente. Id: {GiraId}", id);
                return false;
            }

            _logger.LogInformation("Gira removida com sucesso. Id: {GiraId}", id);
            return true;
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
