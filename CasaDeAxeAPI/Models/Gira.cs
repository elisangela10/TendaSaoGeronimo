namespace CasaDeAxeAPI.Models
{
    public class Gira
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public string Responsavel { get; set; }
    }
}
