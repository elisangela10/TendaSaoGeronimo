using CasaDeAxe.Domain.Enums;

namespace CasaDeAxe.Domain.Entities
{
    public class Gira
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string Cura { get; set; }
        public string Responsavel { get; set; }
        public DateTime DataHora { get; set; }
        public StatusGira Status { get; set; } = StatusGira.Ativo;
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    }
}
