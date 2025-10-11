namespace CasaDeAxe.Application.DTOs
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string NomeCompleto { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Username { get; set; }
        public string RoleNome { get; set; }
        public string StatusNome { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? UtimoLogin { get; set; }
    }
}
