using CasaDeAxe.Application.DTOs;
using CasaDeAxe.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CasaDeAxeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GiraController : ControllerBase
    {
        private readonly IGiraService _giraService;

        public GiraController(IGiraService giraService)
        {
            _giraService = giraService;
        }

        [HttpGet]
        public async Task<IActionResult> GetGiras()
        {
            var giras = await _giraService.GetAllAsync();
            return Ok(giras);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGiraById(int id)
        {
            var gira = await _giraService.GetByIdAsync(id);
            if (gira == null)
                return NotFound("Gira não encontrada.");

            return Ok(gira);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGira([FromBody] CreateGiraRequest request)
        {
            if (request == null)
                return BadRequest("Dados inválidos.");

            var created = await _giraService.CreateAsync(request);
            return CreatedAtAction(nameof(GetGiraById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGira(int id, [FromBody] UpdateGiraRequest request)
        {
            if (request == null)
                return BadRequest("Dados inválidos.");

            var updated = await _giraService.UpdateAsync(id, request);
            if (updated == null)
                return NotFound("Gira não encontrada.");

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGira(int id)
        {
            var deleted = await _giraService.DeleteAsync(id);
            if (!deleted)
                return NotFound("Gira não encontrada.");

            return NoContent();
        }
    }
}
