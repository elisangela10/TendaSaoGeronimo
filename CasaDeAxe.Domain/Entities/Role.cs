using CasaDeAxe.Domain.Entities;

public class Role
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public bool Ativo { get; set; }

    public ICollection<User> Users { get; set; }
}
