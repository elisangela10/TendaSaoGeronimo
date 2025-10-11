namespace CasaDeAxe.Domain.Entities;
public class User
{
    public int Id { get; set; }

    public string NomeCompleto { get; set; }
    public string Email { get; set; }
    public string Telefone { get; set; }

    public string Username { get; set; }
    public string Password { get; set; }

    public int RoleId { get; set; }
    public Role Role { get; set; }

    public int StatusUsuarioId { get; set; }
    public StatusUsuario StatusUsuario { get; set; }

    public DateTime DataCriacao { get; set; }
    public DateTime? UltimoLogin { get; set; }
}
