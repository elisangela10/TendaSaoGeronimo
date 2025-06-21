using CasaDeAxeAPI.Data;
using CasaDeAxeAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CasaDeAxeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiraController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GiraController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Gira
        [HttpGet]
        public IActionResult GetGiras()
        {
            var giras = _context.Giras.ToList();
            return Ok(giras);
        }

        // GET: api/Gira/{id}
        [HttpGet("{id}")]
        public IActionResult GetGiraById(int id)
        {
            var gira = _context.Giras.Find(id);
            if (gira == null) return NotFound("Gira não encontrada.");
            return Ok(gira);
        }

        // POST: api/Gira
        [HttpPost]
        public IActionResult CreateGira([FromBody] Gira gira)
        {
            if (gira == null) return BadRequest("Dados inválidos.");
            _context.Giras.Add(gira);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetGiraById), new { id = gira.Id }, gira);
        }

        // PUT: api/Gira/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateGira(int id, [FromBody] Gira gira)
        {
            var existingGira = _context.Giras.Find(id);
            if (existingGira == null) return NotFound("Gira não encontrada.");

            existingGira.Nome = gira.Nome;
            existingGira.Descricao = gira.Descricao;
            existingGira.Data = gira.Data;
            existingGira.Responsavel = gira.Responsavel;

            _context.SaveChanges();
            return NoContent();
        }

        // DELETE: api/Gira/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteGira(int id)
        {
            var gira = _context.Giras.Find(id);
            if (gira == null) return NotFound("Gira não encontrada.");

            _context.Giras.Remove(gira);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
