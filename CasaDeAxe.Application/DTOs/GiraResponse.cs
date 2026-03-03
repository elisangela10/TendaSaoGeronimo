namespace CasaDeAxe.Application.DTOs
{
    public class GiraResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Cura { get; set; } = string.Empty;
        public string Responsavel { get; set; } = string.Empty;
        public DateTime DataHora { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
    }
}
