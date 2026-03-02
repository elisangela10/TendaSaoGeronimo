using CasaDeAxe.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/giras")]
public class GiraController : ControllerBase
{
    private readonly GiraService _service;

    public GiraController(GiraService service)
    {
        _service = service;
    }

    //  Criar Gira
    [HttpPost]
    [Authorize(Roles = "Admin,PaiDeSanto")]
    public async Task<IActionResult> Criar([FromBody] CreateGiraRequest request)
    {
        var result = await _service.CriarAsync(request);
        return Ok(result);
    }

    //  Atualizar Gira
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,PaiDeSanto")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] UpdateGiraRequest request)
    {
        await _service.AtualizarAsync(id, request);
        return NoContent();
    }

    //  Listar todas
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Listar()
    {
        var giras = await _service.ObterTodasAsync();
        return Ok(giras);
    }
}