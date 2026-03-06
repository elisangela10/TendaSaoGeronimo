using CasaDeAxe.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasaDeAxe.Domain.Entities
{
    public class Gira
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Cura { get; set; } = string.Empty;
        public string Responsavel { get; set; } = string.Empty;

        // Compatibilidade com schema legado: no banco a coluna original é "Data"
        [Column("Data")]
        public DateTime DataHora { get; set; }

        public StatusGira Status { get; set; } = StatusGira.Ativo;
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    }
}
