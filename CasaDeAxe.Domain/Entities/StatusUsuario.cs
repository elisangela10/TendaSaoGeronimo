using CasaDeAxe.Domain.Entities;

public class StatusUsuario
{
    public int Id { get; set; }
    public string Nome { get; set; }

    public ICollection<User> Users { get; set; }
}
