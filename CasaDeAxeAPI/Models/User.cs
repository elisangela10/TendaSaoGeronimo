using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CasaDeAxeAPI.Models
{
    public class User
    {
       
        [Key]
        public int Id { get; set; } // Iremos gerar esse valor manualmente (ex: 101, 102...)
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public string? Role { get; set; }

        [Required]
        public string Status { get; set; } = "Ativo";
    }
}
