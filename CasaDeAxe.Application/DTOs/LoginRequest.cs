using System.ComponentModel.DataAnnotations;

namespace CasaDeAxe.Application.DTOs
{
    public class LoginRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(200, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;
    }
}
