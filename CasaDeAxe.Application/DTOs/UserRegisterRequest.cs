using System.ComponentModel.DataAnnotations;

namespace CasaDeAxe.Application.DTOs
{
    public class UserRegisterRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(200, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(30, MinimumLength = 8)]
        public string Telefone { get; set; } = string.Empty;

        public int RoleId { get; set; }
        public int StatusUsuarioId { get; set; }
    }
}
