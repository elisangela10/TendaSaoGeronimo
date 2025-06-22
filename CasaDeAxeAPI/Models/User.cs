public class User
{
    public int Id { get; set; }

    public string NomeCompleto { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Telefone { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string Role { get; set; } = "Visitante"; // Visitante, Filho, PaiDeSanto, ADM

    public string Status { get; set; } = "Ativo";    // Ativo, Inativo, Aguardando, etc.

    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public DateTime? UltimoLogin { get; set; }
}
