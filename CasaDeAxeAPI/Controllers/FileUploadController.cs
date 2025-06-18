using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace CasaDeAxeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        // Define o diretório onde os arquivos serão salvos
        private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

        public FileUploadController()
        {
            // Cria o diretório de uploads, caso ele não exista
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            // Verifica se o arquivo foi enviado
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Define o caminho completo onde o arquivo será salvo
            var filePath = Path.Combine(_storagePath, file.FileName);

            // Salva o arquivo no diretório
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Retorna o caminho do arquivo armazenado
            return Ok(new { FilePath = filePath });
        }

        // Endpoint para listar arquivos
        [HttpGet("list")]
        public IActionResult ListFiles()
        {
            var files = Directory.GetFiles(_storagePath)
                .Select(filePath => new FileInfo(filePath))
                .Select(file => new
                {
                    fileName = file.Name,
                    size = file.Length,
                    lastModified = file.LastWriteTime
                })
                .ToList();

            if (files.Count == 0)
            {
                return NotFound("No files found.");
            }

            return Ok(files);
        }







    }
}
