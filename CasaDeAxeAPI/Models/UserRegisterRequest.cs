using System.ComponentModel.DataAnnotations;

public class UserRegisterRequest
{
    [Required]
    public string Username { get; set; }

    [Required]
    [MinLength(8, ErrorMessage = "Senha deve conter no mínimo 8 caracteres.")]
    public string Password { get; set; }

    [Required]
    public string NomeCompleto { get; set; }

    public string Email { get; set; }
    public string Telefone { get; set; }
    public string Role { get; set; } = "Assistencia";
}
