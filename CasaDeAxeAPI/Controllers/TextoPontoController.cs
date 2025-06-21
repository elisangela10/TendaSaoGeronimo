using CasaDeAxeAPI.Data;
using CasaDeAxeAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDeAxeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TextoPontoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TextoPontoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/TextoPonto
        [HttpPost]
        public async Task<IActionResult> CreateTextoPonto([FromBody] TextoPonto textoPonto)
        {
            if (textoPonto == null)
            {
                return BadRequest("Texto, link do YouTube ou pontos não podem ser nulos.");
            }

            _context.TextoPontos.Add(textoPonto); // <- PLURAL
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTextoPonto), new { id = textoPonto.Id }, textoPonto);
        }

        // GET: api/TextoPonto
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TextoPonto>>> GetTextoPontos()
        {
            var textoPontos = await _context.TextoPontos.ToListAsync(); // <- PLURAL
            return Ok(textoPontos);
        }

        // GET: api/TextoPonto/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TextoPonto>> GetTextoPonto(int id)
        {
            var textoPonto = await _context.TextoPontos.FindAsync(id); // <- PLURAL

            if (textoPonto == null)
            {
                return NotFound();
            }

            return Ok(textoPonto);
        }
    }
}
