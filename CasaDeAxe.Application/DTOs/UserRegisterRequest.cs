public class UserRegisterRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string NomeCompleto { get; set; }
    public string Email { get; set; }
    public string Telefone { get; set; }
    public int RoleId { get; set; }
    public int StatusUsuarioId { get; set; }
}
