using System.ComponentModel.DataAnnotations;

namespace CasaDeAxe.Application.DTOs
{
    public class UpdateGiraRequest
    {
        [Required]
        [StringLength(2000, MinimumLength = 3)]
        public string Descricao { get; set; } = string.Empty;

        [Required]
        [StringLength(500, MinimumLength = 2)]
        public string Cura { get; set; } = string.Empty;

        [Required]
        [StringLength(120, MinimumLength = 2)]
        public string Responsavel { get; set; } = string.Empty;

        [Required]
        public DateTime DataHora { get; set; }

        [Range(0, 2)]
        public int Status { get; set; }
    }
}
